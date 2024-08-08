using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleRowDestroyer : MonoBehaviour
{

    List<int> touching = new List<int>();
    [SerializeField] GameObject otherSide;
    [SerializeField] GameObject CandleDestroyParticle;
    [SerializeField] GameObject gameManager;
    [SerializeField] ParticleSystem leftParticleSystem;
    [SerializeField] GameObject defaultBonusTextLocation;
    GameManager gameManagerScript;
    RightWall otherSideScript;

    [SerializeField] GameObject bonusTextPrefab;

    [SerializeField] float rowDestructionBonusTime;
    float rowDestructionInitialTime = 0;
    

    
    void Start(){
        otherSideScript = otherSide.GetComponent<RightWall>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
        leftParticleSystem.Stop();
    }

    
    void Update(){
        
    }


    void FixedUpdate() { 
        List<int> r = findRow();

        if (r.Count > 0) {

            int multiplier = 1;
            int points;
            BonusText bonusText;

            //if the last row destruction bonus has not passed yet
            if (rowDestructionInitialTime + rowDestructionBonusTime > Time.time) {
                multiplier *= 2;
                bonusText = Instantiate(bonusTextPrefab, defaultBonusTextLocation.transform.position, Quaternion.identity).GetComponent<BonusText>();
                bonusText.setSprite(gameManagerScript.getBonusText(2));
            }

            rowDestructionInitialTime = Time.time;

            for (int i = 0; i < r.Count; i++) {
                CandleId can = GameManager.getCandleById(r[i]).getParentObject().GetComponent<CandleId>();
                bonusText = Instantiate(bonusTextPrefab, can.transform.position, Quaternion.identity).GetComponent<BonusText>();

                if (can.isStarterCandle()) {
                    multiplier *= 2;
                    bonusText.setSprite(gameManagerScript.getBonusText(1));
                }
                else {
                    bonusText.setSprite(gameManagerScript.getBonusText(0));
                }

            }

            points = r.Count * multiplier;
            gameManagerScript.addScore(points);

            destroyRow(r);
        }
        
    }


    void destroyRow(List<int> r) {
        for (int i = 0; i < r.Count; i++) {
            GameObject c = GameManager.getCandleById(r[i]).getParentObject();
            GameObject p = Instantiate(CandleDestroyParticle);
            p.transform.position = c.transform.position;
            gameManagerScript.destroyCandle(r[i]);
        }
    }


    List<int> findRow() {
        List<int> result = new List<int>();

        //if the left wall is touching nothing exit early
        if(touching.Count == 0) {
            return result;
        }

        //go through each candle and record the ids of all touching candles
        for(int i = 0; i < touching.Count; i++) {
            if (!result.Contains(touching[i])) {
                result.Add(touching[i]);
                GameManager.getCandleById(touching[i]).traverse(result);
            }
        }

        for (int i = 0; i < result.Count; i++) { 
            //Debug.Log(result[i]);
        }
        //Debug.Log("-------");
        //check if any recorded candles are touching the right wall
        for (int i = 0; i < result.Count; i++) {
            if (otherSideScript.containsId(result[i])) {
                return result;
            }
        }

        return new List<int>();
    }



    void OnTriggerEnter2D(Collider2D collision) {
        CandleIgniter o = collision.gameObject.GetComponent<CandleIgniter>();
        
        if (o != null) {
            CandleLightController other = o.getParentCandleScript();
            //keep track of any candle that comes into contact with this one
            addToList(other);
            
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        CandleIgniter o = collision.gameObject.GetComponent<CandleIgniter>();

        if (o != null) {
            CandleLightController other = o.getParentCandleScript();

            //keep track of any candle that comes into contact with this one
            removeFromList(other);

        }
    }




    //need to keep track of all other candle ids that are touching the wall
    public void addToList(CandleLightController other) {
        //make sure this candle isnt already in the list
        if (findIndex(other.getId()) == -1) {
            touching.Add(other.getId());
            
            //start wall shine if something is touching the wall
            if(touching.Count > 0) {
                leftParticleSystem.Play();
            }
        }
    }

    public void removeFromList(CandleLightController other) {
        //make sure this candle is in the list
        int index = findIndex(other.getId());
        if (index != -1) {
            touching.RemoveAt(index);

            //stop wall shine if something is touching the wall
            if (touching.Count < 1) {
                leftParticleSystem.Stop();
            }
        }
    }


    private int findIndex(int id) {
        for (int i = 0; i < touching.Count; i++) {
            if (touching[i] == id) {
                return i;
            }
        }
        return -1;
    }


    public bool containsId(int id) {
        return findIndex(id) != -1;
    }


}
