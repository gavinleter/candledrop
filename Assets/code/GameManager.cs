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

    private static List<CandleLightController> currentCandles = new List<CandleLightController>();
    [SerializeField] List<ButtonPress> buttons;

    [SerializeField] GameObject startingCandlePrefab;
    [SerializeField] float startingCandleGravity;
    [SerializeField] Camera mainCamera;

    [SerializeField] GameObject pauseMenuObject;


    private void Start()
    {

        //pause the game and pull up pause menu when a settings button is pressed
        System.Action settingsAction = delegate () {
            pause();
            pauseMenuObject.GetComponent<IMenu>().pause();
        };

        //both the top and bottom pause buttons
        buttons[0].onPress(settingsAction);
        buttons[1].onPress(settingsAction);
        //skip the intro transition when the game starts if the screen is pressed
        buttons[2].onPress(delegate () {
            mainCamera.GetComponent<camCtrl>().skipIntroTransition();
        });

        GameObject x = Instantiate(startingCandlePrefab);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera);
        //Debug.Log(x.name);

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

                if (selectedCan != null)
                {
                    selectedCan.transform.position = new Vector3(teleCoords.position.x, teleCoords.position.y, 0);

                    rb.gravityScale = 1;
                    rb.isKinematic = false;

                    Vector3 newPosition = new Vector3(tapPosition.x, selectedCan.transform.position.y, 0);
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

        addCandleLight(selectedCan);
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

        for(int i = 0; i < currentCandles.Count; i++) {
            Destroy(currentCandles[i].getParentObject());
        }
        currentCandles.Clear();

        CandleLightController.reset();

        GameObject x = Instantiate(startingCandlePrefab);
        x.GetComponent<StartCandleFall>().setFields(startingCandleGravity, gameObject, mainCamera);
        //Debug.Log(x.name);
        //currentCandles.Add(x.transform.GetChild(0).GetChild(0).GetComponent<CandleLightController>());
        //Debug.Log(currentCandles[0].gameObject.name);
        //addCandleLight(x);
        /*
        for (int i = 0; i < currentCandles.Count; i++) {
            Debug.Log(currentCandles[i].transform.parent.parent.gameObject.name);
            //Debug.Log(getStartingCandleObject().name);
        }
        */
    }


    GameObject getStartingCandleObject() {
        //If the first candle in the array has a StartCandleFall script on it, its a starter candle. Otherwise its a normal candle
        if (currentCandles[0] != null && currentCandles[0].transform.parent.parent.GetComponent<StartCandleFall>() != null) {
            return currentCandles[0].getParentObject();
        }
        return null;
    }

}
