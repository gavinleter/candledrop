using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverChain : MonoBehaviour
{

    [SerializeField] GameOverMenuController gameOverMenu;
    [SerializeField] AchievementMenuController achievementMenu;
    [SerializeField] GameManager gameManager;

    //a list of any menu that could potentially be open that needs to be closed before a game over happens
    //this is a list of GameObject rather than IMenu because unity doesn't work with serializing interfaces
    [SerializeField] GameObject[] menusToClose;
    //also need to clear any end transition actions that may happen from closing a menu that starts a transition
    //like how the achievement menu opens the pause menu after transitioning
    [SerializeField] CameraController mainCam;

    [SerializeField] float timeToGameOver;
    [SerializeField] float sensingDelay;

    [SerializeField] LosingVignette losingVignette;

    SpriteRenderer sr;

    float initialTouchTime;

    float lerp = 0;

    bool gameOver = false;

    Collider2D coll;
    ContactFilter2D contactFilter;
    List<Collider2D> touching = new List<Collider2D>();
    int touchingLength = 0;

    float blackHoleSaveTime = float.MinValue;
    float blackHoleSaveDelay = 0.5f;

    float miniSunSaveTime = float.MinValue;
    float miniSunSaveDelay = 0.5f;


    void Start(){
        
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        contactFilter = new ContactFilter2D().NoFilter();
        
    }

  
    void Update(){

        losingVignette.updateParticlePosition(lerp);

        //if currently touching a candle and the initial delay has passed, start lerping
        if (isTouching() && Time.time > initialTouchTime + sensingDelay) {

            lerp = Mathf.Min(lerp + Time.deltaTime / timeToGameOver, 1);

            //if the threshold has reached for a game over
            if(lerp >= 0.7 && !gameOver) {
                gameManager.pause();
                gameManager.setGameOverTime();
                gameOverMenu.setScores(gameManager.getScore(), gameManager.getLastHighScore());
                gameOver = true;

                bool achievementsOpen = achievementMenu.isMenuActive();

                for (int i = 0; i < menusToClose.Length; i++) {
                    menusToClose[i].GetComponent<IMenu>().unpause();
                }

                if (achievementsOpen) {
                    //the achievement menu will attempt to transition and pull up the pause menu if it is closed
                    //this hijacks the end action to pull up the game over menu instead if that happens
                    mainCam.setEndTransitionAction(() => {
                        gameOverMenu.pause();
                    });
                }
                else {
                    //otherwise the game over menu can just be opened immediately
                    gameOverMenu.pause();
                }

            }
            else if (!gameOver){
                losingVignette.startParticles();
            }
            

        }
        else {

            lerp = Mathf.Max(lerp - Time.deltaTime / (timeToGameOver / 2f), 0);
            losingVignette.stopParticles();
        }

        Color newColor = sr.color;
        newColor.a = lerp;
        sr.color = newColor;

    }


    private void FixedUpdate() {

        updateTouchingList();

    }


    void updateTouchingList() {

        //update touching list and last touching time
        bool wasPreviouslyTouching = isTouching();
        touchingLength = coll.Overlap(contactFilter, touching);

        if (!wasPreviouslyTouching && isTouching()) {
            initialTouchTime = Time.time;
        }

        if (blackHoleSaveTime + blackHoleSaveDelay > Time.time && !isAboutToLose()) {
            //"Void Savior" unlocked by preventing a near loss using a black hole
            Settings.setAchievementUnlocked(37);
        }

        if (miniSunSaveTime + miniSunSaveDelay > Time.time && !isAboutToLose()) {
            //"Solar Savior" unlocked by preventing a near loss using a mini sun
            Settings.setAchievementUnlocked(41);
        }

    }


    bool isTouching() {

        CandleLightController can;

        for (int i = 0; i < touchingLength; i++) {

            if (touching[i] != null) {
                can = touching[i].GetComponentInChildren<CandleLightController>();

                if(can && !can.isBeingDestroyed()) {
                    return true;
                }
                
            }

        }

        return false;
    }


    public void resetChain() {
        gameOver = false;
        lerp = 0;
    }


    public bool isAboutToLose() {
        return lerp >= 0.3 && isTouching();
    }


    public void setBlackHoleSaveTime() {
        blackHoleSaveTime = Time.time;
    }


    public void setMiniSunSaveTime() {
        miniSunSaveTime = Time.time;
    }


    public float getChainProgress() {
        return lerp;
    }


    public void setChainProgress(float x) {
        lerp = x;
    }

}
