using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PaidFastTravelPoint : MonoBehaviour
{
    public Transform destination;

    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public Text promptTextLegacy;

    public float fadeOutDuration = 1f;
    public float waitDuration = 0.5f;
    public float fadeInDuration = 1f;
    public bool disablePlayerDuringFade = true;

    public GameObject teleportEffect;
    public Color gizmoColor = Color.cyan;

    public int firstTimeCost = 500;
    public int subsequentCost = 100;

    // Confirm Dialog UI
    public GameObject confirmDialogUI;
    public TextMeshProUGUI confirmText;
    public Button yesButton;
    public Button noButton;
    public TextMeshProUGUI insufficientFundsText;

    private Transform player;
    private bool playerInRange = false;
    private bool isTeleporting = false;
    private bool isFirstTime = true;
    private int currentCost;

    // Static lock to prevent multiple teleports at once
    private static bool isAnyPointTeleporting = false;
    private static PaidFastTravelPoint activeTravelPoint = null; // Track which point opened the dialog

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        if (destination == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Destination not set!");
        }

        // Load whether this is the first time using PlayerPrefs
        isFirstTime = PlayerPrefs.GetInt("PaidFor_" + gameObject.name, 1) == 1;

        if (confirmDialogUI != null)
        {
            confirmDialogUI.SetActive(false);
        }

        // Setup button listeners
        if (yesButton != null)
        {
            yesButton.onClick.AddListener(OnYesButtonClicked);
        }
        if (noButton != null)
        {
            noButton.onClick.AddListener(OnNoButtonClicked);
        }

        if (insufficientFundsText != null)
        {
            insufficientFundsText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null || isTeleporting || isAnyPointTeleporting) return;

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        if (promptUI != null)
        {
            promptUI.SetActive(playerInRange && !isAnyPointTeleporting);
            if (playerInRange)
            {
                int cost = isFirstTime ? firstTimeCost : subsequentCost;
                if (promptText != null)
                {
                    promptText.text = $"Press {interactKey} to fast travel (Cost: {cost})";
                }
                else if (promptTextLegacy != null)
                {
                    promptTextLegacy.text = $"Press {interactKey} to fast travel (Cost: {cost})";
                }
            }
        }

        if (playerInRange && Input.GetKeyDown(interactKey) && !isAnyPointTeleporting)
        {
            currentCost = isFirstTime ? firstTimeCost : subsequentCost;
            ShowConfirmDialog();
        }
    }

    private void ShowConfirmDialog()
    {
        if (confirmDialogUI == null) return;

        // Prevent multiple dialogs from opening simultaneously
        if (isAnyPointTeleporting) return;

        // Mark this travel point as the active one
        activeTravelPoint = this;

        bool hasEnough = MoneyManager.Instance.HasEnoughMoney(currentCost);

        if (confirmText != null)
        {
            confirmText.text = $"Cần {currentCost} để dịch chuyển đến {destination.name}";
        }

        Debug.Log($"[{gameObject.name}] ShowConfirmDialog - Destination: {destination?.name ?? "NULL"}");

        if (yesButton != null)
        {
            yesButton.interactable = hasEnough;
        }

        if (insufficientFundsText != null)
        {
            insufficientFundsText.gameObject.SetActive(!hasEnough);
            if (!hasEnough)
            {
                insufficientFundsText.text = "Không đủ tiền!";
            }
        }

        confirmDialogUI.SetActive(true);

        // Optional: Pause game or disable player input while dialog is open
        // Time.timeScale = 0f; // If you want to pause the game
    }

    private void OnYesButtonClicked()
    {
        // CRITICAL: Only respond if THIS is the travel point that opened the dialog
        if (activeTravelPoint != this)
        {
            Debug.Log($"[{gameObject.name}] Ignoring Yes click - not the active travel point");
            return;
        }

        if (isAnyPointTeleporting)
        {
            Debug.LogWarning($"[{gameObject.name}] Another travel point is already teleporting!");
            return;
        }

        if (confirmDialogUI != null)
        {
            confirmDialogUI.SetActive(false);
        }

        activeTravelPoint = null; // Clear active point

        // Resume game if paused
        // Time.timeScale = 1f;

        if (MoneyManager.Instance.SpendMoney(currentCost))
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                PlayerPrefs.SetInt("PaidFor_" + gameObject.name, 0);
                PlayerPrefs.Save();
            }
            StartCoroutine(FastTravelWithFade());
        }
    }

    private void OnNoButtonClicked()
    {
        if (confirmDialogUI != null)
        {
            confirmDialogUI.SetActive(false);
        }

        activeTravelPoint = null; // Clear active point

        // Resume game if paused
        // Time.timeScale = 1f;
    }

    private IEnumerator FastTravelWithFade()
    {
        Debug.Log($"[{gameObject.name}] Starting teleport to {destination?.name ?? "NULL"}");

        if (destination == null)
        {
            Debug.LogWarning("Destination not set!");
            yield break;
        }

        if (player == null) yield break;

        isTeleporting = true;
        isAnyPointTeleporting = true; // Lock all travel points

        PlayerController playerController = null;
        if (disablePlayerDuringFade)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, player.position, Quaternion.identity);
        }

        yield return SceneFadeManager.FadeOut(fadeOutDuration);

        yield return new WaitForSeconds(waitDuration);

        player.position = destination.position;
        Debug.Log($"[{gameObject.name}] Teleported player to {destination.name} at position {destination.position}");

        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, destination.position, Quaternion.identity);
        }

        yield return SceneFadeManager.FadeIn(fadeInDuration);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isTeleporting = false;
        isAnyPointTeleporting = false; // Unlock all travel points
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            playerInRange = true;

            if (promptUI != null && !isTeleporting && !isAnyPointTeleporting)
            {
                promptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        if (destination != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, destination.position);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(destination.position, 0.5f);
            Gizmos.DrawLine(destination.position, destination.position + Vector3.up * 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.3f);

        if (destination != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(destination.position, 0.3f);
        }
    }
}