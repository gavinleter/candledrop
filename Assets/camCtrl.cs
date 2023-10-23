using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camCtrl : MonoBehaviour
{
    public Vector3 initialPosition = new Vector3(0f, 76f, 0f);
    public Vector3 targetPosition = new Vector3(0f, 55f, 0f);
    public float transitionSpeed = 5f;
    public float delayBeforeTransition = 1f;

    private float transitionStartTime;
    private bool isTransitioning = false;

    void Start()
    {
        // Set the camera's initial position
        transform.position = initialPosition;

        // Start the transition delay
        Invoke("StartTransition", delayBeforeTransition);
    }

    void Update()
    {
        if (isTransitioning)
        {
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

    void StartTransition()
    {
        isTransitioning = true;
        transitionStartTime = Time.time;
    }
}
