using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointActivator : MonoBehaviour
{
    public float activateRange = 2f;
    public LayerMask checkpointLayer;
    public GameObject levelUpCanvas;

    private PlayerHealth playerHealth;
    private Stamina playerStamina;
    private Sanity playerSanity;
    private PlayerInputActions inputActions;
    private bool isProcessing = false;

    void Awake()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Enable();
            inputActions.Player.AddItem.performed += OnActivate;
        }
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.AddItem.performed -= OnActivate;
            inputActions.Player.Disable();
        }
    }

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerStamina = GetComponent<Stamina>();
        playerSanity = GetComponent<Sanity>();
    }

    private void OnActivate(InputAction.CallbackContext ctx)
    {
        if (this == null || !gameObject.activeInHierarchy) return;
        if (isProcessing) return;
        isProcessing = true;

        TryActivateCheckpoint();
        Invoke(nameof(ResetProcessing), 0.2f);
    }

    void ResetProcessing()
    {
        isProcessing = false;
    }

    void TryActivateCheckpoint()
    {
        if (levelUpCanvas != null && levelUpCanvas.activeSelf)
        {
            levelUpCanvas.SetActive(false);
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, activateRange, checkpointLayer);

        foreach (var hit in hits)
        {
            if (hit == null) continue;
            Checkpoint checkpoint = hit.GetComponent<Checkpoint>();
            if (checkpoint == null) continue;

            if (playerHealth != null) playerHealth.SetFullHealth();
            if (playerStamina != null) playerStamina.RestoreFullStamina();
            if (playerSanity != null) playerSanity.IncreaseSanity(playerSanity.GetMaxSanity());

            if (levelUpCanvas != null) levelUpCanvas.SetActive(true);

            checkpoint.Activate();
            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activateRange);
    }
}
