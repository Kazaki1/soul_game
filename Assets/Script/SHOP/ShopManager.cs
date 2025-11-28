using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Data")]
    public ShopData shopData;

    [Header("Shop Mode")]
    public ShopMode currentMode = ShopMode.Buy;

    [Header("UI References")]
    public GameObject shopPanel;
    public Transform shopItemsContainer;
    public GameObject shopItemSlotPrefab;
    public TextMeshProUGUI shopTitleText;
    public Button closeButton;

    [Header("Mode Buttons")]
    public Button buyModeButton;
    public Button sellModeButton;

    [Header("Sell Settings")]
    [Range(0.1f, 1f)]
    public float sellPriceMultiplier = 0.5f; // Bán được 50% giá mua

    [Header("Notification")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;
    public float notificationDuration = 2f;

    private List<ShopItemUI> spawnedItems = new List<ShopItemUI>();
    private float notificationTimer = 0f;

    private void Start()
    {
        shopPanel.SetActive(false);

        if (notificationPanel != null)
            notificationPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);

        // Setup mode buttons
        if (buyModeButton != null)
            buyModeButton.onClick.AddListener(() => SwitchMode(ShopMode.Buy));

        if (sellModeButton != null)
            sellModeButton.onClick.AddListener(() => SwitchMode(ShopMode.Sell));

        LoadShopItems();

        // Rebuild inventory items từ shop data
        if (SimpleInventory.Instance != null)
            SimpleInventory.Instance.RebuildItemsFromData(shopData);
    }

    // Load tất cả items vào shop
    private void LoadShopItems()
    {
        if (shopData == null)
        {
            Debug.LogError("ShopData chưa được gán!");
            return;
        }

        // Clear items cũ
        foreach (var item in spawnedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        spawnedItems.Clear();

        if (currentMode == ShopMode.Buy)
        {
            LoadBuyMode();
        }
        else
        {
            LoadSellMode();
        }

        UpdateModeButtons();
    }

    // Load chế độ MUA
    private void LoadBuyMode()
    {
        // Set shop title
        if (shopTitleText != null)
            shopTitleText.text = $"{shopData.shopName} - MUA";

        // Spawn shop items
        foreach (var itemData in shopData.shopItems)
        {
            GameObject itemSlot = Instantiate(shopItemSlotPrefab, shopItemsContainer);
            ShopItemUI shopItemUI = itemSlot.GetComponent<ShopItemUI>();

            if (shopItemUI != null)
            {
                shopItemUI.SetupForBuy(itemData.ToShopItem(), this);
                spawnedItems.Add(shopItemUI);
            }
        }
    }

    // Load chế độ BÁN
    private void LoadSellMode()
    {
        // Set shop title
        if (shopTitleText != null)
            shopTitleText.text = $"{shopData.shopName} - BÁN";

        if (SimpleInventory.Instance == null)
        {
            Debug.LogWarning("SimpleInventory not found!");
            return;
        }

        // Spawn inventory items
        List<InventoryItem> inventoryItems = SimpleInventory.Instance.GetItems();

        if (inventoryItems.Count == 0)
        {
            // Hiển thị thông báo không có gì để bán
            ShowNotification("Không có vật phẩm để bán!");
        }

        foreach (var invItem in inventoryItems)
        {
            if (invItem.item != null)
            {
                GameObject itemSlot = Instantiate(shopItemSlotPrefab, shopItemsContainer);
                ShopItemUI shopItemUI = itemSlot.GetComponent<ShopItemUI>();

                if (shopItemUI != null)
                {
                    int sellPrice = GetSellPrice(invItem.item.price);
                    shopItemUI.SetupForSell(invItem.item, invItem.quantity, sellPrice, this);
                    spawnedItems.Add(shopItemUI);
                }
            }
        }
    }

    // Switch mode
    public void SwitchMode(ShopMode mode)
    {
        if (currentMode == mode) return;

        currentMode = mode;
        LoadShopItems();
    }

    // Update button states
    private void UpdateModeButtons()
    {
        if (buyModeButton != null)
        {
            var colors = buyModeButton.colors;
            colors.normalColor = currentMode == ShopMode.Buy ? Color.green : Color.white;
            buyModeButton.colors = colors;
        }

        if (sellModeButton != null)
        {
            var colors = sellModeButton.colors;
            colors.normalColor = currentMode == ShopMode.Sell ? Color.green : Color.white;
            sellModeButton.colors = colors;
        }
    }

    // Tính giá bán
    public int GetSellPrice(int buyPrice)
    {
        return Mathf.RoundToInt(buyPrice * sellPriceMultiplier);
    }

    // Mua item
    public void BuyItem(ShopItem item)
    {
        if (item == null)
        {
            ShowNotification("Lỗi: Item không tồn tại!");
            return;
        }

        // Kiểm tra tiền
        if (!MoneyManager.Instance.HasEnoughMoney(item.price))
        {
            ShowNotification("Không đủ tiền!");
            return;
        }

        // Trừ tiền
        if (MoneyManager.Instance.SpendMoney(item.price))
        {
            // Thêm vào inventory
            if (SimpleInventory.Instance != null)
            {
                if (SimpleInventory.Instance.AddItem(item))
                {
                    ShowNotification($"Đã mua {item.itemName}!");
                    Debug.Log($"Mua thành công: {item.itemName}");
                }
                else
                {
                    // Hoàn tiền nếu inventory đầy
                    MoneyManager.Instance.AddMoney(item.price);
                    ShowNotification("Inventory đầy!");
                }
            }
            else
            {
                // Không có inventory, áp dụng hiệu ứng trực tiếp
                ApplyItemEffect(item);
                ShowNotification($"Đã mua {item.itemName}!");
            }
        }
    }

    // Bán item
    public void SellItem(ShopItem item)
    {
        if (item == null || SimpleInventory.Instance == null)
        {
            ShowNotification("Lỗi: Không thể bán item!");
            return;
        }

        // Kiểm tra có item trong inventory không
        if (!SimpleInventory.Instance.HasItem(item.itemID))
        {
            ShowNotification("Không có item này trong inventory!");
            return;
        }

        // Tính giá bán
        int sellPrice = GetSellPrice(item.price);

        // Xóa khỏi inventory
        if (SimpleInventory.Instance.RemoveItem(item.itemID))
        {
            // Thêm tiền
            MoneyManager.Instance.AddMoney(sellPrice);
            ShowNotification($"Đã bán {item.itemName} với giá {sellPrice}G!");
            Debug.Log($"Bán thành công: {item.itemName} - Nhận {sellPrice}G");

            // Reload để cập nhật số lượng
            LoadShopItems();
        }
    }

    // Áp dụng hiệu ứng item (Nếu bạn không dùng inventory)
    private void ApplyItemEffect(ShopItem item)
    {
        // Tạm thời chỉ log, bạn có thể kết nối với player script sau
        switch (item.itemType)
        {
            case ItemType.Potion:
                if (item.healthRestore > 0)
                {
                    Debug.Log($"[ITEM EFFECT] Restored {item.healthRestore} HP");
                    // TODO: Kết nối với player health system
                    // PlayerHealth.Instance.Heal(item.healthRestore);
                }
                break;

            case ItemType.Weapon:
                if (item.attackBonus > 0)
                {
                    Debug.Log($"[ITEM EFFECT] Attack +{item.attackBonus}");
                    // TODO: Kết nối với player stats
                    // Player.Instance.AddAttack(item.attackBonus);
                }
                break;

            case ItemType.Armor:
                if (item.defenseBonus > 0)
                {
                    Debug.Log($"[ITEM EFFECT] Defense +{item.defenseBonus}");
                    // TODO: Kết nối với player stats
                    // Player.Instance.AddDefense(item.defenseBonus);
                }
                break;

            case ItemType.Upgrade:
                Debug.Log($"[ITEM EFFECT] Permanent upgrade: {item.itemName}");
                // TODO: Thêm logic nâng cấp
                break;

            case ItemType.Consumable:
                Debug.Log($"[ITEM EFFECT] Used consumable: {item.itemName}");
                // TODO: Thêm logic vật phẩm tiêu hao
                break;
        }
    }

    // Hiển thị notification
    public void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            notificationText.text = message;
            notificationPanel.SetActive(true);
            notificationTimer = notificationDuration;
        }

        Debug.Log($"Shop: {message}");
    }

    private void Update()
    {
        // Tự động ẩn notification
        if (notificationPanel != null && notificationPanel.activeSelf)
        {
            notificationTimer -= Time.unscaledDeltaTime;
            if (notificationTimer <= 0f)
            {
                notificationPanel.SetActive(false);
            }
        }
    }

    // Mở shop
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    // Đóng shop
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
    }

    // Toggle shop
    public void ToggleShop()
    {
        if (shopPanel.activeSelf)
            CloseShop();
        else
            OpenShop();
    }
}

// Enum chế độ shop
public enum ShopMode
{
    Buy,    // Chế độ mua
    Sell    // Chế độ bán
}