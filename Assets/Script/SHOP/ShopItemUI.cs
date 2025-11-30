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
    public TextMeshProUGUI quantityText; 
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

    public void SetupForSell(ShopItem shopItem, int quantity, int sellPrice, ShopManager manager)
    {
        item = shopItem;
        shopManager = manager;
        isSellMode = true;
        itemQuantity = quantity;

        SetupUI();
        if (priceText != null)
            priceText.text = $"{sellPrice}G";
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.text = $"x{quantity}";
        }
        if (buyButton != null)
            buyButton.interactable = true;

        if (buttonText != null)
            buttonText.text = "BÁN";
    }

    private void SetupUI()
    {
        if (itemIcon != null)
            itemIcon.sprite = item.itemIcon;

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (priceText != null)
            priceText.text = $"{item.price}G";

        if (descriptionText != null)
            descriptionText.text = item.description;
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

    private void UpdateVisuals()
    {
        if (MoneyManager.Instance == null) return;

        bool canBuy = MoneyManager.Instance.HasEnoughMoney(item.price);

        if (buyButton != null)
        {
            buyButton.interactable = canBuy;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = canBuy ? normalColor : cannotAffordColor;
        }
        if (buttonText != null)
        {
            buttonText.text = canBuy ? "MUA" : "KHÔNG ĐỦ TIỀN";
        }
    }

    private void Update()
    {
        UpdateVisuals();
    }
}