using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopData", menuName = "Shop/Shop Data")]
public class ShopData : ScriptableObject
{
    public string shopName = "General Store";
    public List<ShopItemData> shopItems = new List<ShopItemData>();

    public ShopItemData GetItemByID(string itemID)
    {
        return shopItems.Find(item => item.itemID == itemID);
    }
}

[System.Serializable]
public class ShopItemData
{
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    public int price;
    [TextArea(2, 4)]
    public string description;
    public ItemType itemType;

    [Header("Item Effects (Optional)")]
    public int healthRestore = 0;
    public int attackBonus = 0;
    public int defenseBonus = 0;

    public ShopItem ToShopItem()
    {
        ShopItem item = new ShopItem(itemID, itemName, itemIcon, price, description, itemType);
        item.healthRestore = healthRestore;
        item.attackBonus = attackBonus;
        item.defenseBonus = defenseBonus;
        return item;
    }
}