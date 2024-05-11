using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretButton : ButtonPress
{

    [SerializeField] int secretButtonId;

    //these include the particle systems for the three "you found 1 / 3 secret buttons!"
    [SerializeField] GameObject[] particleSystemObjects;
    [SerializeField] GameObject confettiParticleSystemObject;
    [SerializeField] GameObject alreadyFoundParticleSystemObject;

    //for the victory sound that also plays when finding a secret button
    [SerializeField] AudioClip[] extraSounds;
    AudioSource[] audioSourceExtra;

    [SerializeField] float extraSoundVolume;

    ParticleSystem[] foundText;
    ParticleSystem confettiParticle;
    ParticleSystem alreadyFoundParticle;

    protected override void Start(){
        base.Start();

        foundText = new ParticleSystem[particleSystemObjects.Length];

        //get references to each child particle system as they are instantiated
        for (int i = 0; i < particleSystemObjects.Length; i++) {
            foundText[i] = Instantiate(particleSystemObjects[i], transform).GetComponent<ParticleSystem>();
        }
        confettiParticle = Instantiate(confettiParticleSystemObject, transform).GetComponent<ParticleSystem>();
        alreadyFoundParticle= Instantiate(alreadyFoundParticleSystemObject, transform).GetComponent<ParticleSystem>();


        //an audiosource can only play one sound at a time, so a new one needs to be made for each sound
        audioSourceExtra = new AudioSource[extraSounds.Length];

        for (int i = 0; i < extraSounds.Length; i++) {
            audioSourceExtra[i] = gameObject.AddComponent<AudioSource>();
            audioSourceExtra[i].clip = extraSounds[i];
            audioSourceExtra[i].volume = extraSoundVolume;
        }
    }


    protected override void MouseUp() {
        base.MouseUp();

        //if the button has already been found, play the already found particle
        if (Settings.isSecretButtonFound(secretButtonId)) {
            alreadyFoundParticle.Play();
        }
        //otherwise, find how many buttons have been found and play the appropriate particle system
        else{
            confettiParticle.Play();
            foundText[Settings.getSecretButtonCounter()].Play();
            Settings.setSecretButtonFound(secretButtonId);
        }
    }


    protected override void audioUp() {
        base.audioUp();

        for (int i = 0; i < audioSourceExtra.Length; i++) { 
            audioSourceExtra[i].Play();
        }
    }

}
