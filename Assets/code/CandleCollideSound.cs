using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CandleCollideSound : MonoBehaviour
{

    [SerializeField] AudioClip candleHitSound;
    float minVelocityToPlaySound = 1f;


    private void OnCollisionEnter2D(Collision2D collision){

        if (Settings.isSoundEnabled() && collision.relativeVelocity.magnitude > minVelocityToPlaySound) {

            AudioSource a = transform.AddComponent<AudioSource>();

            a.clip = candleHitSound;
            a.loop = false;
            a.volume = 5f;
            a.pitch = Random.Range(0.9f, 1.3f);
            a.Play();

            Destroy(a, candleHitSound.length);

        }

    }



}
