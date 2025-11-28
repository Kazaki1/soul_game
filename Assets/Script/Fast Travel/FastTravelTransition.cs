using UnityEngine;
using System.Collections;

public class FastTravelTransition : MonoBehaviour
{
    public static FastTravelTransition instance;

    public CanvasGroup fadeUI;
    public float fadeTime = 0.4f;

    private bool isFading = false;

    private void Awake()
    {
        instance = this;
        fadeUI.alpha = 0;
        fadeUI.blocksRaycasts = false;
        fadeUI.gameObject.SetActive(false);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(0.001f, 1, true); // alpha start nhỏ tránh nháy
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(1, 0, false);
    }

    private IEnumerator Fade(float from, float to, bool blockInput)
    {
        if (isFading) yield break;
        isFading = true;

        fadeUI.gameObject.SetActive(true);
        fadeUI.blocksRaycasts = blockInput;
        fadeUI.alpha = from;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            fadeUI.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }
        fadeUI.alpha = to;

        // Nếu fade out, tắt panel
        if (to == 0) fadeUI.gameObject.SetActive(false);

        fadeUI.blocksRaycasts = false;
        isFading = false;
    }
}
