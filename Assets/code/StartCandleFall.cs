using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCandleFall : MonoBehaviour
{

    [SerializeField] float initialGravity;
    [SerializeField] GameObject gameControllerObject;
    [SerializeField] Camera mainCam;
    GameManager gameManager;
    Rigidbody2D rb;
    bool activated = false;
    bool gameStarted = false;
    bool readyToDrop = true;

    // Start is called before the first frame update
    void Start(){
        //Debug.Log(name);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnMouseDown() {
        if (!activated) {
            activated = true;
            rb.gravityScale = initialGravity;
            mainCam.GetComponent<camCtrl>().startGameTransition();
        }
    }


    void OnCollisionEnter2D(Collision2D collision) {
        if (!gameStarted) {
            gameStarted = true;
            gameManager.StartTurn();
        }
    }


    public void setFields(float initialGravity, GameObject gameControllerObject, Camera mainCam, Sprite skin) {
        this.initialGravity = initialGravity;
        this.gameControllerObject = gameControllerObject;
        this.mainCam = mainCam;
        rb = GetComponent<Rigidbody2D>();
        gameManager = gameControllerObject.GetComponent<GameManager>();
        gameManager.addCandleLight(gameObject);
        GetComponent<SpriteRenderer>().sprite = skin;
    }


    public void setReadyToDrop(bool x) {
        readyToDrop = x;
    }

}
