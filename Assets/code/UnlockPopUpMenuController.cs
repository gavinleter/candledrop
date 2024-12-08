using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPopUpMenuController : MonoBehaviour, IMenu
{

    [SerializeField] CameraController mainCam;
    [SerializeField] AchievementMenuController achievementMenu;
    [SerializeField] GameManager gameManager;
    [SerializeField] ButtonPress[] btns;
    bool isActive = false;
    //target position is where the menu should be when active, off position is when the menu isnt active
    float targetPosition;
    float offPosition;
    Vector3 resultPosition = new Vector3();
    float lerp = 0;

    //how long the popup stays before closing
    [SerializeField] float closeTime;
    float initialTime = 0;

    //which achievement to go to in the achievement menu
    int targetAchievement = -1;


    void Start() {

        targetPosition = mainCam.getCamHeight() - GetComponent<SpriteRenderer>().bounds.extents.y;
        offPosition = targetPosition + 10;

        resultPosition.z = transform.position.z;

        btns[0].onPress(() => {

            achievementMenu.setTransitionPosition(targetAchievement);
            gameManager.pause();
            unpause();
            achievementMenu.pause();

        });

    }


    void Update() {

        if (isMenuActive()) {
            lerp = Mathf.Min(1, lerp + Time.deltaTime);

            if (initialTime + closeTime < Time.time) { 
                unpause();
            }
        }
        else {
            lerp = Mathf.Max(0, lerp - Time.deltaTime);
        }

        resultPosition.y = Mathf.SmoothStep(offPosition + mainCam.transform.position.y, targetPosition + mainCam.transform.position.y, lerp);
        transform.position = resultPosition;

    }


    public void pause() {
        
        //menu should not open if no target achievement has been set
        if(targetAchievement != -1) {
            isActive = true;

            initialTime = Time.time;

            for (int i = 0; i < btns.Length; i++) {
                btns[i].setActive(true);
            }
        }

    }


    public void unpause() {
        if (isMenuActive()) {

            isActive = false;

            for (int i = 0; i < btns.Length; i++) {
                btns[i].setActive(false);
            }

        }

        targetAchievement = -1;
    }


    public bool isMenuActive() {
        return isActive;
    }


    public void setTargetAchievement(int x) {
        targetAchievement = x;
    }

}
