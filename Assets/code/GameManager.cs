using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

    private static List<CandleLightController> currentCandles = new List<CandleLightController>();
    [SerializeField] List<ButtonPress> buttons = new List<ButtonPress>();
    //for special non candle objects like the black hole
    private List<SpecialObject> specialObjects = new List<SpecialObject>();

    [SerializeField] GameObject startingCandlePrefab;
    [SerializeField] float startingCandleGravity;
    [SerializeField] Camera mainCamera;

    [SerializeField] GameObject pauseMenuObject;

    [SerializeField] GameObject creditsTransitionLocation;
    [SerializeField] GameObject basementTransitionLocation;

    [SerializeField] GameObject leftCandlePlacementLimit;
    [SerializeField] GameObject rightCandlePlacementLimit;


    private void Start()
    {

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
        //skip the intro transition when the game starts if the screen is pressed
        buttons[2].onPress(delegate () {
            mainCamera.GetComponent<camCtrl>().skipIntroTransition();
        });
        //credits button
        buttons[3].onPress(delegate () {
            if (canPause) {
                canPause = false;
                mainCamera.GetComponent<camCtrl>().setNewTarget(creditsTransitionLocation.transform.position);
                mainCamera.GetComponent<camCtrl>().startTransition();
            }
        });
        //button to go back down from credits
        buttons[4].onPress(delegate () {
            canPause = true;
            mainCamera.GetComponent<camCtrl>().transitionToTop(20f);
        });
        //button to go to the basement
        buttons[5].onPress(delegate () {
            if (canPause) {
                canPause = false;
                mainCamera.GetComponent<camCtrl>().setNewTarget(basementTransitionLocation.transform.position, 40f);
                mainCamera.GetComponent<camCtrl>().startTransition();
            }
        });
        //button to go back up from basement
        buttons[6].onPress(delegate () {
            canPause = true;
            mainCamera.GetComponent<camCtrl>().transitionToTop(40f);
        });


        GameObject x = Instantiate(startingCandlePrefab);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera);

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
                // Object has stopped moving, start a new turn
                StartTurn();
            }
            else if (Time.time - turnStartTime >= minTurnDuration && Input.GetMouseButtonDown(0) && !hasMoved)
            {
                Vector3 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                tapPosition.z = 0;

                if (selectedCan != null && pauseMenuObject.GetComponent<PauseMenuController>().unpauseFinished())
                {
                    selectedCan.transform.position = new Vector3(teleCoords.position.x, teleCoords.position.y, 0);

                    rb.gravityScale = 1;
                    rb.isKinematic = false;

                    Vector3 newPosition = new Vector3(tapPosition.x, selectedCan.transform.position.y, 0);

                    //check if the tapped position is too far off screen first and adjust if necessary
                    if(tapPosition.x + selectedCan.GetComponent<Collider2D>().bounds.size.x / 2 > rightCandlePlacementLimit.transform.position.x){

                        newPosition.x = rightCandlePlacementLimit.transform.position.x - selectedCan.GetComponent<Collider2D>().bounds.size.x / 2;
                    }
                    if(tapPosition.x - selectedCan.GetComponent<Collider2D>().bounds.size.x / 2 < leftCandlePlacementLimit.transform.position.x) {

                        newPosition.x = leftCandlePlacementLimit.transform.position.x + selectedCan.GetComponent<Collider2D>().bounds.size.x / 2;
                    }

                    selectedCan.transform.position = newPosition;

                    canMove = true;
                    lastMoveTime = Time.time + velocityCheckDelay; // Delay before checking velocity
                    hasMoved = true; // Candle has been moved
                }
            }
        }
    }

    public void StartTurn()
    {
        gameStarted = true;
        int randomIndex = UnityEngine.Random.Range(0,canObjects.Length); // Specify UnityEngine.Random
        if(randomIndex > 7) {
            randomIndex = 12;
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
        hasMoved = false; // Reset the moved flag

        //if a candle has spawned instead of a special item like the black hole, add it to the list to keep track of
        if (selectedCan.GetComponentInChildren<CandleLightController>() != null) {
            addCandleLight(selectedCan);

        } else if(selectedCan.GetComponent<SpecialObject>() != null) {

            int specialObjId = nextSpecialObjectId();
            selectedCan.GetComponent<SpecialObject>().setup(this, specialObjId);
            specialObjects[specialObjId] = selectedCan.GetComponent<SpecialObject>();
        }
    }


    public void addCandleLight(GameObject candle) {
        //add the created candles lights to the list for candleRowDestroyer to reference by id later on
        //the for loop is in the case a candle has multiple lights
        for (int i = 0; i < candle.transform.childCount; i++) {
            CandleLightController c = candle.transform.GetChild(i).GetChild(0).GetComponent<CandleLightController>();
            currentCandles.Add(c);
            c.assignId();
            candle.name = candle.name + " ID: " + c.getId();
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
            buttons[i].active = false;
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
            buttons[i].active = true;
        }
    }


    public bool isGameStarted() {
        return gameStarted;
    }


    public void resetGame() {
        selectedCan = null;
        isTurnActive = false;
        gameStarted = false;

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

        GameObject x = Instantiate(startingCandlePrefab);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera);

    }


    GameObject getStartingCandleObject() {
        //If the first candle in the array has a StartCandleFall script on it, its a starter candle. Otherwise its a normal candle
        if (currentCandles[0] != null && currentCandles[0].transform.parent.parent.GetComponent<StartCandleFall>() != null) {
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

}
