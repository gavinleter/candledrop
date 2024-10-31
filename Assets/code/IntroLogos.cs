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
        unityLogo.gameObject.SetActive(false);
        waffleLogo.gameObject.SetActive(false);
    }


    private void Update() {
        
        if(Time.time > unityDelay) {

            unityLogo.gameObject.SetActive(true);
            unityLogo.lerpIn();

        }

        if (Time.time > waffleDelay) {

            waffleLogo.gameObject.SetActive(true);
            waffleLogo.lerpIn();

        }

    }


}
