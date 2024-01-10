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

    // Particle System prefab to be cloned
    public ParticleSystem lightningParticleSystemPrefab;

    // Bounds for random position
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = 0f;
    public float maxY = 10f;

    private float timer = 0f;
    private float timerDuration = 57.3f;

    void Start()
    {
        // Start the timer coroutine when the game starts
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        //Debug.Log("Timer Coroutine Started");

        while (true)
        {
            yield return new WaitForSeconds(0.1f); // Update the timer every tenth of a second

            // Update the timer with tenths of a second precision
            timer += 0.1f;
            
            // Check if the timer has reached or exceeded the desired duration
            if (inCheckpointArray(timer))
            {
                // Trigger the "lightning_event"
                TriggerLightningEvent();

                // Reset the timer back to zero
                timer = 0f;

                //Debug.Log("Timer Reset at Time: " + Time.time);
            }
        }
    }

    bool IsCheckpointTime(float currentTime)
    {
        // Check if the current time is a lightning event checkpoint
        foreach (float checkpoint in lightningEventCheckpoints)
        {
            if (Mathf.Approximately(currentTime, checkpoint))
            {
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

    bool inCheckpointArray(float x) {
        for(int i = 0; i < lightningEventCheckpoints.Length; i++) {
            if(lightningEventCheckpoints[i] + 0.01f > x && lightningEventCheckpoints[i] - 0.01f < x) {
                return true;
            }
        }
        return false;
    }
}
