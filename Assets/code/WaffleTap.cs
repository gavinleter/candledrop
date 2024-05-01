using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaffleTap : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;
    
    private int timesBitten = 0;

    [SerializeField] Sprite bittenWaffle;
    [SerializeField] Sprite EatenWaffle;
    [SerializeField] Sprite moreBittenWaffle;
    [SerializeField] AudioClip WaffleBite;
    [SerializeField] AudioClip burpy;

    [SerializeField] ParticleSystem nomnom;

    [SerializeField] ParticleSystem nomnom2;

    [SerializeField] ParticleSystem nomnom3;

    AudioSource audioSourceWaffleBite;

    AudioSource audioSourceBurp;

    // Start is called before the first frame update
    void Start()
    {

        audioSourceWaffleBite = gameObject.AddComponent<AudioSource>();
        audioSourceWaffleBite.clip = WaffleBite;
        audioSourceWaffleBite.volume = 0.0f;

        audioSourceBurp = gameObject.AddComponent<AudioSource>();
        audioSourceBurp.clip = burpy;
        audioSourceBurp.volume = 0.0f;
        
        timesBitten = 0;
    }

    virtual protected void OnMouseDown(){
        if(timesBitten == 0){
            if (Settings.soundEnabled){

            audioSourceWaffleBite.volume = 1.0f;
            audioSourceWaffleBite.Play();

        }
        spriteRenderer.sprite = bittenWaffle;
        nomnom.Play();
        timesBitten = timesBitten + 1;
        }
        else if(timesBitten == 1){
            if (Settings.soundEnabled){

            audioSourceWaffleBite.volume = 1.0f;
            audioSourceWaffleBite.Play();

        }
        spriteRenderer.sprite = moreBittenWaffle;
        nomnom2.Play();
        timesBitten += 1;
        }
        else if(timesBitten == 2){
            if (Settings.soundEnabled){

            audioSourceWaffleBite.volume = 1.0f;
            audioSourceWaffleBite.Play();

        }
        spriteRenderer.sprite = EatenWaffle;
        nomnom3.Play();
        timesBitten += 1;
        }
        else if(timesBitten == 3){
            if (Settings.soundEnabled){

            audioSourceBurp.volume = 1.0f;
            audioSourceBurp.Play();}
        
        }
    }




}