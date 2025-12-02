using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    private static SceneFadeManager instance;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup fade image
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
            fadeImage.raycastTarget = false;
        }
    }

    /// <summary>
    /// Fade to black (đen dần)
    /// </summary>
    public static IEnumerator FadeOut(float duration = 1f)
    {
        if (instance == null || instance.fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = instance.fadeColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            instance.fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        instance.fadeImage.color = new Color(color.r, color.g, color.b, 1f);
    }

    /// <summary>
    /// Fade from black (sáng dần)
    /// </summary>
    public static IEnumerator FadeIn(float duration = 1f)
    {
        if (instance == null || instance.fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = instance.fadeColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            instance.fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        instance.fadeImage.color = new Color(color.r, color.g, color.b, 0f);
    }

    /// <summary>
    /// Fade out → Wait → Fade in
    /// </summary>
    public static IEnumerator FadeOutAndIn(float fadeOutDuration = 1f, float waitDuration = 0.5f, float fadeInDuration = 1f)
    {
        yield return FadeOut(fadeOutDuration);
        yield return new WaitForSeconds(waitDuration);
        yield return FadeIn(fadeInDuration);
    }
}