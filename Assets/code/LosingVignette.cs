using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LosingVignette : MonoBehaviour
{

    [SerializeField] ParticleSystem leftParticle;
    [SerializeField] ParticleSystem rightParticle;
    [SerializeField] ParticleSystem topParticle;
    [SerializeField] ParticleSystem bottomParticle;

    [SerializeField] float offsetDistance;

    ParticleSystem[] sideParticles = new ParticleSystem[4];
    Vector3[] initialPositions;
    Vector3[] offset;
    ParticleSystem mainParticle;


    private void Awake() {
        
        mainParticle = GetComponent<ParticleSystem>();
        initialPositions = new Vector3[sideParticles.Length];
        offset = new Vector3[sideParticles.Length];

        //this is just to absolutely guarantee the order of the particles in the array
        sideParticles[0] = leftParticle;
        sideParticles[1] = rightParticle;
        sideParticles[2] = topParticle;
        sideParticles[3] = bottomParticle;

        for (int i = 0; i < sideParticles.Length; i++) {
            initialPositions[i] = sideParticles[i].transform.localPosition;
        }

    }


    public void updateParticlePosition(float lerp) {
        float amt = Mathf.SmoothStep(offsetDistance, 0, lerp + 0.3f);

        offset[0].x = -amt;
        offset[1].x = amt;
        offset[2].y = amt;
        offset[3].y = -amt;

        for (int i = 0; i < sideParticles.Length; i++) {

            sideParticles[i].transform.localPosition = offset[i] + initialPositions[i];

        }
    }


    public void startParticles() {

        if (!mainParticle.isEmitting) {
            mainParticle.Play();
        }

        for (int i = 0; i < sideParticles.Length; i++) {

            if (!sideParticles[i].isEmitting) { 
                sideParticles[i].Play();
            }

        }

    }


    public void stopParticles() {

        if (mainParticle.isPlaying) {
            mainParticle.Stop();
        }

        for (int i = 0; i < sideParticles.Length; i++) {
            if (sideParticles[i].isPlaying) {
                sideParticles[i].Stop();
            }
        }

    }


    public void clearParticles() {
        stopParticles();

        mainParticle.Clear();

        for (int i = 0; i < sideParticles.Length; i++) {
            sideParticles[i].Clear();
        }
    }

}
