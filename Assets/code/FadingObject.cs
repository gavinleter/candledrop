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
    protected CanvasGroup[] canvasGroup;

    [SerializeField] float fadeSpeed;

    [SerializeField] bool playParticlesOnFadeIn;
    [SerializeField] bool destroyParticlesOnFadeOut;


    virtual protected void Start(){

        sr = GetComponent<SpriteRenderer>();
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        childrenParticles = GetComponentsInChildren<ParticleSystem>(true);
        canvasGroup = GetComponentsInChildren<CanvasGroup>();

        //default fading speed
        if(fadeSpeed == 0f) {
            fadeSpeed = 0.1f;
        }
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
        lerp += fadeSpeed * Time.deltaTime;
        opacity = Mathf.Lerp(opacity, 1f, lerp);
        setAlpha();
    }


    virtual protected void decreaseLerp() {
        lerp -= fadeSpeed * Time.deltaTime;
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

            for (int i = 0; i < canvasGroup.Length; i++) {
                canvasGroup[i].alpha = opacity;
            }

        }
    }

    virtual public void fadeIn() {
        lerp = 0f;
        active = true;

        if (playParticlesOnFadeIn) {
            for (int i = 0; i < childrenParticles.Length; i++) {
                childrenParticles[i].Play();
            }
        }
    }

    //when the object fades out, start lerping in reverse
    virtual public void fadeOut() {
        lerp = 1f;
        active = false;

        if (destroyParticlesOnFadeOut) {
            for (int i = 0; i < childrenParticles.Length; i++) {
                childrenParticles[i].Clear();
                childrenParticles[i].Stop();
            }
        }
    }

    //instantly make this object appear
    virtual public void forceAppear() {
        fadeIn();
        lerp = 1f;
        opacity = 1f;
    }

    //instantly make this object disappear
    virtual public void forceDisappear() {
        fadeOut();
        lerp = 0f;
        opacity = 0f;
    }


    //if the object has finished going away, this returns true
    virtual public bool fadeOutFinished() {
        return opacity < 0.01f;
    }


    virtual public bool fadeInFinished() {
        return opacity > 0.99f;
    }


    public void setSpeed(float speed) {
        fadeSpeed = speed;
    }
}
