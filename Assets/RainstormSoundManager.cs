using UnityEngine;

public class RainstormSoundManager : MonoBehaviour
{
    public AudioClip rainstormSound;
    public Transform mainCamera;
    public float volumeTransitionSpeed = 3.0f;

    private AudioSource audioSource;
    
    private void Start()
    {
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = rainstormSound;
        audioSource.loop = true;
        audioSource.Play();
        audioSource.volume = 0f;
    }


    void Update()
    {

        if (mainCamera.position.y > 70.0)
        {

                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.0f, Time.deltaTime * volumeTransitionSpeed);
        }


        else if (mainCamera.position.y > 45.0 && mainCamera.position.y < 60.0)
        {
    
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.7f, Time.deltaTime * volumeTransitionSpeed);
        }


        else if (mainCamera.position.y > 0.0 && mainCamera.position.y < 45.0)
        {
                // Camera is above 65, set volume to 0% instantly
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.5f, Time.deltaTime * volumeTransitionSpeed);
        }


        else
        {
                // Camera is above 65, set volume to 0% instantly
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0.0f, Time.deltaTime * volumeTransitionSpeed);
        }


    }
}
