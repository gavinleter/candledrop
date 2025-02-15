using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CandleRowDestroyer : CandleLightCollector
{

    [SerializeField] RightWall otherSide;
    [SerializeField] GameObject CandleDestroyParticle;
    [SerializeField] GameManager gameManager;
    [SerializeField] MusicManager musicManager;
    [SerializeField] ParticleSystem leftParticleSystem;

    [SerializeField] float rowDestructionBonusTime;
    float rowDestructionInitialTime = 0;
    int comboCount = 0;
    

    
    void Start(){
        leftParticleSystem.Stop();
    }



    void FixedUpdate() { 
        CandleLightController[] r = findRow().ToArray();

        if (r.Length > 0) {

            destroyRow(r);

            //"I know how to play!" unlocks after lighting your first row of candles
            Settings.setAchievementUnlocked(0);

            if(r.Length <= 6) {
                //"Minimalist" unlocks after completing a row of 6 or less candles
                Settings.setAchievementUnlocked(4);
            }

            if(r.Length >= 10) {
                //"Maximalist" unlocks after completing a row of 10 or more candles
                Settings.setAchievementUnlocked(5);
            }

            if (r.Length >= 15) {
                //"God of maximalism" unlocks after completing a row of 15 or more candles
                Settings.setAchievementUnlocked(6);
            }

        }
        
    }


    void destroyRow(CandleLightController[] r) {
        musicManager.toggleIntenseMusic();

        //count how many candles of each color group were found in the row for achievements 29-21
        int colorGroupLength = System.Enum.GetNames(typeof(CandleColorGroup)).Length;
        int[] colorsFound = new int[colorGroupLength];
        int multiplier = 1;
        int points = 0;
        //for "Flaring Focus" (#7) achievement
        bool allFlares = true;
        //for achievements 40 & 42 involving completing a row with supercharged wicks
        int solarLightCount = 0;

        //if the last row destruction bonus has not passed yet
        if (rowDestructionInitialTime + rowDestructionBonusTime > Time.time) {
            comboCount++;

            multiplier *= 2 * comboCount;
            gameManager.createRowDestructionBonusText();

            //"Baby's first combo" unlocked by getting a combo
            Settings.setAchievementUnlocked(28);

            if(comboCount >= 3) {
                //"Combo god" unlocked by getting a triple combo
                Settings.setAchievementUnlocked(28);
            }

        }
        else {
            comboCount = 0;
        }

        rowDestructionInitialTime = Time.time;

        for (int i = 0; i < r.Length; i++) {

            //"r" holds ids for every candle light, this null check is necessary because of candles with multiple lights being entered multiple times
            if (r[i] != null && !r[i].isBeingDestroyed()) {

                points += r[i].getPoints();
                multiplier = gameManager.createBonusText(r[i].getParentObject(), multiplier);

                //increase the amount of candles of this color have been found
                colorsFound[ r[i].getCandleId().getColorGroup() ]++;

                destroyCandle(r[i]);

                if (!r[i].isCurrentlyFlare()) {
                    allFlares = false;
                }

                if (r[i].isMiniSunIgnited()) {
                    solarLightCount++;
                }

            }

        }

        gameManager.addScore(points * multiplier);

        if (allFlares) {
            //"Flaring Focus" unlocked after completing a row of only flares
            Settings.setAchievementUnlocked(7);
        }

        int uniqueColorsFound = 0;
        //check how many unique colors are found in the row
        for (int i = 0; i < colorsFound.Length; i++) {
            if (colorsFound[i] != 0) {
                uniqueColorsFound++;
            }
        }

        if (uniqueColorsFound == 1) {
            //"Monochromatic" unlocked by making a row with candles of one color group
            Settings.setAchievementUnlocked(20);
        } 
        else if (uniqueColorsFound == 2) {
            //"Dichromatic" unlocked by making a row with candles of two color groups
            Settings.setAchievementUnlocked(21);
        } 
        else if (uniqueColorsFound == colorGroupLength) {
            //"Color Conga Line" unlocked by making a row with candles of all color groups
            Settings.setAchievementUnlocked(19);
        }

        if(solarLightCount > 0) {
            //"You Are My Sunshine" unlocked by completing a row using a sun
            Settings.setAchievementUnlocked(40);
        }
        if(solarLightCount == r.Length) {
            //"Praise the Suns" unlocked by completing a row with only supercharged wicks
            Settings.setAchievementUnlocked(42);
        }

    }


    void destroyCandle(CandleLightController can) {
        GameObject c = can.getParentObject();
        //GameObject p = Instantiate(CandleDestroyParticle);
        //p.transform.position = c.transform.position;
        gameManager.destroyCandle(c);
    }


    CandleLightController[] findRow() {
        //this is a hashset because .Contains() is called on it several times
        //it gets coverted back into an array so that candles inside of it can be deleted
        HashSet<CandleLightController> result = new HashSet<CandleLightController>();

        updateTouchingList();

        //if the left wall is touching nothing exit early
        if(isTouchingAnyCandles()) {

            //start wall shine if something is touching the wall
            if (!leftParticleSystem.isPlaying) {
                leftParticleSystem.Play();
            }

            CandleIgniter canLight;
            CandleLightController can;

            //go through each candle and all of the touching candles
            for (int i = 0; i < touchingLength; i++) {

                canLight = touching[i].GetComponent<CandleIgniter>();

                if (canLight) {
                    can = canLight.getParentCandleScript();

                    if (can != null && can.isEnabled() && !result.Contains(can)) {
                        result.Add(can);
                        can.traverse(result);
                    }
                }

            }

            //check if any recorded candles are touching the right wall
            foreach (CandleLightController c in result) {
                if (otherSide.containsCandle(c)) {
                    return result.ToArray();
                }
            }

            //if it has gotten to this point, then there are no candles touching the
            //right wall so we should return nothing
            return new CandleLightController[0];

        }
        else {
            leftParticleSystem.Stop();
        }

        return result.ToArray();
    }




}
