using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour, ISpecialObject
{
    [SerializeField] GameObject holeExplodePrefab;
    [SerializeField] AudioClip holeExplode;
    [SerializeField] float destructionRadius;

    CircleCollider2D coll;
    ContactFilter2D contactFilter;
    GameManager gameManagerScript;
    int id = 0;


    void Awake() {
        coll = GetComponent<CircleCollider2D>();
        contactFilter = new ContactFilter2D().NoFilter();
    }


    public void setup(GameManager g, int id) {
        gameManagerScript = g;
        this.id = id;

    }

    private void OnCollisionEnter2D(Collision2D other){

        //create empty gameobject to temporaily hold black hole explosion audiosource
        //this sound should not play when colliding with a black hole or sun
        if (Settings.isSoundEnabled() && other.transform.GetComponent<ISpecialObject>() == null) {

            GameObject tempAudio = new GameObject("BlackHoleExplosionTempAudioSource");
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();

            tempAudio.transform.position = transform.position;

            tempSource.clip = holeExplode;
            tempSource.pitch = Random.Range(1.5f, 2.5f);
            tempSource.volume = 0.5f;
            tempSource.Play();

            Destroy(tempAudio, holeExplode.length);

        }


        //CandleLightController o = other.gameObject.GetComponentInChildren<CandleLightController>();

        //destroyCandle(o);

        //destroy self only when touching something that is not another black hole
        if (other.gameObject.GetComponentInChildren<BlackHole>() == null) {
            destroySelf();
        }

    }


    public void destroySelf() {

        int totalPoints = 0;

        coll.radius = destructionRadius;
        List<Collider2D> hits = new List<Collider2D>(); 
        int hitCount = coll.OverlapCollider(contactFilter, hits);

        for(int i = 0; i < hitCount; i++) {

            CandleLightController o = hits[i].gameObject.GetComponentInChildren<CandleLightController>();

            totalPoints += destroyCandle(o);

        }
        

        GameObject holeExplode = Instantiate(holeExplodePrefab, transform.position, Quaternion.identity);
        Destroy(holeExplode, 2f);
        gameManagerScript.removeSpecialObject(id);
        Destroy(gameObject);

        //"Void-maxxer" unlocked by getting 10 or more points from a single black hole
        if(totalPoints >= 10) {
            Settings.setAchievementUnlocked(38);
        }

    }


    public int getId() {
        return id;
    }


    int destroyCandle(CandleLightController o) {

        if(o != null && !o.isBeingDestroyed()) {

            if (o.isEnabled()) {
                //"Friendly Fire!" unlocked by destroying a lit candle
                Settings.setAchievementUnlocked(36);
            }
            else {
                //"Waxicide" unlocked by destroying an unlit candle
                Settings.setAchievementUnlocked(35);
            }
            

            CandleId can = o.getParentObject().GetComponent<CandleId>();
            gameManagerScript.createMultiplierlessBonusText(can, 0);
            gameManagerScript.addScore(o.getPoints());

            gameManagerScript.destroyCandle(o.getParentObject(), true);

            return o.getPoints();
        }

        return 0;
    }
}
