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

    public bool active = true;

    public SpriteRenderer defaultSprite;
    public Sprite chirpingSprite;
    virtual protected void Start(){

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

            if (Settings.soundEnabled) 
            {
                int chirpSound = Random.Range(0,3);

                
                if (chirpSound == 0) {

                    audioSourceChirp1.Play();
                    ChangeSprite(chirpingSprite);
                    public float currentChirpTimer = 3.0f
                    chirpAnimation();

                }
                
                else if (chirpSound == 1) {

                    audioSourceChirp2.Play();
                    ChangeSprite(chirpingSprite);
                    public float currentChirpTimer = 3.0f
                    chirpAnimation();
                }
                else if (chirpSound == 2) {

                    audioSourceChirp3.Play();
                    ChangeSprite(chirpingSprite);
                    public float currentChirpTimer = 3.0f
                    chirpAnimation();
                }
            }
        }
    }  


    void chirpAnimation(){

        currentChirpTimer -= Time.deltaTime;
        if (currentChirpTimer <= 0){

            ChangeSprite(defaultSprite);
        }

        else{
            chirpAnimation();
        }

    }
}

