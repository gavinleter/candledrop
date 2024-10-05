using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleIgniter : CandleLightCollector
{
    bool isActive = true;
    CandleLightController parentCandle;


    private void FixedUpdate() {

        updateTouchingList();

        CandleIgniter otherLight;
        CandleLightController otherLightController;

        bool isTouchingCandles = isTouchingAnyCandles();

        for (int i = 0; i < touchingLength; i++) {
            otherLight = touching[i].GetComponent<CandleIgniter>();
            otherLightController = touching[i].GetComponent<CandleLightController>();

            //if the currently touched object is a CandleIgniter
            if (otherLight && otherLight != this) {

                if (isTouchingCandles && isActive) {
                    parentCandle.enableBackLight();
                }

                //continue since we know at this point this object is not a CandleLightController
                continue;
            }

            //if the currently touched object is a CandleLightController
            if (otherLightController && otherLightController.getCandleIgniter() != this) {

                //if this candle is active and the other can ignite, turn on its light
                if (isActive && otherLightController.canIgnite()) {
                    otherLightController.enableLight();
                }

            }

        }

        if(!isTouchingCandles && isActive) {
            parentCandle.disableBackLight();
        }

    }


    public void setParentCandleScript(CandleLightController p) {
        parentCandle = p;
    }


    public CandleLightController getParentCandleScript() { 
        return parentCandle;
    }



    public void setActive(bool x) {
        isActive = x;
    }


    public bool isLightActive() {
        return isActive;
    }


    public void printCollisionsList() {
        updateTouchingList();
        Debug.Log("Origin: " + getParentCandleScript().getName());

        for (int i = 0; i < touchingLength; i++) {
            CandleIgniter c = touching[i].GetComponent<CandleIgniter>();

            if (c != null) {
                Debug.Log(c.getParentCandleScript().getName());
            }
            else {
                Debug.Log(touching[i].name);
            }


        }

    }

}
