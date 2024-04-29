using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : FadingMenuController
{
    
    [SerializeField] GameObject parentMenuObject;
    IMenu parentMenu;

    [SerializeField] GameObject achievementMenuObject;


    // Start is called before the first frame update
    protected override void Start(){
        base.Start();


        parentMenu = parentMenuObject.GetComponent<IMenu>();

        GameObject mainCam = transform.parent.gameObject;

        //resume button
        btns[0].onPress(delegate() { 
            unpause();
            parentMenu.unpause();
            /*if (parentMenuObject.GetComponent<GameManager>().isGameStarted()) {
                parentMenu.unpause();
            }*/
        });

        //restart button
        btns[1].onPress(delegate() {
            parentMenuObject.GetComponent<GameManager>().resetGame();
            mainCam.GetComponent<camCtrl>().restartTransition();
            unpause();
            parentMenu.unpause();
        });

        //achievements button
        btns[2].onPress(delegate() {
            unpause();
            achievementMenuObject.GetComponent<AchievementMenuController>().unpause();
        });

        //sound enable/disable button
        btns[3].onPress(delegate () {
            Settings.soundEnabled = !Settings.soundEnabled;
            btns[3].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = !Settings.soundEnabled;
        });

    }


    //when the game pauses, start lerping
    override public void pause() {
        base.pause();

        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);

    }

    //when the game unpauses, start lerping in reverse
    override public void unpause() {
        base.unpause();
    }


}
