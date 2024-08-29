using System.Collections;
using System.Collections.Generic;
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


    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        
        setSoundButtonSprite();

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
            achievementMenuObject.GetComponent<AchievementMenuController>().unpause();
        });

        //sound enable/disable button
        btns[3].onPress(() => {
            Settings.toggleSound(!Settings.isSoundEnabled());

            setSoundButtonSprite();

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

    }


    //when the game pauses, start lerping
    override public void pause() {
        base.pause();

        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);

    }

    //when the game unpauses, start lerping in reverse
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

}
