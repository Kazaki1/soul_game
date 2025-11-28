using UnityEngine;
using TMPro;

public class ShopTrigger : MonoBehaviour
{
    [Header("References")]
    public ShopManager shopManager;

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionRange = 2f;
    public bool useColliderTrigger = true; // true = dùng OnTrigger, false = dùng distance check

    [Header("UI")]
    public GameObject interactionPrompt; // Canvas hiển thị "Press E"
    public TextMeshProUGUI promptText;

    private Transform player;
    private bool playerInRange = false;

    private void Start()
    {
        // Tìm player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Không tìm thấy Player! Đảm bảo Player có tag 'Player'");

        // Ẩn prompt lúc đầu
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // Set text
        if (promptText != null)
            promptText.text = $"Nhấn [{interactKey}] để mở shop";

        // Tìm ShopManager nếu chưa gán
        if (shopManager == null)
            shopManager = FindObjectOfType<ShopManager>();
    }

    private void Update()
    {
        if (player == null || shopManager == null) return;

        // Kiểm tra khoảng cách nếu không dùng trigger
        if (!useColliderTrigger)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            bool wasInRange = playerInRange;
            playerInRange = distance <= interactionRange;

            // Update prompt
            if (playerInRange != wasInRange)
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(playerInRange);
            }
        }

        // Mở shop khi nhấn phím
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            shopManager.OpenShop();
        }

        // Đóng shop khi nhấn ESC
        if (Input.GetKeyDown(KeyCode.Escape) && shopManager.shopPanel.activeSelf)
        {
            shopManager.CloseShop();
        }
    }

    // === DÙNG CHO TRIGGER COLLIDER ===
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!useColliderTrigger) return;

        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!useColliderTrigger) return;

        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }

    // === DEBUG ===
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}