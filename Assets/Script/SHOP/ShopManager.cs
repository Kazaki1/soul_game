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
    public float sellPriceMultiplier = 0.5f; 

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

        if (buyModeButton != null)
            buyModeButton.onClick.AddListener(() => SwitchMode(ShopMode.Buy));

        if (sellModeButton != null)
            sellModeButton.onClick.AddListener(() => SwitchMode(ShopMode.Sell));

        LoadShopItems();

        if (SimpleInventory.Instance != null)
            SimpleInventory.Instance.RebuildItemsFromData(shopData);
    }

    private void LoadShopItems()
    {
        if (shopData == null)
        {
            Debug.LogError("ShopData chưa được gán!");
            return;
        }

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

    
    private void LoadBuyMode()
    {
        if (shopTitleText != null)
            shopTitleText.text = $"{shopData.shopName} - MUA";

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
    private void LoadSellMode()
    {
        if (shopTitleText != null)
            shopTitleText.text = $"{shopData.shopName} - BÁN";

        if (SimpleInventory.Instance == null)
        {
            Debug.LogWarning("SimpleInventory not found!");
            return;
        }

        List<InventoryItem> inventoryItems = SimpleInventory.Instance.GetItems();

        if (inventoryItems.Count == 0)
        {
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

    public void SwitchMode(ShopMode mode)
    {
        if (currentMode == mode) return;

        currentMode = mode;
        LoadShopItems();
    }

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

    public int GetSellPrice(int buyPrice)
    {
        return Mathf.RoundToInt(buyPrice * sellPriceMultiplier);
    }

    public void BuyItem(ShopItem item)
    {
        if (item == null)
        {
            ShowNotification("Lỗi: Item không tồn tại!");
            return;
        }

        if (!MoneyManager.Instance.HasEnoughMoney(item.price))
        {
            ShowNotification("Không đủ tiền!");
            return;
        }

        if (MoneyManager.Instance.SpendMoney(item.price))
        {
            if (Inventory_mananegment.Instance != null)
            {
                bool added = Inventory_mananegment.Instance.AddItemFromShop(item);

                if (added)
                {
                    ShowNotification($"Đã mua {item.itemName}!");
                    Debug.Log($"Đã thêm {item.itemName} vào Inventory!");
                }
                else
                {
                    MoneyManager.Instance.AddMoney(item.price);
                    ShowNotification("Inventory đầy!");
                }
            }
            else
            {
              ApplyItemEffect(item);
                ShowNotification($"Đã mua {item.itemName}!");
            }
        }
    }

    public void SellItem(ShopItem item)
    {
        if (item == null || SimpleInventory.Instance == null)
        {
            ShowNotification("Lỗi: Không thể bán item!");
            return;
        }

        if (!SimpleInventory.Instance.HasItem(item.itemID))
        {
            ShowNotification("Không có item này trong inventory!");
            return;
        }

        int sellPrice = GetSellPrice(item.price);

        if (SimpleInventory.Instance.RemoveItem(item.itemID))
        {
            MoneyManager.Instance.AddMoney(sellPrice);
            ShowNotification($"Đã bán {item.itemName} với giá {sellPrice}G!");
            Debug.Log($"Bán thành công: {item.itemName} - Nhận {sellPrice}G");

            LoadShopItems();
        }
    }

    private void ApplyItemEffect(ShopItem item)
    {
        switch (item.itemType)
        {
            case ItemType.Potion:
                if (item.healthRestore > 0)
                {
                    Debug.Log($"[ITEM EFFECT] Restored {item.healthRestore} HP");
               }
                break;

            case ItemType.Weapon:
                if (item.attackBonus > 0)
                {
                    Debug.Log($"[ITEM EFFECT] Attack +{item.attackBonus}");
                }
                break;

            case ItemType.Armor:
                if (item.defenseBonus > 0)
                {
                    Debug.Log($"[ITEM EFFECT] Defense +{item.defenseBonus}");
                }
                break;
        }
    }

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
        if (notificationPanel != null && notificationPanel.activeSelf)
        {
            notificationTimer -= Time.unscaledDeltaTime;
            if (notificationTimer <= 0f)
            {
                notificationPanel.SetActive(false);
            }
        }
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1f;  }

    
    public void ToggleShop()
    {
        if (shopPanel.activeSelf)
            CloseShop();
        else
            OpenShop();
    }
}

public enum ShopMode
{
    Buy,     Sell 
}