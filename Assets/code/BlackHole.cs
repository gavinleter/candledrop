using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour, ISpecialObject
{
    [SerializeField] GameObject holeExplodePrefab;

    //next two lines are gavin code for sound
    [SerializeField] private AudioClip holeExplode;
    GameManager gameManagerScript;
    int id = 0;

    public void setup(GameManager g, int id) {
        gameManagerScript = g;
        this.id = id;

    }

    private void OnCollisionEnter2D(Collision2D other){
        
        //next eight lines are gavin code for sound
        //all below steps ensures a sound can still be produced from a game object that has been destroyed.
        GameObject tempAudio = new GameObject("TempAudioSource");   //create extra instance of gameObject to hold temporary source temporarily
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>(); //create temporary source to be held
        tempSource.clip = holeExplode;  //set clip
        tempSource.pitch = Random.Range(1.5f, 2.5f);  //randomizes pitch of sound
        tempSource.volume = 0.5f;
        tempSource.Play();  //plays sound
        DontDestroyOnLoad(tempAudio);  // Prevents destruction when transitioning scenes or objects
        Destroy(tempAudio, holeExplode.length);  // Destroy the audio object after the sound finishes


        CandleLightController o = other.gameObject.GetComponentInChildren<CandleLightController>();

        //check if the colliding object is a candle
        if (o != null){
            CandleId can = o.getParentObject().GetComponent<CandleId>();
            gameManagerScript.createMultiplierlessBonusText(can, 0);
            gameManagerScript.addScore(1);

            gameManagerScript.destroyCandle(o.getParentObject(), true);

        }

        //destroy self only when touching something that is not another black hole
        if (other.gameObject.GetComponentInChildren<BlackHole>() == null) {
            destroySelf();
        }

    }


    public void destroySelf() {
        
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, gameManagerScript.blackHoleDestroyRadius, Vector2.one);

        for (int i = 0; i < hits.Length; i++) {
            CandleLightController o = hits[i].collider.gameObject.GetComponentInChildren<CandleLightController>();
            if (o != null) {

                gameManagerScript.destroyCandle(o.getParentObject(), true);
            }
        }

        GameObject holeExplode = Instantiate(holeExplodePrefab, transform.position, Quaternion.identity);
        Destroy(holeExplode, 2f); // Destroy the explosion effect after 2 seconds
        gameManagerScript.removeSpecialObject(id);
        Destroy(gameObject);
    }


    public int getId() {
        return id;
    }
}
