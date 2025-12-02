using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Fast Travel Hub - Travel đến nhiều địa điểm, cần unlock bằng tiền
/// </summary>
public class MultiFastTravelHub : MonoBehaviour
{
    [System.Serializable]
    public class TravelDestination
    {
        public string locationName = "Unknown Location";
        public Transform destination;
        public int unlockCost = 1000;
        public Sprite locationIcon;
        [HideInInspector] public bool isUnlocked = false;
    }

    [Header("Destinations")]
    [SerializeField] private List<TravelDestination> destinations = new List<TravelDestination>();

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("UI References")]
    [SerializeField] private GameObject travelMenuUI;
    [SerializeField] private GameObject destinationButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject promptUI;

    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float waitDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 1f;

    private Transform player;
    private bool playerInRange = false;
    private bool isTeleporting = false;
    private bool isMenuOpen = false;

    private void Start()
    {
        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Ẩn UI
        if (travelMenuUI != null)
            travelMenuUI.SetActive(false);

        if (promptUI != null)
            promptUI.SetActive(false);

        // Load unlock status
        LoadUnlockStatus();
    }

    private void Update()
    {
        if (player == null || isTeleporting) return;

        // Check khoảng cách
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        // Show/Hide prompt
        if (promptUI != null && !isMenuOpen)
        {
            promptUI.SetActive(playerInRange);
        }

        // Mở menu
        if (playerInRange && Input.GetKeyDown(interactKey) && !isMenuOpen)
        {
            OpenTravelMenu();
        }

        // Đóng menu
        if (isMenuOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTravelMenu();
        }
    }

    /// <summary>
    /// Mở menu chọn địa điểm
    /// </summary>
    private void OpenTravelMenu()
    {
        isMenuOpen = true;

        if (travelMenuUI != null)
        {
            travelMenuUI.SetActive(true);
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        // Tạo buttons cho từng địa điểm
        PopulateDestinationButtons();

        // Pause game (optional)
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Đóng menu
    /// </summary>
    private void CloseTravelMenu()
    {
        isMenuOpen = false;

        if (travelMenuUI != null)
        {
            travelMenuUI.SetActive(false);
        }

        // Unpause game
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Tạo buttons cho destinations
    /// </summary>
    private void PopulateDestinationButtons()
    {
        if (buttonContainer == null || destinationButtonPrefab == null) return;

        // Xóa buttons cũ
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Tạo button cho mỗi destination
        for (int i = 0; i < destinations.Count; i++)
        {
            int index = i; // Capture for lambda
            TravelDestination dest = destinations[i];

            GameObject buttonObj = Instantiate(destinationButtonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();

            // Setup button text
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (dest.isUnlocked)
                {
                    buttonText.text = dest.locationName;
                }
                else
                {
                    buttonText.text = $"{dest.locationName}\n🔒 {dest.unlockCost} Gold";
                }
            }

            // Setup button icon (optional)
            Image buttonIcon = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
            if (buttonIcon != null && dest.locationIcon != null)
            {
                buttonIcon.sprite = dest.locationIcon;
            }

            // Setup button click
            if (button != null)
            {
                button.onClick.AddListener(() => OnDestinationButtonClicked(index));
            }
        }
    }

    /// <summary>
    /// Khi click vào destination button
    /// </summary>
    private void OnDestinationButtonClicked(int index)
    {
        if (index < 0 || index >= destinations.Count) return;

        TravelDestination dest = destinations[index];

        // Nếu chưa unlock
        if (!dest.isUnlocked)
        {
            TryUnlockDestination(index);
        }
        // Nếu đã unlock
        else
        {
            StartCoroutine(TravelToDestination(dest));
        }
    }

    /// <summary>
    /// Thử unlock destination
    /// </summary>
    private void TryUnlockDestination(int index)
    {
        TravelDestination dest = destinations[index];

        if (MoneyManager.Instance == null)
        {
            Debug.LogError("MoneyManager not found!");
            return;
        }

        // Check tiền
        if (MoneyManager.Instance.HasEnoughMoney(dest.unlockCost))
        {
            // Trả tiền
            if (MoneyManager.Instance.SpendMoney(dest.unlockCost))
            {
                // Unlock
                dest.isUnlocked = true;
                SaveUnlockStatus();

                Debug.Log($"✅ Unlocked: {dest.locationName}");

                // Refresh UI
                PopulateDestinationButtons();
            }
        }
        else
        {
            Debug.Log($"❌ Not enough money! Need {dest.unlockCost}, have {MoneyManager.Instance.currentMoney}");
            // TODO: Show "Not enough money" popup
        }
    }

    /// <summary>
    /// Travel đến destination
    /// </summary>
    private IEnumerator TravelToDestination(TravelDestination dest)
    {
        if (dest.destination == null || player == null) yield break;

        isTeleporting = true;

        // Đóng menu
        CloseTravelMenu();

        // Disable player
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Fade out
        yield return SceneFadeManager.FadeOut(fadeOutDuration);

        // Wait
        yield return new WaitForSeconds(waitDuration);

        // Teleport
        player.position = dest.destination.position;
        Debug.Log($"✈️ Traveled to {dest.locationName}");

        // Fade in
        yield return SceneFadeManager.FadeIn(fadeInDuration);

        // Enable player
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isTeleporting = false;
    }

    /// <summary>
    /// Save unlock status
    /// </summary>
    private void SaveUnlockStatus()
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            PlayerPrefs.SetInt($"Destination_{i}_Unlocked", destinations[i].isUnlocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load unlock status
    /// </summary>
    private void LoadUnlockStatus()
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            destinations[i].isUnlocked = PlayerPrefs.GetInt($"Destination_{i}_Unlocked", 0) == 1;
        }
    }

    /// <summary>
    /// Unlock destination (gọi từ script khác)
    /// </summary>
    public void UnlockDestination(int index)
    {
        if (index >= 0 && index < destinations.Count)
        {
            destinations[index].isUnlocked = true;
            SaveUnlockStatus();
        }
    }

    /// <summary>
    /// Unlock destination by name
    /// </summary>
    public void UnlockDestination(string locationName)
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            if (destinations[i].locationName == locationName)
            {
                destinations[i].isUnlocked = true;
                SaveUnlockStatus();
                return;
            }
        }
    }

    // Trigger alternative
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            playerInRange = true;

            if (promptUI != null && !isMenuOpen)
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

            if (isMenuOpen)
            {
                CloseTravelMenu();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // Vẽ line đến tất cả destinations
        for (int i = 0; i < destinations.Count; i++)
        {
            if (destinations[i].destination != null)
            {
                Gizmos.color = destinations[i].isUnlocked ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position, destinations[i].destination.position);
            }
        }
    }
}