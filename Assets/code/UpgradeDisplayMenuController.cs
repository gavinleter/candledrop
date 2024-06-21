using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDisplayMenuController : FadingMenuController
{

    SpriteRenderer[] upgradeSpriteRenderers;

    bool closeMenu = false;
    float initialTime;
    [SerializeField] float closeDelay = 1.5f;

    [SerializeField] GameManager gameManager;

    protected override void Start() {
        base.Start();

        upgradeSpriteRenderers = new SpriteRenderer[transform.childCount];
        for(int i = 0; i < transform.childCount; i++) {
            upgradeSpriteRenderers[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }

        disableSpriteRenderers();
    }


    protected override void Update() {
        base.Update();

        //close the menu after the menu is brought up and a time delay has passed
        if(closeMenu && Time.time > initialTime + closeDelay) {
            unpause();
            gameManager.unpause();
        }
    }


    public void setDisplayedUpgrade(int x) {
        disableSpriteRenderers();

        upgradeSpriteRenderers[x].enabled = true;
    }


    private void disableSpriteRenderers() {
        for (int i = 0; i < upgradeSpriteRenderers.Length; i++) {
            upgradeSpriteRenderers[i].enabled = false;
        }
    }


    public override void pause() {
        base.pause();

        closeMenu = true;
        initialTime = Time.time;
        transform.localPosition = new Vector3(0, 0, -1);

    }


    public override void unpause() {
        base.unpause();

        closeMenu = false;
    }

}
