using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWall : CandleLightCollector
{

    [SerializeField] ParticleSystem rightParticleSystem;

    void FixedUpdate() {

        if (isTouchingAnyCandles()) {

            if (!rightParticleSystem.isPlaying) {
                rightParticleSystem.Play();
            }
          
        }
        else{
            rightParticleSystem.Stop();
        }

    }

}
