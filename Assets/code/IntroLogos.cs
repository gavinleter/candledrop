using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroLogos : MonoBehaviour
{

    [SerializeField] FadingObject unityLogo;
    [SerializeField] FadingObject waffleLogo;

    [SerializeField] float unityDelay;
    [SerializeField] float waffleDelay;

    bool unityLogoStarted = false;
    bool waffleLogoStarted = false;

    private void Awake() {
        unityLogo.GetComponent<SpriteRenderer>().enabled = false;
        waffleLogo.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Start() {
        waffleLogo.GetComponent<WaffleButton>().setActive(false);
        unityLogo.setLimits(0, 1);
        waffleLogo.setLimits(0, 1);
    }


    private void Update() {

        if(Time.time > unityDelay && !unityLogoStarted) {

            unityLogo.GetComponent<SpriteRenderer>().enabled = true;
            unityLogo.lerpIn();
            unityLogoStarted = true;

        }

        if (Time.time > waffleDelay && !waffleLogoStarted) {

            waffleLogo.GetComponent<WaffleButton>().setActive(true);
            waffleLogo.GetComponent<SpriteRenderer>().enabled = true;
            waffleLogo.lerpIn();
            waffleLogoStarted = true;

        }

    }


}
