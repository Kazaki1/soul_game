using UnityEngine;
using System.Collections;

public class Inventory_mananegment : MonoBehaviour
{
    public static Inventory_mananegment Instance { get; private set; }

    [Header("Inventory Slots")]
    public inventory_slot[] slots;

    [Header("Special Slots (Weapon Slots)")]
    public inventory_slot leftHandSlot;
    public inventory_slot rightHandSlot;
    [Header("Armor Slots")]
    public inventory_slot headSlot;
    public inventory_slot chestSlot;
    public inventory_slot legSlot;
    public inventory_slot bootsSlot;
    [Header("Starting Items")]
    public Scriptable_object[] startingItems;
    public GameObject dropPrefab;
    [Header("Drop Settings")]
    public float dropForce = 5f;
    public float dropDistance = 1.5f;

    private inventory_slot hoveredSlot = null;
    [Header("Special Multi Slot (5 sub-slots)")]
    public inventory_slot specialParentSlot;
    public inventory_slot[] specialChildSlots = new inventory_slot[5];

    private const int PotionSlotIndexA = 0;
    private const int PotionSlotIndexB = 1;
    private const int SpecialSlotStart = 2;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].slotIndex = i;
                slots[i].isSpecialSlot = false;
            }
        }
        if (leftHandSlot != null)
        {
            leftHandSlot.slotIndex = 0;
            leftHandSlot.isSpecialSlot = true;
            leftHandSlot.slotType = inventory_slot.SlotType.LeftHand;
        }

        if (rightHandSlot != null)
        {
            rightHandSlot.slotIndex = 1;
            rightHandSlot.isSpecialSlot = true;
            rightHandSlot.slotType = inventory_slot.SlotType.RightHand;
        }
        if (headSlot != null)
        {
            headSlot.slotIndex = 2;
            headSlot.isSpecialSlot = true;
            headSlot.slotType = inventory_slot.SlotType.Head;
        }

        if (chestSlot != null)
        {
            chestSlot.slotIndex = 3;
            chestSlot.isSpecialSlot = true;
            chestSlot.slotType = inventory_slot.SlotType.Chest;
        }

        if (legSlot != null)
        {
            legSlot.slotIndex = 4;
            legSlot.isSpecialSlot = true;
            legSlot.slotType = inventory_slot.SlotType.Legs;
        }

        if (bootsSlot != null)
        {
            bootsSlot.slotIndex = 5;
            bootsSlot.isSpecialSlot = true;
            bootsSlot.slotType = inventory_slot.SlotType.Boots;
        }
    }

    void Start()
    {
        if (startingItems != null && startingItems.Length > 0)
        {
            StartCoroutine(AddStartingItems());
        }
    }

    private IEnumerator AddStartingItems()
    {
        yield return null;
        for (int i = 0; i < startingItems.Length && i < slots.Length; i++)
        {
            if (startingItems[i] != null && slots[i] != null)
            {
                slots[i].AddItem(startingItems[i]);
            }
        }
    }

    public bool Add(Scriptable_object newItem)
    {
        foreach (inventory_slot slot in slots)
        {
            if (slot != null && slot.IsEmpty())
            {
                slot.AddItem(newItem);
                slot.RefreshSlot();
                return true;
            }
        }
        return false;
    }

    public void OnSlotDoubleClick(int slotIndex, bool isSpecial)
    {
        inventory_slot slot;
        if (isSpecial)
        {
            if (slotIndex == 0) slot = leftHandSlot;
            else if (slotIndex == 1) slot = rightHandSlot;
            else return;
        }
        else
        {
            if (slotIndex < 0 || slotIndex >= slots.Length) return;
            slot = slots[slotIndex];
        }

        if (slot == null || slot.IsEmpty()) return;

        Scriptable_object item = slot.GetItem();
        if (item == null) return;

        switch (item.itemType)
        {
            case ItemType.Weapon: HandleWeaponItem(slot, item); break;
            case ItemType.Consumable: HandleConsumableItem(slot, item); break;
            case ItemType.Armor: HandleArmorItem(slot, item); break;
            case ItemType.Potion: MovePotionToSlot(slot, item); break;
        }
    }

    private void MovePotionToSlot(inventory_slot fromSlot, Scriptable_object item)
    {
        inventory_slot slotA = specialChildSlots[PotionSlotIndexA];
        inventory_slot slotB = specialChildSlots[PotionSlotIndexB];
        inventory_slot target = null;
        if (slotA.IsEmpty()) target = slotA;
        else if (slotB.IsEmpty()) target = slotB;
        if (target == null) target = slotA;
        if (!target.IsEmpty()) Add(target.GetItem());
        target.AddItem(item);
        target.RefreshSlot();
        fromSlot.DecreaseQuantity(1);
        fromSlot.RefreshSlot();
    }

    private void HandleWeaponItem(inventory_slot slot, Scriptable_object item)
    {

        PlayerWeaponController pwc = FindObjectOfType<PlayerWeaponController>();

        if (pwc == null)
        {
            return;
        }

        if (item.weaponPrefab == null)
        {
            return;
        }

        if (slot.isSpecialSlot)
        {
            UnequipToInventory(slot);
            return;
        }

        inventory_function invFunc = inventory_function.instance;
        if (invFunc != null)
        {
            bool equipped = invFunc.equip_item(item);
            if (!equipped)
            {
                return;
            }
        }

        if (leftHandSlot != null)
        {
            if (!leftHandSlot.IsEmpty())
            {
              if (rightHandSlot != null && rightHandSlot.IsEmpty())
                {
                    Scriptable_object oldWeapon = leftHandSlot.GetItem();
                    rightHandSlot.AddItem(oldWeapon);
                    rightHandSlot.RefreshSlot();
                    leftHandSlot.ClearSlot();
                }
                else
                {
                    return;
                }
            }

            leftHandSlot.AddItem(item);

            leftHandSlot.RefreshSlot();
        }

        slot.ClearSlot();
        slot.RefreshSlot();

    }

    private void HandleConsumableItem(inventory_slot slot, Scriptable_object item)
    {
        if (item.itemType == ItemType.Consumable && item.slotCategory == SlotCategory.BuffSlot)
        {
            MoveBuffToSpecialSlot(slot, item);
            return;
        }
        slot.DecreaseQuantity(1);
        slot.RefreshSlot();
    }

    private void HandleArmorItem(inventory_slot slot, Scriptable_object item)
    {
        inventory_slot targetSlot = null;
        switch (item.armorTag)
        {
            case ArmorTag.Head: targetSlot = headSlot; break;
            case ArmorTag.Chest: targetSlot = chestSlot; break;
            case ArmorTag.Legs: targetSlot = legSlot; break;
            case ArmorTag.Boots: targetSlot = bootsSlot; break;
            case ArmorTag.none: return;
        }

        if (targetSlot == null) return;

        Scriptable_object oldArmor = targetSlot.IsEmpty() ? null : targetSlot.GetItem();
        slot.ClearSlot();
        if (oldArmor != null) slot.AddItem(oldArmor);
        targetSlot.ClearSlot();
        targetSlot.AddItem(item);
        slot.RefreshSlot();
        targetSlot.RefreshSlot();
    }

    public void DeleteItemFromSlot(int slotIndex, bool isSpecialSlot)
    {
        inventory_slot slot;
        if (isSpecialSlot)
        {
            slot = (slotIndex == 0) ? leftHandSlot :
                   (slotIndex == 1) ? rightHandSlot : null;
        }
        else
        {
            if (slotIndex < 0 || slotIndex >= slots.Length) return;
            slot = slots[slotIndex];
        }

        if (slot == null || slot.IsEmpty()) return;
        slot.ClearSlot();
        slot.RefreshSlot();
    }

    public bool IsFull()
    {
        if (slots == null || slots.Length == 0) return true;
        foreach (inventory_slot slot in slots)
            if (slot != null && slot.IsEmpty()) return false;
        return true;
    }

    public int GetEmptySlotCount()
    {
        if (slots == null || slots.Length == 0) return 0;
        int count = 0;
        foreach (inventory_slot slot in slots)
            if (slot != null && slot.IsEmpty()) count++;
        return count;
    }

    public void RefreshAllSlots()
    {
        if (slots != null)
            foreach (inventory_slot slot in slots)
                if (slot != null) slot.RefreshSlot();

        if (leftHandSlot != null) leftHandSlot.RefreshSlot();
        if (rightHandSlot != null) rightHandSlot.RefreshSlot();
        Canvas.ForceUpdateCanvases();
    }

    public void DropItemFromSlot(inventory_slot slot)
    {
        if (slot == null || slot.IsEmpty()) return;

        Scriptable_object item = slot.GetItem();
        if (slot.isSpecialSlot && (slot == leftHandSlot || slot == rightHandSlot))
        {
            DropEquippedWeaponNew(slot);
            return;
        }

        Vector3 dropPos = GetDropPosition();
        GameObject droppedObject = null;

        if (item.itemType == ItemType.Weapon && item.weaponPrefab != null)
        {
            droppedObject = Instantiate(item.weaponPrefab, dropPos, Quaternion.identity);
        }
        else if (dropPrefab != null)
        {
            droppedObject = Instantiate(dropPrefab, dropPos, Quaternion.identity);
            ItemPickup pickup = droppedObject.GetComponent<ItemPickup>();
            if (pickup != null) pickup.item = item;
            SpriteRenderer sr = droppedObject.GetComponent<SpriteRenderer>();
            if (sr != null && item.itemIcon != null) sr.sprite = item.itemIcon;
        }
        else return;

        Rigidbody2D rb = droppedObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dropDirection = GetPlayerFacingDirection();
            rb.AddForce(dropDirection * dropForce, ForceMode2D.Impulse);
        }

        slot.ClearSlot();
        slot.RefreshSlot();
    }

    private void DropEquippedWeaponNew(inventory_slot weaponSlot)
    {
        if (weaponSlot == null || weaponSlot.IsEmpty()) return;

        Scriptable_object item = weaponSlot.GetItem();
        if (item.weaponPrefab == null) return;

        Vector3 dropPos = GetDropPosition();
        GameObject dropped = Instantiate(item.weaponPrefab, dropPos, Quaternion.identity);

        Rigidbody2D rb = dropped.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dropDirection = GetPlayerFacingDirection();
            rb.AddForce(dropDirection * dropForce, ForceMode2D.Impulse);
        }

        weaponSlot.ClearSlot();
        weaponSlot.RefreshSlot();

        PlayerWeaponController pwc = FindObjectOfType<PlayerWeaponController>();
        if (pwc != null) pwc.UnequipWeapon();
    }

    private Vector3 GetDropPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 direction = (Vector3)GetPlayerFacingDirection();
            return player.transform.position + direction * dropDistance + Vector3.up * 0.5f;
        }
        return Vector3.zero;
    }

    private Vector2 GetPlayerFacingDirection()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null) return sr.flipX ? Vector2.left : Vector2.right;
            return Vector2.right;
        }
        return Vector2.right;
    }

    public void SetHoveredSlot(inventory_slot slot)
    {
        hoveredSlot = slot;
    }

    public void ClearHoveredSlot(inventory_slot slot)
    {
        StartCoroutine(ClearHoverNextFrame(slot));
    }

    private IEnumerator ClearHoverNextFrame(inventory_slot slot)
    {
        yield return null;
        if (hoveredSlot == slot) hoveredSlot = null;
    }

    public void DeleteHoveredItem()
    {
        if (hoveredSlot == null || hoveredSlot.IsEmpty()) return;

        if (hoveredSlot.isSpecialSlot && (hoveredSlot == leftHandSlot || hoveredSlot == rightHandSlot))
        {
            PlayerWeaponController pwc = FindObjectOfType<PlayerWeaponController>();
            if (pwc != null) pwc.UnequipWeapon();
        }

        hoveredSlot.ClearSlot();
        hoveredSlot.RefreshSlot();
    }

    private void UnequipToInventory(inventory_slot specialSlot)
    {
        if (specialSlot == null || specialSlot.IsEmpty()) return;
        Scriptable_object item = specialSlot.GetItem();
        foreach (inventory_slot normalSlot in slots)
        {
            if (normalSlot.IsEmpty())
            {
                normalSlot.AddItem(item);
                normalSlot.RefreshSlot();
                specialSlot.ClearSlot();
                specialSlot.RefreshSlot();
                return;
            }
        }
    }

    public bool AddItemFromShop(ShopItem shopItem)
    {
        if (shopItem == null) return false;

        Scriptable_object newItem = ScriptableObject.CreateInstance<Scriptable_object>();
        newItem.item_name = shopItem.itemName;
        newItem.itemIcon = shopItem.itemIcon;
        newItem.itemType = shopItem.itemType;

        return Add(newItem);
    }

    private void MoveBuffToSpecialSlot(inventory_slot fromSlot, Scriptable_object item)
    {
        inventory_slot slot2 = specialChildSlots[2];
        inventory_slot slot3 = specialChildSlots[3];
        inventory_slot slot4 = specialChildSlots[4];
        inventory_slot target = null;
        if (slot2.IsEmpty()) target = slot2;
        else if (slot3.IsEmpty()) target = slot3;
        else if (slot4.IsEmpty()) target = slot4;
        if (target == null) target = slot2;
        if (!target.IsEmpty()) Add(target.GetItem());
        target.AddItem(item);
        target.RefreshSlot();
        fromSlot.DecreaseQuantity(1);
        fromSlot.RefreshSlot();
    }
}
