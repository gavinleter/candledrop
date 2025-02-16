using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerpable : MonoBehaviour
{

    protected float lerp = 0f;
    protected bool lerpingIn = false;

    [SerializeField] protected float lerpSpeed;
    [SerializeField] bool currentlyActive;

    protected float lowerLimit = 0;
    protected float upperLimit = 1;


    protected virtual void Start() {

        //default fading speed
        if (lerpSpeed == 0f) {
            lerpSpeed = 2f;
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
        lerp = Mathf.Min(lerp + (lerpSpeed * Time.deltaTime), upperLimit);
    }


    protected virtual void decreaseLerp() {
        //dont let lerp go below 0
        lerp = Mathf.Max(lerp - (lerpSpeed * Time.deltaTime), lowerLimit);
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
        lerp = upperLimit;
    }


    public virtual void forceLerpOut() {
        lerpOut();
        lerp = lowerLimit;
    }


    public virtual bool lerpOutFinished() {
        return lerp <= lowerLimit + 0.01f;
    }


    public virtual bool lerpInFinished() {
        return lerp >= upperLimit - 0.01f;
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


    virtual public void setLerp(float x) {
        lerp = x;
    }


    virtual public float getLerp() {
        return lerp;
    }


    virtual public void setLimits(float lower, float upper) {
        lowerLimit = lower;
        upperLimit = upper;
    }

}
