using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
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

    private static List<CandleLightController> currentCandles = new List<CandleLightController>();

    private void Start()
    {
        //StartTurn();
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

}
