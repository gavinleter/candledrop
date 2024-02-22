using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camCtrl : MonoBehaviour
{
    Vector3 initialPosition = new Vector3(0f, 76f, -10f);
    Vector3 targetPosition;

    Vector3 gameStartPosition = new Vector3(0f, 55f, -10f);
    Vector3 gamePosition = new Vector3(0f, 10.55f, -10f);

    [SerializeField] float transitionSpeed;
    [SerializeField] float delayBeforeTransition;
    [SerializeField] float candleFallTransitionSpeed;

    private float transitionStartTime;
    private bool isTransitioning = false;

    private bool introDelayFinished = false;
    private float introStartTime;


    void Start()
    {
        // Set the camera's initial position
        transform.position = initialPosition;
        targetPosition = gameStartPosition;

        introStartTime = Time.time;
        // Start the transition delay
        //Invoke("startTransition", delayBeforeTransition);
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

            // Calculate the progress of the transition with a smooth curve
            float journeyFraction = Mathf.SmoothStep(0f, 1f, timeSinceStart / (journeyLength / transitionSpeed));

            // Smoothly move the camera using Lerp

            transform.position = Vector3.Lerp(initialPosition, targetPosition, journeyFraction);

            // Check if the transition is complete
            if (journeyFraction >= 1.0f)
            {
                isTransitioning = false;
            }
        }
    }


    public void skipIntroTransition() {
        introDelayFinished = true;
        transitionSpeed = 20f;
        startTransition();
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
        isTransitioning = false;
        initialPosition = transform.position;
        this.targetPosition = targetPosition;
        this.transitionSpeed = transitionSpeed;
    }


    public void setNewTarget(Vector3 targetPosition) {
        setNewTarget(targetPosition, transitionSpeed);
    }
}
