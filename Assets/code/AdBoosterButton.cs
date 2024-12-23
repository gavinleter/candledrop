using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdBoosterButton : ButtonPress
{

    [SerializeField] float speed;
    [SerializeField] GameManager gameManager;
    [SerializeField] AdSpinnerMenuController adSpinnerMenu;

    [SerializeField] float spawnDelayMin;
    [SerializeField] float spawnDelayMax;

    //if the booster button has been pressed, this will be true to stop the player from getting multiple ad rewards
    bool alreadyPressed = false;
    bool waitingToRespawn = false;
    float initialTime;
    float currentSpawnDelay;

    AdController adController;

    protected override void Start(){
        base.Start();

        initialTime = Time.time;
        adController = GetComponent<AdController>();

        //disable gamemanager so that buttons are not pressed when exiting the ad
        adController.setAdOpenAction(() => {
            gameManager.pause();
        });
        adController.loadRewardedAd();

        onPress(() => {
            alreadyPressed = true;

            bool x = adController.showRewardedAd((Reward r) => {
                adSpinnerMenu.pause();
                adSpinnerMenu.increaseUsageThisGame();
            });

            if (!x) {
                Debug.Log("Failed to show rewarded ad");
                gameManager.unpause();
            }

        });
    }


    protected override void Update(){

        transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);

        //once the button has moved far away enough, prepare to respawn it
        if (!waitingToRespawn && transform.position.x < -10){
            waitingToRespawn = true;
            initialTime = Time.time;
            currentSpawnDelay = UnityEngine.Random.Range(spawnDelayMin, spawnDelayMax);
        }

        if (waitingToRespawn && initialTime + currentSpawnDelay < Time.time) {
            resetBooster();
        }
        
        base.Update();
    }


    protected override void MouseDown(){
        
        if (!alreadyPressed){
            base.MouseDown();
        }
        
    }

    public void resetBooster(){
        waitingToRespawn = false;
        alreadyPressed = false;
        transform.position = new Vector3(10f, transform.position.y, transform.position.z);
    }


}
