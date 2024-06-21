using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareLightController : CandleLightController
{


    override protected void Awake() {
        parentObject = transform.parent.gameObject;

        flickerObject = transform.Find("flicker").gameObject;
        superGlowObject = transform.Find("super glow").gameObject;
        staticFlickerObject = gameObject;
        layer = LayerMask.NameToLayer("flame");

        candleIgniter = GetComponent<CandleIgniter>();
        candleIgniter.setParentCandleScript(this);
        disableBackLight();

    }

    //flares should never be extinguished
    public override void disableLight() {
        return;
    }


}
