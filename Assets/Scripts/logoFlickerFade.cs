using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogoFlickerFade : MonoBehaviour
{
    [Header("UI Image to Flicker")]
    public Image logoImage;

    [Header("Flicker Settings")]
    public float minFlickDelay = 1f;
    public float maxFlickDelay = 4f;
    public float fadeDuration = 0.15f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;

    void Start()
    {
        if (logoImage == null)
            logoImage = GetComponent<Image>();

        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Wait before flickering
            yield return new WaitForSeconds(Random.Range(minFlickDelay, maxFlickDelay));

            // Fade out
            yield return StartCoroutine(FadeToAlpha(minAlpha));

            // Quick pause while "flickered"
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

            // Fade back in
            yield return StartCoroutine(FadeToAlpha(maxAlpha));
        }
    }

    IEnumerator FadeToAlpha(float targetAlpha)
    {
        float startAlpha = logoImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color c = logoImage.color;
            c.a = newAlpha;
            logoImage.color = c;

            yield return null;
        }

        // Ensure exact value at end
        Color finalColor = logoImage.color;
        finalColor.a = targetAlpha;
        logoImage.color = finalColor;
    }
}

