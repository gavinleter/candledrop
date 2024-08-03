using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CandleLightController : MonoBehaviour {

    [SerializeField] bool isFlare;
    Color32 flareFlickerColor = new Color32(108, 0, 11, 100);
    Color32 flareBackLightColor = new Color32(108, 0, 11, 150);
    Color32 flareGlowColor = new Color32(0xCA, 0x0C, 0x0E, 0xFF);

    static int instances = 0;
    int id = -1;

    bool candleEnabled = true;
    GameObject flickerObject;
    GameObject staticFlickerObject;
    GameObject parentObject;
    GameObject superGlowObject;
    ParticleSystem flareEmberParticles;
    CandleIgniter candleIgniter;
    int layer;
    //overlaps is used to count how many solid objects the light is colliding with
    //the candle can only reignite if overlaps == 0
    int overlaps = 0;
    //how many candle lights that are enabled this is touching
    int lightOverlaps = 0;

    //keeps track of the id of each candle this candle is touching
    List<int> touching = new List<int>();


    void Awake(){

        parentObject = transform.parent.gameObject;

        flickerObject = transform.Find("flicker").gameObject;
        superGlowObject = transform.Find("super_glow").gameObject;
        staticFlickerObject = transform.Find("back light").gameObject;
        flareEmberParticles = transform.Find("flare ember particles").GetComponent<ParticleSystem>();
        flareEmberParticles.Stop();
        flareEmberParticles.Clear();

        layer = LayerMask.NameToLayer("flame");

        candleIgniter = staticFlickerObject.GetComponent<CandleIgniter>();
        candleIgniter.setParentCandleScript(this);
        disableBackLight();

        if (isFlare) {
            convertToFlare();
        }

    }



    void OnTriggerEnter2D(Collider2D collider){
        
        testCollisionCandleLight(collider);
        //addTouchingCandleToList(collider);

    }


    private void OnTriggerExit2D(Collider2D collision) {
        //nothing should happen if touching a button, ember, the ad spinner lever, etc
        if (colliderContainsScript(collision)) {
            return;
        }

        if (collision.GetComponent<CandleIgniter>() != null) {
            lightOverlaps--;
        }

        if (collision.gameObject.layer != layer && !isParent(collision.gameObject)) {
            overlaps--;
        }
        
        //if there are no more non-flame overlaps and there is currently a candle touching this, reignite
        if(overlaps == 0 && lightOverlaps > 0) {
            enableLight();
            enableBackLight();
        }
    }


    //keep track of any incoming collisions and turn off candle light if it hits something
    private void testCollisionCandleLight(Collider2D collider) {

        //nothing should happen if touching a button, ember, the ad spinner lever, etc
        if(colliderContainsScript(collider)) {
            return;
        }

        if(collider.GetComponent<CandleIgniter>() != null) {
            lightOverlaps++;
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


    private bool colliderContainsScript(Collider2D collider) {
        return (collider.GetComponent<ButtonPress>() != null || collider.GetComponent<AdSpinnerLever>() != null || collider.GetComponent<EmberController>() != null
            || collider.GetComponent<MiniSunExplosion>());
    }


    public void disableLight() {
        //flares cannot be extinguished
        if (!isFlare) {
            candleEnabled = false;
            staticFlickerObject.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            candleIgniter.setActive(false);
            staticFlickerObject.GetComponent<CircleCollider2D>().enabled = false;
            disableBackLight();
        }
    }


    public void enableLight() {
        staticFlickerObject.GetComponent<CircleCollider2D>().enabled = true;
        candleEnabled = true;
        staticFlickerObject.GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        candleIgniter.setActive(true);
    }

    void enableBackLight() {
        flickerObject.GetComponent<SpriteRenderer>().enabled = true;
        superGlowObject.GetComponent<ParticleSystem>().Play();
    }

    void disableBackLight() {
        flickerObject.GetComponent<SpriteRenderer>().enabled = false;
        superGlowObject.GetComponent<ParticleSystem>().Stop();
        superGlowObject.GetComponent<ParticleSystem>().Clear();
    }


    public void convertToFlare() {
        isFlare = true;

        enableLight();

        staticFlickerObject.GetComponent<SpriteRenderer>().color = flareBackLightColor;
        flickerObject.GetComponent<SpriteRenderer>().color = flareFlickerColor;

        //change color of super glow
        ParticleSystem.MainModule x = superGlowObject.GetComponent<ParticleSystem>().main;
        x.startColor = new ParticleSystem.MinMaxGradient(flareGlowColor);
        flareEmberParticles.Play();

        //make the candle fire invisible
        Color temp = GetComponent<SpriteRenderer>().color;
        temp.a = 0;
        GetComponent<SpriteRenderer>().color = temp;

        transform.localScale = new Vector3(0.75f, 0.75f, 1);
    }


    //need to keep track of all other candle ids that are touching this one
    public void addToList(CandleLightController other) {
        
        //if this candle has an invalid id (in the case of candles just out in the open not being used in game)
        if (other.getId() == -1) {
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
        return parentObject == obj;
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

    public bool isCurrentlyFlare() {
        return isFlare;
    }

}