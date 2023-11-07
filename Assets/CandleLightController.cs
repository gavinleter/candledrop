using UnityEngine;
using System.Collections;

public class CandleLightController : MonoBehaviour {

    private static int instances = 0;
    private int id = -1;

    bool candleEnabled = true;
    GameObject flickerObject;
    GameObject staticFlickerObject;
    int layer;
    //overlaps is used to count how many solid objects the light is colliding with
    //the candle can only reignite if overlaps == 0
    int overlaps = 0;


    void Start(){
        flickerObject = transform.Find("flicker").gameObject;
        staticFlickerObject = flickerObject.transform.Find("better_flicker_0").gameObject;
        layer = LayerMask.NameToLayer("flame");

        id = instances;
        instances++;
    }

    void OnTriggerEnter2D(Collider2D collider){

        enableDisableCandleLight(collider);
        checkForRow(collider);

    }


    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer != layer) {
            overlaps--;
        }
    }


    private void enableDisableCandleLight(Collider2D collider) {
        if (collider.gameObject.layer != layer) {
            overlaps++;
        }
        //disable candle light if it hits something solid
        if (candleEnabled && collider.gameObject.layer != layer && !isParent(collider.gameObject)) {
            Debug.Log(collider.gameObject.name);
            candleEnabled = false;
            flickerObject.GetComponent<SpriteRenderer>().enabled = false;
            staticFlickerObject.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        } //enable candle light if it hits a flame and is not colliding with a wall
        else if (!candleEnabled && collider.gameObject.layer == layer && overlaps == 0) {
            CandleLightController other = collider.gameObject.GetComponent<CandleLightController>();
            if (other != null && other.isEnabled()) {
                candleEnabled = true;
                flickerObject.GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<SpriteRenderer>().enabled = true;
                staticFlickerObject.GetComponent<SpriteRenderer>().enabled = true;
            }

        }
    }


    private void checkForRow(Collider2D collider) {

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
}