using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string playerTag = "Player";
    public GameObject interactUI;
    public GameObject levelUpCanvas;

    private bool playerInRange = false;
    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
        if (levelUpCanvas != null)
            levelUpCanvas.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = true;
        if (interactUI != null)
            interactUI.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInRange = false;
        if (interactUI != null)
            interactUI.SetActive(false);
        if (levelUpCanvas != null)
            levelUpCanvas.SetActive(false);
    }

    public void Activate()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null) return;

        PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();
        Stamina playerStamina = playerObj.GetComponent<Stamina>();
        Transform playerTransform = playerObj.transform;

        if (playerHealth == null || playerStamina == null) return;

        SaveData data = new SaveData
        {
            playerPos = playerTransform.position,
            playerRot = playerTransform.eulerAngles,
            health = playerHealth.GetCurrentHealth(),
            stamina = playerStamina.currentStamina
        };

        SaveSystem.SaveSnapshot(data);
    }
}
