using UnityEngine;
using System.Collections;

public class CandleLightController : MonoBehaviour
{
    bool candleEnabled = true;
    GameObject flickerObject;
    int layer;

    void Start()
    {
        flickerObject = transform.Find("flicker").gameObject;
        layer = LayerMask.NameToLayer("flame");
    }

    void OnTriggerEnter2D(Collider2D collider){
        
        if(candleEnabled && collider.gameObject.layer != layer && !isParent(collider.gameObject)) {
            candleEnabled = false;
            flickerObject.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else if(!candleEnabled && collider.gameObject.layer == layer) {
            CandleLightController other = collider.gameObject.GetComponent<CandleLightController>();
            //Debug.Log(collider.gameObject.name);
            if(other != null & other.isEnabled()) {
                candleEnabled = true;
                flickerObject.GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }

    }


    public bool isEnabled() {
        return candleEnabled;
    }

    //make sure the candle flame cant be extinguished by its own candle base
    public bool isParent(GameObject obj) {
        return transform.parent.parent.gameObject == obj;
    }
}