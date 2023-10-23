using UnityEngine;

[System.Serializable]
public class ObjectFadingParameters
{
    public GameObject objectToFade;
    public float fadeSpeed = 1.0f;
    public float fadeInDelay = 2.0f; // Time to wait before fading in
    public float fadeOutTime = 2.0f;
}

public class logoFade : MonoBehaviour
{
    public ObjectFadingParameters[] objectsToFade;

    private void Start()
    {
        // Set objects to be invisible initially
        foreach (ObjectFadingParameters parameters in objectsToFade)
        {
            if (parameters.objectToFade != null)
            {
                Color objectColor = parameters.objectToFade.GetComponent<Renderer>().material.color;
                objectColor.a = 0f; // Start with an alpha of 0 to make the object invisible
                parameters.objectToFade.GetComponent<Renderer>().material.color = objectColor;
                parameters.objectToFade.SetActive(false); // Make sure it's inactive initially
            }
        }
    }

    private void Update()
    {
        foreach (ObjectFadingParameters parameters in objectsToFade)
        {
            if (parameters.objectToFade == null)
            {
                // Skip objects that are not set
                continue;
            }

            parameters.fadeInDelay -= Time.deltaTime;

            if (parameters.fadeInDelay <= 0f)
            {
                float delta = Time.deltaTime * parameters.fadeSpeed;

                // Fading in logic
                if (!parameters.objectToFade.activeSelf)
                {
                    parameters.objectToFade.SetActive(true);
                }

                Color objectColor = parameters.objectToFade.GetComponent<Renderer>().material.color;
                objectColor.a = Mathf.Clamp01(objectColor.a + delta / parameters.fadeSpeed);
                parameters.objectToFade.GetComponent<Renderer>().material.color = objectColor;

                if (objectColor.a == 1f)
                {
                    // When the object is fully visible, stop fading
                    parameters.fadeInDelay = 0f;
                }
            }
        }
    }
}
