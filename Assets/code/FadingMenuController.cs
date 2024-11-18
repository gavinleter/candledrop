using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for menus that fade in and out
public class FadingMenuController : FadingObject, IMenu
{

    [SerializeField] protected List<ButtonPress> btns = new List<ButtonPress>();
    bool menuActive = false;


    protected override void decreaseLerp() {
        base.decreaseLerp();
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(-1f, transform.localPosition.y, lerp), transform.localPosition.z);
    }


    virtual public void pause() {
        base.lerpIn();

        menuActive = true;

        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(true);
        }

    }

    //when the game unpauses, start lerping in reverse
    virtual public void unpause() {
        base.lerpOut();

        menuActive = false;

        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(false);
        }

    }


    //instantly make this object appear
    override public void forceLerpIn() {
        pause();
        lerp = 1f;
    }

    //instantly make this object disappear
    override public void forceLerpOut() {
        unpause();
        lerp = 0f;
    }


    //if the unpause menu has finished going away, this returns true
    virtual public bool unpauseFinished() {
        return base.fadeOutFinished();
    }


    virtual public bool isMenuActive() {
        return menuActive;
    }
}
