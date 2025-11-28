using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI quantityText; // Hiển thị số lượng trong inventory
    public Button buyButton;
    public TextMeshProUGUI buttonText;

    [Header("Visual Feedback")]
    public Image backgroundImage;
    public Color normalColor = Color.white;
    public Color cannotAffordColor = Color.gray;

    private ShopItem item;
    private ShopManager shopManager;
    private bool isSellMode = false;
    private int itemQuantity = 1;

    // Setup cho chế độ MUA
    public void SetupForBuy(ShopItem shopItem, ShopManager manager)
    {
        item = shopItem;
        shopManager = manager;
        isSellMode = false;

        SetupUI();

        if (quantityText != null)
            quantityText.gameObject.SetActive(false);

        UpdateVisuals();
    }

    // Setup cho chế độ BÁN
    public void SetupForSell(ShopItem shopItem, int quantity, int sellPrice, ShopManager manager)
    {
        item = shopItem;
        shopManager = manager;
        isSellMode = true;
        itemQuantity = quantity;

        SetupUI();

        // Override giá với giá bán
        if (priceText != null)
            priceText.text = $"{sellPrice}G";

        // Hiển thị số lượng
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.text = $"x{quantity}";
        }

        // Button luôn enable khi bán
        if (buyButton != null)
            buyButton.interactable = true;

        if (buttonText != null)
            buttonText.text = "BÁN";
    }

    private void SetupUI()
    {
        // Setup UI
        if (itemIcon != null)
            itemIcon.sprite = item.itemIcon;

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (priceText != null)
            priceText.text = $"{item.price}G";

        if (descriptionText != null)
            descriptionText.text = item.description;

        // Setup button
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (shopManager == null || item == null) return;

        if (isSellMode)
        {
            shopManager.SellItem(item);
        }
        else
        {
            shopManager.BuyItem(item);
        }
    }

    // Update visual state
    private void UpdateVisuals()
    {
        if (MoneyManager.Instance == null) return;

        bool canBuy = MoneyManager.Instance.HasEnoughMoney(item.price);

        // Update button
        if (buyButton != null)
        {
            buyButton.interactable = canBuy;
        }

        // Update background color
        if (backgroundImage != null)
        {
            backgroundImage.color = canBuy ? normalColor : cannotAffordColor;
        }

        // Update button text
        if (buttonText != null)
        {
            buttonText.text = canBuy ? "MUA" : "KHÔNG ĐỦ TIỀN";
        }
    }

    private void Update()
    {
        // Update visuals mỗi frame để phản ánh số tiền hiện tại
        UpdateVisuals();
    }
}