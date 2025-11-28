using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    public int price;
    [TextArea(2, 4)]
    public string description;
    public ItemType itemType;

    // Giá trị item khi mua (tùy chọn)
    public int healthRestore = 0;    // Cho potion
    public int attackBonus = 0;       // Cho weapon
    public int defenseBonus = 0;      // Cho armor

    public ShopItem(string id, string name, Sprite icon, int price, string desc, ItemType type)
    {
        itemID = id;
        itemName = name;
        itemIcon = icon;
        this.price = price;
        description = desc;
        itemType = type;
    }
}

public enum ItemType
{
    Weapon,      // Vũ khí
    Armor,       // Giáp
    Potion,      // Thuốc
    Consumable,  // Vật phẩm tiêu hao
    Upgrade      // Nâng cấp
}