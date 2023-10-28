using UnityEngine;
using System.Collections;

public class CandleLightController : MonoBehaviour
{
    bool candleEnabled = true;
    GameObject flickerObject;
    GameObject staticFlickerObject;
    int layer;
    int overlaps = 0;

    void Start(){
        flickerObject = transform.Find("flicker").gameObject;
        staticFlickerObject = flickerObject.transform.Find("better_flicker_0").gameObject;
        layer = LayerMask.NameToLayer("flame");
    }

    void OnTriggerEnter2D(Collider2D collider){

        if(collider.gameObject.layer != layer) {
            overlaps++;
        }
        
        if(candleEnabled && collider.gameObject.layer != layer && !isParent(collider.gameObject)) {
            candleEnabled = false;
            flickerObject.GetComponent<SpriteRenderer>().enabled = false;
            staticFlickerObject.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else if(!candleEnabled && collider.gameObject.layer == layer && overlaps == 0) {
            CandleLightController other = collider.gameObject.GetComponent<CandleLightController>();
            //Debug.Log(collider.gameObject.name);
            if(other != null & other.isEnabled()) {
                candleEnabled = true;
                flickerObject.GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<SpriteRenderer>().enabled = true;
                staticFlickerObject.GetComponent<SpriteRenderer>().enabled = true;
            }
        }

    }


    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer != layer) {
            overlaps--;
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