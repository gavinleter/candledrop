using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementMenuController : MonoBehaviour, IMenu
{

    [SerializeField] GameObject topObject;
    [SerializeField] GameObject bottomObject;
    [SerializeField] float topBoundOffset;
    [SerializeField] float bottomBoundOffset;

    [SerializeField] GameObject parentMenuObject;
    IMenu parentMenu;
    [SerializeField] List<ButtonPress> btns;
    [SerializeField] Camera mainCam;
    [SerializeField] GameManager gameManager;

    Vector3 transitionPosition;
    bool active = false;

    // Start is called before the first frame update
    void Start(){

        GetComponent<achcam>().setBounds(topObject.transform.position.y + topBoundOffset, bottomObject.transform.position.y + bottomBoundOffset);
        transitionPosition = new Vector3(topObject.transform.position.x, topObject.transform.position.y + topBoundOffset, -10);

        //return back up button
        btns[0].onPress(()=> {
            
            unpause();
            parentMenuObject.GetComponent<PauseMenuController>().pause();

            transitionBackUp();
            
        });

    } 
    
    //go back up to where the camera was before
    void transitionBackUp() {
        if (gameManager.isGameStarted()) {
            //mainCam.GetComponent<CameraController>().transitionToBottom(60f);
            mainCam.GetComponent<CameraController>().fadeToBlackTransitionToBottom(0.1f);
        }
        else {
            //mainCam.GetComponent<CameraController>().transitionToTop(60f);
            mainCam.GetComponent<CameraController>().fadeToBlackTransitionToTop(0.1f);
        }
    }


    public void unpause() {
        //this method is called when a game over happens to boot the player out of this menu
        //so the transition back up should not happen if the player wasn't in this menu to begin with
        if (active) {
            transitionBackUp();
        }

        GetComponent<achcam>().setActive(false);
        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(false);
        }

        active = false;
    }


    public void pause() {
        active = true;

        //mainCam.GetComponent<CameraController>().setNewTarget(transitionPosition, 60f);
        //mainCam.GetComponent<CameraController>().startTransition();
        mainCam.GetComponent<CameraController>().fadeToBlackTransition(transitionPosition, 0.1f);
        GetComponent<achcam>().setActive(true);
        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(true);
        }
    }

}
