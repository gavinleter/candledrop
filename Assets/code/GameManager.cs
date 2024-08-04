using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IMenu
{
    public GameObject[] canObjects = new GameObject[12];
    public Transform teleCoords;
    float minTurnDuration = 0.0f; // Minimum turn duration in seconds
    float velocityCheckDelay = 0.05f; // Delay before checking velocity

    private GameObject selectedCan;
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
    Sprite startingCandleSkin;
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

    [SerializeField] GameObject startingCandleSpawnLocation;

    SkinManager skinManager;

    [SerializeField] AudioClip snowyChirpSound;

    [SerializeField] GameOverChain gameOverChain;
    [SerializeField] GameOverMenuController gameOverMenuController;

    private void Start()
    {

        skinManager = GetComponent<SkinManager>();

        //get the starting candle prefab and skin
        setStarterCandle(Settings.getStarterCandleId(), Settings.getStarterCandleSkinId());

        startingFloor.forceAppear();


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
        });
        //credits button
        buttons[3].onPress(delegate () {
            if (canPause) {
                canPause = false;
                mainCamera.GetComponent<CameraController>().setNewTarget(creditsTransitionLocation.transform.position);
                mainCamera.GetComponent<CameraController>().startTransition();
            }
        });
        //button to go back down from credits
        buttons[4].onPress(delegate () {
            canPause = true;
            mainCamera.GetComponent<CameraController>().transitionToTop(20f);
        });
        //button to go to the basement
        buttons[5].onPress(delegate () {
            if (canPause) {
                canPause = false;
                mainCamera.GetComponent<CameraController>().setNewTarget(basementTransitionLocation.transform.position, 40f);
                mainCamera.GetComponent<CameraController>().startTransition();
            }
        });
        //button to go back up from basement
        buttons[6].onPress(delegate () {
            canPause = true;
            mainCamera.GetComponent<CameraController>().transitionToTop(40f);
        });
        //button to drop a candle (invisible over game area)
        buttons[7].onPress(delegate () {
            dropCandle();
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
                buttons[i + 8].gameObject.GetComponent<SpriteRenderer>().sprite = skinManager.getSkin(i, 0);
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
        buttons[18].onPress(() => {
            //pause();
            //adSpinnerMenu.pause();
            //convertAllCandlesToFlares();
            //spawnEmber(-1.5f);
            startEventHorizonEvent();
        });
        buttons[19].onPress(() => {
            //startSolarRainEvent();
            startMiniSunEvent();
        });
        buttons[20].onPress(() => {
            startFlaringFieldsEvent();
        });

        buttons[21].onPress(() => {
            pause();
            gameOverMenuController.setScores(20, 0);
            gameOverMenuController.pause();
        });

        //spawn the starting candle
        GameObject x = Instantiate(startingCandlePrefab);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera, startingCandleSkin);

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
            //the next "candle" can also spawn if an event horizon is active and a shorter timer runs out while the previous black hole is moving still
            if ( rb == null || (canMove && Time.time - lastMoveTime >= velocityCheckDelay && rb.velocity.magnitude < 0.01f) || 
                eventHorizonEventActive && Time.time - lastMoveTime >= blackHoleSpawnDelay && rb.velocity.magnitude > 0.1f)
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
        int randomIndex = UnityEngine.Random.Range(0,canObjects.Length);

        //only black holes can spawn during the event horizon event
        if (eventHorizonEventActive) {
            randomIndex = 12;
        }
        //only mini suns can spawn during solar rain event
        else if (miniSunEventActive) {
            randomIndex = 13;
        }
        
        selectedCan = Instantiate(canObjects[randomIndex], teleCoords.position, Quaternion.identity);
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

        } else if(selectedCan.GetComponent<ISpecialObject>() != null) {

            int specialObjId = nextSpecialObjectId();
            selectedCan.GetComponent<ISpecialObject>().setup(this, specialObjId);
            specialObjects[specialObjId] = selectedCan.GetComponent<ISpecialObject>();
        }
    }


    public void addCandleLight(GameObject candle) {
        //add the created candles lights to the list for candleRowDestroyer to reference by id later on
        //the for loop is in the case a candle has multiple lights
        CandleLightController[] c = candle.GetComponentsInChildren<CandleLightController>();

        for (int i = 0; i < candle.transform.childCount; i++) {
            currentCandles.Add(c[i]);
            c[i].assignId();
            candle.name = candle.name + " ID: " + c[i].getId();
        }
    }


    public static CandleLightController getCandleById(int id) {
        /*for (int i = 0; i < currentCandles.Count; i++) {
            Debug.Log(i + " " + id + " " + currentCandles[i].transform.parent.parent.name);
        }*/
        for (int i = 0; i < currentCandles.Count; i++) {
            if (currentCandles[i].getId() == id) {
                return currentCandles[i];
            }
        }
        return null;
    }


    //destroys the held candle and starts a new turn, but does not destroy a held special object
    void destroyHeldCandle() {

        if (selectedCan != null && selectedCan.GetComponentInChildren<CandleLightController>() != null) {
            //if the candle has not been dropped yet, destroy it
            if (!canMove) {
                int heldCandleId = selectedCan.GetComponentInChildren<CandleLightController>().getId();
                destroyCandle(heldCandleId);
            }
            //always spawn the next "candle" in case the previously dropped one is still moving
            //this is to stop a moving candle from taking the event horizon event hostage
            StartTurn();
        }

    }


    public void destroyCandle(int id) {
        if(currentCandles[id] == selectedCan) {
            selectedCan = null;
        }

        Destroy(getCandleById(id).getParentObject());
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
            if(buttons[i] is SecretButton) {
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
        selectedCan = null;
        isTurnActive = false;
        gameStarted = false;

        startingFloor.forceAppear();

        //clear all candles
        for(int i = 0; i < currentCandles.Count; i++) {
            Destroy(currentCandles[i].getParentObject());
        }
        currentCandles.Clear();

        //clear all special objects
        for (int i = 0; i < specialObjects.Count; i++) {
            if(specialObjects[i] != null) {
                specialObjects[i].destroySelf();
            }
        }
        specialObjects.Clear();

        CandleLightController.reset();
        gameOverChain.resetChain();

        GameObject x = Instantiate(startingCandlePrefab, startingCandleSpawnLocation.transform.position, Quaternion.identity);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera, startingCandleSkin);

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
        startingCandlePrefab = starterCandles[candle];
        startingCandleSkin = skinManager.getSkin(candle, skin);
        currentSkinId = skin;
        currentCandlePrefabId = candle;
    }

    //set only the skin of the starting candle by its id
    public void setStarterCandleSkin(int skin) {
        startingCandlePrefab = starterCandles[currentCandlePrefabId];
        startingCandleSkin = skinManager.getSkin(currentCandlePrefabId, skin);
        currentSkinId = skin;
    }


    public int getCurrentStarterCandleSkinId() {
        return currentSkinId;
    }

    public int getCurrentStarterCandleId() {
        return currentCandlePrefabId;
    }


    public void fadeOutStartingFloor() {
        startingFloor.fadeOut();
    }

}
