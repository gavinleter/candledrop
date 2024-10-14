using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField] AudioClip[] music;
    [SerializeField] float[] fadeInMultiplier;
    [SerializeField] float[] fadeOutMultiplier;
    AudioSource[] musicSources;
    int selectedMusic;


    private void Awake() {

        musicSources = GetComponentsInChildren<AudioSource>();

        for (int i = 0; i < musicSources.Length; i++) {

            musicSources[i].clip = music[i];
            musicSources[i].loop = true;
            musicSources[i].volume = 0;
            musicSources[i].Play();

        }

    }


    private void Update() {
        
        updateVolumes();

    }


    void updateVolumes() {

        int sign = 1;
        float multiplier = 1;

        for(int i = 0; i < musicSources.Length; i++) {

            //add to the volume of the selected track, subtract from the volume of all others
            sign = i == selectedMusic ? 1 : -1;
            //select between fade in/fade out multiplier
            multiplier = i == selectedMusic ? fadeInMultiplier[i] : fadeOutMultiplier[i];

            musicSources[i].volume = musicSources[i].volume + (Time.deltaTime * sign * multiplier);

            //force volume to 0 if music is off
            if (!Settings.isMusicEnabled()) {
                musicSources[i].volume = 0;
            }

        }

    }


    public void setSelectedMusic(int x) {
        selectedMusic = x;
    }


    public void setMusicVolume(float volume) {
        musicSources[selectedMusic].volume = volume;
    }

}
