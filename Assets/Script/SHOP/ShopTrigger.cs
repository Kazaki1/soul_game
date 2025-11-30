using UnityEngine;
using TMPro;

public class ShopTrigger : MonoBehaviour
{
    [Header("References")]
    public ShopManager shopManager;

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionRange = 2f;
    public bool useColliderTrigger = true; 

    [Header("UI")]
    public GameObject interactionPrompt;
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

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

       if (promptText != null)
            promptText.text = $"Nhấn [{interactKey}] để mở shop";

        if (shopManager == null)
            shopManager = FindObjectOfType<ShopManager>();
    }

    private void Update()
    {
        if (player == null || shopManager == null) return;

        if (!useColliderTrigger)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            bool wasInRange = playerInRange;
            playerInRange = distance <= interactionRange;

            if (playerInRange != wasInRange)
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(playerInRange);
            }
        }

           if (playerInRange && Input.GetKeyDown(interactKey))
        {
            shopManager.OpenShop();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && shopManager.shopPanel.activeSelf)
        {
            shopManager.CloseShop();
        }
    }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}