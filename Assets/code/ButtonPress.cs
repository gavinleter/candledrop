using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    SpriteRenderer rend;

    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color pressedColor = Color.gray;

    [SerializeField] AudioClip btnDownSound;
    [SerializeField] AudioClip btnUpSound;

    float pressUpEffectDelay = 0.1f;
    float pressDownTime = 0;
    bool waitingForPressUpEffects = false;
    bool pressCompleted = false;

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

        rend = GetComponent<SpriteRenderer>();
        buttonCollider = GetComponent<Collider2D>();

        buttonCollider.enabled = active;
    }

    virtual protected void Start(){
        

    }


    virtual protected void Update() {

        if (waitingForPressUpEffects && pressCompleted && Time.time > (pressDownTime + pressUpEffectDelay)) {

            pressCompleted = false;
            waitingForPressUpEffects = false;
            rend.material.color = normalColor;

            if (btnUpSound != null && Settings.isSoundEnabled()) {
                audioUp();
            }

        }

    }



    private void OnMouseDown(){
        if (active) {

            MouseDown();
        }
    }

    virtual protected void MouseDown() {
        if (btnDownSound != null && Settings.isSoundEnabled()) {
            audioDown();
        }

        isPressed = true;
        rend.material.color = pressedColor;

        for (int i = 0; i < downActions.Count; i++) {
            downActions[i]();
        }

        pressDownTime = Time.time;
        waitingForPressUpEffects = true;
    }



    virtual protected void OnMouseUp(){
        if (active && isPressed) {

            MouseUp();
        }
    }

    virtual protected void MouseUp() {
        isPressed = false;
        pressCompleted = true;

        //execute each action when this button is pressed
        for (int i = 0; i < actions.Count; i++) {
            actions[i]();
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

        AudioSource a = transform.AddComponent<AudioSource>();
        a.clip = btnUpSound;
        a.Play();

        Destroy(a, btnUpSound.length);
    }


    virtual protected void audioDown() {

        AudioSource a = transform.AddComponent<AudioSource>();
        a.clip = btnDownSound;
        a.Play();

        Destroy(a, btnDownSound.length);
    }


    public void setAudioUp(AudioClip a) {
        btnUpSound = a;
    }


    public void setAudioDown(AudioClip a) {
        btnDownSound = a;
    }


    virtual protected void OnMouseExit(){
        isPressed = false;
        if (!pressCompleted) {
            rend.material.color = normalColor;
        }
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
