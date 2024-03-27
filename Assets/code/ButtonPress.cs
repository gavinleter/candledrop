using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    private Color originalColor;
    private Color highlightColor;
    private Renderer rend;

    // Customize these colors to your liking
    public Color normalColor = Color.white;
    public Color pressedColor = Color.gray;

    [SerializeField] AudioClip btnDownSound;
    [SerializeField] AudioClip btnUpSound;

    AudioSource audioSourceUp;
    AudioSource audioSourceDown;

    private bool isPressed = false;

    private List<System.Action> actions;

    public bool active = false;

    virtual protected void Start(){
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        highlightColor = normalColor;

        audioSourceUp = gameObject.AddComponent<AudioSource>();
        audioSourceDown = gameObject.AddComponent<AudioSource>();
        audioSourceUp.clip = btnUpSound;
        audioSourceDown.clip = btnDownSound;

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
                audioSourceUp.Play();
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
