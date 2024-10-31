using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
class MusicCheckpoint {

    public AudioClip track;
    public float maxVolume;
    public GameObject checkpoint;
    public System.Func<bool> playCondition;

}



public class MusicManager : MonoBehaviour
{

    [SerializeField] LightningManager lightningManager;

    [SerializeField] MusicCheckpoint[] music;
    [SerializeField] float timeToSwitchTracks;

    [SerializeField] float fadeInMultiplier;
    [SerializeField] float fadeOutMultiplier;

    [SerializeField] float intenseMusicDuration;

    float intenseMusicInitialTime = float.MinValue;

    bool gameOver = false;

    AudioSource[] musicSources;
    AudioSource[] lastMusicSources;

    float[] lightningEventCheckpoints = {
        0.9f, 2.7f, 4.5f, 6.3f, 6.7f, 6.9f, 8.1f, 9.9f, 11.6f, 13.4f, 13.9f, 14.1f, 15.2f,
        17.0f, 18.8f, 20.6f, 21.0f, 21.2f, 22.4f, 24.2f, 26.0f, 27.8f, 28.0f, 28.2f, 28.4f,
        29.6f, 31.3f, 33.1f, 34.9f, 35.4f, 35.6f, 36.7f, 38.5f, 40.3f, 42.1f, 42.5f, 42.7f,
        43.9f, 45.7f, 47.5f, 49.3f, 49.7f, 49.9f, 51.0f, 52.8f, 54.6f, 56.4f, 56.6f, 56.8f, 57.0f
    };

    int currentLightingCheckpoint = 0;


    private void Awake() {
        
        //main track
        //enabled if alt track is not toggled, game is not over, and the intense track is not playing
        music[3].playCondition = () => {
            return !Settings.isAltMusicEnabled() && !gameOver && (intenseMusicInitialTime + intenseMusicDuration) <= Time.time;
        };

        //intense main track
        music[5].playCondition = () => {
            return !Settings.isAltMusicEnabled() && !gameOver && (intenseMusicInitialTime + intenseMusicDuration) > Time.time;
        };

        //game over
        music[6].playCondition = () => {
            return gameOver;
        };

        //alt main track
        music[7].playCondition = () => {
            return Settings.isAltMusicEnabled() && !gameOver && (intenseMusicInitialTime + intenseMusicDuration) <= Time.time;
        };

        //intense alt main track
        music[8].playCondition = () => {
            return Settings.isAltMusicEnabled() && !gameOver && (intenseMusicInitialTime + intenseMusicDuration) > Time.time;
        };


        musicSources = new AudioSource[music.Length];
        lastMusicSources = new AudioSource[music.Length];

        if(timeToSwitchTracks <= 0) {
            Debug.Log("Invalid time on timeToSwitchTracks in MusicManager");
        }

        createNewAudioSources();
        
    }


    private void Update() {

        updateVolumes();

        //step through each lightning strike as the timings line up
        //if the lightning events reach the end, they cannot play again until reset by creating new audio sources (aka when the music loops)
        if (currentLightingCheckpoint < lightningEventCheckpoints.Length && musicSources[0].time > lightningEventCheckpoints[currentLightingCheckpoint]) {
            
            currentLightingCheckpoint++;
            lightningManager.triggerLightning();
        }

        if (musicSources[0].time > timeToSwitchTracks) {
            
            createNewAudioSources();
        }

    }


    void updateVolumes() {

        //the selected checkpoint will have the volume of its associated music increase
        //all others will decrease in volume

        int sign = 1;
        float multiplier = 1;
        //Debug.Log(Settings.isAltMusicEnabled() + " " + gameOver + " " + (intenseMusicInitialTime + intenseMusicDuration) + " " + Time.time);
        for (int i = 0; i < musicSources.Length; i++) {

            //add to the volume of the selected track, subtract from the volume of all others
            //if the condition to play the track cannot be met, then the track can only decrease in volume
            if (trackEnabled(i)) {
                sign = 1;
                multiplier = fadeInMultiplier;
            }
            else {
                sign = -1;
                multiplier = fadeOutMultiplier;
            }

            //increase/decrease the volume but make sure the volume cannot exceed maxVolume
            musicSources[i].volume = Mathf.Min(musicSources[i].volume + (Time.deltaTime * sign * multiplier), music[i].maxVolume);

            //music sources that are about to be deleted should slowly be muted
            if (lastMusicSources[i] != null) {
                lastMusicSources[i].volume = lastMusicSources[i].volume - (Time.deltaTime * fadeOutMultiplier);
            }

            //force volume to 0 if music is off
            if (!Settings.isMusicEnabled()) {
                musicSources[i].volume = 0;
            }

        }

    }


    void createNewAudioSources() {

        currentLightingCheckpoint = 0;

        for (int i = 0; i < musicSources.Length; i++) {

            lastMusicSources[i] = musicSources[i];

            musicSources[i] = transform.AddComponent<AudioSource>();
            musicSources[i].clip = music[i].track;

            //make sure the new sources have the same volume as the old ones for a smooth loop
            if (lastMusicSources[i] != null) {
                musicSources[i].volume = lastMusicSources[i].volume;
            }
            else {
                //if this is the first loop, start everything at 0 volume instead
                musicSources[i].volume = 0;
            }

            musicSources[i].Play();

            Destroy(musicSources[i], music[i].track.length);

        }

    }


    bool trackEnabled(int i) {

        return music[i].checkpoint == getClosestCheckpoint() && (music[i].playCondition == null || music[i].playCondition());

    }


    GameObject getClosestCheckpoint() {
        int selectedIndex = 0;
        float lastDist = float.MaxValue;

        //get the closest checkpoint that the camera is ABOVE
        for (int i = 0; i < music.Length; i++) {
            float dist = transform.position.y - music[i].checkpoint.transform.position.y;
            
            if (dist >= 0 && dist < lastDist) {
                lastDist = dist;
                selectedIndex = i;
            }

        }
        
        return music[selectedIndex].checkpoint;
    }


    //maxes out the volume of the current track
    public void maxOutMusicVolume() {

        if (Settings.isMusicEnabled()) {
            for (int i = 0; i < musicSources.Length; i++) {

                if (trackEnabled(i)) {
                    musicSources[i].volume = music[i].maxVolume;
                }

            }
        }

    }


    public void toggleIntenseMusic() {
        intenseMusicInitialTime = Time.time;
    }


    public void toggleGameOverTrack(bool x) {
        gameOver = x;
    }

}
