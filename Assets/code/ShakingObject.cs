using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakingObject : MonoBehaviour
{

    [SerializeField] float shakeAmount;
    [SerializeField] bool active;

    Vector3 positionOffset = new Vector3();
    

    void Update(){

        if (active) {

            //return to initial position
            transform.position = transform.position - positionOffset;

            positionOffset.x = UnityEngine.Random.Range(-shakeAmount, shakeAmount);
            positionOffset.y = UnityEngine.Random.Range(-shakeAmount, shakeAmount);

            //move to new position with shake
            transform.position = transform.position + positionOffset;

        }

    }


    public void setActive(bool a) {
        active = a;
    }


    public bool isActive() {
        return active;
    }

}
