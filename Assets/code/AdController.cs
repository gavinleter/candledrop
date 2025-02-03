using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdController : MonoBehaviour
{


    #if UNITY_ANDROID
        int deviceType = 1;
    #elif UNITY_IPHONE
        int deviceType = 2;
    #else
        int deviceType = 0;
    #endif

    [SerializeField] string AndroidAdUnitId;
    [SerializeField] string IOSAdUnitId;
    string adUnitId = "unknown platform";

    [SerializeField] bool rewardedAd;
    [SerializeField] bool bannerAd;

    RewardedAd nextRewardedAd;
    RewardedAd rewardedAdToDestroy;
    float lastRewardedAdLoadTime = 0;

    BannerView nextBannerAd;
    float lastBannerAdLoadTime = 0;

    System.Action adOpenAction;

    static bool initialized = false;
    static bool initStarted = false;

    void Awake() {


        if(deviceType == 1) {
            adUnitId = AndroidAdUnitId;
        } 
        else if(deviceType == 2) {
            adUnitId = IOSAdUnitId;
        }


        //admob only needs to be initialized once
        if (!initStarted) {
            initStarted = true;
            startInit();
        }

    }


    private void Update() {
        
        testForAdRefresh();

    }


    void startInit() {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) => {

            initialized = true;

        });
    }


    //ads are valid for 1 hour so they need to be replaced before they expire
    //give a bit of wiggle room because of loading time and awaiting for main thread to return to use Time.time
    void testForAdRefresh() {
        
        if(rewardedAd && lastRewardedAdLoadTime + 3500 < Time.time) {
            loadRewardedAd();
        }

        if (bannerAd && lastBannerAdLoadTime + 3500 < Time.time) {
            loadBannerAd();
        }

    }


    public async void loadBannerAd() {

        if (!initialized) {
            return;
        }

        //if an ad is already loaded, destroy it
        if (nextBannerAd != null) { 
            nextBannerAd.Destroy();
        }

        AdRequest req = new AdRequest();
        AdSize size = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        nextBannerAd = new BannerView(adUnitId, size, AdPosition.Bottom);

        nextBannerAd.LoadAd(req);
        nextBannerAd.Hide();

        await Awaitable.MainThreadAsync();

        lastBannerAdLoadTime = Time.time;
        
    }


    public void loadRewardedAd() {

        if (!initialized) {
            return;
        }

        //if an ad is already loaded, destroy it
        if(nextRewardedAd != null) {
            nextRewardedAd.Destroy();
        }

        AdRequest req = new AdRequest();


        RewardedAd.Load(adUnitId, req, async (RewardedAd ad, LoadAdError err) => {

            //if the ad failed to load
            if (err != null || ad == null) {
                Debug.LogError("Failed to load rewarded ad: " + err);
                return;
            }

            Debug.Log("Rewarded ad loaded successfully");
            nextRewardedAd = ad;
            nextRewardedAd.OnAdFullScreenContentOpened += adOpenAction;

            //have to return to main thread before using Time.time
            await Awaitable.MainThreadAsync();

            lastRewardedAdLoadTime = Time.time;

        });

    }


    public bool canShowRewardedAd() {
        return nextRewardedAd != null && nextRewardedAd.CanShowAd();
    }


    public bool showRewardedAd(System.Action<Reward> rewardAction) {


        if (canShowRewardedAd()) {

            //destroy the last ad
            destroyRewardedAd();

            //mark the current ad to be deleted later
            rewardedAdToDestroy = nextRewardedAd;
            nextRewardedAd = null;
            rewardedAdToDestroy.Show(rewardAction);

            loadRewardedAd();

            return true;
        }


        return false;

    }


    public bool showBannerAd() {

        if (nextBannerAd != null) {

            nextBannerAd.Show();
            //Debug.Log("Banner ad shown");
            return true;

        }
        //Debug.Log("Failed to show banner ad");
        return false;
    }


    public bool hideBannerAd() {

        if (nextBannerAd != null) {

            nextBannerAd.Hide();
            return true;

        }

        return false;
    }


    void destroyRewardedAd() {

        if (rewardedAdToDestroy != null) {
            rewardedAdToDestroy.Destroy();
            rewardedAdToDestroy = null;
        }

    }


    public void setAdOpenAction(System.Action x) {
        adOpenAction = x;
    }


}
