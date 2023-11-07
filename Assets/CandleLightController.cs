using UnityEngine;
using System.Collections;

public class CandleLightController : MonoBehaviour {

    private static int instances = 0;
    private int id = -1;

    bool candleEnabled = true;
    GameObject flickerObject;
    GameObject staticFlickerObject;
    CandleIgniter candleIgniter;
    int layer;
    //overlaps is used to count how many solid objects the light is colliding with
    //the candle can only reignite if overlaps == 0
    int overlaps = 0;


    void Start(){
        flickerObject = transform.Find("flicker").gameObject;
        staticFlickerObject = flickerObject.transform.Find("better_flicker_0").gameObject;
        layer = LayerMask.NameToLayer("flame");
        candleIgniter = staticFlickerObject.GetComponent<CandleIgniter>();

        id = instances;
        instances++;
    }

    void OnTriggerEnter2D(Collider2D collider){

        enableDisableCandleLight(collider);
        checkForRow(collider);

    }


    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer != layer && !isParent(collision.gameObject)) {
            overlaps--;
        }
    }


    private void enableDisableCandleLight(Collider2D collider) {
        if (collider.gameObject.layer != layer && !isParent(collider.gameObject)) {
            overlaps++;
        }
        //disable candle light if it hits something solid
        //candles are re-enabled inside of CandleIgniter, attached to the static flicker child object
        if (candleEnabled && collider.gameObject.layer != layer && !isParent(collider.gameObject)) {
            disableLight();
        }
    }


    private void checkForRow(Collider2D collider) {

    }


    public void disableLight() {
        candleEnabled = false;
        flickerObject.GetComponent<SpriteRenderer>().enabled = false;
        staticFlickerObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        candleIgniter.setActive(false);
    }


    public void enableLight() {
        candleEnabled = true;
        flickerObject.GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        staticFlickerObject.GetComponent<SpriteRenderer>().enabled = true;
        candleIgniter.setActive(true);
    }


    public bool isEnabled() {
        return candleEnabled;
    }

    //make sure the candle flame cant be extinguished by its own candle base
    public bool isParent(GameObject obj) {
        return transform.parent.parent.gameObject == obj;
    }

    public int getId() {
        return id;
    }

    public bool canIgnite() {
        return overlaps == 0 && !candleEnabled;
    }

    public int ove() {
        return overlaps;
    }
}