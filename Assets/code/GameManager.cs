using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class GameManager : MonoBehaviour, IMenu
{
    public GameObject[] canObjects = new GameObject[12];
    public Transform teleCoords;
    float minTurnDuration = 0.0f; // Minimum turn duration in seconds
    float velocityCheckDelay = 0.05f; // Delay before checking velocity

    private GameObject selectedCan;
    //the initial candle that needs to be dropped at the title screen
    private GameObject starterCandle;
    private bool isTurnActive = false;
    private bool canMove = false;
    private float turnStartTime;
    private float lastMoveTime;
    private bool hasMoved = false;

    private bool gameStarted = false;
    //user should not be able to press pause buttons while transitioning to credits or basement
    private bool canPause = true;
    bool menuActive = true;

    [SerializeField] public float blackHoleDestroyRadius = 0f;

    private static List<CandleLightController> currentCandles = new List<CandleLightController>();
    [SerializeField] List<ButtonPress> buttons = new List<ButtonPress>();
    //for special non candle objects like the black hole
    private List<ISpecialObject> specialObjects = new List<ISpecialObject>();

    GameObject startingCandlePrefab;
    //Sprite startingCandleSkin;
    int currentSkinId = 0;
    int currentCandlePrefabId = 0;


    bool eventHorizonEventActive = false;
    float eventHorizonEventStartTime;
    [SerializeField] float blackHoleSpawnDelay;
    [SerializeField] float eventHorizonEventDuration;
    [SerializeField] ParticleSystem[] eventHorizonParticles;

    //the mini sun event is what the solar rain event is currently
    bool miniSunEventActive = false;
    float miniSunEventStartTime;
    [SerializeField] float miniSunSpawnDelay;
    [SerializeField] float miniSunEventDuration;

    //old unused event
    bool solarRainEventActive = false;
    float solarRainEventStartTime;
    float solarRainEventLastEmberTime;
    float solarRainEventNextEmberDelay;
    [SerializeField] float solarRainEventDuration;
    [SerializeField] float solarRainEventSpawnDelayRange;
    [SerializeField] ParticleSystem[] solarRainParticles;


    [SerializeField] float startingCandleGravity;
    [SerializeField] Camera mainCamera;

    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] FadingObject startingFloor;

    [SerializeField] GameObject creditsTransitionLocation;
    [SerializeField] GameObject basementTransitionLocation;

    [SerializeField] GameObject leftCandlePlacementLimit;
    [SerializeField] GameObject rightCandlePlacementLimit;

    [SerializeField] GameObject emberPrefab;

    [SerializeField] GameObject[] starterCandles;

    [SerializeField] SkinSelectMenuController skinSelectMenu;
    [SerializeField] LockedFeatureMenuController lockedFeatureMenu;
    [SerializeField] AdSpinnerMenuController adSpinnerMenu;

    [SerializeField] MusicManager musicManager;

    [SerializeField] GameObject startingCandleSpawnLocation;

    SkinManager skinManager;

    [SerializeField] AudioClip snowyChirpSound;

    [SerializeField] GameOverChain gameOverChain;
    [SerializeField] GameOverMenuController gameOverMenuController;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    int currentScore = 0;
    int lastHighScore = 0;

    [SerializeField] Sprite[] bonusTexts;
    [SerializeField] GameObject bonusTextPrefab;
    [SerializeField] GameObject defaultBonusTextLocation;

    [SerializeField] GameObject waffleRainParentObject;
    [SerializeField] GameObject snowyParentObject;

    [SerializeField] CandleRowDestroyer leftWall;
    [SerializeField] RightWall rightWall;

    //the size and color that candles lerp to when they are destroyed by a black hole
    [SerializeField] Vector3 candleBlackHoleDestructionSize;
    [SerializeField] Color candleBlackHoleDestructionColorFade;

    [SerializeField] ParticleSystem guideArrows;

    [SerializeField] LosingVignette losingVignette;

    [SerializeField] float chanceForFlare;
    [SerializeField] bool debugMode;
    //if debug mode is on the candle that spawns will be chosen by candleToSpawn
    [SerializeField] int candleToSpawn;

    //used to determine how long a game has been running for achievements 8 and 9
    float timeSinceGameStarted = 0;
    float timeOfLastActivity = 0;
    //how long the player can do nothing for until the timer stops going up
    float allowedIdleTime = 60;
    //only set when the game ends from a candle touching the chain for too long
    bool trueGameOver = true;

    List<int> spawnPotentials = new List<int>();


    //unlocks all achievements at game start
    [SerializeField] bool grantAllAchievements;

    [SerializeField] UnlockPopUpMenuController unlockPopUpMenu;
    //in the case that an achievement is unlocked while in a menu and cant be displayed immediately, this
    //is used to store what achievement was most recently unlocked so it can be displayed when the conditions are right
    //-1 means there are no achievements to display
    int nextAchievementPopupToDisplay = -1;


    [SerializeField] ParticleSystem nFallingParticle;
    [SerializeField] Sprite normalTitleSprite;
    [SerializeField] Sprite nlessTitleSprite;
    [SerializeField] SpriteRenderer titleObject;
    //keeps track of when the user has stayed idle on the top part of the game for achievement 3
    float topIdleInitialTime = 0;
    float allowedTopIdleTime = 180;
    bool nTopIdle = false;

    [SerializeField] FailedSaveMenuController failedSaveMenu;

    AdController adController;
    float bannerInitialTime = 0;
    bool bannerToggled = false;
    [SerializeField] float bannerSpawnDelay;
    [SerializeField] float bannerShowDuration;


    private void Start()
    {

        setSpawnPotentials();

        skinManager = GetComponent<SkinManager>();
        adController = GetComponent<AdController>();
        adController.loadBannerAd();

        //get the starting candle prefab and skin
        setStarterCandle(Settings.getStarterCandleId(), Settings.getStarterCandleSkinId());

        musicManager.maxOutMusicVolume();

        //the credits return button can be seen at game start on some taller devices, so it needs to be hidden
        buttons[4].GetComponent<SpriteRenderer>().enabled = false;

        //pause the game and pull up pause menu when a settings button is pressed
        System.Action settingsAction = () => {
            if (canPause) {
                pause();
                pauseMenuObject.GetComponent<IMenu>().pause();
            }
        };

        //both the top and bottom pause buttons
        buttons[0].onPress(settingsAction);
        buttons[1].onPress(settingsAction);
        //skip the intro transition when the game starts if the screen is pressed (invisible over intro area)
        buttons[2].onPress(() => {
            mainCamera.GetComponent<CameraController>().skipIntroTransition();
        });
        //credits button
        buttons[3].onPress(() => {
            if (canPause) {
                //credits button is normally hiden from view on game start
                buttons[4].GetComponent<SpriteRenderer>().enabled = true;
                nTopIdle = false;
                canPause = false;
                mainCamera.GetComponent<CameraController>().setNewTarget(creditsTransitionLocation.transform.position);
                mainCamera.GetComponent<CameraController>().startTransition();
            }
        });
        //button to go back down from credits
        buttons[4].onPress(() => {
            resetNTopIdleTimer();
            canPause = true;
            mainCamera.GetComponent<CameraController>().transitionToTop(20f);
        });
        //button to go to the basement
        buttons[5].onPress(() => {
            if (canPause) {
                //trying to enter the achievement menu here causes issues with the canPause variable, so its disabled
                unlockPopUpMenu.unpause();
                nTopIdle = false;
                canPause = false;
                mainCamera.GetComponent<CameraController>().setNewTarget(basementTransitionLocation.transform.position, 40f);
                mainCamera.GetComponent<CameraController>().startTransition();
                //mainCamera.GetComponent<CameraController>().fadeToBlackTransition(basementTransitionLocation.transform.position, 0.1f);
                refreshBasementCandleCovers();
            }
        });
        //button to go back up from basement
        buttons[6].onPress(() => {
            resetNTopIdleTimer();
            canPause = true;
            //mainCamera.GetComponent<CameraController>().transitionToTop(40f);
            mainCamera.GetComponent<CameraController>().fadeToBlackTransitionToTop(0.1f);
        });
        //button to drop a candle (invisible over game area)
        buttons[7].onPress(() => {
            dropCandle();

            guideArrows.Stop();
        });
        //if the player holds on the drop candle button, the guide arrows appear
        buttons[7].onMouseStay(() => {

            if (selectedCan != null && !hasMoved) {

                if (!guideArrows.isPlaying) {
                    guideArrows.Play();
                }

                float mouseXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                Vector3 newPosition = new Vector3(mouseXPos, selectedCan.transform.position.y, selectedCan.transform.position.z);
                selectedCan.transform.position = clampObjectPositionToGameArea(newPosition, selectedCan);

                guideArrows.transform.position = selectedCan.transform.position;

            }
            
        });
        


        //generates an anonymous method for each candle prefab select button
        System.Func<int, System.Action> candlePrefabSelectionButton = (int id) => {
            return delegate () {

                if (Settings.candleUnlocked(id)) {
                    currentCandlePrefabId = id;
                    pause();
                    skinSelectMenu.pause();
                }
                else {
                    lockedFeatureMenu.setExitAction(delegate () {
                        lockedFeatureMenu.unpause();
                        unpause();
                    });

                    pause();
                    lockedFeatureMenu.pause();
                }

            };
        };

        //adds each respective anonymous method and then swaps the skin of each button if its unlocked
        for(int i = 0; i < 5; i++) {
            buttons[i + 8].onPress(candlePrefabSelectionButton(i));
        }

        //buttons 13, 14, and 15 dont need their onPress methods set because they use the SecretButton and WaffleButton class for their behaviour

        //snowy button
        Animator animator = buttons[16].GetComponent<Animator>();
        AudioSource audioSource = buttons[16].gameObject.AddComponent<AudioSource>();
        audioSource.clip = snowyChirpSound;
        buttons[16].onPress(delegate () {

            if (!audioSource.isPlaying) {

                animator.SetTrigger("chirp");
                audioSource.Play();

            }

        });

        //button 17 doesn't need its onPress method set because it uses the AdBoosterButton class

        //the inivisble button that drops the initial candle on the title screen
        buttons[18].onPress(() => {
            //stop buttons from being pressed while initial game transition is happening
            pause();

            getInitialCandle().GetComponent<StartCandleFall>().dropCandle();
            losingVignette.clearParticles();
        });

        //tap glass button
        buttons[19].onPress(() => {
            
            //"Don't tap it!" unlocked by tapping the stained glass
            Settings.setAchievementUnlocked(34);

        });

        buttons[20].onPress(() => {
            //startSolarRainEvent();
            startMiniSunEvent();
        });
        buttons[21].onPress(() => {
            startFlaringFieldsEvent();
        });

        buttons[22].onPress(() => {
            pause();
            gameOverMenuController.setScores(getScore(), getLastHighScore());
            gameOverMenuController.pause();
        });

        buttons[23].onPress(() => {
            string x = "";
            for (int i = 0; i < currentCandles.Count; i++) {
                x = i + ": ";
                if (currentCandles[i] != null) {
                    x += currentCandles[i].getParentObject().name;
                }
                else {
                    x += "null";
                }

                /*x += " | ";

                if (currentCandles[i] != null &&  getCandleById(currentCandles[i].getId()) != null) {
                    x += getCandleById(currentCandles[i].getId()).getParentObject().name;
                }
                else {
                    x += "null";
                }*/
                Debug.Log(x);
            }

        });

        buttons[24].onPress(() => { 
            
            /*for(int i = 0; i < currentCandles.Count; i++) {
                if(currentCandles[i] == null) {
                    Debug.Log("this candle slot is empty");
                }
                else {
                    //currentCandles[i].getCandleIgniter().printTouchingList();
                    currentCandles[i].printCollisionsList();
                }
            }*/
            destroyAllCandles(true);
            
        });

        buttons[25].onPress(() => {
            //pause();
            //adSpinnerMenu.pause();
            //convertAllCandlesToFlares();
            //spawnEmber(-1.5f);
            startEventHorizonEvent();
        });




        Settings.initSettings(this);

        //check if there is save data and load it if there is
        if (SaveManager.initSaveData()) {
            resetGame(true, false);

            try {
                loadFromSave();
                mainCamera.GetComponent<CameraController>().skipIntroToBottom();
                gameStarted = true;

            } catch(Exception e) {
                Debug.LogError(e);
                Debug.LogWarning("Invalid save data");
                SaveManager.clearSaveData();

                //start game normally if loading from save fails
                resetGame(true);
            }

        }
        //if there isnt, start the game normally
        else {
            resetGame(true);
        }

        if(Settings.loadFromSaveFailed() || SaveManager.loadFromSaveFailed()) {
            openFailedSaveMenu();
        }
        

    }

    private void Update()
    {

        if (isTurnActive)
        {

            Rigidbody2D rb = null;
            if (selectedCan != null) {
                rb = selectedCan.GetComponent<Rigidbody2D>();
            }

            //velocityCheckDelay is important because the candle starts out at 0 velocity, so we have to wait for it to fall a bit first
            //the next "candle" can also spawn if an event horizon/mini sun event is active and a shorter timer runs out while the previous black hole is moving still
            if ( rb == null || (canMove && Time.time - lastMoveTime >= velocityCheckDelay && rb.linearVelocity.magnitude < 0.01f) || 
                (eventHorizonEventActive || miniSunEventActive) && Time.time - lastMoveTime >= blackHoleSpawnDelay && rb.linearVelocity.magnitude > 0.1f)
            {
                //object has stopped moving, start a new turn
                StartTurn();
            }
        }

        updateEvents();
        updateIdleTimers();
        updateBannerTimer();

    }


    //attempt to drop the candle if it has spawned, other dropped candles aren't moving, and a time delay has finished between now and the last drop
    private void dropCandle() {

        timeOfLastActivity = Time.time;
        
        if (isTurnActive && Time.time - turnStartTime >= minTurnDuration && !hasMoved) {
            Rigidbody2D rb = rb = selectedCan.GetComponent<Rigidbody2D>();

            Vector3 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tapPosition.z = 0;

            if (selectedCan != null && pauseMenuObject.GetComponent<PauseMenuController>().unpauseFinished()) {
                selectedCan.transform.position = new Vector3(teleCoords.position.x, teleCoords.position.y, 0);

                rb.gravityScale = 1;
                rb.bodyType = RigidbodyType2D.Dynamic;

                Vector3 newPosition = new Vector3(tapPosition.x, selectedCan.transform.position.y, 0);

                selectedCan.transform.position = clampObjectPositionToGameArea(newPosition, selectedCan);

                canMove = true;
                lastMoveTime = Time.time + velocityCheckDelay;
                hasMoved = true;
            }
        }

    }


    //used to ensure that a candle or ember being spawned does not go too far to the left or right of the screen
    private Vector3 clampObjectPositionToGameArea(Vector3 initialPosition, GameObject obj) {
        Vector3 newPosition = initialPosition;

        //check if the tapped position is too far off screen first and adjust if necessary
        if (initialPosition.x + obj.GetComponent<Collider2D>().bounds.size.x / 2 > rightCandlePlacementLimit.transform.position.x) {

            newPosition.x = rightCandlePlacementLimit.transform.position.x - obj.GetComponent<Collider2D>().bounds.size.x / 2;
        }
        if (initialPosition.x - obj.GetComponent<Collider2D>().bounds.size.x / 2 < leftCandlePlacementLimit.transform.position.x) {

            newPosition.x = leftCandlePlacementLimit.transform.position.x + obj.GetComponent<Collider2D>().bounds.size.x / 2;
        }

        return newPosition;
    }


    //spawn a new candle to be dropped
    public void StartTurn(){

        nTopIdle = false;

        //reset activity timer when the game starts
        if (!gameStarted) {
            timeSinceGameStarted = 0;
        }

        gameStarted = true;
        int randomIndex = spawnPotentials[UnityEngine.Random.Range(0, spawnPotentials.Count)];
        //randomIndex = 1;

        if (debugMode) {
            randomIndex = candleToSpawn;
        }

        //only black holes can spawn during the event horizon event
        if (eventHorizonEventActive) {
            randomIndex = canObjects.Length - 2;
        }
        //only mini suns can spawn during solar rain event
        else if (miniSunEventActive) {
            randomIndex = canObjects.Length - 1;
        }

        //if the index is -1, spawn the starter candle again
        if(randomIndex == -1) {
            selectedCan = Instantiate(startingCandlePrefab, teleCoords.position, Quaternion.identity);
            Destroy(selectedCan.GetComponent<StartCandleFall>());
        }
        else {
            selectedCan = Instantiate(canObjects[randomIndex], teleCoords.position, Quaternion.identity);
        }

        prepareSelectedCandle();

        //if a candle has spawned instead of a special item like the black hole, add it to the list to keep track of
        if (selectedCan.GetComponentInChildren<CandleLightController>() != null) {
            addCandleLight(selectedCan);
            setCandleId(selectedCan, randomIndex);

            //random chance for the candle to turn into a flare
            if (chanceForFlare > UnityEngine.Random.Range(0f, 1f)) {

                CandleLightController[] c = selectedCan.GetComponentsInChildren<CandleLightController>();

                for(int i = 0; i < c.Length; i++) {
                    c[i].convertToFlare();
                }

            }

        } 
        else if(selectedCan.GetComponent<ISpecialObject>() != null) {

            int specialObjId = nextSpecialObjectId();
            selectedCan.GetComponent<ISpecialObject>().setup(this, specialObjId);
            specialObjects[specialObjId] = selectedCan.GetComponent<ISpecialObject>();
        }

        //every time a new held candle spawns, update the save data
        updateSaveData();
    }


    //set a bunch of values for selectedCan when it is created so it doesnt immediately fall
    void prepareSelectedCandle() {
        selectedCan.SetActive(true);

        Rigidbody2D rb = selectedCan.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        canMove = false;
        isTurnActive = true;
        turnStartTime = Time.time;
        hasMoved = false;
    }


    public void addCandleLight(GameObject candle) {
        //add the created candles lights to the list for candleRowDestroyer to reference by id later on
        //the for loop is in the case a candle has multiple lights
        CandleLightController[] c = candle.GetComponentsInChildren<CandleLightController>();

        for (int i = 0; i < c.Length; i++) {
            int id = nextCandleId();
            c[i].assignId(id);
            currentCandles[id] = c[i];
            candle.name += " ID: " + c[i].getId();
            c[i].gameObject.name += " ID: " + c[i].getId();
        }
    }


    //returns the index of the next empty slot available for a candle light in the currentCandles array
    public int nextCandleId() {

        for (int i = 0; i < currentCandles.Count; i++) {
            if (currentCandles[i] == null) {
                return i;
            }
        }

        currentCandles.Add(null);
        return currentCandles.Count - 1;
    }


    //destroys the held candle and starts a new turn, but does not destroy a held special object
    void destroyHeldCandle() {

        if (selectedCan != null && selectedCan.GetComponentInChildren<CandleLightController>() != null) {
            //if the candle has not been dropped yet, destroy it
            if (!canMove) {
                //int heldCandleId = selectedCan.GetComponentInChildren<CandleLightController>().getId();
                destroyCandle(selectedCan);
            }
            //always spawn the next "candle" in case the previously dropped one is still moving
            //this is to stop a moving candle from taking the event horizon event hostage
            StartTurn();
        }

    }


    public void destroyCandle(GameObject can) {
        destroyCandle(can, false);
    }


    public void destroyCandle(GameObject can, bool destroyedByBlackHole) {

        if (destroyedByBlackHole && gameOverChain.isAboutToLose()) {
            gameOverChain.setBlackHoleSaveTime();
        }

        CandleLightController[] c = can.GetComponentsInChildren<CandleLightController>();

        for (int i = 0; i < c.Length; i++) {
            c[i].startDestroy();

            if (!destroyedByBlackHole && c[i].isMiniSunIgnited() && gameOverChain.isAboutToLose()) {
                gameOverChain.setMiniSunSaveTime();
            }
        }

        if (destroyedByBlackHole) {

            ColorFadingObject col = can.AddComponent<ColorFadingObject>();
            GrowingObject grow = can.AddComponent<GrowingObject>();

            col.setTargetColor(candleBlackHoleDestructionColorFade);
            col.canChangeAlpha(true);
            grow.setTargetSizeMultiplier(candleBlackHoleDestructionSize);
            col.setSpeed(2f);
            grow.setSpeed(0.7f);
            col.setActive(true);
            grow.setActive(true);
            col.lerpIn();
            grow.lerpIn();

            Rigidbody2D canRB = can.GetComponent<Rigidbody2D>();
            canRB.gravityScale = -0.1f;
            canRB.AddTorque(UnityEngine.Random.Range(-5f, 5f));

            can.GetComponent<SpriteRenderer>().sortingOrder = 5;

            Destroy(can, 1.5f);

        }
        else {
            Destroy(can);
        }

    }


    void destroyAllCandles() {
        destroyAllCandles(false);
    }

    void destroyAllCandles(bool x) {
        for (int i = 0; i < currentCandles.Count; i++) {
            if (currentCandles[i] != null) {
                destroyCandle(currentCandles[i].getParentObject(), x);
            }
        }
        currentCandles.Clear();
    }


    public void pause() {
        nTopIdle = false;
        menuActive = false;
        isTurnActive = false;

        adController.hideBannerAd();

        if(getStartingCandleObject() != null) {
            getStartingCandleObject().GetComponent<StartCandleFall>().setReadyToDrop(false);
        }

        for (int i = 0; i < buttons.Count; i++) {
            //the tap glass button is the only one that should never be disabled
            if (i == 19) {
                continue;
            }

            buttons[i].setActive(false);
        }

        //make all particles from buttons stop playing
        for(int i = 0; i < buttons.Count; i++) {

            //the particles on the ad booster button should never stop
            if (buttons[i] is AdBoosterButton) {
                continue;
            }

            ParticleSystem[] ps = buttons[i].GetComponentsInChildren<ParticleSystem>();
            for (int j = 0; j < ps.Length; j++) { 
                ps[j].Stop();
                ps[j].Clear();
            }
        }

        //unlock pop up menu should never be active when this isnt
        unlockPopUpMenu.unpause();
    }


    public void unpause() {
        
        //if the banner should be shown and the game is active
        if (bannerToggled && gameStarted) {
            adController.showBannerAd();
        }

        if (!gameStarted) {
            resetNTopIdleTimer();
        }

        menuActive = true;

        if (getStartingCandleObject() != null) {
            getStartingCandleObject().GetComponent<StartCandleFall>().setReadyToDrop(true);
        }

        if (gameStarted) {
            isTurnActive = true;
        }

        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].setActive(true);
        }

        //make all particles from buttons start playing again
        for (int i = 0; i < buttons.Count; i++) {

            //the exception are secretbuttons since this code would make all found text appear after unpausing
            if(buttons[i] is SecretButton || buttons[i] is WaffleButton) {
                continue;
            }

            ParticleSystem[] ps = buttons[i].GetComponentsInChildren<ParticleSystem>();
            for (int j = 0; j < ps.Length; j++) {
                ps[j].Play();
            }
        }

        //if theres an achievement that needs to be displayed, show it
        if(nextAchievementPopupToDisplay != -1) {
            achievementUnlockPopup(nextAchievementPopupToDisplay);
            nextAchievementPopupToDisplay = -1;
        }

    }


    public bool isMenuActive() {
        return menuActive;
    }


    public bool isGameStarted() {
        return gameStarted;
    }


    public void resetGame() {
        resetGame(false, true);
    }

    public void resetGame(bool initialStart) {
        resetGame(initialStart, true);
    }

    public void resetGame(bool initialStart, bool spawnStarterCandle) {
        
        if (spawnStarterCandle) {
            resetNTopIdleTimer();
        }

        checkForGameEndAchievments(initialStart);

        selectedCan = null;
        isTurnActive = false;
        gameStarted = false;
        trueGameOver = false;

        lastHighScore = Settings.getHighScore();
        highScoreText.text = "" + lastHighScore;
        setScore(0);

        startingFloor.forceLerpIn();

        destroyAllCandles();

        //clear all special objects
        for (int i = 0; i < specialObjects.Count; i++) {
            if(specialObjects[i] != null) {
                specialObjects[i].destroySelf();
            }
        }
        specialObjects.Clear();

        gameOverChain.resetChain();

        //not everything may be initialized when this is called at the start because both fadingObject
        //and starting the game use the Start() method, so these aren't called if the game is doing its
        //initial game reset when the game first opens
        if (!initialStart) {
            adSpinnerMenu.unpause();
            pauseMenuObject.GetComponent<IMenu>().unpause();
            lockedFeatureMenu.unpause();

            //clear save data when restarting the game
            //since save data is also cleared immediately when the game over menu appears,
            //this line should only ever clear the save data when the user restarts rather than loses
            SaveManager.clearSaveData();
        }

        adSpinnerMenu.resetGame();

        //reset starter candle and skin in case the player wipes data and resets
        setStarterCandle(Settings.getStarterCandleId(), Settings.getStarterCandleSkinId());

        if (spawnStarterCandle) {
            starterCandle = Instantiate(startingCandlePrefab, startingCandleSpawnLocation.transform.position, Quaternion.identity);
            starterCandle.GetComponent<CandleId>().setInfo(-1, true);
            starterCandle.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera);
        }

    }


    //this returns the candle that needs to be dropped from the title screen
    //only used for the button that drops this candle
    public GameObject getInitialCandle() {
        return starterCandle;
    }


    private void convertAllCandlesToFlares() {

        for(int i = 0; i < currentCandles.Count; i++) {
            if (currentCandles[i] != null && !currentCandles[i].isCurrentlyFlare()) {

                currentCandles[i].convertToFlare();

            }
        }

    }


    private void spawnEmber(float yOffset) {

        GameObject x = Instantiate(emberPrefab);
        x.transform.position = new Vector3(teleCoords.position.x + UnityEngine.Random.Range(-1f, 1f), teleCoords.position.y + yOffset, x.transform.position.z);
        for (int i = 0; i < buttons.Count; i++) {
            Physics2D.IgnoreCollision(x.GetComponent<Collider2D>(), buttons[i].GetComponent<Collider2D>());
        }
        //x.transform.position = clampObjectPositionToGameArea(x.transform.position, x);

    }


    //make only black holes spawn for a limited time
    public void startEventHorizonEvent() {
        eventHorizonEventActive = true;
        eventHorizonEventStartTime = Time.time;

        for (int i = 0; i < eventHorizonParticles.Length; i++) {
            eventHorizonParticles[i].Play();
        }

        destroyHeldCandle();
    }


    public bool isEventHorizonEventActive() { 
        return eventHorizonEventActive;
    }


    //make embers start falling from the top of the screen
    //this is the old solar rain event and isnt used currently
    public void startSolarRainEvent() {
        solarRainEventActive = true;
        solarRainEventStartTime = Time.time;
        solarRainEventLastEmberTime = Time.time;
        solarRainEventNextEmberDelay = UnityEngine.Random.Range(0, solarRainEventSpawnDelayRange);

        for (int i = 0; i < solarRainParticles.Length; i++) {
            solarRainParticles[i].Play();
        }
    }


    //convert all candles on screen to flares
    public void startFlaringFieldsEvent() {
        convertAllCandlesToFlares();
    }


    //make only mini suns appear for a limited time, this is the current solar rain event
    public void startMiniSunEvent() {
        miniSunEventActive = true;
        miniSunEventStartTime = Time.time;

        for (int i = 0; i < solarRainParticles.Length; i++) {
            solarRainParticles[i].Play();
        }

        destroyHeldCandle();
    }


    void updateEvents() {

        if (eventHorizonEventActive && eventHorizonEventStartTime + eventHorizonEventDuration < Time.time) {
            eventHorizonEventActive = false;

            for(int i = 0; i < eventHorizonParticles.Length; i++) {
                eventHorizonParticles[i].Stop();
            }

        }

        if (miniSunEventActive && miniSunEventStartTime + miniSunEventDuration < Time.time) {
            miniSunEventActive = false;

            for (int i = 0; i < solarRainParticles.Length; i++) {
                solarRainParticles[i].Stop();
            }

        }

        if (solarRainEventActive) {

            //if enough time has passed between the last ember spawn and now, set a new random time to wait between ember spawns and spawn an ember
            if(solarRainEventLastEmberTime + solarRainEventNextEmberDelay < Time.time) {
                solarRainEventLastEmberTime = Time.time;
                solarRainEventNextEmberDelay = UnityEngine.Random.Range(0, solarRainEventSpawnDelayRange);
                spawnEmber(3f);
            }

            if (solarRainEventStartTime + solarRainEventDuration < Time.time) {
                solarRainEventActive = false;

                for (int i = 0; i < solarRainParticles.Length; i++) {
                    solarRainParticles[i].Stop();
                }

            }
        }

    }


    GameObject getStartingCandleObject() {
        //If the first candle in the array has a StartCandleFall script on it, its a starter candle. Otherwise its a normal candle
        if (currentCandles[0] != null && currentCandles[0].GetComponentInChildren<StartCandleFall>() != null) {
            return currentCandles[0].getParentObject();
        }
        return null;
    }


    //go through the special object array and try to find an empty space
    //if there is no empty space, make a new one
    int nextSpecialObjectId() {
        for (int i = 0; i < specialObjects.Count; i++) {
            if (specialObjects[i] == null) {
                return i;
            }
        }
        specialObjects.Add(null);
        return specialObjects.Count - 1;
    }


    public void removeSpecialObject(int id) {
        specialObjects[id] = null;
    }


    //set the starting candle prefab and skin by their respective ids
    public void setStarterCandle(int candle, int skin) {
        /*startingCandlePrefab = starterCandles[candle];
        startingCandleSkin = skinManager.getSkin(candle, skin);
        currentSkinId = skin;
        currentCandlePrefabId = candle;*/
        startingCandlePrefab = skinManager.getCandlePrefab(candle, skin);
        currentSkinId = skin;
        currentCandlePrefabId = candle;
        Settings.setStarterCandleId(candle);
        Settings.setStarterCandleSkinId(skin);
    }

    //set only the skin of the starting candle by its id
    public void setStarterCandleSkin(int skin) {
        /*startingCandlePrefab = starterCandles[currentCandlePrefabId];
        startingCandleSkin = skinManager.getSkin(currentCandlePrefabId, skin);
        currentSkinId = skin;*/
        setStarterCandle(currentCandlePrefabId, skin);
    }


    /*public int getCurrentStarterCandleSkinId() {
        return currentSkinId;
    }*/

    public int getCurrentStarterCandleId() {
        return currentCandlePrefabId;
    }


    public void fadeOutStartingFloor() {
        startingFloor.lerpOut();
    }


    void setCandleId(GameObject can, int id) {
        can.GetComponent<CandleId>().setInfo(id, id == -1);
    }


    void setScore(int s) {
        currentScore = s;
        scoreText.text = "" + s;

        if(s > lastHighScore) {
            highScoreText.text = "" + s;
            Settings.setHighScore(s);
        }


        if(s >= 100000 && Settings.getAchievementCount() - 2 == Settings.achievementsUnlockedCount()) {
            //"Full Completionist" unlocked by getting 100000 score and unlocking every other achievement
            Settings.setAchievementUnlocked(46);
        }

    }


    public void addScore(int s) {
        setScore(currentScore + s);
    }


    public int getScore() {
        return currentScore;
    }


    public int getLastHighScore() {
        return lastHighScore;
    }


    public Sprite getBonusText(int x) {
        return bonusTexts[x];
    }


    private void setSpawnPotentials() {

        //the "normal" 12 candles all have twice as high of a spawn chance than the special ones
        for(int i = 0; i < canObjects.Length - 2; i++) {
            spawnPotentials.Add(i);
            spawnPotentials.Add(i);
        }

        //special chance of a starter candle spawning
        spawnPotentials.Add(-1);

        //black hole and mini sun
        spawnPotentials.Add(canObjects.Length - 2);
        spawnPotentials.Add(canObjects.Length - 1);

    }


    //creates a new bonus text given the candle being destroyed and the current point multiplier, returns the new multiplier
    public int createBonusText(GameObject candle, int multiplier) {

        int newMult = multiplier;

        CandleId can = candle.GetComponent<CandleId>();
        int spriteId = BonusText.getBonusTextSpriteIdByPoints(can.getPoints());

        if (can.isStarterCandle()) {

            newMult *= 2;
            //starter candles will always have the "starter candle! x2" sprite
            spriteId = 1;
            createMultiplierlessBonusText(can, spriteId, true);

        }
        else {
            createMultiplierlessBonusText(can, spriteId);
        }

        

        return newMult;
    }


    public void createMultiplierlessBonusText(CandleId can, int bonusTextId) {
        createMultiplierlessBonusText(can, bonusTextId, false);
    }


    public void createMultiplierlessBonusText(CandleId can, int bonusTextId, bool highlighted) {
        BonusText bonusText = Instantiate(bonusTextPrefab, can.transform.position, Quaternion.identity).GetComponent<BonusText>();
        bonusText.setSprite(getBonusText(bonusTextId));

        if(can.getPoints() > 1) {
            bonusText.enableGrowing();
        }

        if (highlighted) {
            bonusText.enableHighlight();
        }
    }


    //creates the bonus text for the bonus from destroying a single row
    public void createRowDestructionBonusText() {
        BonusText bonusText = Instantiate(bonusTextPrefab, defaultBonusTextLocation.transform.position, Quaternion.identity).GetComponent<BonusText>();
        bonusText.setSprite(getBonusText(2));
        bonusText.enableHighlight();
    }



    public void toggleWaffleRain(bool x) {

        ParticleSystem[] particles = waffleRainParentObject.GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < particles.Length; i++) {

            if (x) {
                particles[i].Play();
            }
            else {
                particles[i].Stop();
                particles[i].Clear();
            }

        }

    }


    //moves snowy far off screen if disabled
    public void toggleSnowy(bool x) {
        
        if (x) {
            snowyParentObject.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        }
        else {
            snowyParentObject.transform.GetChild(0).transform.localPosition = new Vector3(100f, 0, 0);
        }

    }


    public void setGameOverTime() {
        trueGameOver = true;
    }


    //only update the timer if the player is not idle
    void updateIdleTimers() {

        if(timeOfLastActivity + allowedIdleTime > Time.time && !trueGameOver) {
            timeSinceGameStarted += Time.deltaTime;
        }

        if (nTopIdle && topIdleInitialTime + allowedTopIdleTime < Time.time) {
            nGameStart();
        }
        
    }


    //checks for achievments that can only be unlocked after ending a game
    void checkForGameEndAchievments(bool initialStart) {

        if (currentScore >= 50) {
            //"Poopy Doopy" unlocked by getting 50 score
            Settings.setAchievementUnlocked(10);
        }

        if (currentScore >= 250) {
            //"Not Too Shabby" unlocked by getting 250 score
            Settings.setAchievementUnlocked(11);
        }
        
        if (currentScore >= 500) {
            //"Pretty Good" unlocked by getting 500 score
            Settings.setAchievementUnlocked(12);
        }

        if (currentScore >= 1000) {
            //"Impressive" unlocked by getting 1000 score
            Settings.setAchievementUnlocked(13);
        }

        if (currentScore >= 5000) {
            //"Insanely High" unlocked by getting 5000 score
            Settings.setAchievementUnlocked(14);
        }

        if (currentScore >= 10000) {
            //"Unbelieveable" unlocked by getting 10000 score
            Settings.setAchievementUnlocked(15);
        }

        if (currentScore >= 50000) {
            //"Impossible" unlocked by getting 50000 score
            Settings.setAchievementUnlocked(16);
        }

        if (currentScore == 0 && !initialStart && trueGameOver) {
            //"Catastrophe" unlocked by losing with 0 points
            Settings.setAchievementUnlocked(2);
        }

        if (Settings.getHighScore() >= lastHighScore * 2 && Settings.isAchievementUnlocked(17) && !initialStart) {
            //"Major Improvement!" unlocked by doubling previous high score (can only be done if achievment #17 is unlocked)
            Settings.setAchievementUnlocked(18);
        }

        if (Settings.getHighScore() > 0) {
            //"Only up from here" unlocked by getting a high score of at least 1
            Settings.setAchievementUnlocked(17);
        }

        if(!initialStart && trueGameOver && timeSinceGameStarted < 20) {
            //"Failure Speedrun" unlocked by losing in less than 20 seconds
            Settings.setAchievementUnlocked(8);
        }

        if (!initialStart && trueGameOver && timeSinceGameStarted >= 3600) {
            //"The Long Haul" unlocked by ending a game that lasted at least an hour (excluding idle time)
            Settings.setAchievementUnlocked(9);
        }

        if (currentScore > lastHighScore && !initialStart) {
            if (Settings.getStarterCandleSkinId() != 0) {
                //"Skin diff" unlocked by getting a high score and using a skin other than the default one
                Settings.setAchievementUnlocked(31);
            }

            if (Settings.getStarterCandleId() != 0) {
                //"Kit diff" unlocked by getting a high score and using a starter candle other than the default one
                Settings.setAchievementUnlocked(32);
            }
        }

    }


    public void resetHighScoreText() {
        highScoreText.text = "" + Settings.getHighScore();
    }


    public void achievementUnlockPopup(int x) {
        Debug.Log("achievement " + x);

        //dont have pop up happen if the camera is moving or if there is a menu other than the gameManager open
        if (!mainCamera.GetComponent<CameraController>().currentlyScrollTransitioning()) {

            if (isMenuActive()) {
                unlockPopUpMenu.setTargetAchievement(x);
                unlockPopUpMenu.pause();
            }
            else {
                //if the pop up cant be displayed store it for later
                //only allowed to display the pop up later if it was blocked because of another menu being open
                nextAchievementPopupToDisplay = x;
            }

        }

    }


    public bool unlockAllAchievements() {
        return grantAllAchievements;
    }


    //if an achievement that unlocks a candle is granted, the cover for the unlocked candle needs to be removed
    public void refreshBasementCandleCovers() {

        for (int i = 0; i < 5; i++) {
            if (Settings.candleUnlocked(i)) {
                buttons[i + 8].gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.getCandleCover(i);
            }
            else {
                buttons[i + 8].gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.getLockedCandleSprite(i);
            }
        }

    }


    void updateSaveData() {

        List<GameObject> livingCandles = new List<GameObject>();
        List<GameObject> currentSpecialObjects = new List<GameObject>();

        //get all the candles in the scene
        for (int i = 0; i < currentCandles.Count; i++) { 
        
            //make sure this candle still exists, is not being destroyed, and is not the held candle
            if(currentCandles[i] != null && !currentCandles[i].isBeingDestroyed() && currentCandles[i].getParentObject() != selectedCan) {
                livingCandles.Add(currentCandles[i].getParentObject());
            }

        }

        //get all the special objects in the scene
        for (int i = 0; i < specialObjects.Count; i++) {
            
            //make sure this object still exists and is not the held object
            if (specialObjects[i] != null && specialObjects[i].getGameObject() != selectedCan) {
                currentSpecialObjects.Add(specialObjects[i].getGameObject());
            }

        }


        SaveManager.updateSave(livingCandles.Distinct().ToArray(), selectedCan, currentSpecialObjects.ToArray(), gameOverChain.getChainProgress(), timeSinceGameStarted);

    }


    void loadFromSave() {

        CandleData[] objs = SaveManager.getSavedObjects();
        List<GameObject> createdCandles = new List<GameObject>();

        selectedCan = instantiateCandleData(SaveManager.getHeldCandle());
        prepareSelectedCandle();


        //create all dropped objects from the save
        for(int i = 0; i < objs.Length; i++) {

            createdCandles.Add(instantiateCandleData(objs[i]));

        }


        CandleLightController[] lights;

        //once all candles are created update their touching lists
        for (int i = 0; i < createdCandles.Count; i++) { 
            
            lights = createdCandles[i].GetComponentsInChildren<CandleLightController>();

            for (int j = 0; j < lights.Length; j++) {
                lights[j].updateCollisionList();
            }

        }


        //set chain touching time and time since game start
        gameOverChain.setChainProgress(SaveManager.getChainProgress());
        timeSinceGameStarted = SaveManager.getTimeSinceGameStart();

    }


    //takes a candle data object and turn it into a gameobject
    GameObject instantiateCandleData(CandleData x) {

        CandleLightController[] lights;
        GameObject result = null;

        //prepare position and rotation
        Quaternion rot = new Quaternion(x.rotation[0], x.rotation[1], x.rotation[2], x.rotation[3]);

        Vector3 pos = new Vector3();
        pos.x = x.position[0];
        pos.y = x.position[1];
        pos.z = x.position[2];


        switch (x.specialObject) {

            //candle
            case -1:
                
                //starter candle
                if(x.candleId == -1) {
                    result = Instantiate(startingCandlePrefab, pos, rot);
                    Destroy(result.GetComponent<StartCandleFall>());
                }
                //normal candle
                else {
                    result = Instantiate(canObjects[x.candleId], pos, rot);
                }

                addCandleLight(result);
                setCandleId(result, x.candleId);

                //set each light status
                //each light also needs to have their collision list updated after each candle is created
                lights = result.GetComponentsInChildren<CandleLightController>();

                for(int i = 0; i < lights.Length; i++) {
                    lights[i].setLightStatusById(x.lightState[i]);
                }

                break;

            //black hole
            case 0:
                result = Instantiate(canObjects[canObjects.Length - 2], pos, rot);
                break;

            //mini sun
            case 1:
                result = Instantiate(canObjects[canObjects.Length - 1], pos, rot);
                break;

        }

        //extra set up if it is a special object
        if (x.specialObject != -1) {
            int specialObjId = nextSpecialObjectId();
            result.GetComponent<ISpecialObject>().setup(this, specialObjId);
            specialObjects[specialObjId] = result.GetComponent<ISpecialObject>();
        }


        //apply velocity
        Rigidbody2D rb = result.GetComponent<Rigidbody2D>();

        Vector2 vel = new Vector2(x.linearVelocityX, x.linearVelocityY);
        rb.linearVelocity = vel;

        rb.angularVelocity = x.angularVelocity;

        //starting candles have no gravity by default
        rb.gravityScale = 1;


        return result;

    }


    //used to start the game with n candle at the bottom after user idles on the top for long enough
    void nGameStart() {

        pause();
        fadeOutStartingFloor();
        nTopIdle = false;
        nFallingParticle.Emit(1);
        titleObject.sprite = nlessTitleSprite;

        destroyCandle(starterCandle, true);

        //set the chosen starter candle to the n candle
        setStarterCandle(1, 0);

        StartCoroutine(nTransitionDown());

    }

    IEnumerator nTransitionDown() {
        yield return new WaitForSeconds(2.5f);

        //spawn the n candle at the bottom
        GameObject x = Instantiate(startingCandlePrefab, teleCoords.position, Quaternion.identity);
        addCandleLight(x);
        setCandleId(x, -1);

        x.GetComponent<Rigidbody2D>().gravityScale = 1;
        Destroy(x.GetComponent<StartCandleFall>());

        //after the transition ends start spawning candles normally
        mainCamera.GetComponent<CameraController>().transitionToBottom(20, () => {
            unpause();
            StartTurn();

            //reset the title sprite to normal
            titleObject.sprite = normalTitleSprite;

            //"Afraid of heights?" unlocked by sitting at main menu for 3 minutes
            Settings.setAchievementUnlocked(3);
        });
    }


    void resetNTopIdleTimer() {
        nTopIdle = true;
        topIdleInitialTime = Time.time;
    }


    void updateBannerTimer() {

        //if the banner is disabled and its time to show the banner
        if (!bannerToggled && bannerInitialTime + bannerSpawnDelay < Time.time) {

            //banner should never show if the user hasnt started the game yet or is in another menu
            if (gameStarted && menuActive) {
                adController.showBannerAd();
            }
            bannerToggled = true;
            bannerInitialTime = Time.time;

        }
        //if the banner is enabled and its time to hide the banner
        else if (bannerInitialTime + bannerShowDuration < Time.time){

            adController.hideBannerAd();
            bannerToggled = false;
            bannerInitialTime = Time.time;

        }

    }


    public void openFailedSaveMenu() {

        if (!failedSaveMenu.isMenuActive()) {
            pause();
            failedSaveMenu.pause();
        }

    }


}
