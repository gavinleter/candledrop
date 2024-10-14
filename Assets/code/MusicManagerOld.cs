using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerOld : MonoBehaviour
{
    public List<AudioClip> songs;
    [SerializeField] float fadeTime = 3f;
    [SerializeField] GameObject targetObject;
    [SerializeField] float maxVolume = 0.9f; // Maximum volume for the songs
    private float loopTimer = 0f;
    private float loopTimerDuration = 57.3f;
    private int currentSongIndex = 0;
    private List<AudioSource> audioSources = new List<AudioSource>();
    private float startTime;
    private AudioSource audioSource;

    void Start()
    {
        startTime = Time.time;
        audioSource = gameObject.AddComponent<AudioSource>();
        // Create AudioSource for each song and play them simultaneously
        for (int i = 0; i < songs.Count; i++)
        {
            audioSource.clip = songs[i];
            audioSource.volume = (i == 0) ? maxVolume : 0f; // Set volume to max for the first song, 0 for others
            audioSource.Play();
            audioSources.Add(audioSource);
        }

        StartCoroutine(CheckYPosition());
    }

    void Update()
    {

        //get time since last loop reset
        loopTimer = Time.time - startTime;

        //if its time for a loop, reset
        if(loopTimer > loopTimerDuration)
        {
            loopTimer = 0;
            startTime = Time.time;
            audioSource.Play();

            for (int i = 0; i < songs.Count; i++)
            {
                audioSource.clip = songs[i];
                audioSource.Play();


            }
        }

    }
    IEnumerator CheckYPosition()
    {
        while (true)
        {
            float yValue = targetObject.transform.position.y;
            int newIndex = 0;

            if (yValue > 69)
                newIndex = 0;
            else if (yValue <= 69 && yValue >= 45)
                newIndex = 1;
            else if (yValue <= 33 && yValue >= 9)
                newIndex = 2;
            else if (yValue < 9)
                newIndex = 3;

            if (currentSongIndex != newIndex)
                SetCurrentSongIndex(newIndex);

            yield return new WaitForSeconds(0.1f); // Adjust the frequency here
        }
    }

    void SetCurrentSongIndex(int index)
    {
        // Start fading out the old song and fading in the new one
        StartCoroutine(FadeOutAndIn(index));
        currentSongIndex = index;
    }

    IEnumerator FadeOutAndIn(int newIndex)
    {
        float startTime = Time.time;
        float[] startVolumes = new float[audioSources.Count];

        // Store the initial volumes of all audio sources
        for (int i = 0; i < audioSources.Count; i++)
        {
            startVolumes[i] = audioSources[i].volume;
        }

        while (Time.time < startTime + fadeTime)
        {
            float elapsedTime = Time.time - startTime;
            float fadeRatio = elapsedTime / fadeTime;

            // Update volume for each audio source
            for (int i = 0; i < audioSources.Count; i++)
            {
                float newVolume = Mathf.Lerp(startVolumes[i], i == newIndex ? maxVolume : 0, fadeRatio);
                audioSources[i].volume = newVolume;
            }

            yield return null;
        }

        // Ensure that volume is set correctly after the fade
        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[i].volume = i == newIndex ? maxVolume : 0;
        }
    }
}
