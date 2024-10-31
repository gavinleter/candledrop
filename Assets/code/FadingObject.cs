
using UnityEngine;

public class FadingObject : Lerpable
{

    protected float opacity = 0f;

    protected SpriteRenderer sr;
    protected SpriteRenderer[] childrenRenderers;
    protected ParticleSystem[] childrenParticles;
    protected CanvasGroup[] canvasGroup;

    [SerializeField] bool playParticlesOnFadeIn;
    [SerializeField] bool destroyParticlesOnFadeOut;


    protected virtual void Awake() {

        sr = GetComponent<SpriteRenderer>();
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        childrenParticles = GetComponentsInChildren<ParticleSystem>(true);
        canvasGroup = GetComponentsInChildren<CanvasGroup>();

    }


    override protected void Start(){
        base.Start();

    }


    override protected void increaseLerp() {
        base.increaseLerp();
        opacity = Mathf.Lerp(opacity, 1f, lerp);
        setAlpha();
    }


    override protected void decreaseLerp() {
        base.decreaseLerp();
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

    override public void lerpIn() {
        base.lerpIn();
        lerp = 0f;

        if (playParticlesOnFadeIn) {
            for (int i = 0; i < childrenParticles.Length; i++) {
                childrenParticles[i].Play();
            }
        }

        setAlpha();
    }

    //when the object fades out, start lerping in reverse
    override public void lerpOut() {
        base.lerpOut();
        lerp = 1f;

        if (destroyParticlesOnFadeOut) {
            for (int i = 0; i < childrenParticles.Length; i++) {
                childrenParticles[i].Clear();
                childrenParticles[i].Stop();
            }
        }

        setAlpha();
    }

    //instantly make this object appear
    override public void forceLerpIn() {
        base.forceLerpIn();
        opacity = 1f;

        setAlpha();
    }

    //instantly make this object disappear
    override public void forceLerpOut() {
        base.forceLerpOut();
        opacity = 0f;

        setAlpha();
    }


    //if the object has finished going away, this returns true
    virtual public bool fadeOutFinished() {
        return opacity < 0.01f;
    }


    virtual public bool fadeInFinished() {
        return opacity > 0.99f;
    }

}
