using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSelectMenuController : FadingMenuController
{

    [SerializeField] GameManager gameManager;
    [SerializeField] LockedFeatureMenuController lockedFeatureMenu;

    [SerializeField] GameObject selectedSkinIndicator;

    int selectedSkin = 0;


    protected override void Start(){
        base.Start();

        //generates an anonymous method for each skin select button
        System.Func<int, System.Action> skinButton = (int id) => {
            return delegate () {
                
                //if the skin is unlocked, switch skins immediately
                if (Settings.skinUnlocked(id)) {
                    selectedSkin = id;
                    refreshSelectorPosition();
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

        //go through each skin select button and assign their methods
        for(int i = 0; i < 11; i++) {
            btns[i].onPress(skinButton(i));
        }

        //confirm select skin button
        btns[12].onPress(delegate () {
            gameManager.setStarterCandleSkin(selectedSkin);
            unpause();
            gameManager.unpause();
        });
    }


    protected override void Update(){
        base.Update();
        

    }



    override public void pause() {
        refreshSkinButtons();
        selectedSkin = gameManager.getCurrentStarterCandleSkinId();
        refreshSelectorPosition();

        base.pause();
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);

    }

    override public void unpause() {
        base.unpause();
        gameManager.unpause();
        //the game needs to be reset so that the starter candle can be switched out when it respawns
        gameManager.resetGame();
    }


    //remove covers of unlocked skins and remove normal sprite of locked skins
    void refreshSkinButtons() {

        for (int i = 0; i < 12; i++) {

            if (Settings.skinUnlocked(i)) {
                btns[i].transform.Find("skin_select_cover").GetComponent<SpriteRenderer>().enabled = false;
                btns[i].GetComponent<SpriteRenderer>().enabled = true;
            }
            else {
                btns[i].GetComponent<SpriteRenderer>().enabled = false;
            }

        }

    }


    void refreshSelectorPosition() {

        selectedSkinIndicator.transform.localPosition = new Vector3(-0.25f + (selectedSkin % 3 * 0.25f), 0.5f - (selectedSkin / 3 * 0.25f), 0);

    }


}
