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

    // Start is called before the first frame update
    void Start(){

        GetComponent<achcam>().setBounds(topObject.transform.position.y + topBoundOffset, bottomObject.transform.position.y + bottomBoundOffset);
        transitionPosition = new Vector3(topObject.transform.position.x, topObject.transform.position.y + topBoundOffset, -10);

        //Debug.Log(btns[0].gameObject.name);
        btns[0].onPress(delegate () {
            
            pause();
            parentMenuObject.GetComponent<PauseMenuController>().pause();

            if (gameManager.isGameStarted()) {
                //mainCam.GetComponent<CameraController>().transitionToBottom(60f);
                mainCam.GetComponent<CameraController>().fadeToBlackTransitionToBottom(0.1f);
            }
            else {
                //mainCam.GetComponent<CameraController>().transitionToTop(60f);
                mainCam.GetComponent<CameraController>().fadeToBlackTransitionToTop(0.1f);
            }
            
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void unpause() {
        //mainCam.GetComponent<CameraController>().setNewTarget(transitionPosition, 60f);
        //mainCam.GetComponent<CameraController>().startTransition();
        mainCam.GetComponent<CameraController>().fadeToBlackTransition(transitionPosition, 0.1f);
        GetComponent<achcam>().setActive(true);
        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(true);
        }
    }


    public void pause() {
        GetComponent<achcam>().setActive(false);
        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(false);
        }
    }

}
