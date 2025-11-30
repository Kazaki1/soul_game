using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Scriptable_object : ScriptableObject
{
    [Header("Item Basic Info")]
    public string item_name = "New Item";
    public Sprite itemIcon;  

    [TextArea(3, 5)]
    public string description = "Item description here";

    [Header("Item Properties")]
    public ItemType itemType;
    public bool IsUseableAnywhere = false;
    public int maxStack = 99;

    [Header("Item Stats (Optional)")]
    public int healAmount = 0;
    public int damage = 0;
    public int defense = 0;
    [Header("Weapon Settings")]
    public GameObject weaponPrefab;
    public RuntimeAnimatorController weaponAnimator;
    public GameObject dropPrefab;

    public SlotCategory slotCategory;
    public ArmorTag armorTag;
}


public enum ItemType
{
    Consumable,   
    Weapon,       
    Armor,           
    Quest,        
    Potion,
    Special
}
public enum ArmorTag { Head, Chest, Legs, Boots, none }

public enum SlotCategory
{
    PotionSlot,     // 2 canvas ngoài cùng
    BuffSlot,       // 3 canvas giữa
    NormalBag       // bag slot
}