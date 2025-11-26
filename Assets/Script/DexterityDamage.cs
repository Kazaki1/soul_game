using UnityEngine;

public class DexterityDamage : MonoBehaviour
{
    [Header("Dexterity Damage Settings")]
    [SerializeField] private float baseDexterityDamage = 8f;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
        }
        else
        {
            Debug.Log($"[DexterityDamage] PlayerStats found! DEX Level: {playerStats.dexterity}");
        }
    }

    /// <summary>
    /// Tính damage dựa trên Dexterity stat với soft cap scaling
    /// Lv 1-30: 0.3 scaling
    /// Lv 31-60: 0.27 scaling
    /// Lv 61-99: 0.2 scaling
    /// Formula: (BaseDamage + (BaseDamage × (scaling × Lv) × 0.6) + (Lv × 0.5)) × 0.5
    /// </summary>
    public float CalculateDexterityDamage()
    {
        if (playerStats == null)
        {
            Debug.LogError("[DexterityDamage] playerStats is NULL!");
            return 0f;
        }

        int dexLevel = playerStats.dexterity;

        Debug.Log($"[DexterityDamage] DEX Level: {dexLevel}, Base Damage: {baseDexterityDamage}");

        // Xác định scaling dựa trên level (soft cap)
        float scaling;
        if (dexLevel <= 30)
            scaling = 0.3f;
        else if (dexLevel <= 60)
            scaling = 0.27f;
        else
            scaling = 0.2f;

        float damage = (baseDexterityDamage +
                       (baseDexterityDamage * (scaling * dexLevel) * 0.6f) +
                       (dexLevel * 0.5f)) * 0.5f;

        Debug.Log($"[DexterityDamage] Calculated Damage: {damage} (Scaling: {scaling})");

        return damage;
    }

    /// <summary>
    /// Lấy damage với modifier bổ sung (vd: critical hit, weapon speed)
    /// </summary>
    public float GetDamageWithModifier(float modifier = 1f)
    {
        return CalculateDexterityDamage() * modifier;
    }

    /// <summary>
    /// Thiết lập base damage mới
    /// </summary>
    public void SetBaseDamage(float newBaseDamage)
    {
        baseDexterityDamage = newBaseDamage;
    }

    /// <summary>
    /// Lấy base damage hiện tại
    /// </summary>
    public float GetBaseDamage()
    {
        return baseDexterityDamage;
    }

    /// <summary>
    /// Lấy stat damage đã tính (giống CalculateDexterityDamage)
    /// </summary>
    public float GetStatDamage()
    {
        return CalculateDexterityDamage();
    }
}