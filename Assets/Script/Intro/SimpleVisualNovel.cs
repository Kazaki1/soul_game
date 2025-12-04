using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SimpleVisualNovel : MonoBehaviour
{
    [Header("Dialogue Lines")]
    [TextArea(3, 10)]
    public List<string> dialogueLines = new List<string>();
    public string nextSceneName; 

    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public CanvasGroup textCanvasGroup;

    [Header("Settings")]
    public KeyCode advanceKey = KeyCode.Space;
    public float textSpeed = 0.05f;
    public float fadeDuration = 0.5f; 

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isFading = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (dialogueLines.Count > 0)
        {
            StartCoroutine(ShowLine(0));
        }
        else
        {
            Debug.LogError("No dialogue lines assigned!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(advanceKey) && !isFading)
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    private IEnumerator ShowLine(int index)
    {
        if (index >= dialogueLines.Count)
        {
            yield return StartCoroutine(EndDialogue());
            yield break;
        }

        currentLineIndex = index;

        // Fade in text
        yield return StartCoroutine(FadeText(0f, 1f));

        // Type out text
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(dialogueLines[index]));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = dialogueLines[currentLineIndex];
        isTyping = false;
    }

    private void NextLine()
    {
        StartCoroutine(FadeAndNext());
    }

    private IEnumerator FadeAndNext()
    {
        yield return StartCoroutine(FadeText(1f, 0f));

        currentLineIndex++;
        yield return StartCoroutine(ShowLine(currentLineIndex));
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha)
    {
        isFading = true;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            textCanvasGroup.alpha = alpha;
            yield return null;
        }

        textCanvasGroup.alpha = endAlpha;
        isFading = false;
    }

    private IEnumerator EndDialogue()
    {
        Debug.Log("Dialogue finished! Loading next scene: " + nextSceneName);

        yield return StartCoroutine(FadeText(1f, 0f));

        yield return new WaitForSeconds(0.5f);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No next scene specified!");
        }
    }
}