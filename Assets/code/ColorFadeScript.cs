using UnityEngine;

public class ColorFadeScript : MonoBehaviour
{
    public GameObject fallObject;  // Drag and drop the "fall object" in the Inspector
    public float fadeDuration = 1f;  // Duration for the color fade, adjustable in the Inspector
    public float delayAfterFade = 3f;  // Delay in seconds after reaching zero opacity

    private float initialAlpha;
    private float timeStartedLerping;

    private void Start()
    {
        if (GetComponent<Renderer>() != null)
        {
            initialAlpha = GetComponent<Renderer>().material.color.a;
        }
        else
        {
            Debug.LogWarning("Renderer not found on the GameObject! Please attach a Renderer component.");
        }
    }

    private void Update()
    {
        // Check if the fall object exists and its Y value has crossed 51.5
        if (fallObject != null && fallObject.transform.position.y < 51.5f)
        {
            // Check if not already fading
            if (initialAlpha > 0)
            {
                // Start fading smoothly
                timeStartedLerping = Time.time;
                StartCoroutine(FadeOutSmoothly());
            }
            else
            {
                // Check if the delay period has passed
                if (Time.time - timeStartedLerping >= delayAfterFade)
                {
                    // Reset to the initial opacity
                    Color resetColor = GetComponent<Renderer>().material.color;
                    resetColor.a = initialAlpha;
                    GetComponent<Renderer>().material.color = resetColor;
                }
            }
        }
    }

    private System.Collections.IEnumerator FadeOutSmoothly()
    {
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float percentageComplete = timeElapsed / fadeDuration;
            Color newColor = GetComponent<Renderer>().material.color;
            newColor.a = Mathf.Lerp(initialAlpha, 0f, percentageComplete);
            GetComponent<Renderer>().material.color = newColor;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set to fully transparent
        Color finalColor = GetComponent<Renderer>().material.color;
        finalColor.a = 0f;
        GetComponent<Renderer>().material.color = finalColor;

        // Check if the delay period has passed
        yield return new WaitForSeconds(delayAfterFade);

        // Reset to the initial opacity
        Color resetColor = GetComponent<Renderer>().material.color;
        resetColor.a = initialAlpha;
        GetComponent<Renderer>().material.color = resetColor;
    }
}
