using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonController : ButtonPress
{

    [SerializeField] GameObject pauseMenu;
    PauseMenuController pauseController;
    //ButtonPress click;

    // Start is called before the first frame update
    protected override void Start(){
        //click = GetComponent<ButtonPress>();
        pauseController = pauseMenu.GetComponent<PauseMenuController>();
        base.Start();
    }

    // Update is called once per frame
    void Update(){

       //if (click.btnPressed()) {
            //pauseMenu.GetComponent<PauseMenuController>().pause();
        //}

    }

    protected override void OnMouseDown() {
        base.OnMouseDown();
        pauseController.pause();
    }

    protected override void OnMouseUp() { 
        base.OnMouseUp();
        //pauseController.unpause();
    }
}
