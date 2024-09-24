using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleIgniter : CandleLightCollector
{
    bool isActive = true;
    CandleLightController parentCandle;


    public void setParentCandleScript(CandleLightController p) {
        parentCandle = p;
    }


    public CandleLightController getParentCandleScript() { 
        return parentCandle;
    }


    /*private void OnTriggerEnter2D(Collider2D collision) {
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

    }*/

    protected override void OnTriggerEnter2D(Collider2D collision) {
        base.OnTriggerEnter2D(collision);

        CandleIgniter other = collision.GetComponent<CandleIgniter>();
        
        if (other != null) {

            CandleLightController otherCan = other.getParentCandleScript();
            
            //if this candle is active and the other candle can be ignited, enable its light
            if (otherCan.canIgnite() && isActive) {
                otherCan.enableLight();
            }
            
            //if this candle is active and touching any other candle, enable the backlight
            if (isTouchingAnyCandles() && isActive) {
                parentCandle.enableBackLight();
            }

            //since this is hitting a candleIgniter, we can stop and return at this point
            return;
        }

        CandleLightController can = collision.GetComponent<CandleLightController>();

        //if the candle touching this can ignite and this is active, turn it on
        if (can != null && can.canIgnite() && isActive) {
            can.enableLight();
            //can.enableBackLight();
        }
    }


    protected override void OnTriggerExit2D(Collider2D collision) {
        base.OnTriggerExit2D(collision);

        CandleIgniter other = collision.GetComponent<CandleIgniter>();

        if (other != null) {
            //Debug.Log(isTouchingAnyCandles());
            //printTouchingList();
            if (!isTouchingAnyCandles()) {
                parentCandle.disableBackLight();
            }

        }

    }

    public void setActive(bool x) {
        isActive = x;
    }


    public bool isLightActive() {
        return isActive;
    }

}
