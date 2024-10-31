using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;


public class RainSoundManager : MonoBehaviour
{

    [SerializeField] AudioClip rainSoundOutside;
    [SerializeField] AudioClip rainSoundInside;

    [SerializeField] GameObject[] checkpoints;

    [SerializeField] float[] outsideVolumes;
    [SerializeField] float[] insideVolumes;

    [SerializeField] float[] volumeTransitionSpeeds;

    AudioSource outsideRain;
    AudioSource insideRain;


    private void Awake() {

        outsideRain = transform.AddComponent<AudioSource>();
        insideRain = transform.AddComponent<AudioSource>();

        outsideRain.clip = rainSoundOutside;
        insideRain.clip = rainSoundInside;

        setupAudioSource(outsideRain);
        setupAudioSource(insideRain);

    }


    private void Update() {

        updateVolumes();

    }


    void updateVolumes() {

        if (Settings.isSoundEnabled()) {

            int selectedCheckpoint = getClosestCheckpoint();

            outsideRain.volume = Mathf.Lerp(outsideRain.volume, outsideVolumes[selectedCheckpoint], Time.deltaTime * volumeTransitionSpeeds[selectedCheckpoint]);
            insideRain.volume = Mathf.Lerp(insideRain.volume, insideVolumes[selectedCheckpoint], Time.deltaTime * volumeTransitionSpeeds[selectedCheckpoint]);

        }
        else {

            outsideRain.volume = 0;
            insideRain.volume = 0;
        }

    }


    int getClosestCheckpoint() {
        int selectedIndex = 0;
        float lastDist = float.MaxValue;

        for (int i = 0; i < checkpoints.Length; i++) {
            float dist = transform.position.y - checkpoints[i].transform.position.y;

            if (dist >= 0 && dist < lastDist) {
                lastDist = dist;
                selectedIndex = i;
            }
        
        }

        return selectedIndex;

    }


    void setupAudioSource(AudioSource x) {
        x.loop = true;
        x.volume = 0;
        x.Play();
    }

}
