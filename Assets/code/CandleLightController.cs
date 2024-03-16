using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CandleLightController : MonoBehaviour {

    private static int instances = 0;
    private int id = -1;

    bool candleEnabled = true;
    GameObject flickerObject;
    GameObject staticFlickerObject;
    GameObject parentObject;
    GameObject superGlowObject;
    CandleIgniter candleIgniter;
    int layer;
    //overlaps is used to count how many solid objects the light is colliding with
    //the candle can only reignite if overlaps == 0
    int overlaps = 0;

    //keeps track of the id of each candle this candle is touching
    private List<int> touching = new List<int>();


    void Awake(){
        parentObject = transform.parent.parent.gameObject;

        flickerObject = transform.Find("flicker").gameObject;
        superGlowObject = transform.Find("super_glow").gameObject;
        staticFlickerObject = flickerObject.transform.Find("better_flicker_0").gameObject;
        layer = LayerMask.NameToLayer("flame");

        candleIgniter = staticFlickerObject.GetComponent<CandleIgniter>();
        candleIgniter.setParentCandleScript(this);
        disableBackLight();

    }


    void Start() {
        
    }


    private void Update() {
        //Debug.Log(transform.parent.parent.name + " " + touching.Count);
    }


    void OnTriggerEnter2D(Collider2D collider){

        testCollisionCandleLight(collider);
        //addTouchingCandleToList(collider);

    }


    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer != layer && !isParent(collision.gameObject)) {
            overlaps--;
        }
        //removeTouchingCandleFromList(collision);
    }


    //keep track of any incoming collisions and turn off candle light if it hits something
    private void testCollisionCandleLight(Collider2D collider) {

        //nothing should happen if touching a button
        if(collider.GetComponent<ButtonPress>() != null) {
            return;
        }

        //make sure what is colliding is not the parent of the candle
        if (collider.gameObject.layer != layer && !isParent(collider.gameObject)) {
            overlaps++;
        }
        //disable candle light if it hits something solid
        //candles are re-enabled inside of CandleIgniter, attached to the static flicker child object
        if (candleEnabled && collider.gameObject.layer != layer && !isParent(collider.gameObject) && collider.GetComponent<CandleIgniter>() == null) {
            disableLight();
        }
    }


    public void disableLight() {
        candleEnabled = false;
        flickerObject.GetComponent<SpriteRenderer>().enabled = false;
        staticFlickerObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        candleIgniter.setActive(false);
        staticFlickerObject.GetComponent<CircleCollider2D>().enabled = false;
    }


    public void enableLight() {
        staticFlickerObject.GetComponent<CircleCollider2D>().enabled = true;
        candleEnabled = true;
        flickerObject.GetComponent<SpriteRenderer>().enabled = true;
        staticFlickerObject.GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        candleIgniter.setActive(true);
    }

    public void enableBackLight() {
        flickerObject.GetComponent<SpriteRenderer>().enabled = true;
        superGlowObject.GetComponent<ParticleSystem>().Play();
    }

    public void disableBackLight() {
        flickerObject.GetComponent<SpriteRenderer>().enabled = false;
        superGlowObject.GetComponent<ParticleSystem>().Stop();
    }


    //need to keep track of all other candle ids that are touching this one
    public void addToList(CandleLightController other) {

        //if this candle has an invalid id (in the case of candles just out in the open not being used in game)
        if (other.getId() == -1) {
            Debug.Log(other.transform.parent.parent.name + " " + other.getId() + " " + transform.parent.parent.name);
            return;
        }

        //make sure this candle isnt already in the list
        if (findIndex(other.getId()) == -1) {
            touching.Add(other.getId());
        }

        if (candleEnabled) {
            enableBackLight();
        }
    }

    public void removeFromList(CandleLightController other) {
        //make sure this candle is in the list
        int index = findIndex(other.getId());
        if (index != -1) {
            touching.RemoveAt(index);
        }

        if(touching.Count == 0) {
            disableBackLight();
        }
    }


    //go through every candle that is touching this one and is ignited
    //if the candle being looked at has not been visited, repeat the process for it
    public void traverse(List<int> visited) {
        for (int i = 0; i < touching.Count; i++) {
            if (!visited.Contains(touching[i])) {
                CandleLightController c = GameManager.getCandleById(touching[i]);
                if (c.isEnabled()) {
                    visited.Add(touching[i]);
                    c.traverse(visited);
                }
            }
        }

    }


    private bool containsId(List<int> visited, int id) {
        for (int i = 0; i < visited.Count; i++) {
            if (visited[i] == id) {
                return true;
            }
        }
        return false;
    }


    private int findIndex(int id) {
        for (int i = 0; i < touching.Count; i++) {
            if (touching[i] == id) {
                return i;
            }
        }
        return -1;
    }


    public int assignId() {
        id = instances;
        instances++;
        return id;
    }


    public bool isEnabled() {
        return candleEnabled;
    }

    //make sure the candle flame cant be extinguished by its own candle base
    //this is for checking if a top level candle gameObject is the same as this object's top level candle gameObject
    public bool isParent(GameObject obj) {
        return transform.parent.parent.gameObject == obj;
    }

    public int getId() {
        return id;
    }

    public bool canIgnite() {
        return overlaps == 0 && !candleEnabled;
    }

    public GameObject getParentObject() {
        return parentObject;
    }

    public static void reset() {
        instances = 0;
    }

    public void destroySelf() {
        
    }

}