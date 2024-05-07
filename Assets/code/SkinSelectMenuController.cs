using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSelectMenuController : FadingMenuController
{

    [SerializeField] GameManager gameManager;
    [SerializeField] LockedFeatureMenuController lockedFeatureMenu;


    protected override void Start(){
        base.Start();

        //generates an anonymous method for each skin select button
        System.Func<int, System.Action> skinButton = (int id) => {
            return delegate () {

                //if the skin is unlocked, switch skins immediately
                if (Settings.skinUnlocked(gameManager.getCurrentStarterCandleId(), id)) {
                    gameManager.setStarterCandleSkin(id);
                }
                else {
                    //otherwise bring up the locked feature menu
                    unpause();
                    //the gamemanager becomes active when this menu is closed, so it needs to be stopped again
                    gameManager.pause();
                    lockedFeatureMenu.setDefaultExitAction(this);
                    lockedFeatureMenu.pause();
                }

            };
        };

        //go through each skin select button and assign their methods, also remove cover if they're unlocked
        for(int i = 0; i < 5; i++) {
            btns[i].onPress(skinButton(i));

            if (Settings.skinUnlocked(gameManager.getCurrentStarterCandleId(), i)) {
                btns[i].transform.Find("skin_select_cover").GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        //confirm select skin button
        btns[5].onPress(delegate () {
            unpause();
            gameManager.unpause();
        });
    }


    protected override void Update(){
        base.Update();
        

    }



    override public void pause() {
        base.pause();
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);
    }

    override public void unpause() {
        base.unpause();
        gameManager.unpause();
        //the game needs to be reset so that the starter candle can be switched out when it respawns
        gameManager.resetGame();
    }

}
