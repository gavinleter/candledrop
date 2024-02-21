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

        btns[0].onPress(delegate () {
            pause();
            parentMenuObject.GetComponent<PauseMenuController>().pause();

            if (gameManager.isGameStarted()) {
                mainCam.GetComponent<camCtrl>().transitionToBottom(60f);
            }
            else {
                mainCam.GetComponent<camCtrl>().transitionToTop(60f);
            }

        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void unpause() {
        mainCam.GetComponent<camCtrl>().setNewTarget(transitionPosition, 20f);
        mainCam.GetComponent<camCtrl>().startTransition();
        GetComponent<achcam>().setActive(true);
        for (int i = 0; i < btns.Count; i++) {
            btns[i].active = true;
        }
    }


    public void pause() {
        GetComponent<achcam>().setActive(false);
        for (int i = 0; i < btns.Count; i++) {
            btns[i].active = false;
        }
    }

}
