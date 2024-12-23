using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSaveMenuController : FadingMenuController
{

    [SerializeField] PauseMenuController pauseMenuController;
    [SerializeField] GameManager gameManager;

    protected override void Start() {
        base.Start();

        //delete save data button
        btns[0].onPress(() => {
            Settings.deleteAllSaveData(gameManager);
            unpause();
            pauseMenuController.pause();
        });

        //cancel button
        btns[1].onPress(() => {
            unpause();
            pauseMenuController.pause();
        });

    }


    public override void pause() {
        base.pause();

        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);
    }
}
