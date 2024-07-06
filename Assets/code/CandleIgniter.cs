using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleIgniter : MonoBehaviour
{
    bool isActive = true;
    CandleLightController parentCandle;


    public void setParentCandleScript(CandleLightController p) {
        parentCandle = p;
    }


    public CandleLightController getParentCandleScript() { 
        return parentCandle;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        CandleLightController other = collision.gameObject.GetComponent<CandleLightController>();

        CandleIgniter o = collision.gameObject.GetComponent<CandleIgniter>();
        
        if (o != null) {
            
            //keep track of any candle light that comes into contact with this one
            parentCandle.addToList(o.getParentCandleScript());

            //this object is a CandleIgniter, so we don't check if its a CandleLightController. Exit early
            return;
        }

        if (other != null) {

            //if this candle is active and the other candle can be ignited, enable its light
            if(other.canIgnite() && isActive) {
                other.enableLight();
            }

        }

    }


    private void OnTriggerExit2D(Collider2D collision) {

        CandleIgniter o = collision.gameObject.GetComponent<CandleIgniter>();

        if (o != null) {

            //keep track of any candle light that comes into contact with this one
            parentCandle.removeFromList(o.getParentCandleScript());

        }

    }

    public void setActive(bool x) {
        isActive = x;
    }
}
