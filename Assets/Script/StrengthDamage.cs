using UnityEngine;

public class StrengthDamage : MonoBehaviour
{
    [Header("Strength Damage Settings")]
    [SerializeField] private float baseStrengthDamage = 10f;

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
            Debug.Log($"[StrengthDamage] PlayerStats found! STR Level: {playerStats.strength}");
        }
    }

    /// <summary>
    /// Tính damage dựa trên Strength stat với soft cap scaling
    /// Lv 1-30: 0.3 scaling
    /// Lv 31-60: 0.27 scaling
    /// Lv 61-99: 0.2 scaling
    /// Formula: (BaseDamage + (BaseDamage × (scaling × Lv) × 0.6) + (Lv × 0.5)) × 0.5
    /// </summary>
    public float CalculateStrengthDamage()
    {
        if (playerStats == null)
        {
            Debug.LogError("[StrengthDamage] playerStats is NULL!");
            return 0f;
        }

        int strLevel = playerStats.strength;

        Debug.Log($"[StrengthDamage] STR Level: {strLevel}, Base Damage: {baseStrengthDamage}");

        // Xác định scaling dựa trên level (soft cap)
        float scaling;
        if (strLevel <= 30)
            scaling = 0.3f;
        else if (strLevel <= 60)
            scaling = 0.27f;
        else
            scaling = 0.2f;

        float damage = (baseStrengthDamage +
                       (baseStrengthDamage * (scaling * strLevel) * 0.6f) +
                       (strLevel * 0.5f)) * 0.5f;

        Debug.Log($"[StrengthDamage] Calculated Damage: {damage} (Scaling: {scaling})");

        return damage;
    }

    /// <summary>
    /// Lấy damage với modifier bổ sung (vd: weapon multiplier)
    /// </summary>
    public float GetDamageWithModifier(float modifier = 1f)
    {
        return CalculateStrengthDamage() * modifier;
    }

    /// <summary>
    /// Thiết lập base damage mới
    /// </summary>
    public void SetBaseDamage(float newBaseDamage)
    {
        baseStrengthDamage = newBaseDamage;
    }

    /// <summary>
    /// Lấy base damage hiện tại
    /// </summary>
    public float GetBaseDamage()
    {
        return baseStrengthDamage;
    }

    /// <summary>
    /// Lấy stat damage đã tính (giống CalculateStrengthDamage)
    /// </summary>
    public float GetStatDamage()
    {
        return CalculateStrengthDamage();
    }
}