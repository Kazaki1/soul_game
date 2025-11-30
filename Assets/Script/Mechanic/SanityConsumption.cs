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
        public bool consumesSanity = false; 
        public float sanityMultiplier = 1f;   }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

   
    public float CalculateSanityConsumption(int weaponType)
    {
          WeaponTypeSanityConfig config = GetConfigForWeaponType(weaponType);

        if (config == null || !config.consumesSanity)
        {
            return 0f; 
        }

        if (playerStats == null)
            return baseSanityConsumption * config.sanityMultiplier;

        int intLevel = playerStats.intelligence;
        float costMultiplier;

          if (intLevel <= 30)
        {
            costMultiplier = 1.0f;
        }
        else if (intLevel <= 60)
        {
            costMultiplier = 0.85f;
        }
        else        {
            costMultiplier = 0.7f;
        }

        float finalCost = baseSanityConsumption * costMultiplier * config.sanityMultiplier;

        return finalCost;
    }
    public bool DoesWeaponTypeConsumeSanity(int weaponType)
    {
        WeaponTypeSanityConfig config = GetConfigForWeaponType(weaponType);
        return config != null && config.consumesSanity;
    }

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
    public float GetBaseConsumption() => baseSanityConsumption;
    public void SetBaseConsumption(float value) => baseSanityConsumption = Mathf.Max(0f, value);
}