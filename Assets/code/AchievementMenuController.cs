using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


class AchievementSection {

    public ButtonPress btn;
    public SpriteRenderer sr;
    public ParticleSystem unlockParticleSystem;
    public ParticleSystem confettiParticleSystem;
    public SpriteRenderer checkMark;
    public AudioSource unlockSound;

    public AchievementSection(GameObject obj, SpriteRenderer checkMark, AudioClip btnUpSound, AudioClip btnDownSound) {

        btn = obj.GetComponent<ButtonPress>();
        sr = obj.GetComponent<SpriteRenderer>();
        this.checkMark = checkMark;
        unlockParticleSystem = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
        confettiParticleSystem = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
        unlockSound = obj.transform.GetChild(1).GetComponent<AudioSource>();

        btn.setActive(false);
        btn.setAudioUp(btnUpSound);
        btn.setAudioDown(btnDownSound);


    }

}


public class AchievementMenuController : MonoBehaviour, IMenu
{

    [SerializeField] GameObject topObject;
    [SerializeField] GameObject bottomObject;
    [SerializeField] ParticleSystem topParticles;
    [SerializeField] ParticleSystem bottomParticles;
    float topBound;
    float bottomBound;

    [SerializeField] GameObject parentMenuObject;
    IMenu parentMenu;
    [SerializeField] List<ButtonPress> btns;
    [SerializeField] Camera mainCam;
    [SerializeField] GameManager gameManager;

    CameraController cameraController;

    Vector3 transitionPosition;
    bool active = false;


    [SerializeField] GameObject achievementContainer;
    AchievementSection[] achs;

    [SerializeField] ParticleSystem achievementHighlightParticleSystem;
    [SerializeField] AudioClip buttonUpSound;
    [SerializeField] AudioClip buttonDownSound;
    [SerializeField] Sprite lockedIcon;
    [SerializeField] Sprite unlockedIcon;

    float lerp = 0;
    [SerializeField] float iconFadeSpeed;

    [SerializeField] TextMeshProUGUI achievementsUnlockedText;
    [SerializeField] TextMeshProUGUI candlesUnlockedText;
    [SerializeField] TextMeshProUGUI skinsUnlockedText;
    [SerializeField] TextMeshProUGUI secretsUnlockedText;
    
    void Awake() {
        cameraController = mainCam.GetComponent<CameraController>();
        topBound = topObject.GetComponent<SpriteRenderer>().bounds.max.y;
        bottomBound = bottomObject.GetComponent<SpriteRenderer>().bounds.min.y;

        initializeAchievements();
    }


    void Start(){

        transitionPosition = new Vector3(topObject.transform.position.x, topBound - cameraController.getCamHeight(), cameraController.transform.position.z);

        //return back up button
        btns[0].onPress(()=> {
            
            unpause();

            transitionBackUp();
            
        });

    } 


    void Update() {

        updateIconFades();

    }


    
    //go back up to where the camera was before
    void transitionBackUp() {
        if (gameManager.isGameStarted()) {
            //mainCam.GetComponent<CameraController>().transitionToBottom(60f);

            cameraController.fadeToBlackTransitionToBottom(0.1f, () => {
                parentMenuObject.GetComponent<IMenu>().pause();
            });

        }
        else {
            //mainCam.GetComponent<CameraController>().transitionToTop(60f);

            cameraController.fadeToBlackTransitionToTop(0.1f, () => {
                parentMenuObject.GetComponent<IMenu>().pause();
            });

        }
    }


    public void unpause() {
        //this method can be called when a game over happens to boot the player out of this menu
        //so the transition back up should not happen if the player wasn't in this menu to begin with
        if (active) {
            transitionBackUp();
        }

        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(false);
        }

        for (int i = 0; i < achs.Length; i++) {
            achs[i].btn.setActive(false);
        }

        active = false;
        
    }


    public void pause() {
        active = true;

        refreshAchievements();

        //mainCam.GetComponent<CameraController>().setNewTarget(transitionPosition, 60f);
        //mainCam.GetComponent<CameraController>().startTransition();

        cameraController.fadeToBlackTransition(transitionPosition, 0.1f, () => {
            cameraController.toggleScrollMode(true, topBound, bottomBound);

            //make the top and bottom particles play when the user tries to scroll too far
            cameraController.setScrollModeLimitActions(() => {
                if (!topParticles.IsAlive()) {
                    topParticles.Emit(1);
                }
            },
            () => {
                if (!bottomParticles.IsAlive()) {
                    bottomParticles.Emit(1);
                }
            });
        });
        

        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(true);
        }
    }


    public bool isMenuActive() {
        return active;
    }


    void initializeAchievements() {

        achs = new AchievementSection[achievementContainer.transform.childCount];
        GameObject[] achievementContainers = new GameObject[achievementContainer.transform.childCount];

        for (int i = 0; i < achievementContainers.Length; i++) {
            achievementContainers[i] = achievementContainer.transform.GetChild(i).gameObject;
        }


        GameObject obj;
        GameObject checkMark;

        for (int i = 0; i < achs.Length; i++) {

            obj = achievementContainers[i].transform.GetChild(0).gameObject;
            checkMark = achievementContainers[i].transform.GetChild(3).gameObject;

            achs[i] = new AchievementSection(obj, checkMark.GetComponent<SpriteRenderer>(), buttonUpSound, buttonDownSound);
            
        }

        setButtonMethods();
    }


    void setButtonMethods() {

        for (int i = 0; i < achs.Length; i++) {
            int x = i;
            achs[i].btn.onPress(() => {

                if (Settings.isSoundEnabled()) {
                    achs[x].unlockSound.Play();
                }

                achs[x].confettiParticleSystem.Play();
                setAchievementTapped(x);
            });

        }

    }


    void refreshAchievements() {

        for (int i = 0; i < achs.Length; i++) {

            achs[i].sr.enabled = true;
            achs[i].btn.setActive(false);
            achs[i].checkMark.enabled = false;

            if (Settings.isAchievementUnlocked(i)) {

                if (Settings.isAchievementTapped(i)) {
                    achs[i].sr.enabled = false;
                    achs[i].checkMark.enabled = true;
                    achs[i].unlockParticleSystem.Stop();
                    achs[i].unlockParticleSystem.Clear();
                }
                else {
                    achs[i].sr.sprite = unlockedIcon;
                    achs[i].btn.setActive(true);
                    achs[i].unlockParticleSystem.Play();
                }

            }
            else {
                achs[i].sr.sprite = lockedIcon;
                achs[i].unlockParticleSystem.Stop();
                achs[i].unlockParticleSystem.Clear();
            }

        }

        refreshAchievementStats();

    }


    void refreshAchievementStats() {

        string totalAchievements = "" + Settings.achievementsUnlockedCount();
        string totalSkins = "" + Settings.skinsUnlockedCount();
        string totalCandles = "" + Settings.candlesUnlockedCount();
        string totalSecrets = "" + Settings.secretsUnlockedCount();

        //add a 0 so the number always has 2 digits
        if(totalAchievements.Length == 1) {
            totalAchievements = "0" + totalAchievements;
        }

        if (totalSkins.Length == 1) { 
            totalSkins = "0" + totalCandles;
        }

        achievementsUnlockedText.text = totalAchievements;
        skinsUnlockedText.text = totalSkins;
        candlesUnlockedText.text = totalCandles;
        secretsUnlockedText.text = totalSecrets;

    }


    void updateIconFades() {

        lerp += Time.deltaTime * iconFadeSpeed;
        lerp = lerp % 1f;

        Color x;
        for (int i = 0; i < achs.Length; i++) {
            if (!Settings.isAchievementUnlocked(i)) {
                x = achs[i].sr.color;
                //each icon has a slight offset in transparency from one another
                //the values of transparency range from 0.875 to 1
                x.a = 0.875f + (Mathf.Cos(0.4f * (i + lerp * 5) * Mathf.PI)) / 8f;

                achs[i].sr.color = x;
            }

        }


    }


    void setAchievementTapped(int x) {

        Settings.setAchievementTapped(x);
        refreshAchievements();

    }



}
