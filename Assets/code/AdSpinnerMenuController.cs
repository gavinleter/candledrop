using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdSpinnerMenuController : FadingMenuController
{
    //the last sprite in this is the one used for the spinning animation
    [SerializeField] Sprite[] upgradeSprites;

    [SerializeField] Camera mainCamera;
    [SerializeField] AdSpinnerLever lever;
    [SerializeField] UpgradeDisplayMenuController upgradeDisplayMenu;

    [SerializeField] float startingSpeed;
    [SerializeField] float shakeAmount;

    GameObject emptySpinner;
    SpriteRenderer emptySpinnerSpriteRenderer;
    SpriteRenderer spriteRenderer;
    Animator spinnerAnimator;
    int selectedUpgrade = 0;

    Vector3 initialPosition;
    Vector3 positionOffset = new Vector3();
    bool spinning = false;
    //indicates if the cooldown to close the menu should be started
    bool closeMenu = false;
    float closeDelay = 1;
    float startTime;

    int usesThisGame = 0;


    protected override void Start() {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        emptySpinner = transform.GetChild(1).gameObject;
        emptySpinnerSpriteRenderer = emptySpinner.GetComponent<SpriteRenderer>();
        spinnerAnimator = GetComponent<Animator>();

        sr.enabled = false;
        spinnerAnimator.enabled = false;
    }


    protected override void Update() {
        base.Update();

        if (spinning) {
            positionOffset.x = UnityEngine.Random.Range(-1f, 1f) * spinnerAnimator.speed * shakeAmount;
            positionOffset.y = UnityEngine.Random.Range(-1f, 1f) * spinnerAnimator.speed * shakeAmount;

            transform.position = initialPosition + positionOffset;
        }
        else if (closeMenu && Time.time > startTime + closeDelay) {
            //wait until the spin has stopped and then wait a bit before closing the menu
            unpause();
            upgradeDisplayMenu.setDisplayedUpgrade(selectedUpgrade);
            upgradeDisplayMenu.pause();
        }
    }


    public void startSpin() {
        spriteRenderer.enabled = true;
        emptySpinnerSpriteRenderer.enabled = false;
        spinnerAnimator.enabled = true;
        spinnerAnimator.speed = startingSpeed;
        //reset spinner to beginning of animation
        spinnerAnimator.Play("adspinner_anim", -1, 0);

        initialPosition = transform.position;
        spinning = true;

    }

    //this method is called by the animator when one of the upgrades is centered in the spinner
    public void setAnimationPoint(int x) {
        selectedUpgrade = x;

        spinnerAnimator.speed -= 0.4f * UnityEngine.Random.Range(0.5f, 1);

        if(spinnerAnimator.speed < 0.5f) {

            spinnerAnimator.speed = 0;
            spinnerAnimator.enabled = false;
            spinning = false;
            closeMenu = true;
            startTime = Time.time;
            spriteRenderer.sprite = upgradeSprites[x];

        }

    }

    //resets the counter of how many times this has been used in a single game
    public void resetGame() {
        usesThisGame = 0;
    }


    public override void pause() {
        base.pause();

        lever.enable();
        spriteRenderer.sprite = upgradeSprites[upgradeSprites.Length - 1];
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);

        checkForAchievements();
    }


    public override void unpause() {
        base.unpause();

        lever.disable();
        emptySpinnerSpriteRenderer.enabled = true;
        spriteRenderer.enabled = false;
        closeMenu = false;
    }


    public void increaseUsageThisGame() {
        usesThisGame++;
    }

    //achievements that are only unlocked by using the ad spinner
    void checkForAchievements() {

        //"AD-Vantage" unlocked by using an ad powerup for the first time
        Settings.setAchievementUnlocked(22);

        if (usesThisGame >= 5) {
            //"AD-Vanced tactics" unlocked by using 5 ad powerups in a single game
            Settings.setAchievementUnlocked(23);
        }
    }

}
