using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    SpriteRenderer rend;

    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color pressedColor = Color.gray;

    [SerializeField] AudioClip btnDownSound;
    [SerializeField] AudioClip btnUpSound;

    protected AudioSource audioSourceDown;
    protected AudioSource audioSourceUp;

    private bool isPressed = false;
    private Collider2D buttonCollider;

    private List<System.Action> actions;

    [SerializeField] private bool active = false;

    virtual protected void Start(){
        rend = GetComponent<SpriteRenderer>();
        buttonCollider = GetComponent<Collider2D>();

        buttonCollider.enabled = active;

        audioSourceUp = gameObject.AddComponent<AudioSource>();
        audioSourceUp.clip = btnUpSound;

        audioSourceDown = gameObject.AddComponent<AudioSource>();
        audioSourceDown.clip = btnDownSound;


        actions = new List<System.Action>();
    }

    private void OnMouseDown(){
        if (active) {

            MouseDown();
        }
    }

    virtual protected void MouseDown() {
        if (audioSourceDown.clip != null && Settings.soundEnabled) {
            audioSourceDown.Play();
        }

        isPressed = true;
        rend.material.color = pressedColor;
    }



    virtual protected void OnMouseUp(){
        if (active && isPressed) {

            MouseUp();
        }
    }

    virtual protected void MouseUp() {
        isPressed = false;

        rend.material.color = normalColor;

        //execute each action when this button is pressed
        for (int i = 0; i < actions.Count; i++) {
            executeAction(i);
        }

        if (audioSourceUp.clip != null && Settings.soundEnabled) {
            audioUp();
        }
    }

    //this is here because the SecretButton class plays more than one sound when pressed
    virtual protected void audioUp() {
        audioSourceUp.Play();
    }



    virtual protected void OnMouseExit(){
        isPressed = false;
        rend.material.color = normalColor;
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


    public void clearActions() {
        actions.Clear();
    }


    public void setActive(bool a) {
        active = a;
        buttonCollider.enabled = a;
    }

}
