using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleId : MonoBehaviour
{

    //THIS CLASS IS NOT FOR THE UNIQUE ID OF CANDLES
    //IT IS FOR THE ID OF THE PREFAB USED ON THIS CANDLE
    //specifically the id here is the index of the prefab used in GameManager.CanObjects[]
    //this is so that the game can give bonuses for special candles
    [SerializeField] int points = 1;
    [SerializeField] CandleColorGroup colorGroup;
    int id = 0;
    bool starterCandle = false;

    public void setInfo(int id, bool starterCandle) {
        this.id = id;
        this.starterCandle = starterCandle;
    }


    //"starter candle" as in does this candle use the same model as the chosen starting candle?
    public bool isStarterCandle() {
        return starterCandle;
    }

    //id here means the id for the candle model being used, for unique id for this specific
    //candle instance, use getId() in CandleLightController
    public int getId() {
        return id;
    }

    public int getPoints() { 
        return points;
    }

    public int getColorGroup() {
        return (int)colorGroup;
    }

}
