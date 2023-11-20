using UnityEngine;

public class basecamdown : MonoBehaviour
{
    public float slideSecs = 1.0f; // Set in Unity inspector
    public float basementYLevel = 0.0f; // Set in Unity inspector

    private bool isTransitioning = false;
    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private float transitionStartTime;

    void Start()
    {
        // Initial setup
        initialPosition = Camera.main.transform.position;
        targetPosition = new Vector3(initialPosition.x, basementYLevel, initialPosition.z);
    }

    void Update()
    {
        if (isTransitioning)
        {
            // Calculate the normalized time value for the Lerp function
            float t = (Time.time - transitionStartTime) / slideSecs;

            // Use Lerp to smoothly interpolate between the initial and target positions
            Camera.main.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            // Check if the transition is complete
            if (t >= 1.0f)
            {
                isTransitioning = false;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // Start the transition when the mouse button is initially pressed
            StartTransition();
        }
    }

    void StartTransition()
    {
        // Set up the transition parameters
        isTransitioning = true;
        initialPosition = Camera.main.transform.position;
        transitionStartTime = Time.time;
    }
}
