using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicData
{
    public AudioSource audioSource;
    public float crossfadeDuration = 4.0f;
}

public class MusicManager : MonoBehaviour
{
    public MusicData[] songs;
    public float song2Start = 10.0f;
    public float crossfadeYThreshold = 0.0f; // Set this threshold in the Inspector

    private int currentSongIndex = 0;
    private bool song2Started = false;

    private void Start()
    {
        // Ensure all songs are set to loop
        foreach (MusicData song in songs)
        {
            song.audioSource.loop = true;
            song.audioSource.volume = 0f;
        }

        // Play the first song and set it to full volume
        songs[currentSongIndex].audioSource.Play();
        songs[currentSongIndex].audioSource.volume = 1f;
    }

    private void Update()
    {
        if (!song2Started && Time.time >= song2Start)
        {
            // Start crossfading to the second song after the specified delay
            StartCoroutine(CrossfadeToNextSong());
            song2Started = true;
        }

        if (currentSongIndex < 2 && Camera.main.transform.position.y < crossfadeYThreshold)
        {
            // Start crossfading to the third song
            int nextSongIndex = 2;
            StartCoroutine(CrossfadeToNextSong());
            currentSongIndex = nextSongIndex;
        }
    }

    private IEnumerator CrossfadeToNextSong()
    {
        int nextSongIndex = (currentSongIndex + 1) % songs.Length;
        MusicData currentSong = songs[currentSongIndex];
        MusicData nextSong = songs[nextSongIndex];

        float elapsedTime = 0f;

        while (elapsedTime < currentSong.crossfadeDuration)
        {
            float t = elapsedTime / currentSong.crossfadeDuration;
            currentSong.audioSource.volume = Mathf.Lerp(1f, 0f, t);
            nextSong.audioSource.volume = Mathf.Lerp(0f, 1f, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the volumes are set correctly at the end of the crossfade
        currentSong.audioSource.volume = 0f;
        nextSong.audioSource.volume = 1f;
        currentSongIndex = nextSongIndex;
    }
}
