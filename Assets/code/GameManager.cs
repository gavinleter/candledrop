using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IMenu
{
    public GameObject[] canObjects = new GameObject[12];
    public Transform teleCoords;
    public float minTurnDuration = 2.0f; // Minimum turn duration in seconds
    public float velocityCheckDelay = 0.05f; // Delay before checking velocity

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

    [SerializeField] float startingCandleGravity;
    [SerializeField] Camera mainCamera;

    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] FadingObject startingFloor;

    [SerializeField] GameObject creditsTransitionLocation;
    [SerializeField] GameObject basementTransitionLocation;

    [SerializeField] GameObject leftCandlePlacementLimit;
    [SerializeField] GameObject rightCandlePlacementLimit;


    [SerializeField] GameObject[] starterCandles;

    [SerializeField] SkinSelectMenuController skinSelectMenu;
    [SerializeField] LockedFeatureMenuController lockedFeatureMenu;

    [SerializeField] GameObject startingCandleSpawnLocation;

    SkinManager skinManager;

    [SerializeField] AudioClip snowyChirpSound;


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

            if (rb == null || (canMove && Time.time - lastMoveTime >= velocityCheckDelay && rb.velocity.magnitude < 0.01f))
            {
                //object has stopped moving, start a new turn
                StartTurn();
            }
        }


    }


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

                //check if the tapped position is too far off screen first and adjust if necessary
                if (tapPosition.x + selectedCan.GetComponent<Collider2D>().bounds.size.x / 2 > rightCandlePlacementLimit.transform.position.x) {

                    newPosition.x = rightCandlePlacementLimit.transform.position.x - selectedCan.GetComponent<Collider2D>().bounds.size.x / 2;
                }
                if (tapPosition.x - selectedCan.GetComponent<Collider2D>().bounds.size.x / 2 < leftCandlePlacementLimit.transform.position.x) {

                    newPosition.x = leftCandlePlacementLimit.transform.position.x + selectedCan.GetComponent<Collider2D>().bounds.size.x / 2;
                }

                selectedCan.transform.position = newPosition;

                canMove = true;
                lastMoveTime = Time.time + velocityCheckDelay;
                hasMoved = true;
            }
        }

    }


    public void StartTurn()
    {
        gameStarted = true;
        int randomIndex = UnityEngine.Random.Range(0,canObjects.Length);
        /*if(randomIndex > 7) {
            randomIndex = 8;
        }
        else {
            randomIndex = 0;
        }*/
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

        GameObject x = Instantiate(startingCandlePrefab, startingCandleSpawnLocation.transform.position, Quaternion.identity);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera, startingCandleSkin);

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
