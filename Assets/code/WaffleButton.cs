using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaffleButton : ButtonPress {


    [SerializeField] Sprite[] waffleSprites;
    [SerializeField] AudioClip[] waffleBiteSounds;

    [SerializeField] AudioClip jimmySound;

    ParticleSystem[] waffleBiteParticles;

    [SerializeField] ParticleSystem jimmyRain;
    AudioSource jimmyAudioSource;
    int biteCount = 0;

    protected override void Start() {
        base.Start();

        waffleBiteParticles = GetComponentsInChildren<ParticleSystem>();

        jimmyAudioSource =  gameObject.AddComponent<AudioSource>();
        jimmyAudioSource.clip = jimmySound;
        jimmyAudioSource.playOnAwake = false;

    }


    protected override void MouseDown() {

        //"Wafflin About" unlocked by tapping the waffle
        Settings.setAchievementUnlocked(33);

        //using Mathf.Min here to make sure we dont go outside the bounds of the arrays
        setAudioDown(  waffleBiteSounds[ Mathf.Min(biteCount, waffleBiteSounds.Length - 1) ]  );
        GetComponent<SpriteRenderer>().sprite = waffleSprites[ Mathf.Min(biteCount, waffleSprites.Length - 1) ];
        waffleBiteParticles[ Mathf.Min(biteCount, waffleBiteParticles.Length - 2) ].Play();
        
        if (Settings.isSoundEnabled() && biteCount == 24) {

            setAudioDown(null);
            jimmyAudioSource.volume = 1f;
            jimmyAudioSource.Play();
            jimmyRain.Play();

        }

        biteCount++;

        base.MouseDown();
    }


}
