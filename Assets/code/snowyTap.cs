using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowyTap : MonoBehaviour
{

    [SerializeField] AudioClip chirp3;

    AudioSource audioSourceChirp3;


    private List<System.Action> actions;
    private bool active = true;


    public Sprite defaultSnowySprite;
    public Sprite chirpingSprite;

    public Animator snowyAnim;
    virtual protected void Start(){

        snowyAnim = GetComponent<Animator>();

        audioSourceChirp3 = gameObject.AddComponent<AudioSource>();

        audioSourceChirp3.clip = chirp3;

        actions = new List<System.Action>();
    }

    virtual protected void OnMouseDown(){
        if (active) {

            snowyAnim.Play("snowy_chirp");
            StartCoroutine(MyCoroutine());

            if (Settings.soundEnabled) 
            {
                //int chirpSound = Random.Range(0,3); this line shows how to generate a random number, this is 0 - 2.
                audioSourceChirp3.Play();
                Debug.Log("Should chirp");
            }
        }
    }

    IEnumerator MyCoroutine(){

        yield return new WaitForSeconds(0.8f);
        snowyAnim.Play("snowy_idle");

    }
}