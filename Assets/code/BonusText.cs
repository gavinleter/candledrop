using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusText : FadingObject
{

    [SerializeField] float riseSpeed;
    [SerializeField] float riseSpeedRandomRange;
    float newRiseSpeed;


    protected override void Start() {
        base.Start();

        newRiseSpeed = riseSpeed + Random.value * riseSpeedRandomRange;

        forceAppear();
        fadeOut();
    }


    protected override void Update() {
        base.Update();
         
        transform.position = new Vector3(transform.position.x, transform.position.y + newRiseSpeed * Time.deltaTime, transform.position.z);

        if (fadeOutFinished()) {
            Destroy(gameObject);
        }
    }

    public void setSprite(Sprite s) {
        GetComponent<SpriteRenderer>().sprite = s;
    }

}
