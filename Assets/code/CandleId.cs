using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleId : MonoBehaviour
{

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
    
}
