using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CandleRowDestroyer : CandleLightCollector
{

    [SerializeField] RightWall otherSide;
    [SerializeField] GameObject CandleDestroyParticle;
    [SerializeField] GameManager gameManager;
    [SerializeField] ParticleSystem leftParticleSystem;

    [SerializeField] float rowDestructionBonusTime;
    float rowDestructionInitialTime = 0;
    

    
    void Start(){
        leftParticleSystem.Stop();
    }

    
    void Update(){
        
    }



    void FixedUpdate() { 
        CandleLightController[] r = findRow().ToArray();

        if (r.Length > 0) {

            int totalCandles = 0;
            int multiplier = 1;
            int points;

            //if the last row destruction bonus has not passed yet
            if (rowDestructionInitialTime + rowDestructionBonusTime > Time.time) {
                multiplier *= 2;
                if(gameManager != null) {
                    gameManager.createRowDestructionBonusText();
                }
            }

            rowDestructionInitialTime = Time.time;

            for (int i = 0; i < r.Length; i++) {

                //"r" holds ids for every candle light, this null check is necessary because of candles with multiple lights being entered multiple times
                if (r[i] != null) {
                    if (gameManager != null) {
                        multiplier = gameManager.createBonusText(r[i].getParentObject(), multiplier);
                    }
                    destroyCandle(r[i]);
                    totalCandles++;
                }

            }

            points = totalCandles * multiplier;
            if (gameManager != null) {
                gameManager.addScore(points);
            }

        }
        
    }



    void destroyCandle(CandleLightController can) {
        GameObject c = can.getParentObject();
        GameObject p = Instantiate(CandleDestroyParticle);
        p.transform.position = c.transform.position;
        if (gameManager != null) {
            gameManager.destroyCandle(c);
        }
    }


    CandleLightController[] findRow() {
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


    //need to keep track of all other candle ids that are touching the wall
    /*public override void addToList(CandleLightController other) {
        base.addToList(other);

        //start wall shine if something is touching the wall
        if (isTouchingAnyCandles()) {
            leftParticleSystem.Play();
        }
    }*/


}
