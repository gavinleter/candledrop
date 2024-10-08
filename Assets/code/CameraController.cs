using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 initialPosition = new Vector3(0f, 76f, -10f);
    Vector3 targetPosition;

    Vector3 gameStartPosition = new Vector3(0f, 55f, -10f);
    Vector3 gamePosition = new Vector3(0f, 10.55f, -10f);

    [SerializeField] float transitionSpeed;
    [SerializeField] float delayBeforeTransition;
    [SerializeField] float candleFallTransitionSpeed;

    [SerializeField] FadingObject blackFadeObject;

    private float transitionStartTime;
    private bool isTransitioning = false;
    private bool isBlackFadeTransitioning = false;
    private bool initialBlackFadeInCompleted = false;

    private bool introDelayFinished = false;


    void Start()
    {
        transform.position = initialPosition;
        targetPosition = gameStartPosition;

    }

    void Update()
    {

        //wait for x seconds before transitioning downwards when the game starts
        if(!introDelayFinished && delayBeforeTransition < Time.time) {
            introDelayFinished = true;
            startTransition();
        }


        if (isTransitioning)
        {
            //check if the target position is the same as the starting position to prevent division by 0
            if (initialPosition.Equals(targetPosition)) {
                isTransitioning = false;
                return;
            }

            float timeSinceStart = Time.time - transitionStartTime;
            float journeyLength = Vector3.Distance(initialPosition, targetPosition);

            float journeyFraction = Mathf.SmoothStep(0f, 1f, timeSinceStart / (journeyLength / transitionSpeed));

            transform.position = Vector3.Lerp(initialPosition, targetPosition, journeyFraction);

            //check if the transition is complete
            if (journeyFraction >= 1.0f)
            {
                isTransitioning = false;
            }
        }


        if (isBlackFadeTransitioning) {
            if (blackFadeObject.fadeInFinished() && !initialBlackFadeInCompleted) {

                blackFadeObject.lerpOut();
                transform.position = targetPosition;
                initialBlackFadeInCompleted = true;
            }
            if (blackFadeObject.fadeOutFinished() && initialBlackFadeInCompleted) {

                isBlackFadeTransitioning = false;
                initialBlackFadeInCompleted = false;
            }
        }
    }


    public void skipIntroTransition() {
        if (!introDelayFinished) {
            introDelayFinished = true;
            transitionSpeed = 20f;
            startTransition();
        }
    }

    public void setTransitionSpeed(float speed) {
        transitionSpeed = speed;
    }

    public void startTransition()
    {
        isTransitioning = true;
        transitionStartTime = Time.time;
    }

    public void startGameTransition() {
        isTransitioning = false;
        initialPosition = gameStartPosition;
        targetPosition = gamePosition;
        transitionSpeed = candleFallTransitionSpeed;
        startTransition();
    }


    public void restartTransition() {
        setNewTarget(gameStartPosition, candleFallTransitionSpeed);
        startTransition();
    }


    public void transitionToTop(float speed) {
        setNewTarget(gameStartPosition, speed);
        startTransition();
    }


    public void transitionToBottom(float speed) {
        setNewTarget(gamePosition, speed);
        startTransition();
    }


    public void setNewTarget(Vector3 targetPosition, float transitionSpeed) {
        introDelayFinished = true;
        isTransitioning = false;
        initialPosition = transform.position;
        this.targetPosition = targetPosition;
        this.transitionSpeed = transitionSpeed;
    }


    public void setNewTarget(Vector3 targetPosition) {
        setNewTarget(targetPosition, transitionSpeed);
    }


    public bool currentlyTransitioning() {
        return isTransitioning || isBlackFadeTransitioning;
    }


    public void fadeToBlackTransition(Vector3 targetPosition, float fadeSpeed) {
        blackFadeObject.forceLerpOut();
        blackFadeObject.transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        blackFadeObject.setSpeed(fadeSpeed);
        blackFadeObject.lerpIn();
        this.targetPosition = targetPosition;
        isBlackFadeTransitioning = true;
    }


    public void fadeToBlackTransitionToTop(float fadeSpeed) {
        fadeToBlackTransition(gameStartPosition, fadeSpeed);
    }


    public void fadeToBlackTransitionToBottom(float fadeSpeed) {
        fadeToBlackTransition(gamePosition, fadeSpeed);
    }

}
