using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ConfirmUI : MonoBehaviour
{
    public static ConfirmUI instance;

    [Header("UI Components")]
    public GameObject panel;
    public TMP_Text messageText; 
    public Button buttonYes;
    public Button buttonNo;

    private Action onYesCallback;
    private Action onNoCallback;

    private void Awake()
    {
        instance = this;
        panel.SetActive(false);
    }

    public void Show(string message, Action onYes, Action onNo)
    {
        panel.SetActive(true);
        messageText.text = message;

        onYesCallback = onYes;
        onNoCallback = onNo;

        buttonYes.onClick.RemoveAllListeners();
        buttonNo.onClick.RemoveAllListeners();

        buttonYes.onClick.AddListener(() =>
        {
            panel.SetActive(false);
            onYesCallback?.Invoke();
        });

        buttonNo.onClick.AddListener(() =>
        {
            panel.SetActive(false);
            onNoCallback?.Invoke();
        });
    }
}
