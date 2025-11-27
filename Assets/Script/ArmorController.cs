using UnityEngine;

public class ArmorController : MonoBehaviour
{
    public enum ArmorSlot
    {
        Head,
        Chest,
        Legs,
        Boots
    }

    [System.Serializable]
    public class ArmorSlotData
    {
        public ArmorSlot slotType;
        public GameObject equippedArmor;
        [HideInInspector] public ArmorStats stats;
    }

    [Header("Armor Slots")]
    public ArmorSlotData[] armorSlots = new ArmorSlotData[4];

    private PlayerDefend playerDefend;
    private PlayerCapacity playerCapacity;

    private float totalArmorDefense = 0f;
    private float totalArmorWeight = 0f;

    private void Awake()
    {
        playerDefend = GetComponent<PlayerDefend>();
        playerCapacity = GetComponent<PlayerCapacity>();

        if (playerDefend == null) Debug.LogError("PlayerDefend not found!");
        if (playerCapacity == null) Debug.LogError("PlayerCapacity not found!");
    }

    private void Start()
    {
        // Nếu có giáp được gắn sẵn trong inspector → load stats
        foreach (var slot in armorSlots)
        {
            if (slot.equippedArmor != null)
            {
                LoadArmor(slot);
            }
        }
    }

    // ======================================================
    // =============== EQUIP / UNEQUIP SYSTEM ===============
    // ======================================================

    public void EquipArmor(GameObject armorObj)
    {
        ArmorStats armorStats = armorObj.GetComponent<ArmorStats>();
        if (armorStats == null)
        {
            Debug.LogError("Armor không có ArmorStats!");
            return;
        }

        ArmorSlot slotType = (ArmorSlot)armorStats.GetArmorType();

        ArmorSlotData slot = GetSlot(slotType);

        // Unequip giáp cũ trước
        if (slot.equippedArmor != null)
        {
            UnequipArmor(slotType);
        }

        // Equip giáp mới
        slot.equippedArmor = armorObj;
        LoadArmor(slot);

        Debug.Log($"Equipped {armorObj.name} vào slot {slotType}");
    }

    public void UnequipArmor(ArmorSlot slotType)
    {
        ArmorSlotData slot = GetSlot(slotType);

        if (slot.equippedArmor == null)
        {
            Debug.Log($"Slot {slotType} không có armor để unequip");
            return;
        }

        // Trừ defense & weight
        float def = slot.stats.GetArmorDefense();
        float weight = slot.stats.GetArmorWeight();

        totalArmorDefense -= def;
        totalArmorWeight -= weight;

        slot.equippedArmor = null;
        slot.stats = null;

        Debug.Log($"Unequipped armor tại slot {slotType}");
    }

    // ======================================================
    // ===================== LOAD STATS ===================== 
    // ======================================================

    private void LoadArmor(ArmorSlotData slot)
    {
        slot.stats = slot.equippedArmor.GetComponent<ArmorStats>();

        float def = slot.stats.GetArmorDefense();
        float weight = slot.stats.GetArmorWeight();

        totalArmorDefense += def;
        totalArmorWeight += weight;

        Debug.Log($"Loaded armor {slot.equippedArmor.name} - DEF: {def}, Weight: {weight}");
    }

    // ======================================================
    // ====================== UTILITIES ======================
    // ======================================================

    private ArmorSlotData GetSlot(ArmorSlot slotType)
    {
        foreach (var slot in armorSlots)
        {
            if (slot.slotType == slotType)
                return slot;
        }

        Debug.LogError("Không tìm thấy slot " + slotType);
        return null;
    }

    public float GetTotalArmorDefense() => totalArmorDefense;
    public float GetTotalArmorWeight() => totalArmorWeight;

    public void DisplayArmorInfo()
    {
        Debug.Log("========= ARMOR INFO =========");
        foreach (var slot in armorSlots)
        {
            if (slot.equippedArmor != null)
            {
                Debug.Log($"{slot.slotType}: {slot.equippedArmor.name} " +
                          $"- DEF: {slot.stats.GetArmorDefense()}, " +
                          $"Weight: {slot.stats.GetArmorWeight()}");
            }
            else
            {
                Debug.Log($"{slot.slotType}: (Empty)");
            }
        }
        Debug.Log($"Total Defense: {totalArmorDefense}");
        Debug.Log($"Total Weight: {totalArmorWeight}");
        Debug.Log("==============================");
    }
}
