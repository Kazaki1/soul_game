using UnityEngine;

public class PlayerCapacity : MonoBehaviour
{
    [Header("Capacity Settings")]
    [SerializeField] private float baseCapacity = 50f;
    [SerializeField] private float maxEquipLoad;

    [Header("Debug Info (Read Only)")]
    [SerializeField] private float debugCurrentLoad;
    [SerializeField] private float debugRemainingSpace;
    [SerializeField] private float debugLoadPercentage;

    private PlayerStats playerStats;
    private PlayerWeaponController weaponController;
    private ArmorController armorController;
    private ItemController itemController; // Thêm reference đến ItemController

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        weaponController = GetComponent<PlayerWeaponController>();
        armorController = GetComponent<ArmorController>();
        itemController = GetComponent<ItemController>(); // Thêm để lấy item weight

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
        }

        if (weaponController == null)
        {
            Debug.LogWarning("PlayerWeaponController component not found!");
        }
        if (armorController == null)
        {
            Debug.LogWarning("ArmorController component not found!");
        }
        if (itemController == null)
        {
            Debug.LogWarning("ItemController component not found!");
        }
    }

    private void Start()
    {
        UpdateMaxEquipLoad();
    }

    private void Update()
    {
        // Cập nhật debug info liên tục
        UpdateDebugInfo();

        // Test key
        if (Input.GetKeyDown(KeyCode.L))
        {
            DisplayCapacityInfo();
        }
    }

    /// <summary>
    /// Cập nhật debug info để xem trong Inspector
    /// </summary>
    private void UpdateDebugInfo()
    {
        debugCurrentLoad = GetCurrentLoad();
        debugRemainingSpace = GetRemainingEquipSpace();
        debugLoadPercentage = maxEquipLoad > 0 ? (debugCurrentLoad / maxEquipLoad) * 100f : 0f;
    }

    /// <summary>
    /// Hiển thị thông tin capacity
    /// </summary>
    public void DisplayCapacityInfo()
    {
        Debug.Log("========== CAPACITY INFO ==========");
        Debug.Log($"Current Load: {debugCurrentLoad:F1}");
        Debug.Log($"Max Load: {maxEquipLoad:F1}");
        Debug.Log($"Remaining Space: {debugRemainingSpace:F1}");
        Debug.Log($"Load %: {debugLoadPercentage:F1}%");
        Debug.Log($"Can Equip 10 Weight? {CanEquipWeight(10f)}");
        Debug.Log($"Can Equip 50 Weight? {CanEquipWeight(50f)}");
        Debug.Log("===================================");
    }

    /// <summary>
    /// Cập nhật Max Equip Load dựa trên Endurance
    /// Level 9-25: +1.5 to +1.6 (avg: +1.59)
    /// Level 26-60: +1.0 to +1.5 (avg: +1.37)
    /// Level 61-99: +1.0 to +1.1 (avg: +1.03)
    /// </summary>
    public void UpdateMaxEquipLoad()
    {
        if (playerStats == null) return;

        int endLevel = playerStats.endurance;

        // Tính equip load gain dựa trên level
        float totalGain = 0f;

        // Level 1-8: Không có gain
        if (endLevel <= 8)
        {
            totalGain = 0f;
        }
        // Level 9-25
        else if (endLevel <= 25)
        {
            int levelsInRange = endLevel - 8;
            totalGain = levelsInRange * 1.59f;
        }
        // Level 26-60
        else if (endLevel <= 60)
        {
            totalGain = (25 - 8) * 1.59f;
            int levelsInRange = endLevel - 25;
            totalGain += levelsInRange * 1.37f;
        }
        // Level 61-99
        else
        {
            totalGain = (25 - 8) * 1.59f;
            totalGain += (60 - 25) * 1.37f;
            int levelsInRange = endLevel - 60;
            totalGain += levelsInRange * 1.03f;
        }

        maxEquipLoad = baseCapacity + totalGain;

        Debug.Log($"[PlayerCapacity] END Level: {endLevel}, Max Equip Load: {maxEquipLoad:F1}");
    }

    /// <summary>
    /// Lấy current load từ weapon, armor và items
    /// </summary>
    public float GetCurrentLoad()
    {
        float total = 0f;

        if (weaponController != null)
        {
            total += weaponController.GetCurrentEquipLoad(); // Hiện tại chỉ weapon weight
        }

        if (armorController != null)
        {
            total += armorController.GetTotalArmorWeight();
        }

        if (itemController != null)
        {
            total += itemController.GetCurrentItemLoad(); // Thêm item weight
        }

        return total;
    }

    /// <summary>
    /// Lấy equip space còn lại (Max Load - Current Load)
    /// Có thể âm nếu overload
    /// </summary>
    public float GetRemainingEquipSpace()
    {
        float currentLoad = GetCurrentLoad();
        float remaining = maxEquipLoad - currentLoad;

        return remaining; // Cho phép âm
    }

    /// <summary>
    /// Kiểm tra có đủ space để equip item với weight này không
    /// </summary>
    public bool CanEquipWeight(float weight)
    {
        return GetRemainingEquipSpace() >= weight;
    }

    // Getters
    public float GetMaxEquipLoad() => maxEquipLoad;
    public float GetBaseCapacity() => baseCapacity;

    // Setter
    public void SetBaseCapacity(float value)
    {
        baseCapacity = value;
        UpdateMaxEquipLoad();
    }
}