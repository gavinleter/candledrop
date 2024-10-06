using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerpable : MonoBehaviour
{

    protected float lerp = 0f;
    protected bool lerpingIn = false;

    [SerializeField] protected float lerpSpeed;
    [SerializeField] bool currentlyActive;


    protected virtual void Start() {

        //default fading speed
        if (lerpSpeed == 0f) {
            lerpSpeed = 0.1f;
        }

    }


    protected virtual void Update() {

        if (lerpingIn && currentlyActive) {
            increaseLerp();
        }
        else if (lerp > 0f && currentlyActive) {
            decreaseLerp();
        }

    }


    protected virtual void increaseLerp() {
        //dont let lerp go past 1
        lerp = Mathf.Min(lerp + (lerpSpeed * Time.deltaTime), 1);
    }


    protected virtual void decreaseLerp() {
        //dont let lerp go below 0
        lerp = Mathf.Max(lerp - (lerpSpeed * Time.deltaTime), 0);
    }


    public virtual void lerpIn() {
        lerpingIn = true;
        currentlyActive = true;
    }


    public virtual void lerpOut() {
        lerpingIn = false;
        currentlyActive = true;
    }


    public virtual void forceLerpIn() {
        lerpIn();
        lerp = 1;
    }


    public virtual void forceLerpOut() {
        lerpOut();
        lerp = 0;
    }


    public virtual bool lerpOutFinished() {
        return lerp == 0;
    }


    public virtual bool lerpInFinished() {
        return lerp == 1;
    }


    public void setSpeed(float speed) {
        lerpSpeed = speed;
    }


    public void setActive(bool currentlyActive) {
        this.currentlyActive = currentlyActive;
    }

    public bool isActive() {
        return currentlyActive;
    }

}
