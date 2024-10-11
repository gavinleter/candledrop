using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaffleButton : ButtonPress {


    [SerializeField] Sprite[] waffleSprites;
    [SerializeField] AudioClip[] waffleBiteSounds;

    [SerializeField] AudioClip jimmyCube;

    ParticleSystem[] waffleBiteParticles;

    [SerializeField] private ParticleSystem jimmyRain;
    private AudioSource jimmySourceDown;
    int biteCount = 0;

    protected override void Start() {
        base.Start();

        waffleBiteParticles = GetComponentsInChildren<ParticleSystem>();
        jimmySourceDown =  gameObject.AddComponent<AudioSource>();
        jimmySourceDown.clip = jimmyCube;
        audioSourceDown.volume = 1;
        jimmySourceDown.volume = 0;
    }


    protected override void MouseDown() {
        
        if (biteCount < 3) {
            
            audioSourceDown.clip = waffleBiteSounds[0];
            GetComponent<SpriteRenderer>().sprite = waffleSprites[biteCount];
            waffleBiteParticles[biteCount].Play();
            biteCount++;

        }
        else if(biteCount == 11){

            return;
        }


        else if(biteCount == 10){

            jimmySourceDown.volume = 1;
            jimmySourceDown.Play();
            biteCount++;
            jimmyRain.Play();
            return;
        }
        else{

            audioSourceDown.clip = waffleBiteSounds[3];
            biteCount++;

        }

        base.MouseDown();
    }


}
