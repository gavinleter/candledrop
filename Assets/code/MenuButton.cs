using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : ButtonPress
{

    [SerializeField] GameObject nextMenuObject;
    IMenu nextMenu;

    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        nextMenu = nextMenuObject.GetComponent<IMenu>();
    }

    // Update is called once per frame
    void Update(){


    }

    protected override void OnMouseDown() {
        base.OnMouseDown();
        active = false;
        nextMenu.pause();
    }

    protected override void OnMouseUp() { 
        base.OnMouseUp();
    }

 
}
