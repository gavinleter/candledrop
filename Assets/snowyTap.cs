using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowyTap : MonoBehaviour
{

    [SerializeField] AudioClip chirp1;
    [SerializeField] AudioClip chirp2;
    [SerializeField] AudioClip chirp3;


    AudioSource audioSourceChirp1;
    AudioSource audioSourceChirp2;
    AudioSource audioSourceChirp3;


    private List<System.Action> actions;
    private bool active = true;


    public Sprite defaultSnowySprite;
    public Sprite chirpingSprite;

    public Animator snowyAnim;
    virtual protected void Start(){

        snowyAnim = GetComponent<Animator>();

        audioSourceChirp1 = gameObject.AddComponent<AudioSource>();
        audioSourceChirp2 = gameObject.AddComponent<AudioSource>();
        audioSourceChirp3 = gameObject.AddComponent<AudioSource>();

        audioSourceChirp1.clip = chirp1;
        audioSourceChirp2.clip = chirp2;
        audioSourceChirp3.clip = chirp3;

        actions = new List<System.Action>();
    }

    virtual protected void OnMouseDown(){
        if (active) {

            snowyAnim.Play("snowy_chirp");
            StartCoroutine(MyCoroutine());

            if (Settings.soundEnabled) 
            {
                int chirpSound = Random.Range(0,3);

                
                if (chirpSound == 0) {

                    audioSourceChirp1.Play();
                    Debug.Log("Should chirp");

                }
                
                else if (chirpSound == 1) {

                    audioSourceChirp2.Play();
                    Debug.Log("Should chirp");
                }
                else if (chirpSound == 2) {

                    audioSourceChirp3.Play();
                    Debug.Log("Should chirp");
                }
            }
        }
    }

    IEnumerator MyCoroutine(){

        yield return new WaitForSeconds(0.8f);
        snowyAnim.Play("snowy_idle");

    }
}