using UnityEngine;

public class FailedAdMenuController : FadingMenuController
{

    [SerializeField] SpriteRenderer loadingSprite;
    [SerializeField] float newAdWaitTime;

    float newAdStartTime = 0;
    bool waitingForAd = false;

    AdController adController;

    System.Action adLoadedAction = null;
    System.Action exitAction = null;

    protected override void Start() {
        base.Start();

        loadingSprite.enabled = false;

        //close menu
        btns[0].onPress(() => {
            unpause();
            exitAction();
        });

        //attempt to load a new ad
        btns[1].onPress(() => {

            if (!waitingForAd) {
                waitingForAd = true;
                loadingSprite.enabled = true;

                adController.loadRewardedAd();
                newAdStartTime = Time.time;
            }

        });
    }


    protected override void Update() {
        base.Update();

        if (waitingForAd) {
            //if the timeout hasnt ended
            if (newAdStartTime + newAdWaitTime > Time.time) {

                //if the ad has loaded and can be shown
                if (adController.canShowRewardedAd()) {
                    waitingForAd = false;
                    unpause();
                    adLoadedAction();
                }

            }
            else {

                waitingForAd = false;
                loadingSprite.enabled = false;
            }
        }

    }


    override public void pause() {
        base.pause();

        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, transform.position.z);
    }


    public override void unpause() {
        base.unpause();

        waitingForAd = false;
        loadingSprite.enabled = false;
    }


    public void setAdLoadedAction(System.Action a) {
        adLoadedAction = a;
    }


    public void setExitAction(System.Action a) { 
        exitAction = a;
    }


    public void setAdController(AdController a) {
        adController = a;
    }


}
