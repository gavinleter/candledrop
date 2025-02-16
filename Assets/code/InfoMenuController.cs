using UnityEngine;

public class InfoMenuController : FadingMenuController
{

    IMenu returnMenu;

    protected override void Start(){
        base.Start();

        btns[0].onPress(() => {

            unpause();
            returnMenu.unpause();

        });

    }


    public override void pause() {
        base.pause();

        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);
    }


    public void setReturnMenu(IMenu x) {
        returnMenu = x;
    }

    
}
