using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingObject : MonoBehaviour
{

    protected float opacity = 0f;
    protected float lerp = 0f;
    protected bool active = false;

    protected SpriteRenderer sr;
    protected SpriteRenderer[] childrenRenderers;
    protected ParticleSystem[] childrenParticles;
    protected CanvasGroup canvasGroup;


    virtual protected void Start(){

        sr = GetComponent<SpriteRenderer>();
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        childrenParticles = GetComponentsInChildren<ParticleSystem>();
        canvasGroup = GetComponent<CanvasGroup>();

    }


    virtual protected void Update(){

        if (active) {
            increaseLerp();
        }
        else if (lerp > 0f) {
            decreaseLerp();
        }

    }


    virtual protected void increaseLerp() {
        lerp += 0.1f * Time.deltaTime;
        opacity = Mathf.Lerp(opacity, 1f, lerp);
        setAlpha();
    }


    virtual protected void decreaseLerp() {
        lerp -= 0.1f * Time.deltaTime;
        opacity = Mathf.Lerp(0f, opacity, lerp);
        setAlpha();
    }


    virtual protected void setAlpha() {
        Color temp;

        //set opacity of all children
        for (int i = 0; i < childrenRenderers.Length; i++) {
            temp = childrenRenderers[i].color;
            temp.a = opacity;
            childrenRenderers[i].color = temp;
        }

        //set opacity of this object
        temp = sr.color;
        temp.a = opacity;
        sr.color = temp;
        //set opacity of any canvas elements on this menu
        if (canvasGroup != null) {
            canvasGroup.alpha = opacity;
        }
    }

    virtual public void fadeIn() {
        lerp = 0f;
        active = true;
 
        for (int i = 0; i < childrenParticles.Length; i++) {
            childrenParticles[i].Play();
        }
    }

    //when the object fades out, start lerping in reverse
    virtual public void fadeOut() {
        lerp = 1f;
        active = false;

        for (int i = 0; i < childrenParticles.Length; i++) {
            childrenParticles[i].Clear();
            childrenParticles[i].Stop();
        }
    }

    //instantly make this object appear
    virtual public void forceAppear() {
        fadeIn();
        lerp = 1f;
    }

    //instantly make this object disappear
    virtual public void forceDisappear() {
        fadeOut();
        lerp = 0f;
    }


    //if the object has finished going away, this returns true
    virtual public bool fadeOutFinished() {
        return opacity < 0.05f;
    }
}
