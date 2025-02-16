using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used for menus that fade in and out
public class FadingMenuController : FadingObject, IMenu
{

    [SerializeField] protected List<ButtonPress> btns = new List<ButtonPress>();
    bool menuActive = false;
    //this is used for the shift down when the menu is closed 
    float closingYPosition = 0;


    protected override void decreaseLerp() {
        base.decreaseLerp();
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(closingYPosition - 1f, closingYPosition, lerp), transform.localPosition.z);
    }


    virtual public void pause() {
        base.lerpIn();

        menuActive = true;

        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(true);
        }

        SpriteMask[] x = GetComponentsInChildren<SpriteMask>();

        for (int i = 0; i < x.Length; i++) {
            x[i].enabled = true;
        }

    }

    //when the game unpauses, start lerping in reverse
    virtual public void unpause() {

        if (!menuActive) {
            return;
        }

        closingYPosition = transform.localPosition.y;

        base.lerpOut();

        menuActive = false;

        for (int i = 0; i < btns.Count; i++) {
            btns[i].setActive(false);
        }

        //since fading menus generally stay on top of the camera it can cause layering issues with other sprite masks in the scene
        SpriteMask[] x = GetComponentsInChildren<SpriteMask>();

        for (int i = 0; i < x.Length; i++) {
            x[i].enabled = false;
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
