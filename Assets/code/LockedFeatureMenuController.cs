using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedFeatureMenuController : FadingMenuController
{


    public override void pause() {
        base.pause();
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);
    }

    //what happens when the menu is exited needs to be set every time this menu is used
    //this method is for if something special needs to happen
    public void setExitAction(System.Action a) {
        btns[0].clearActions();
        btns[0].onPress(a);
    }

    //this method is for if the menu just needs to be closed and the previous one re-opened (if there is a previous one)
    public void setDefaultExitAction(IMenu previousMenu) {
        btns[0].clearActions();
        btns[0].onPress(delegate () {

            unpause();
            if (previousMenu != null) { 
                previousMenu.pause();
            }

        });
    }


    public void setDefaultExitAction() {
        setDefaultExitAction(null);
    }

}
