using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class rawMusic : MonoBehaviour
{
    [SerializeField] AudioClip credits;
    [SerializeField] AudioClip space;

    [SerializeField] AudioClip MainMenu;

    [SerializeField] AudioClip MainGame;

    [SerializeField] AudioClip Basement;

    [SerializeField] AudioClip GameOver;

    [SerializeField] AudioClip AlternateGame;


    [SerializeField] GameObject targetObject;

    // Start is called before the first frame update
    void Start()
    {
        audioSourceCredits = gameObject.AddComponent<AudioSource>();
        audioSourceCredits.clip = credits;

        audioSourceSpace = gameObject.AddComponent<AudioSource>();
        audioSourceSpace.clip = space;

        audioSourceMainMenu = gameObject.AddComponent<AudioSource>();
        audioSourceMainMenu.clip = MainMenu;

        audioSourceMainGame = gameObject.AddComponent<AudioSource>();
        audioSourceMainGame.clip = MainGame;

        audioSourceBasement = gameObject.AddComponent<AudioSource>();
        audioSourceBasement.clip = Basement;

        audioSourceGameOver = gameObject.AddComponent<AudioSource>();
        audioSourceGameOver.clip = GameOver;

        audioSourceAlternateGame = gameObject.AddComponent<AudioSource>();
        audioSourceAlternateGame.clip = AlternateGame;


    }

    // Update is called once per frame
    void Update()
    {
    

        
    float yValue = targetObject.transform.position.y;

    if (yValue >= 69 && yValue <= 79)
        newIndex = 0;
     else if (yValue <= 69 && yValue >= 45)
        newIndex = 1;
    else if (yValue <= 33 && yValue >= 9)
        newIndex = 2;
    else if (yValue < 9)
        newIndex = 3;
    else if (yValue >= 80)
        newIndex = 4;

    yield return new WaitForSeconds(0.1f); // Adjust the frequency here
    
    }





    void LoopSongs{

        if Settings.musicEnabled(){

            audioSourceCredits.play();
            audioSourceSpace.play();
            audioSourceMainMenu.play();
            audioSourceMainGame.play();
            audioSourceBasement.play();
            audioSourceGameOver.play();
            audioSourceAlternateGame.play();

            StartCoroutine(LoopSongs());

        }

    }


    IENumerator LoopSongs(){
        yield return new WaitForSeconds(57.3);
        LoopSongs();


    }
}

*/