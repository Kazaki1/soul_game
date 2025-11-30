using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class inventory_slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image iconUI;
    public TextMeshProUGUI quantityText;

    [Header("Slot Data")]
    private Scriptable_object currentItem;
    private int currentQuantity = 0;

    [HideInInspector] public int slotIndex;
    [HideInInspector] public bool isSpecialSlot = false;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;
    private Inventory_mananegment inventoryManager;
    private bool isInitialized = false;
    private bool isPointerOver = false;
    public enum SlotType
    {
        Normal, LeftHand, RightHand, Head, Chest, Legs, Boots, five_slot
    }
    public SlotType slotType = SlotType.Normal;
    void Awake()
    {
        if (iconUI == null)
        {
            Transform iconTransform = transform.Find("Icon");
            if (iconTransform != null)
                iconUI = iconTransform.GetComponent<Image>();
        }

        if (quantityText == null)
        {
            Transform qtyTransform = transform.Find("Quantity");
            if (qtyTransform != null)
                quantityText = qtyTransform.GetComponent<TextMeshProUGUI>();
        }
    }
    void Start()
    {
        inventoryManager = Inventory_mananegment.Instance;
        isInitialized = true;
        iconUI.raycastTarget = false;
    }
    public bool IsEmpty() => currentItem == null;
    public Scriptable_object GetItem() => currentItem;

    public int GetQuantity() => currentQuantity;
    public void AddItem(Scriptable_object newItem)
    {
        if (newItem == null) return;
        if (IsEmpty())
            UpdateSlot(newItem, 1);
        else if (currentItem == newItem)
            IncreaseQuantity(1);
    }
    public void UpdateSlot(Scriptable_object newItem, int newQuantity)
    {
        currentItem = newItem;
        currentQuantity = Mathf.Max(0, newQuantity);

        if (newItem != null && newItem.itemIcon != null)
        {
            iconUI.sprite = newItem.itemIcon;
            iconUI.color = Color.white;
            iconUI.enabled = true;
        }
        else
        {
            iconUI.sprite = null;
            iconUI.enabled = false;
            iconUI.color = new Color(1, 1, 1, 0);
        }

        if (quantityText != null)
        {
            if (currentQuantity > 1)
            {
                quantityText.text = currentQuantity.ToString();
                quantityText.enabled = true;
            }
            else
            {
                quantityText.text = "";
                quantityText.enabled = false;
            }
        }
        Canvas.ForceUpdateCanvases();
    }
    public void ClearSlot()
    {
        currentItem = null;
        currentQuantity = 0;
        if (iconUI != null)
        {
            iconUI.sprite = null;
            iconUI.enabled = false;
            iconUI.color = new Color(1, 1, 1, 0);
        }
        if (quantityText != null)
        {
            quantityText.text = "";
            quantityText.enabled = false;
        }
    }
    public void IncreaseQuantity(int amount)
    {
        if (currentItem == null) return;
        UpdateSlot(currentItem, currentQuantity + Mathf.Max(1, amount));
    }
    public void DecreaseQuantity(int amount)
    {
        if (currentItem == null) return;
        int newQty = currentQuantity - Mathf.Max(1, amount);
        if (newQty > 0)
            UpdateSlot(currentItem, newQty);
        else
            ClearSlot();
    }
    public void RefreshSlot()
    {
        if (currentItem != null && currentItem.itemIcon != null)
        {
            iconUI.sprite = currentItem.itemIcon;
            iconUI.enabled = true;
            iconUI.color = Color.white;
        }
        else
        {
            iconUI.sprite = null;
            iconUI.enabled = false;
            iconUI.color = new Color(1, 1, 1, 0);
        }
        Canvas.ForceUpdateCanvases();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryManager == null)
            inventoryManager = Inventory_mananegment.Instance;

        if (eventData.button != PointerEventData.InputButton.Left) return;

        float timeSinceLastClick = Time.time - lastClickTime;
        if (timeSinceLastClick <= doubleClickThreshold)
        {
            OnDoubleClick();
            lastClickTime = 0;
        }
        else
            lastClickTime = Time.time;
    }
    private void OnDoubleClick()
    {
        if (currentItem == null) return;
        inventoryManager.OnSlotDoubleClick(slotIndex, isSpecialSlot);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPointerOver) return;
        isPointerOver = true;
        inventoryManager?.SetHoveredSlot(this);
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.ShowTooltip(currentItem, eventData.position + new Vector2(10f, -80f));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPointerOver) return;
        isPointerOver = false;
        inventoryManager?.ClearHoveredSlot(this);
        TooltipUI.Instance?.HideTooltip();
    }
  void Update()
    {
        if (isPointerOver && Input.GetKeyDown(KeyCode.F))
            Inventory_mananegment.Instance.DeleteHoveredItem();
    }
}
