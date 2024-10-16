using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    // Checkpoints for lightning events in seconds
    private float[] lightningEventCheckpoints = {
        0.9f, 2.7f, 4.5f, 6.3f, 6.7f, 6.9f, 8.1f, 9.9f, 11.6f, 13.4f, 13.9f, 14.1f, 15.2f,
        17.0f, 18.8f, 20.6f, 21.0f, 21.2f, 22.4f, 24.2f, 26.0f, 27.8f, 28.0f, 28.2f, 28.4f,
        29.6f, 31.3f, 33.1f, 34.9f, 35.4f, 35.6f, 36.7f, 38.5f, 40.3f, 42.1f, 42.5f, 42.7f,
        43.9f, 45.7f, 47.5f, 49.3f, 49.7f, 49.9f, 51.0f, 52.8f, 54.6f, 56.4f, 56.6f, 56.8f, 57.0f
    };
    private bool[] usedCheckpoints;

    // Particle System prefab to be cloned
    public ParticleSystem lightningParticleSystemPrefab;

    // Bounds for random position
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = 0f;
    public float maxY = 10f;

    private float timer = 0f;
    private float startTime;
    private float timerDuration = 57.3f;

    void Start()
    {
        usedCheckpoints = new bool[lightningEventCheckpoints.Length];
        
        startTime = Time.time;
    }

    void Update() {
        
        //get time since last loop reset
        timer = Time.time - startTime;

        //if its time for a loop, reset
        if(timer > timerDuration) {
            timer = 0;
            startTime = Time.time;
            resetUsedCheckpoints();
        }

        if (IsCheckpointTime(timer)) {
            TriggerLightningEvent();
        }

    }


    bool IsCheckpointTime(float currentTime)
    {
        for (int i = 0; i < lightningEventCheckpoints.Length; i++) {
            if (!usedCheckpoints[i] && currentTime - 0.05f < lightningEventCheckpoints[i] && currentTime + 0.05f > lightningEventCheckpoints[i]) {

                usedCheckpoints[i] = true;
                return true;
            }
        }
        return false;
    }

    void TriggerLightningEvent()
    {
        //Debug.Log("Lightning Event Triggered at Time: " + Time.time);

        // Create a clone of the particle system
        ParticleSystem clone = Instantiate(lightningParticleSystemPrefab);

        // Set the clone's position to a random x and y within specified bounds
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector3 randomPosition = new Vector3(randomX, randomY, 0f);

        // Clamp the position to ensure it stays within the specified bounds
        randomPosition.x = Mathf.Clamp(randomPosition.x, minX, maxX);
        randomPosition.y = Mathf.Clamp(randomPosition.y, minY, maxY);

        clone.transform.position = randomPosition;

        // Subscribe to the particle system's event to delete the clone after it finishes playing
        var main = clone.main;
        main.playOnAwake = false;
        main.loop = false;

        var emission = clone.emission;
        emission.enabled = true;

        clone.Play();
        Destroy(clone.gameObject, main.duration);
    }

    void resetUsedCheckpoints() {
        for (int i = 0; i < usedCheckpoints.Length; i++) {
            usedCheckpoints[i] = false;
        }
    }

}
