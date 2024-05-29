using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for menus that fade in and out
public class FadingMenuController : MonoBehaviour, IMenu
{
    protected float opacity = 0f;
    protected float lerp = 0f;
    protected bool active = false;

    protected SpriteRenderer sr;
    protected SpriteRenderer[] childrenRenderers;
    protected ParticleSystem[] childrenParticles;
    protected CanvasGroup canvasGroup;

    [SerializeField] protected List<ButtonPress> btns = new List<ButtonPress>();


    virtual protected void Start() {
        sr = GetComponent<SpriteRenderer>();
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        childrenParticles = GetComponentsInChildren<ParticleSystem>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    virtual protected void Update() {

        if (active) {
            lerp += 0.1f * Time.deltaTime;
            opacity = Mathf.Lerp(opacity, 1f, lerp);
            setAlpha();
        }
        else if (lerp > 0f) {
            lerp -= 0.1f * Time.deltaTime;
            opacity = Mathf.Lerp(0f, opacity, lerp);
            setAlpha();
        }

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

    virtual public void pause() {
        lerp = 0f;
        active = true;
        for (int i = 0; i < btns.Count; i++) {
            btns[i].active = true;
        }
        for (int i = 0; i < childrenParticles.Length; i++) {
            childrenParticles[i].Play();
        }
    }

    //when the game unpauses, start lerping in reverse
    virtual public void unpause() {
        lerp = 1f;
        active = false;
        for (int i = 0; i < btns.Count; i++) {
            btns[i].active = false;
        }
        for (int i = 0; i < childrenParticles.Length; i++) {
            childrenParticles[i].Clear();
            childrenParticles[i].Stop();
        }
    }


    //if the unpause menu has finished going away, this returns true
    virtual public bool unpauseFinished() {
        return opacity < 0.05f;
    }
}
