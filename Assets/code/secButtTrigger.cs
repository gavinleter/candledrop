using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secButtTrigger : MonoBehaviour

{
    
    private Color originalColor;
    private Color highlightColor;
    private Renderer rend;
    private bool hasBeenPressed = false;



    // Customize these colors to your liking
    public Color normalColor = Color.white;
    public Color pressedColor = Color.gray;

    [SerializeField] ParticleSystem found1;
    [SerializeField] ParticleSystem found2;
    [SerializeField] ParticleSystem found3;
    [SerializeField] ParticleSystem alreadyFound;
    [SerializeField] ParticleSystem confetti;
    [SerializeField] AudioClip btnDownSound;
    [SerializeField] AudioClip btnUpSound;
    [SerializeField] AudioClip victorySound;

    AudioSource audioSourceUp;
    AudioSource audioSourceDown;
    AudioSource audioSourceVictorySound;

    private bool isPressed = false;

    private List<System.Action> actions;

    public bool active = false;

    virtual protected void Start(){
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        highlightColor = normalColor;

        audioSourceUp = gameObject.AddComponent<AudioSource>();
        audioSourceDown = gameObject.AddComponent<AudioSource>();
        audioSourceVictorySound = gameObject.AddComponent<AudioSource>();
        
        audioSourceUp.clip = btnUpSound;
        audioSourceDown.clip = btnDownSound;
        audioSourceVictorySound.clip = victorySound;

        actions = new List<System.Action>();
    }

    virtual protected void OnMouseDown(){
        if (active) {

            if (btnDownSound != null && Settings.soundEnabled) { 
                audioSourceDown.Play();
            }

            isPressed = true;
            ChangeColor();

        }
    }

    virtual protected void OnMouseUp(){
        if (active && isPressed) {

            isPressed = false;
            ChangeColor();
            //execute each action when this button is pressed
            for (int i = 0; i < actions.Count; i++) {
                executeAction(i);
            }

            if (btnUpSound != null && Settings.soundEnabled) {
                audioSourceUp.volume = 0.2f;
                audioSourceUp.Play();
                confetti.Play();
                audioSourceVictorySound.Play();

            }
            
            // gavin code :3
            if (hasBeenPressed == false) {

                hasBeenPressed = true;
                Settings.increaseSecretButtonCounter();

                Debug.Log(Settings.secretButtonCounter);


                if (Settings.secretButtonCounter == 1) 
                {
                    found1.Play();
                }
                
                else if (Settings.secretButtonCounter == 2) 
                {
                    found2.Play(); 
                }
                
                else if (Settings.secretButtonCounter == 3) 
                {
                    found3.Play(); 
                }
                    }
            else
            {
                alreadyFound.Play();
            }

        

                

            }

        }


    virtual protected void OnMouseExit(){
        isPressed = false;
        ChangeColor();
    }

    virtual protected void ChangeColor()
    {
        if (isPressed)
        {
            rend.material.color = pressedColor;
        }
        else
        {
            rend.material.color = normalColor;
        }
    }


    public bool btnPressed() {
        return isPressed;
    }


    //takes in an anonmymous method that should run when this button is pressed
    public void onPress(System.Action action) {
        actions.Add(action);
    }


    private void executeAction(int index) {
      actions[index]();
    }
}
