using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdSpinnerLever : MonoBehaviour
{
    //310 top
    //50 bottom
    [SerializeField] Camera mainCamera;
    [SerializeField] AdSpinnerMenuController spinnerMenu;
    bool leverGrabbed = false;
    bool leverFullyPulled = false;

    void Start()
    {
        
    }

    
    void Update(){

        //move the lever back up if it is not grabbed and not lowered all the way
        if (!leverGrabbed && !leverFullyPulled) {

            Vector3 newPosition = transform.rotation.eulerAngles;
            newPosition.z -= 150f * Time.deltaTime;

            if(newPosition.z < 310 && newPosition.z > 50) {
                newPosition.z = 310f;
            }

            transform.rotation = Quaternion.Euler(newPosition);
        }

    }


    private void OnMouseDrag() {

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        //dont update lever position if the mouse if behind it or if it has been fully pulled down
        if (mousePosition.x < transform.position.x && ! leverFullyPulled) {

            leverGrabbed = true;

            //get angle between mouse and lever
            float xDist = mousePosition.x - transform.position.x;
            float yDist = mousePosition.y - transform.position.y;
            float angle = Mathf.Atan(yDist / xDist) * Mathf.Rad2Deg;

            angle = Mathf.Clamp(angle, -50, 50);

            if(angle >= 50) {
                leverFullyPulled = true;
                spinnerMenu.startSpin();
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            //Debug.Log(angle + " " + transform.rotation.eulerAngles.z + " | " + mousePosition + " | " + transform.position);
        }

    }


    private void OnMouseUp() {
        leverGrabbed = false;
    }


    public void disable() {
        GetComponent<Collider2D>().enabled = false;
        leverFullyPulled = false;
    }


    public void enable() { 
        GetComponent<Collider2D>().enabled = true;
    }


}
