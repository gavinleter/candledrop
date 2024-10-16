using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

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

    List<int> spawnPotentials = new List<int>();

    private void Start()
    {

        setSpawnPotentials();

        Settings.initSettings(this);

        skinManager = GetComponent<SkinManager>();

        //get the starting candle prefab and skin
        setStarterCandle(Settings.getStarterCandleId(), Settings.getStarterCandleSkinId());

        musicManager.setSelectedMusic(1);
        musicManager.setMusicVolume(1);

        //pause the game and pull up pause menu when a settings button is pressed
        System.Action settingsAction = delegate () {
            if (canPause) {
                pause();
                pauseMenuObject.GetComponent<IMenu>().pause();
            }
        };

        //both the top and bottom pause buttons
        buttons[0].onPress(settingsAction);
        buttons[1].onPress(settingsAction);
        //skip the intro transition when the game starts if the screen is pressed (invisible over intro area)
        buttons[2].onPress(delegate () {
            mainCamera.GetComponent<CameraController>().skipIntroTransition();
            musicManager.setSelectedMusic(2);
        });
        //credits button
        buttons[3].onPress(delegate () {
            if (canPause) {
                canPause = false;
                mainCamera.GetComponent<CameraController>().setNewTarget(creditsTransitionLocation.transform.position);
                mainCamera.GetComponent<CameraController>().startTransition();
                musicManager.setSelectedMusic(0);
            }
        });
        //button to go back down from credits
        buttons[4].onPress(delegate () {
            canPause = true;
            mainCamera.GetComponent<CameraController>().transitionToTop(20f);
            musicManager.setSelectedMusic(2);
        });
        //button to go to the basement
        buttons[5].onPress(delegate () {
            if (canPause) {
                canPause = false;
                mainCamera.GetComponent<CameraController>().setNewTarget(basementTransitionLocation.transform.position, 40f);
                mainCamera.GetComponent<CameraController>().startTransition();
                musicManager.setSelectedMusic(5);
                //mainCamera.GetComponent<CameraController>().fadeToBlackTransition(basementTransitionLocation.transform.position, 0.1f);
            }
        });
        //button to go back up from basement
        buttons[6].onPress(delegate () {
            canPause = true;
            //mainCamera.GetComponent<CameraController>().transitionToTop(40f);
            mainCamera.GetComponent<CameraController>().fadeToBlackTransitionToTop(0.1f);
            musicManager.setSelectedMusic(2);
        });
        //button to drop a candle (invisible over game area)
        buttons[7].onPress(delegate () {
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

            if (Settings.candleUnlocked(i)) {
                buttons[i + 8].gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.getCandleCover(i);
            }
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
            getInitialCandle().GetComponent<StartCandleFall>().dropCandle();
            losingVignette.clearParticles();
            musicManager.setSelectedMusic(3);
        });

        buttons[19].onPress(() => {
            //pause();
            //adSpinnerMenu.pause();
            //convertAllCandlesToFlares();
            //spawnEmber(-1.5f);
            startEventHorizonEvent();
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

        //spawn the starting candle
        /*GameObject x = Instantiate(startingCandlePrefab);
        setCandleId(x, currentCandlePrefabId);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera, startingCandleSkin);*/
        resetGame(true);
        

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
            if ( rb == null || (canMove && Time.time - lastMoveTime >= velocityCheckDelay && rb.velocity.magnitude < 0.01f) || 
                (eventHorizonEventActive || miniSunEventActive) && Time.time - lastMoveTime >= blackHoleSpawnDelay && rb.velocity.magnitude > 0.1f)
            {
                //object has stopped moving, start a new turn
                StartTurn();
            }
        }

        updateEvents();

    }


    //attempt to drop the candle if it has spawned, other dropped candles aren't moving, and a time delay has finished between now and the last drop
    private void dropCandle() {
        
        if (isTurnActive && Time.time - turnStartTime >= minTurnDuration && !hasMoved) {
            Rigidbody2D rb = rb = selectedCan.GetComponent<Rigidbody2D>();

            Vector3 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tapPosition.z = 0;

            if (selectedCan != null && pauseMenuObject.GetComponent<PauseMenuController>().unpauseFinished()) {
                selectedCan.transform.position = new Vector3(teleCoords.position.x, teleCoords.position.y, 0);

                rb.gravityScale = 1;
                rb.isKinematic = false;

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
    public void StartTurn()
    {
        gameStarted = true;
        int randomIndex = spawnPotentials[UnityEngine.Random.Range(0, spawnPotentials.Count)];
        //randomIndex = 1;
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

        selectedCan.SetActive(true);

        Rigidbody2D rb = selectedCan.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        canMove = false;
        isTurnActive = true;
        turnStartTime = Time.time;
        hasMoved = false;

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

        CandleLightController[] c = can.GetComponentsInChildren<CandleLightController>();

        for (int i = 0; i < c.Length; i++) {
            c[i].startDestroy();
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
        isTurnActive = false;
        if(getStartingCandleObject() != null) {
            getStartingCandleObject().GetComponent<StartCandleFall>().setReadyToDrop(false);
        }

        for (int i = 0; i < buttons.Count; i++) {
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
    }


    public void unpause() {

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
    }


    public bool isGameStarted() {
        return gameStarted;
    }


    public void resetGame() {
        resetGame(false);
    }

    public void resetGame(bool initialStart) {
        selectedCan = null;
        isTurnActive = false;
        gameStarted = false;

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
        }

        //reset starter candle and skin in case the player wipes data and resets
        setStarterCandle(Settings.getStarterCandleId(), Settings.getStarterCandleSkinId());

        starterCandle = Instantiate(startingCandlePrefab, startingCandleSpawnLocation.transform.position, Quaternion.identity);
        starterCandle.GetComponent<CandleId>().setInfo(-1, true);
        starterCandle.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera);
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
        int spriteId = getBonusTextSpriteIdByPoints(can.getPoints());

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


    public int getBonusTextSpriteIdByPoints(int points) {

        switch (points) {
            case 2:
                return 9;
            case 3:
                return 10;
            default:
                return 0;
        }

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


}
