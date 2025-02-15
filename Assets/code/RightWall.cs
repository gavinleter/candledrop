using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWall : CandleLightCollector
{

    [SerializeField] ParticleSystem rightParticleSystem;

    void FixedUpdate() {

        updateTouchingList();

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
