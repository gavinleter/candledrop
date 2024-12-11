using UnityEngine;
using System.Collections.Generic;

public class CandleLightController : MonoBehaviour {

    [SerializeField] bool isFlare;
    Color32 flareFlickerColor = new Color32(108, 0, 11, 100);
    Color32 flareBackLightColor = new Color32(108, 0, 11, 150);
    Color32 flareGlowColor = new Color32(0xCA, 0x0C, 0x0E, 0xFF);

    int id = -1;
    //static int instances = 0;

    bool candleEnabled = true;
    bool beingDestroyed = false;

    float miniSunIgnitionTime = 0;
    float miniSunIgnitionDuration = 3;

    GameObject flickerObject;
    GameObject staticFlickerObject;
    GameObject parentObject;
    GameObject superGlowObject;
    GameObject flareOutline;
    SpriteRenderer miniSunLight;
    ParticleSystem flareEmberParticles;
    CandleIgniter candleIgniter;
    Collider2D igniterCollider;
    Collider2D wickCollider;
    CandleId candleId;
    int layer;
    ContactFilter2D contactFilter;
    //overlaps is used to count how many solid non-light objects this light is colliding with
    //the candle can only reignite if overlaps == 0
    int overlaps = 0;
    List<Collider2D> collisions = new List<Collider2D>();
    List<Collider2D> igniterCollisions = new List<Collider2D>();


    void Awake(){
        
        parentObject = transform.parent.gameObject;

        flickerObject = transform.Find("flicker").gameObject;
        superGlowObject = transform.Find("super_glow").gameObject;
        staticFlickerObject = transform.Find("back light").gameObject;
        flareOutline = transform.Find("flare outline").gameObject;
        flareEmberParticles = transform.Find("flare ember particles").GetComponent<ParticleSystem>();
        flareEmberParticles.Stop();
        flareEmberParticles.Clear();
        miniSunLight = transform.Find("Mini sun light").GetComponent<SpriteRenderer>();
        miniSunLight.enabled = false;
        candleId = parentObject.GetComponent<CandleId>();

        wickCollider = GetComponent<Collider2D>();

        layer = LayerMask.NameToLayer("flame");

        candleIgniter = staticFlickerObject.GetComponent<CandleIgniter>();
        candleIgniter.setParentCandleScript(this);
        igniterCollider = candleIgniter.GetComponent<Collider2D>();
        disableBackLight();

        contactFilter = new ContactFilter2D().NoFilter();

        if (isFlare) {
            convertToFlare();
        }

        /*id = instances;
        instances++;
        getParentObject().name += " | ID: " + id;
        name += " | ID: " + id;*/

    }


    private void OnTriggerStay2D(Collider2D collision) {

        updateCollisionList();

        if(overlaps > 0) {
            disableLight();
        }

    }



    private bool colliderContainsScript(Collider2D collider) {
        return (collider.GetComponent<ButtonPress>() != null || collider.GetComponent<AdSpinnerLever>() != null || collider.GetComponent<EmberController>() != null
            || collider.GetComponent<MiniSunExplosion>() || collider.GetComponent<GameOverChain>());
    }


    public int updateCollisionList() {
        int x = wickCollider.OverlapCollider(contactFilter, collisions);
        overlaps = 0;

        for (int i = 0; i < x; i++) {
            //if the colliding object is not a flame, not the base of the candle, and is not a button, then this candle is touching something solid and should be put out
            if (collisions[i].gameObject.layer != layer && !isParent(collisions[i].gameObject) && !colliderContainsScript(collisions[i]) ) {
                overlaps++;
                //Debug.Log("Colliding with: " + collisions[i].name);
            }
        }

        if (!isMiniSunIgnited()) {
            miniSunDeignite();
        }

        return x;
    }


    public void disableLight() {
        disableLight(false);
    }


    public void disableLight(bool forceDisable) {
        //flares and candles ignited by mini suns cannot be extinguished normally
        if ((!isCurrentlyFlare() && !isMiniSunIgnited()) || forceDisable) {
            candleEnabled = false;
            staticFlickerObject.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            candleIgniter.setActive(false);
            staticFlickerObject.GetComponent<CircleCollider2D>().enabled = false;
            disableBackLight();

            if (isCurrentlyFlare()) {
                flareOutline.GetComponent<SpriteRenderer>().enabled = false;
                flareEmberParticles.Stop();
                flareEmberParticles.Clear();
            }

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
        if (!superGlowObject.GetComponent<ParticleSystem>().isPlaying) {
            superGlowObject.GetComponent<ParticleSystem>().Play();
        }
    }

    public void disableBackLight() {
        flickerObject.GetComponent<SpriteRenderer>().enabled = false;
        superGlowObject.GetComponent<ParticleSystem>().Stop();
        superGlowObject.GetComponent<ParticleSystem>().Clear();
    }


    //mini sun explosions force non-flare candles to stay on for a short time
    public void miniSunIgnite() {

        if (!isCurrentlyFlare()) {
            miniSunIgnitionTime = Time.time;
            enableLight();
            miniSunLight.enabled = true;
        }

    }


    void miniSunDeignite() {
        miniSunLight.enabled = false;
    }


    public bool isMiniSunIgnited() {
        return Time.time < miniSunIgnitionTime + miniSunIgnitionDuration;
    }


    public void convertToFlare() {
        isFlare = true;

        enableLight();

        staticFlickerObject.GetComponent<SpriteRenderer>().color = flareBackLightColor;
        flickerObject.GetComponent<SpriteRenderer>().color = flareFlickerColor;
        flareOutline.GetComponent<SpriteRenderer>().enabled =  true;

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


    //go through every candle that is touching this one and is ignited
    //if the candle being looked at has not been visited, repeat the process for it
    public void traverse(HashSet<CandleLightController> visited) {
        
        CandleLightController can;
        CandleIgniter otherCanIgniter;

        int touchingCount = candleIgniter.updateTouchingList();


        for (int i = 0; i < touchingCount; i++) {
            otherCanIgniter = candleIgniter.touching[i].GetComponent<CandleIgniter>();
            
            if (otherCanIgniter != null) {
                can = otherCanIgniter.getParentCandleScript();
                
                if (can != null && can.isEnabled() && !visited.Contains(can)) { 
                    visited.Add(can);
                    can.traverse(visited);
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
        return overlaps == 0 && !candleEnabled && !beingDestroyed;

    }

    public GameObject getParentObject() {
        return parentObject;
    }

    public bool isCurrentlyFlare() {
        return isFlare;
    }

    public CandleIgniter getCandleIgniter() { 
        return candleIgniter;
    }

    public string getName() {
        return getParentObject().name + " | " + getId();
    }


    public void printCollisionsList() {
        /*updateCollisionList();
        Debug.Log("Origin: " + getName());

        for (int i = 0; i < collisions.Count; i++) {
            CandleIgniter c = collisions[i].GetComponent<CandleIgniter>();

            if (c != null) {
                Debug.Log(c.getParentCandleScript().getName());
            }
            else {
                Debug.Log(collisions[i].name);
            }

            
        }*/
        candleIgniter.printCollisionsList();

    }


    //this is called when black holes destroy this candle on a delay which is why the light is disabled
    //but also when the candle is being destroyed normally because Destroy() does so on the next Update() loop
    public void startDestroy() {
        beingDestroyed = true;
        disableLight(true);
    }


    public bool isBeingDestroyed() {
        return beingDestroyed;
    }


    //how many points this candle should give for destroying it
    public int getPoints() {
        return candleId.getPoints();
    }


    public CandleId getCandleId() {
        return candleId;
    }


    //used when loading candles from a save
    public void setLightStatusById(int x) {
        
        switch (x) {

            case 0:
                disableLight(true);
                break;

            case 1:
                enableLight();
                break;

            case 2:
                convertToFlare();
                break;

            case 3:
                miniSunIgnite();
                break;

        }

    }


}