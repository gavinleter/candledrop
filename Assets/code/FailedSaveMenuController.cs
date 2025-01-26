using UnityEngine;

public class FailedSaveMenuController : FadingMenuController
{

    [SerializeField] GameManager gameManager;

    protected override void Start() {
        base.Start();

        //if the user decides to continue and clear their save data
        btns[0].onPress(() => {

            unpause();
            Settings.deleteAchievementData();
            SaveManager.clearSaveData();
            //gameManager.resetGame(true);
            gameManager.unpause();

        });

    }


    override public void pause() {
        base.pause();

        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, 0f);
    }


}
