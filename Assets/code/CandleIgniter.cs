using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleIgniter : MonoBehaviour
{
    bool isActive = true;
    CandleLightController parentCandle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void setParentCandleScript(CandleLightController p) {
        parentCandle = p;
    }


    public CandleLightController getParentCandleScript() { 
        return parentCandle;
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        CandleLightController other = collision.gameObject.GetComponent<CandleLightController>();

        if (other != null) {

            //if this candle is active and the other candle can be ignited, enable its light
            if(other.canIgnite() && isActive) {
                other.enableLight();
            }

            //exit early if this is a CandleLightController
            return;
        }

        CandleIgniter o = collision.gameObject.GetComponent<CandleIgniter>();

        if (o != null) {

            //keep track of any candle light that comes into contact with this one
            parentCandle.addToList(o.getParentCandleScript());

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
