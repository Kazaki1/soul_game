using UnityEngine;

/// <summary>
/// Tính toán Sanity Consumption cho từng Weapon Type
/// Có thể setup riêng từng loại weapon có tốn sanity hay không
/// </summary>
public class SanityConsumption : MonoBehaviour
{
    [Header("Base Consumption")]
    [SerializeField] private float baseSanityConsumption = 10f;

    [Header("Weapon Type Consumption Settings")]
    [SerializeField] private WeaponTypeSanityConfig[] weaponTypeConfigs;

    private PlayerStats playerStats;

    [System.Serializable]
    public class WeaponTypeSanityConfig
    {
        public string weaponTypeName = "Type 0";
        public int weaponType = 0;
        public bool consumesSanity = false; // Có tốn sanity không
        public float sanityMultiplier = 1f; // Multiplier riêng cho weapon type này
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
        }
    }

    /// <summary>
    /// Tính Sanity Consumption cho weapon type cụ thể
    /// Trả về 0 nếu weapon type đó không tốn sanity
    /// </summary>
    public float CalculateSanityConsumption(int weaponType)
    {
        // Kiểm tra weapon type này có tốn sanity không
        WeaponTypeSanityConfig config = GetConfigForWeaponType(weaponType);

        if (config == null || !config.consumesSanity)
        {
            return 0f; // Không tốn sanity
        }

        if (playerStats == null)
            return baseSanityConsumption * config.sanityMultiplier;

        int intLevel = playerStats.intelligence;
        float costMultiplier;

        // Xác định multiplier dựa trên Intelligence level
        if (intLevel <= 30)
        {
            costMultiplier = 1.0f;
        }
        else if (intLevel <= 60)
        {
            costMultiplier = 0.85f;
        }
        else // 61-99
        {
            costMultiplier = 0.7f;
        }

        float finalCost = baseSanityConsumption * costMultiplier * config.sanityMultiplier;

        return finalCost;
    }

    /// <summary>
    /// Kiểm tra weapon type có tốn sanity không
    /// </summary>
    public bool DoesWeaponTypeConsumeSanity(int weaponType)
    {
        WeaponTypeSanityConfig config = GetConfigForWeaponType(weaponType);
        return config != null && config.consumesSanity;
    }

    /// <summary>
    /// Lấy config cho weapon type
    /// </summary>
    private WeaponTypeSanityConfig GetConfigForWeaponType(int weaponType)
    {
        if (weaponTypeConfigs == null) return null;

        foreach (var config in weaponTypeConfigs)
        {
            if (config.weaponType == weaponType)
            {
                return config;
            }
        }

        return null;
    }

    // Getters/Setters
    public float GetBaseConsumption() => baseSanityConsumption;
    public void SetBaseConsumption(float value) => baseSanityConsumption = Mathf.Max(0f, value);
}