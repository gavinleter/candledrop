using UnityEngine;

public class RainstormSoundManager : MonoBehaviour
{
    public AudioClip rainstormSound;
    public Transform mainCamera;
    public float volumeTransitionSpeed = 0.5f;

    private AudioSource audioSource;
    
    private void Start()
    {
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = rainstormSound;
        audioSource.loop = true;
        audioSource.Play();
    }


    private void Update()
    {
        if (mainCamera.position.y > 65.0)
        {
            // Camera is above 65, set volume to 0% instantly
            SmoothlyChangeVolume(0f, 0f);
        }
        else if (mainCamera.position.y > 45.0 && mainCamera.position.y < 65.0)
        {
            // Camera is between 45 and 65, smoothly transition volume to 90%
            SmoothlyChangeVolume(0.9f, volumeTransitionSpeed);
        }
        else if (mainCamera.position.y > 0.0 && mainCamera.position.y < 45.0)
        {
            // Camera is between 0 and 45, smoothly transition volume to 50%
            SmoothlyChangeVolume(0.5f, volumeTransitionSpeed);
        }
        else
        {
            // Camera is below 0, set volume to 0%
            SmoothlyChangeVolume(0f, volumeTransitionSpeed);
        }
    }

    private void SmoothlyChangeVolume(float targetVolume, float transitionTime)
    {
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, transitionTime * Time.deltaTime);
    }
}
