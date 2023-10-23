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

    private int currentSongIndex = 0;

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
        if (Time.time >= song2Start && currentSongIndex == 0)
        {
            // Start crossfading to the next song after the specified delay
            int nextSongIndex = (currentSongIndex + 1) % songs.Length;
            StartCoroutine(CrossfadeSongs(currentSongIndex, nextSongIndex));
            currentSongIndex = nextSongIndex;
        }
    }

    private System.Collections.IEnumerator CrossfadeSongs(int currentIndex, int nextIndex)
    {
        MusicData currentSong = songs[currentIndex];
        MusicData nextSong = songs[nextIndex];

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
    }
}
