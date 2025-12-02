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
    public GameObject itemPrefab;
    public GameObject weaponPrefab;
    public int healthRestore = 0;  
    public int attackBonus = 0;    
    public int defenseBonus = 0;   
    public ShopItem(string id, string name, Sprite icon, int price, string desc, ItemType type, GameObject prefab)
    {
        itemID = id;
        itemName = name;
        itemIcon = icon;
        this.price = price;
        description = desc;
        itemType = type;
        itemPrefab = prefab;
    }
    public GameObject WeaponPrefab
    {
        get { return itemPrefab; }
        set { itemPrefab = value; }
    }
}


public enum ShopItemType
{
    Weapon,      
    Armor,       
    Potion,      
    Consumable,  
    Upgrade      
}