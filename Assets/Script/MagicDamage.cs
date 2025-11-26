using UnityEngine;

public class MagicDamage : MonoBehaviour
{
    [Header("Magic Damage Settings")]
    [SerializeField] private float baseMagicDamage = 12f;

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
            Debug.Log($"[MagicDamage] PlayerStats found! INT Level: {playerStats.intelligence}");
        }
    }

    /// <summary>
    /// Tính damage dựa trên Intelligence stat với soft cap scaling
    /// Lv 1-30: 0.3 scaling
    /// Lv 31-60: 0.27 scaling
    /// Lv 61-99: 0.2 scaling
    /// Formula: (BaseDamage + (BaseDamage × (scaling × Lv) × 0.6) + (Lv × 0.5)) × 0.5
    /// </summary>
    public float CalculateMagicDamage()
    {
        if (playerStats == null) return 0f;

        int intLevel = playerStats.intelligence;

        // Xác định scaling dựa trên level (soft cap)
        float scaling;
        if (intLevel <= 30)
            scaling = 0.3f;
        else if (intLevel <= 60)
            scaling = 0.27f;
        else
            scaling = 0.2f;

        float damage = (baseMagicDamage +
                       (baseMagicDamage * (scaling * intLevel) * 0.6f) +
                       (intLevel * 0.5f)) * 0.5f;

        return damage;
    }

    /// <summary>
    /// Lấy damage với modifier bổ sung (vd: spell power, elemental bonus)
    /// </summary>
    public float GetDamageWithModifier(float modifier = 1f)
    {
        return CalculateMagicDamage() * modifier;
    }

    /// <summary>
    /// Thiết lập base damage mới
    /// </summary>
    public void SetBaseDamage(float newBaseDamage)
    {
        baseMagicDamage = newBaseDamage;
    }

    /// <summary>
    /// Lấy base damage hiện tại
    /// </summary>
    public float GetBaseDamage()
    {
        return baseMagicDamage;
    }

    /// <summary>
    /// Lấy stat damage đã tính (giống CalculateMagicDamage)
    /// </summary>
    public float GetStatDamage()
    {
        return CalculateMagicDamage();
    }
}