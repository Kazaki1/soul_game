using UnityEngine;
using System.Collections.Generic;

public class ItemController : MonoBehaviour
{
    [Header("Equipped Items")]
    [SerializeField] private GameObject[] equippedItems = new GameObject[3];

    private ItemStats[] itemStats = new ItemStats[3];

    // Tổng % buff cho từng loại
    private float totalDamageBuffPercent = 0f;
    private float totalDodgeSpeedBuffPercent = 0f;
    private float totalHealthBuffPercent = 0f;
    private float totalArmorBuffPercent = 0f;
    private float totalStaminaBuffPercent = 0f;

    private float cachedTotalWeight = 0f;

    private PlayerCapacity playerCapacity;
    private void Awake()
    {
        playerCapacity = GetComponent<PlayerCapacity>();

        if (playerCapacity == null)
        {
            Debug.LogWarning("PlayerCapacity component not found!");
        }

        // Move loading here
        for (int i = 0; i < 3; i++)
        {
            if (equippedItems[i] != null)
            {
                LoadItem(i);
            }
        }
    }
    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            if (equippedItems[i] != null)
            {
                LoadItem(i);
            }
        }
    }

    /// <summary>
    /// Load item stats và buffs từ equipped item tại slot
    /// </summary>
    private void LoadItem(int slotIndex)
    {
        if (equippedItems[slotIndex] == null)
        {
            itemStats[slotIndex] = null;
            Debug.LogWarning($"Không có item tại slot {slotIndex}!");
            return;
        }

        itemStats[slotIndex] = equippedItems[slotIndex].GetComponent<ItemStats>();

        if (itemStats[slotIndex] == null)
        {
            Debug.LogError($"Item {equippedItems[slotIndex].name} tại slot {slotIndex} không có ItemStats component!");
            return;
        }

        // Cập nhật tổng stats và buffs
        UpdateTotalStats();
        Debug.Log($"Loaded item: {equippedItems[slotIndex].name} tại slot {slotIndex} - Weight: {itemStats[slotIndex].GetItemWeight():F1}");
    }

    /// <summary>
    /// Cập nhật tổng weight và buffs từ tất cả items
    /// </summary>
    public void UpdateTotalStats()
    {
        totalDamageBuffPercent = 0f;
        totalDodgeSpeedBuffPercent = 0f;
        totalHealthBuffPercent = 0f;
        totalArmorBuffPercent = 0f;
        totalStaminaBuffPercent = 0f;
        cachedTotalWeight = 0f;

        for (int i = 0; i < 3; i++)
        {
            if (equippedItems[i] != null && itemStats[i] != null) // Thêm kiểm tra itemStats[i] != null
            {
                cachedTotalWeight += itemStats[i].GetItemWeight();

                // Quét các buff component trên item
                DamageBuff damageBuff = equippedItems[i].GetComponent<DamageBuff>();
                if (damageBuff != null) totalDamageBuffPercent += damageBuff.GetBuffPercent();

                DodgeSpeedBuff dodgeBuff = equippedItems[i].GetComponent<DodgeSpeedBuff>();
                if (dodgeBuff != null) totalDodgeSpeedBuffPercent += dodgeBuff.GetBuffPercent();

                HealthBuff healthBuff = equippedItems[i].GetComponent<HealthBuff>();
                if (healthBuff != null) totalHealthBuffPercent += healthBuff.GetBuffPercent();

                ArmorBuff armorBuff = equippedItems[i].GetComponent<ArmorBuff>();
                if (armorBuff != null) totalArmorBuffPercent += armorBuff.GetBuffPercent();

                StaminaBuff staminaBuff = equippedItems[i].GetComponent<StaminaBuff>();
                if (staminaBuff != null) totalStaminaBuffPercent += staminaBuff.GetBuffPercent();
            }
        }
    }
    /// <summary>
    /// Equip item mới vào slot cụ thể
    /// </summary>
    public void EquipItem(GameObject newItem, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= 3)
        {
            Debug.LogError($"Slot index {slotIndex} không hợp lệ! Chỉ hỗ trợ 0-2.");
            return;
        }

        // Unequip item cũ nếu có
        if (equippedItems[slotIndex] != null)
        {
            Debug.Log($"Unequipped {equippedItems[slotIndex].name} từ slot {slotIndex}");
        }

        // Equip item mới
        equippedItems[slotIndex] = newItem;
        LoadItem(slotIndex);
    }

    /// <summary>
    /// Unequip item tại slot cụ thể
    /// </summary>
    public void UnequipItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= 3)
        {
            Debug.LogError($"Slot index {slotIndex} không hợp lệ! Chỉ hỗ trợ 0-2.");
            return;
        }

        if (equippedItems[slotIndex] != null)
        {
            Debug.Log($"Unequipped {equippedItems[slotIndex].name} từ slot {slotIndex}");
        }

        equippedItems[slotIndex] = null;
        itemStats[slotIndex] = null;
        UpdateTotalStats();
    }

    /// <summary>
    /// Kiểm tra có item tại slot không
    /// </summary>
    public bool HasItemInSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < 3 && itemStats[slotIndex] != null;
    }

    /// <summary>
    /// Hiển thị thông tin items và buffs
    /// </summary>
    public void DisplayItemInfo()
    {
        UpdateTotalStats();

        Debug.Log("========== ITEM INFO ==========");
        for (int i = 0; i < 3; i++)
        {
            if (equippedItems[i] != null)
            {
                Debug.Log($"Slot {i}: {equippedItems[i].name} - Weight: {itemStats[i].GetItemWeight():F1}");
                // Liệt kê buffs nếu có
                DamageBuff damageBuff = equippedItems[i].GetComponent<DamageBuff>();
                if (damageBuff != null) Debug.Log($"  - Damage Buff: +{damageBuff.GetBuffPercent()}%");

                DodgeSpeedBuff dodgeBuff = equippedItems[i].GetComponent<DodgeSpeedBuff>();
                if (dodgeBuff != null) Debug.Log($"  - Dodge Speed Buff: +{dodgeBuff.GetBuffPercent()}%");

                HealthBuff healthBuff = equippedItems[i].GetComponent<HealthBuff>();
                if (healthBuff != null) Debug.Log($"  - Health Buff: +{healthBuff.GetBuffPercent()}%");

                ArmorBuff armorBuff = equippedItems[i].GetComponent<ArmorBuff>();
                if (armorBuff != null) Debug.Log($"  - Armor Buff: +{armorBuff.GetBuffPercent()}%");

                StaminaBuff staminaBuff = equippedItems[i].GetComponent<StaminaBuff>();
                if (staminaBuff != null) Debug.Log($"  - Stamina Buff: +{staminaBuff.GetBuffPercent()}%");
            }
            else
            {
                Debug.Log($"Slot {i}: (Empty)");
            }
        }
        Debug.Log($"Total Weight: {cachedTotalWeight:F1}");
        Debug.Log($"Total Damage Buff: +{totalDamageBuffPercent}%");
        Debug.Log($"Total Dodge Speed Buff: +{totalDodgeSpeedBuffPercent}%");
        Debug.Log($"Total Health Buff: +{totalHealthBuffPercent}%");
        Debug.Log($"Total Armor Buff: +{totalArmorBuffPercent}%");
        Debug.Log($"Total Stamina Buff: +{totalStaminaBuffPercent}%");
        Debug.Log("===============================");

        // Hiển thị capacity info nếu có
        if (playerCapacity != null)
        {
            playerCapacity.DisplayCapacityInfo();
        }
    }

    /// <summary>
    /// Lấy tổng current equip load từ items (weight)
    /// </summary>
    public float GetCurrentItemLoad()
    {
        return cachedTotalWeight;
    }

    // Getters cho tổng % buff từng loại
    public float GetTotalDamageBuffPercent() => totalDamageBuffPercent;
    public float GetTotalDodgeSpeedBuffPercent() => totalDodgeSpeedBuffPercent;
    public float GetTotalHealthBuffPercent() => totalHealthBuffPercent;
    public float GetTotalArmorBuffPercent() => totalArmorBuffPercent;
    public float GetTotalStaminaBuffPercent() => totalStaminaBuffPercent;

    /// <summary>
    /// Lấy item stats tại slot
    /// </summary>
    public ItemStats GetItemStats(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < 3)
            return itemStats[slotIndex];
        return null;
    }
}