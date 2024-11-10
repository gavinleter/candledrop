using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroLogos : MonoBehaviour
{

    [SerializeField] FadingObject unityLogo;
    [SerializeField] FadingObject waffleLogo;

    [SerializeField] float unityDelay;
    [SerializeField] float waffleDelay;


    private void Awake() {
        unityLogo.GetComponent<SpriteRenderer>().enabled = false;
        waffleLogo.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Start() {
        waffleLogo.GetComponent<WaffleButton>().setActive(false);
    }


    private void Update() {
        
        if(Time.time > unityDelay) {

            unityLogo.GetComponent<SpriteRenderer>().enabled = true;
            unityLogo.lerpIn();

        }

        if (Time.time > waffleDelay) {

            waffleLogo.GetComponent<WaffleButton>().setActive(true);
            waffleLogo.GetComponent<SpriteRenderer>().enabled = true;
            waffleLogo.lerpIn();

        }

    }


}
