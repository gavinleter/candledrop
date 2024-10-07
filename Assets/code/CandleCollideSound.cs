using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleCollideSound : MonoBehaviour
{

    [SerializeField] private AudioClip candleHitSound;
    private AudioSource audioSource;



    private void Start(){

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = candleHitSound;
        audioSource.loop = false;
        audioSource.volume = 1f;
    }
    private void OnCollisionEnter2D(Collision2D collision){

        //CURTIS add an if statement to only play the collision if one of the objects is moving fast enough

        audioSource.pitch = Random.Range(0.9f,1.3f);
        audioSource.Play();
    }
}
