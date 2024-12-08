using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusText : FadingObject
{

    [SerializeField] float riseSpeed;
    [SerializeField] float riseSpeedRandomRange;
    float highlightRiseSpeedMultiplier = 1;
    float newRiseSpeed;
    RainbowObject rainbow;
    GrowingObject growing;


    protected override void Awake() {
        base.Awake();

        rainbow = GetComponent<RainbowObject>();
        growing = GetComponent<GrowingObject>();
    }

    protected override void Start() {
        base.Start();

        newRiseSpeed = riseSpeed + Random.value * riseSpeedRandomRange;

        forceLerpIn();
        lerpOut();
    }


    protected override void Update() {
        base.Update();
         
        transform.position = new Vector3(transform.position.x, transform.position.y + newRiseSpeed * highlightRiseSpeedMultiplier * Time.deltaTime, transform.position.z);

        if (fadeOutFinished()) {
            Destroy(gameObject);
        }
    }

    public void setSprite(Sprite s) {
        GetComponent<SpriteRenderer>().sprite = s;
    }

    //turns on the growing and rainbow effects for this
    public void enableHighlight() {
        enableGrowing();
        rainbow.setActive(true);
        highlightRiseSpeedMultiplier = 1.3f;
    }


    public void enableGrowing() {
        growing.setActive(true);
    }


    public static int getBonusTextSpriteIdByPoints(int points) {

        switch (points) {
            case 2:
                return 9;
            case 3:
                return 10;
            default:
                return 0;
        }

    }

}
