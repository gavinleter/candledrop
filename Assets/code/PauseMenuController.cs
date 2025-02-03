using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuController : FadingMenuController
{
    
    [SerializeField] GameObject parentMenuObject;
    IMenu parentMenu;

    [SerializeField] DeleteSaveMenuController deleteMenu;
    [SerializeField] LockedFeatureMenuController lockedFeatureMenu;
    [SerializeField] GameObject achievementMenuObject;

    [SerializeField] GameManager gameManager;

    [SerializeField] Sprite soundEnabledSprite;
    [SerializeField] Sprite soundDisabledSprite;

    [SerializeField] Sprite[] musicButtonSprites;

    [SerializeField] TextMeshProUGUI pointlessAdCounterText;
    [SerializeField] TextMeshProUGUI highScoreText;

    [SerializeField] FailedAdMenuController failedAdMenu;

    AdController adController;

    protected override void Start(){
        base.Start();

        adController = GetComponent<AdController>();

        //this is set so that the user closing the ad doesn't trigger any buttons that might be behind the ad
        adController.setAdOpenAction(async () => {
            await Awaitable.MainThreadAsync();
            unpause();
        });
        adController.loadRewardedAd();

        setSoundButtonSprite();
        setMusicButtonSprite();

        parentMenu = parentMenuObject.GetComponent<IMenu>();

        GameObject mainCam = transform.parent.gameObject;

        //resume button
        btns[0].onPress(() => { 
            unpause();
            parentMenu.unpause();
        });

        //restart button
        btns[1].onPress(() => {
            parentMenuObject.GetComponent<GameManager>().resetGame();
            //mainCam.GetComponent<CameraController>().restartTransition();
            mainCam.GetComponent<CameraController>().fadeToBlackTransitionToTop(0.1f);
            unpause();
            parentMenu.unpause();
        });

        //achievements button
        btns[2].onPress(() => {
            unpause();
            achievementMenuObject.GetComponent<AchievementMenuController>().pause();
        });

        //sound enable/disable button
        btns[3].onPress(() => {
            Settings.toggleSound(!Settings.isSoundEnabled());

            setSoundButtonSprite();

            deafAchievementCheck();
        });

        //delete save data button
        btns[4].onPress(() => {
            unpause();
            deleteMenu.pause();
        });

        //waffle rain button
        btns[5].onPress(() => {

            //toggle waffle rain if unlocked, otherwise bring up locked feature menu
            if (Settings.waffleRainUnlocked()) {

                Settings.toggleWaffleRain(!Settings.isWaffleRainEnabled());
                gameManager.toggleWaffleRain(Settings.isWaffleRainEnabled());
                unpause();
                parentMenu.unpause();

            }
            else {

                unpause();
                lockedFeatureMenu.setDefaultExitAction(this);
                lockedFeatureMenu.pause();

            }

        });

        //snowy button
        btns[6].onPress(() => {

            //toggle snowy if unlocked, otherwise bring up locked feature menu
            if (Settings.snowyUnlocked()) {

                Settings.toggleSnowy(!Settings.isSnowyEnabled());
                gameManager.toggleSnowy(Settings.isSnowyEnabled());
                unpause();
                parentMenu.unpause();

            }
            else {

                unpause();
                lockedFeatureMenu.setDefaultExitAction(this);
                lockedFeatureMenu.pause();

            }

        });

        //music enable/disable button
        btns[7].onPress(() => { 
        
            int x = Settings.getMusicStatus();

            //"Secret Button Hunter" (#27) is the achievement that unlocks the alt track
            int altTrackUnlocked = Settings.altTrackUnlocked() ? 1 : 0;

            //music status is modulo 2 when alt track is locked and modulo 3 when its unlocked
            //because 0 == muted, 1 == default track, 2 == alt track
            Settings.setMusicStatus((x + 1) % (2 + altTrackUnlocked) );
            setMusicButtonSprite();

            deafAchievementCheck();
        
        });

        //rate in app store button
        btns[8].onPress(() => {

            //"Candle Megafan" unlocked by pressing the rate app button
            Settings.setAchievementUnlocked(30);
        
        });

        //pointless ad counter
        btns[9].onPress(() => {

            //reset actions since they may be overwritten by the booster button
            failedAdMenu.setAdLoadedAction(() => {
                showAd();
            });

            failedAdMenu.setExitAction(async () => {
                await Awaitable.MainThreadAsync();
                failedAdMenu.unpause();
                pause();
            });

            failedAdMenu.setAdController(adController);

            bool x = showAd();

            //show pop up if the ad fails to load
            if (!x) {
                Debug.Log("Failed to show ad");
                unpause();
                failedAdMenu.pause();
            }

        });

    }



    override public void pause() {
        base.pause();

        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);
        setTexts();

    }

    
    override public void unpause() {
        base.unpause();
    }


    void setSoundButtonSprite() {
        if (Settings.isSoundEnabled()) {
            btns[3].GetComponent<SpriteRenderer>().sprite = soundEnabledSprite;
        }
        else {
            btns[3].GetComponent<SpriteRenderer>().sprite = soundDisabledSprite;
        }
    }


    void setMusicButtonSprite() {

        btns[7].GetComponent<SpriteRenderer>().sprite = musicButtonSprites[Settings.getMusicStatus()];

    }


    void deafAchievementCheck() {

        if (!Settings.isMusicEnabled() && !Settings.isSoundEnabled()) {
            //"Deaf" unlocks when both music and sound are disabled
            Settings.setAchievementUnlocked(1);
        }
    }


    void setTexts() {

        pointlessAdCounterText.text = "" + Settings.getPointlessAdCount();
        highScoreText.text = "" + Settings.getHighScore();

    }


    bool showAd() {
        return adController.showRewardedAd(async (Reward r) => {
            //menu is closed but opened again after ad is closed
            await Awaitable.MainThreadAsync();
            pause();
            Settings.increasePointlessAdCount();
            setTexts();
        });
    }

}
