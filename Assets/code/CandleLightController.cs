using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CandleLightController : MonoBehaviour {

    [SerializeField] bool isFlare;
    Color32 flareFlickerColor = new Color32(108, 0, 11, 100);
    Color32 flareBackLightColor = new Color32(108, 0, 11, 150);
    Color32 flareGlowColor = new Color32(0xCA, 0x0C, 0x0E, 0xFF);

    int id = -1;
    //static int instances = 0;

    bool candleEnabled = true;
    GameObject flickerObject;
    GameObject staticFlickerObject;
    GameObject parentObject;
    GameObject superGlowObject;
    ParticleSystem flareEmberParticles;
    CandleIgniter candleIgniter;
    int layer;
    //overlaps is used to count how many solid non-light objects this light is colliding with
    //the candle can only reignite if overlaps.Count == 0 and all null objects are removed
    List<Collider2D> overlaps = new List<Collider2D>();


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

        /*id = instances;
        instances++;
        getParentObject().name += " | ID: " + id;
        name += " | ID: " + id;*/

    }


    //If a candle is extinguished due to another candle sitting on top of it while it is also in
    //range of a light, it will potentially not re-ignite because OnTriggerExit2D is not called
    //if the collider of the top candle is deleted. Hence this FixedUpdate code.
    /*private void FixedUpdate() {

        if (canIgnite()) {
            enableLight();

            if (lightOverlaps > 0) {
                enableBackLight();
            }
        }

    }*/



    protected void OnTriggerEnter2D(Collider2D collider){

        testCollisionCandleLight(collider);
        //addTouchingCandleToList(collider);

    }


    protected void OnTriggerExit2D(Collider2D collision) {

        //nothing should happen if touching a button, ember, the ad spinner lever, etc
        if (colliderContainsScript(collision)) {
            return;
        }

        if (collision.gameObject.layer != layer && !isParent(collision.gameObject)) {
            overlaps.Remove(collision);
        }

        overlaps.RemoveAll((Collider2D coll) => { return coll == null; });
        
        //if there are no more non-flame overlaps and there is currently a candle light touching this, reignite
        if(overlaps.Count == 0 && candleIgniter.isTouchingAnyCandles()) {
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

        //make sure what is colliding is not the parent of the candle
        if (collider.gameObject.layer != layer && !isParent(collider.gameObject)) {
            if (!overlaps.Contains(collider)) {
                overlaps.Add(collider);
            }
        }
        //disable candle light if it hits something solid
        //candles are re-enabled inside of CandleIgniter, attached to the static flicker child object
        if (candleEnabled && collider.gameObject.layer != layer && !isParent(collider.gameObject) && collider.GetComponent<CandleIgniter>() == null) {
            disableLight();
        }

    }


    private bool colliderContainsScript(Collider2D collider) {
        return (collider.GetComponent<ButtonPress>() != null || collider.GetComponent<AdSpinnerLever>() != null || collider.GetComponent<EmberController>() != null
            || collider.GetComponent<MiniSunExplosion>() || collider.GetComponent<GameOverChain>());
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

    public void enableBackLight() {
        flickerObject.GetComponent<SpriteRenderer>().enabled = true;
        superGlowObject.GetComponent<ParticleSystem>().Play();
    }

    public void disableBackLight() {
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

    //DELeTE ME
    //need to keep track of all other candle ids that are touching this one
    /*public override void addToList(CandleLightController other) {

        //if this candle has an invalid id do nothing (in the case of candles just out in the open not being used in game)
        if (other.getId() != -1) {

            base.addToList(other);

            if (candleEnabled) {
                enableBackLight();
            }
        }

    }*/

    //DELETE ME
    /*public override void removeFromList(CandleLightController other) {
        
        base.removeFromList(other);

        if(isTouchingAnyCandles()) {
            disableBackLight();
        }
    }*/


    //DELETE ME
    //removes this candle light from the touching list of any candle light also touching this
    /*public void removeSelfFromOthers() {

        for (int i = 0; i < touching.Count; i++) {
            CandleLightController c = GameManager.getCandleById(touching[i]);
            if(c == null) {
                Debug.Log(touching[i]);
            }
            c.removeFromList(this);
        }

    }*/


    //go through every candle that is touching this one and is ignited
    //if the candle being looked at has not been visited, repeat the process for it
    public void traverse(HashSet<CandleLightController> visited) {

        foreach (CandleLightController can in candleIgniter.touching) {

            //do the recursive call if the touching candle is not null, not already visited in a previous call, and is enabled
            if (can != null && !visited.Contains(can) && can.isEnabled()) {

                visited.Add(can);
                can.traverse(visited);
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


    //DELETE ME
    /*private int findIndex(int id) {
        for (int i = 0; i < touching.Count; i++) {
            if (touching[i] == id) {
                return i;
            }
        }
        return -1;
    }*/


    public void assignId(int id) {
        this.id = id;
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
        overlaps.RemoveAll((Collider2D coll) => { return coll == null; });
        return overlaps.Count == 0 && !candleEnabled;
    }

    public GameObject getParentObject() {
        return parentObject;
    }

    public bool isCurrentlyFlare() {
        return isFlare;
    }

    //this is for debug purposes and should be deleted later
    public CandleIgniter getCandleIgniter() { 
        return candleIgniter;
    }

    public string getName() {
        return getParentObject().name + " | " + getId();
    }

}