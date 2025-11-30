using UnityEngine;
using UnityEngine.InputSystem;

public class inventory_function : MonoBehaviour
{
    public GameObject InventoryMenu;
    private PlayerWeaponController weaponController;
    public GameObject weaponPrefab;
    private bool activate;

    [System.Serializable]
    public class InventorySlotData
    {
        public Scriptable_object item;
        public int amount;

        public void ClearSlot()
        {
            item = null;
            amount = 0;
        }
    }

    public static inventory_function instance;
    public int slot = 20;
    public InventorySlotData[] bagslot;
    public InventorySlotData[] specialslot;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        bagslot = new InventorySlotData[slot];
        specialslot = new InventorySlotData[2];

        for (int i = 0; i < bagslot.Length; i++)
            if (bagslot[i] == null) bagslot[i] = new InventorySlotData();

        for (int i = 0; i < specialslot.Length; i++)
            if (specialslot[i] == null) specialslot[i] = new InventorySlotData();

        weaponController = FindObjectOfType<PlayerWeaponController>();
    }

    public bool equip_item(Scriptable_object item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            for (int i = 0; i < specialslot.Length; i++)
            {
                if (specialslot[i].item == null)
                {
                    specialslot[i].item = item;
                    specialslot[i].amount = 1;
                    EquipToPlayer(item);
                    UpdateSpecialSlotUI(i, item);
                    return true;
                }
            }
            return false;
        }

        for (int i = 0; i < bagslot.Length; i++)
        {
            if (bagslot[i].item == null)
            {
                bagslot[i].item = item;
                bagslot[i].amount = 1;
                return true;
            }
        }
        return false;
    }

    private void EquipToPlayer(Scriptable_object item)
    {
        if (weaponController == null || item.weaponPrefab == null) return;
        GameObject weaponInstance = Instantiate(item.weaponPrefab, weaponController.transform);
        weaponController.EquipWeapon(weaponInstance);
    }

    private void UpdateSpecialSlotUI(int slotIndex, Scriptable_object item)
    {
        Inventory_mananegment invMgr = Inventory_mananegment.Instance;
        if (invMgr == null) return;

        inventory_slot uiSlot = slotIndex == 0 ? invMgr.leftHandSlot : invMgr.rightHandSlot;

        if (uiSlot != null)
        {
            uiSlot.ClearSlot();
            if (item != null) uiSlot.AddItem(item);
            uiSlot.RefreshSlot();
        }
    }

    public void QuickSwap()
    {
        if (specialslot[0].item == null && specialslot[1].item == null) return;

        InventorySlotData left = specialslot[0];
        InventorySlotData right = specialslot[1];

        (left.item, right.item) = (right.item, left.item);
        (left.amount, right.amount) = (right.amount, left.amount);

        if (weaponController != null)
            weaponController.QuickSwapWeapons();

        Inventory_mananegment invMgr = Inventory_mananegment.Instance;
        if (invMgr != null)
        {
            if (invMgr.leftHandSlot != null)
            {
                invMgr.leftHandSlot.ClearSlot();
                if (left.item != null) invMgr.leftHandSlot.AddItem(left.item);
                invMgr.leftHandSlot.RefreshSlot();
            }

            if (invMgr.rightHandSlot != null)
            {
                invMgr.rightHandSlot.ClearSlot();
                if (right.item != null) invMgr.rightHandSlot.AddItem(right.item);
                invMgr.rightHandSlot.RefreshSlot();
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            QuickSwap();

        HandleHotbarInput();
    }

    public void use_item(int slotIndex, bool isSpecial)
    {
        InventorySlotData slot = isSpecial ? specialslot[slotIndex] : bagslot[slotIndex];
        if (slot == null || slot.item == null) return;

        Scriptable_object item = slot.item;

        switch (item.itemType)
        {
            case ItemType.Potion:
                if (item.weaponPrefab != null)
                {
                    GameObject prefabInstance = Instantiate(item.weaponPrefab);
                    HealPotion heal = prefabInstance.GetComponent<HealPotion>();
                    if (heal != null) heal.UsePotion();
                    slot.ClearSlot();

                    Inventory_mananegment invMgr = Inventory_mananegment.Instance;
                    if (invMgr != null)
                    {
                        inventory_slot uiSlot = isSpecial ? (slotIndex == 0 ? invMgr.leftHandSlot : invMgr.rightHandSlot) : invMgr.slots[slotIndex];
                        if (uiSlot != null)
                        {
                            uiSlot.ClearSlot();
                            uiSlot.RefreshSlot();
                        }
                    }

                    Destroy(prefabInstance);
                }
                break;
            default:
                break;
        }
    }

    private void HandleHotbarInput()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) TryUseSpecialChildSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) TryUseSpecialChildSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) TryUseSpecialChildSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) TryUseSpecialChildSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) TryUseSpecialChildSlot(4);
    }

    private void TryUseSpecialChildSlot(int slotIndex)
    {
        Inventory_mananegment invMgr = Inventory_mananegment.Instance;
        if (invMgr == null || invMgr.specialChildSlots == null) return;
        if (slotIndex < 0 || slotIndex >= invMgr.specialChildSlots.Length) return;

        inventory_slot slot = invMgr.specialChildSlots[slotIndex];
        if (slot == null || slot.IsEmpty()) return;

        Scriptable_object item = slot.GetItem();
        if (item == null) return;

        if (item.itemType == ItemType.Consumable || item.itemType == ItemType.Potion)
            UseConsumableFromSpecialSlot(slot, item);
    }

    private void UseConsumableFromSpecialSlot(inventory_slot slot, Scriptable_object item)
    {
        if (item.weaponPrefab == null) return;

        GameObject instance = Instantiate(item.weaponPrefab);
        HealSanityPotion healSanity = instance.GetComponent<HealSanityPotion>();
        HealPotion healHP = instance.GetComponent<HealPotion>();

        if (healSanity != null)
        {
            Sanity playerSanity = FindObjectOfType<Sanity>();
            if (playerSanity != null)
            {
                healSanity.SetSanityReference(playerSanity);
                healSanity.UsePotion();
            }
            slot.ClearSlot();
            slot.RefreshSlot();
        }
        else if (healHP != null)
        {
            healHP.UsePotion();
            slot.ClearSlot();
            slot.RefreshSlot();
        }
        else
        {
            Sanity playerSanity = FindObjectOfType<Sanity>();
            if (playerSanity != null) playerSanity.IncreaseSanity(30f);
        }

        Destroy(instance);
    }
}
