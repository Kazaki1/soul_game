using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;

    private PlayerInputActions inputActions;
    private InputAction inventoryAction;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        if (inventoryPanel != null)
        {
            RectTransform rect = inventoryPanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(800, 600);
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = Vector2.zero;
            }
            inventoryPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        inventoryAction = inputActions.Player.Inventory;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void Update()
    {
        if (inventoryAction == null) return;

        if (inventoryAction.WasPressedThisFrame())
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);
        Canvas.ForceUpdateCanvases();
    }

    public void OpenInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    public bool IsInventoryOpen()
    {
        return inventoryPanel != null && inventoryPanel.activeSelf;
    }

    public void RefreshInventory()
    {
        var manager = Inventory_mananegment.Instance;
        if (manager?.slots == null) return;

        foreach (var slot in manager.slots)
            slot?.RefreshSlot();
    }
}
