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
    private List<System.Action> downActions;
    private List<System.Action> stayActions;

    [SerializeField] private bool active = false;

    private void Awake() {
        actions = new List<System.Action>();
        downActions = new List<System.Action>();
        stayActions = new List<System.Action>();
    }

    virtual protected void Start(){
        rend = GetComponent<SpriteRenderer>();
        buttonCollider = GetComponent<Collider2D>();

        buttonCollider.enabled = active;

        audioSourceUp = gameObject.AddComponent<AudioSource>();
        audioSourceUp.clip = btnUpSound;

        audioSourceDown = gameObject.AddComponent<AudioSource>();
        audioSourceDown.clip = btnDownSound;

    }

    private void OnMouseDown(){
        if (active) {

            MouseDown();
        }
    }

    virtual protected void MouseDown() {
        if (audioSourceDown.clip != null && Settings.isSoundEnabled()) {
            audioSourceDown.Play();
        }

        isPressed = true;
        rend.material.color = pressedColor;

        for (int i = 0; i < downActions.Count; i++) {
            downActions[i]();
        }
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
            actions[i]();
        }

        if (audioSourceUp.clip != null && Settings.isSoundEnabled()) {
            audioUp();
        }
    }


    virtual protected void OnMouseDrag() {
        if(active && isPressed) {

            for (int i = 0; i < stayActions.Count; i++) {
                stayActions[i]();
            }

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


    //takes in anonymous method that runs when button is pressed and then released
    public void onPress(System.Action action) {
        actions.Add(action);
    }


    //takes in anonymous method that runs when button is pressed but not released
    public void onPressDown(System.Action action) {
        downActions.Add(action);
    }


    public void onMouseStay(System.Action action) {
        stayActions.Add(action);
    }


    public void clearActions() {
        actions.Clear();
    }


    public void setActive(bool a) {
        active = a;
        buttonCollider.enabled = a;
    }

}
