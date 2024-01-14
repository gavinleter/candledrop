using UnityEngine;
using System.Collections;

public class BoomSeq : MonoBehaviour
{
    public float fadeDuration = 1f; // Duration in seconds for fading to white
    public float shrinkDuration = 1f; // Duration in seconds for scaling down
    public GameObject boomParticles; // Reference to the boom particle system prefab

    private bool isActivated = false;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (!isActivated && CheckActivationRequirement())
        {
            isActivated = true;
            ActivateBoomSequence();
        }
    }

    private bool CheckActivationRequirement()
    {
        // Check if the left mouse button is pressed
        return Input.GetMouseButtonDown(0);
    }

    private void ActivateBoomSequence()
    {
        StartCoroutine(FadeToWhiteAndShrink());
    }

    private IEnumerator FadeToWhiteAndShrink()
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale / 8f; // code to the left of this determines just how small da object gets

        while (elapsedTime < fadeDuration)
        {
            // Interpolate color from initial color to white
            float t = elapsedTime / fadeDuration;
            Color lerpedColor = new Color(1f, 1f, 1f, 1f);

            // Directly change the SpriteRenderer color values
            spriteRenderer.color = Color.Lerp(initialColor, lerpedColor, t);

            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PlayBoomParticles();
        UnloadObject();
    }

    private void PlayBoomParticles()
    {
        if (boomParticles != null)
        {
            Instantiate(boomParticles, transform.position, Quaternion.identity);
        }
    }

    private void UnloadObject()
    {
        Destroy(gameObject);
    }
}
