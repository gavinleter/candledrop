using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioClip> songs;
    public float fadeTime = 3f;
    public GameObject targetObject;

    private int currentSongIndex = 0;
    private AudioSource[] songSources;
    private float startTime;
    private Coroutine fadingCoroutine;  // Added variable to keep track of the fading coroutine

    void Start()
    {
        songSources = new AudioSource[songs.Count];

        for (int i = 0; i < songs.Count; i++)
        {
            GameObject newSource = new GameObject("AudioSource" + i);
            newSource.transform.SetParent(this.transform);
            songSources[i] = newSource.AddComponent<AudioSource>();
            songSources[i].clip = songs[i];
            songSources[i].loop = true;
            songSources[i].volume = (i == 0) ? 1 : 0; // Set volume to 1 for the first song, 0 for others
            songSources[i].Play();
        }

        startTime = Time.time;
        StartCoroutine(CheckYPosition());
    }

    IEnumerator CheckYPosition()
    {
        while (true)
        {
            float yValue = targetObject.transform.position.y;

            if (yValue > 69)
            {
                SetCurrentSongIndex(0);
            }
            else if (yValue <= 69 && yValue >= 45)
            {
                SetCurrentSongIndex(1);
            }
            else if (yValue <= 33 && yValue >= 9)
            {
                SetCurrentSongIndex(2);
            }
            else if (yValue < 9)
            {
                SetCurrentSongIndex(3);
            }

            yield return new WaitForSeconds(0.1f); // Adjust the frequency here
        }
    }

    void SetCurrentSongIndex(int index)
    {
        if (currentSongIndex != index && fadingCoroutine == null && songSources[currentSongIndex].volume > 0)
        {
            fadingCoroutine = StartCoroutine(FadeOutAndIn(index));
        }
    }

    IEnumerator FadeOutAndIn(int newIndex)
    {
        float currentTime = 0;
        float startVolume = songSources[currentSongIndex].volume;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;

            // Check if a new transition has started, and adjust timing accordingly
            if (fadingCoroutine != null && fadingCoroutine != StartCoroutine(FadeOutAndInCoroutine(newIndex, currentTime)))
            {
                yield break;  // Exit the coroutine early
            }

            float newVolume = Mathf.Lerp(startVolume, 0, currentTime / fadeTime);
            songSources[currentSongIndex].volume = newVolume;
            songSources[newIndex].volume = 1 - newVolume;
            yield return null;
        }

        // Ensure a smooth transition, even if fadeTime is very short
        songSources[currentSongIndex].volume = 0;
        songSources[newIndex].volume = 1;

        currentSongIndex = newIndex;
        startTime = Time.time;
        fadingCoroutine = null;  // Reset the fading coroutine variable
    }

    // Additional coroutine to handle the transition during an ongoing fade
    IEnumerator FadeOutAndInCoroutine(int newIndex, float currentTime)
    {
        float remainingTime = fadeTime - currentTime;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;

            float adjustedVolume = Mathf.Lerp(songSources[currentSongIndex].volume, 0, 1 - remainingTime / fadeTime);
            songSources[currentSongIndex].volume = adjustedVolume;
            songSources[newIndex].volume = 1 - adjustedVolume;
            yield return null;
        }

        // Ensure a smooth transition, even if fadeTime is very short
        songSources[currentSongIndex].volume = 0;
        songSources[newIndex].volume = 1;

        currentSongIndex = newIndex;
        startTime = Time.time;
        fadingCoroutine = null;  // Reset the fading coroutine variable
    }

    void Update()
    {
        // No fading for the first song
        if (currentSongIndex != 0)
        {
            float currentTime = Time.time - startTime;

            if (currentTime <= fadeTime)
            {
                songSources[currentSongIndex].volume = Mathf.Lerp(0, 1, currentTime / fadeTime);
            }
        }
    }
}

