using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField] AudioClip[] music;
    [SerializeField] float[] fadeInMultiplier;
    [SerializeField] float[] fadeOutMultiplier;
    [SerializeField] float maxVolume;
    [SerializeField] float timeToSwitchTracks;

    AudioSource[] musicSources;
    AudioSource[] lastMusicSources;
    int selectedMusic;


    private void Awake() {

        musicSources = new AudioSource[music.Length];
        lastMusicSources = new AudioSource[music.Length];

        if(timeToSwitchTracks <= 0) {
            Debug.Log("Invalid time on timeToSwitchTracks in MusicManager");
        }

        createNewAudioSources();
        
    }


    private void Update() {

        updateVolumes();

        if (musicSources[0].time > timeToSwitchTracks) {
            Debug.Log(musicSources[0].time);

            createNewAudioSources();
            setMusicVolume(lastMusicSources[selectedMusic].volume);
        }

    }


    void updateVolumes() {

        int sign = 1;
        float multiplier = 1;

        for(int i = 0; i < musicSources.Length; i++) {

            //add to the volume of the selected track, subtract from the volume of all others
            sign = i == selectedMusic ? 1 : -1;
            //select between fade in/fade out multiplier
            multiplier = i == selectedMusic ? fadeInMultiplier[i] : fadeOutMultiplier[i];

            musicSources[i].volume = Mathf.Min(musicSources[i].volume + (Time.deltaTime * sign * multiplier), maxVolume);
            //music sources that are about to be deleted should slowly be muted
            if (lastMusicSources[i] != null) {
                lastMusicSources[i].volume = lastMusicSources[i].volume - (Time.deltaTime);
            }

            //force volume to 0 if music is off
            if (!Settings.isMusicEnabled()) {
                musicSources[i].volume = 0;
            }

        }

    }


    void createNewAudioSources() {

        for (int i = 0; i < musicSources.Length; i++) {

            lastMusicSources[i] = musicSources[i];

            musicSources[i] = transform.AddComponent<AudioSource>();
            musicSources[i].clip = music[i];
            musicSources[i].volume = 0;

            musicSources[i].Play();

            Destroy(musicSources[i], 65f);

        }

    }


    public void setSelectedMusic(int x) {
        selectedMusic = x;
    }


    public void setMusicVolume(float volume) {
        if (musicSources[selectedMusic] != null) {
            musicSources[selectedMusic].volume = volume;
        }
    }


    public float getMaxVolume() {
        return maxVolume;
    }

}
