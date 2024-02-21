using UnityEngine;

public class SoundOnEffectManager : MonoBehaviour
{
    // AudioSources for the sounds
    public AudioSource fireSound; // Sound for fire effect
    public AudioSource candleHitSound; // Sound for candle hit effect

    // Particle system for fire effect
    public ParticleSystem fireParticleSystem;

    // Called when the fire particle system starts
    private void OnEnable()
    {
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Play();
        }
    }

    // Called when a collider enters this GameObject's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider belongs to a candle
        if (other.CompareTag("Candle"))
        {
            PlayCandleHitSound();
        }
    }

    // Called when this GameObject collides with another GameObject
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding GameObject belongs to a candle
        if (collision.gameObject.CompareTag("Candle"))
        {
            PlayCandleHitSound();
        }
    }

    // Method to play the candle hit sound
    private void PlayCandleHitSound()
    {
        if (candleHitSound != null)
        {
            candleHitSound.Play();
        }
    }

    // Called every frame
    private void Update()
    {
        // Check if fire particle system is playing and fire sound is not already playing
        if (fireParticleSystem != null && fireSound != null)
        {
            if (fireParticleSystem.isPlaying && !fireSound.isPlaying)
            {
                // If fire particle system is playing but fire sound is not, play fire sound
                fireSound.Play();
            }
        }
    }
}
