using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverChain : MonoBehaviour
{

    [SerializeField] GameOverMenuController gameOverMenu;
    [SerializeField] GameManager gameManager;

    //a list of any menu that needs to be closed before a game over happens
    //this is a list of GameObject rather than IMenu because unity doesn't work with serializing interfaces
    [SerializeField] GameObject[] menusToClose;

    [SerializeField] float timeToGameOver;
    [SerializeField] float sensingDelay;

    SpriteRenderer sr;

    float initialTouchTime;

    float lerp = 0;
    int totalTouches = 0;

    bool gameOver = false;

    void Start(){
        
        sr = GetComponent<SpriteRenderer>();

    }

  
    void Update(){

        //if currently touching a candle and the initial delay has passed, start lerping
        if (isTouching() && Time.time > initialTouchTime + sensingDelay) {

            lerp = Mathf.Min(lerp + Time.deltaTime / timeToGameOver, 1);

            if(lerp >= 0.7 && !gameOver) {
                gameManager.pause();
                gameOverMenu.setScores(gameManager.getScore(), gameManager.getLastHighScore());
                gameOverMenu.pause();
                gameOver = true;

                for (int i = 0; i < menusToClose.Length; i++) {
                    menusToClose[i].GetComponent<IMenu>().unpause();
                }
            }

        }
        else {

            lerp = Mathf.Max(lerp - Time.deltaTime / (timeToGameOver / 2f), 0);
        }

        Color newColor = sr.color;
        newColor.a = lerp;
        sr.color = newColor;

    }


    private void OnTriggerEnter2D(Collider2D collision) {
        
        CandleLightController c = collision.GetComponentInChildren<CandleLightController>();

        if (c != null) {
            if(totalTouches == 0) {
                initialTouchTime = Time.time;
            }
            
            totalTouches++;
        }

    }


    private void OnTriggerExit2D(Collider2D collision) {

        CandleLightController c = collision.GetComponentInChildren<CandleLightController>();

        if (c != null) {
            totalTouches--;
        }

    }


    private bool isTouching() {
        return totalTouches > 0;
    }


    public void resetChain() {
        gameOver = false;
    }


}
