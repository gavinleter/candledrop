using UnityEngine;

public class RainstormSoundManager : MonoBehaviour
{
    public AudioClip rainstormSound;
    public AudioClip insideRainstormSound;
    public Transform mainCamera;
    public float volumeTransitionSpeed = 3.0f;

    private AudioSource audioSource1;
    private AudioSource audioSource2;
    
    private void Start()
    {
        
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource1.clip = rainstormSound;
        audioSource1.loop = true;
        audioSource1.Play();
        audioSource1.volume = 0f;

        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource2.clip = insideRainstormSound;
        audioSource2.loop = true;
        audioSource2.Play();
        audioSource2.volume = 0f;
    }


    void Update()
    {

        if (mainCamera.position.y > 70.0)
        {

                audioSource1.volume = Mathf.Lerp(audioSource1.volume, 0.0f, Time.deltaTime * volumeTransitionSpeed);
                audioSource2.volume = Mathf.Lerp(audioSource2.volume, 0.0f, Time.deltaTime * volumeTransitionSpeed);

        }


        else if (mainCamera.position.y > 45.0 && mainCamera.position.y < 70.0)
        {
    
                audioSource1.volume = Mathf.Lerp(audioSource1.volume, 0.7f, Time.deltaTime * volumeTransitionSpeed * 0.05f);
                audioSource2.volume = Mathf.Lerp(audioSource2.volume, 0.0f, Time.deltaTime * volumeTransitionSpeed * 0.05f);
        }


        else if (mainCamera.position.y > 0.0 && mainCamera.position.y < 45.0)
        {
        
                audioSource1.volume = Mathf.Lerp(audioSource1.volume, 0.05f, Time.deltaTime * volumeTransitionSpeed * 0.2f);
                audioSource2.volume = Mathf.Lerp(audioSource2.volume, 0.3f, Time.deltaTime * volumeTransitionSpeed * 0.2f);
        }

        else
        {
                audioSource1.volume = Mathf.Lerp(audioSource1.volume, 0.0f, Time.deltaTime * volumeTransitionSpeed);
                audioSource2.volume = Mathf.Lerp(audioSource2.volume, 0.0f, Time.deltaTime * volumeTransitionSpeed);

        }
    }
}
