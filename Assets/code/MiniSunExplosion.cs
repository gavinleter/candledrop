using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSunExplosion : MonoBehaviour
{

    [SerializeField] float maxRadius;
    [SerializeField] AudioClip sunExplodeSound;
    [SerializeField] float sunExplodeVolume;
    [SerializeField] float sunExplodeMaxPitch;
    [SerializeField] float sunExplodeMinPitch;
    float lerp = 0;

    CircleCollider2D c;
    AudioSource audioSource;

    void Start() {
        
        c = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();

        audioSource.pitch = Random.Range(sunExplodeMinPitch, sunExplodeMaxPitch);
        audioSource.volume = sunExplodeVolume;

        if (Settings.isSoundEnabled()) {
            audioSource.Play();
        }

    }


    void Update() {
        
        c.radius = Mathf.Lerp(0, maxRadius, lerp);
        lerp += Time.deltaTime;

    }


    private void OnTriggerEnter2D(Collider2D collision) {

        //if touching a candle light, ignite it
        CandleLightController can = collision.GetComponent<CandleLightController>();

        if (can != null) {
            can.miniSunIgnite();
        }

    }

}
