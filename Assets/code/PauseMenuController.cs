using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour, IMenu
{

    float opacity = 0f;
    float lerp = 0f;
    SpriteRenderer sr;
    SpriteRenderer[] childrenRenderers;
    ParticleSystem[] childrenParticles;
    bool active = false;
    
    [SerializeField] GameObject parentMenuObject;
    IMenu parentMenu;
    [SerializeField] List<ButtonPress> btns;

    [SerializeField] GameObject achievementMenuObject;


    // Start is called before the first frame update
    void Start(){
        sr = GetComponent<SpriteRenderer>();
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        childrenParticles = GetComponentsInChildren<ParticleSystem>();
        parentMenu = parentMenuObject.GetComponent<IMenu>();

        GameObject mainCam = transform.parent.gameObject;

        //resume button
        btns[0].onPress(delegate() { 
            unpause();
            parentMenu.unpause();
            /*if (parentMenuObject.GetComponent<GameManager>().isGameStarted()) {
                parentMenu.unpause();
            }*/
        });

        //restart button
        btns[1].onPress(delegate() {
            parentMenuObject.GetComponent<GameManager>().resetGame();
            mainCam.GetComponent<camCtrl>().restartTransition();
            unpause();
            parentMenu.unpause();
        });

        //achievements button
        btns[2].onPress(delegate() {
            unpause();
            achievementMenuObject.GetComponent<AchievementMenuController>().unpause();
        });

        //sound enable/disable button
        btns[3].onPress(delegate () {
            Settings.soundEnabled = !Settings.soundEnabled;
            btns[3].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = !Settings.soundEnabled;
        });

    }

    // Update is called once per frame
    void Update(){

        if (active) { 
            lerp += 0.1f * Time.deltaTime;
            opacity = Mathf.Lerp(opacity, 1f, lerp);
            setAlpha();
            //Debug.Log(transform.position);
        } else if(lerp > 0f) {
            lerp -= 0.1f * Time.deltaTime;
            opacity = Mathf.Lerp(0f, opacity, lerp);
            setAlpha();
        }

    }


    void setAlpha() {
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
    }

    //when the game pauses, start lerping
    public void pause() {
        lerp = 0f;
        active = true;
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);
        for (int i = 0; i < btns.Count; i++) {
            btns[i].active = true;
        }
        for (int i = 0; i < childrenParticles.Length; i++) {
            childrenParticles[i].Play();
        }
    }

    //when the game unpauses, start lerping in reverse
    public void unpause() {
        lerp = 1f;
        active = false;
        for (int i = 0; i < btns.Count; i++) {
            btns[i].active = false;
        }
        for (int i = 0; i < childrenParticles.Length; i++) { 
            childrenParticles[i].Stop();
        }
        //transform.position = new Vector3(100f, 0f, 0f);
        //parentMenu.pause();
    }


    //if the unpause menu has finished going away, this returns true
    public bool unpauseFinished() {
        return opacity < 0.05f;
    }


}
