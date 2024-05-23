using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaffleButton : ButtonPress {


    [SerializeField] Sprite[] waffleSprites;
    [SerializeField] AudioClip[] waffleBiteSounds;

    ParticleSystem[] waffleBiteParticles;
    int biteCount = 0;

    protected override void Start() {
        base.Start();

        waffleBiteParticles = GetComponentsInChildren<ParticleSystem>();

        audioSourceDown.volume = 1;
    }


    protected override void MouseDown() {

        if (biteCount < waffleSprites.Length) {

            audioSourceDown.clip = waffleBiteSounds[biteCount];
            GetComponent<SpriteRenderer>().sprite = waffleSprites[biteCount];
            waffleBiteParticles[biteCount].Play();
            biteCount++;

        }

        base.MouseDown();
    }


}
