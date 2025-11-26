using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponStats weaponStats;
    [SerializeField] private StrengthDamage strengthDamage;
    [SerializeField] private DexterityDamage dexterityDamage;
    [SerializeField] private MagicDamage magicDamage;

    private void Awake()
    {
        // Lấy WeaponStats từ chính GameObject này
        if (weaponStats == null)
            weaponStats = GetComponent<WeaponStats>();
        
        if (weaponStats == null)
        {
            Debug.LogError("WeaponStats component not found on weapon!");
        }
    }

    private void FindPlayerDamageComponents()
    {
        // Tìm các damage components từ Player (chỉ tìm khi cần)
        if (strengthDamage == null)
        {
            strengthDamage = FindObjectOfType<StrengthDamage>();
            Debug.Log($"[WeaponDamage] StrengthDamage found: {strengthDamage != null}");
        }
        
        if (dexterityDamage == null)
        {
            dexterityDamage = FindObjectOfType<DexterityDamage>();
            Debug.Log($"[WeaponDamage] DexterityDamage found: {dexterityDamage != null}");
        }
        
        if (magicDamage == null)
        {
            magicDamage = FindObjectOfType<MagicDamage>();
            Debug.Log($"[WeaponDamage] MagicDamage found: {magicDamage != null}");
        }
    }

    /// <summary>
    /// Tính tổng damage của weapon
    /// Formula: WeaponDamage = Base + (Base × StrScale × StrDmg/100) + (Base × DexScale × DexDmg/100) + (Base × IntScale × IntDmg/100)
    /// </summary>
    public float CalculateWeaponDamage()
    {
        if (weaponStats == null)
        {
            Debug.LogError("WeaponStats not found!");
            return 0f;
        }

        // Tìm player damage components khi cần
        FindPlayerDamageComponents();

        float baseDamage = weaponStats.GetBaseDamage();
        float totalDamage = baseDamage;

        Debug.Log($"[WeaponDamage] === CALCULATION START ===");
        Debug.Log($"[WeaponDamage] Base Damage: {baseDamage}");

        // Cộng Strength scaling
        if (strengthDamage != null && weaponStats.GetStrScale() > 0)
        {
            float strDmg = strengthDamage.CalculateStrengthDamage();
            float strScale = weaponStats.GetStrScale();
            float strBonus = baseDamage * strScale * (strDmg / 100f);
            
            Debug.Log($"[WeaponDamage] STR - Stat Damage: {strDmg:F1}, Scale: {strScale}, Bonus: {strBonus:F1}");
            totalDamage += strBonus;
        }
        else
        {
            Debug.LogWarning($"[WeaponDamage] STR skipped - Component: {strengthDamage != null}, Scale: {weaponStats.GetStrScale()}");
        }

        // Cộng Dexterity scaling
        if (dexterityDamage != null && weaponStats.GetDexScale() > 0)
        {
            float dexDmg = dexterityDamage.CalculateDexterityDamage();
            float dexScale = weaponStats.GetDexScale();
            float dexBonus = baseDamage * dexScale * (dexDmg / 100f);
            
            Debug.Log($"[WeaponDamage] DEX - Stat Damage: {dexDmg:F1}, Scale: {dexScale}, Bonus: {dexBonus:F1}");
            totalDamage += dexBonus;
        }
        else
        {
            Debug.LogWarning($"[WeaponDamage] DEX skipped - Component: {dexterityDamage != null}, Scale: {weaponStats.GetDexScale()}");
        }

        // Cộng Intelligence scaling
        if (magicDamage != null && weaponStats.GetIntScale() > 0)
        {
            float intDmg = magicDamage.CalculateMagicDamage();
            float intScale = weaponStats.GetIntScale();
            float intBonus = baseDamage * intScale * (intDmg / 100f);
            
            Debug.Log($"[WeaponDamage] INT - Stat Damage: {intDmg:F1}, Scale: {intScale}, Bonus: {intBonus:F1}");
            totalDamage += intBonus;
        }

        Debug.Log($"[WeaponDamage] TOTAL DAMAGE: {totalDamage:F1}");
        Debug.Log($"[WeaponDamage] === CALCULATION END ===");

        return totalDamage;
    }

    /// <summary>
    /// Hiển thị breakdown chi tiết damage
    /// </summary>
    public void DisplayDamageBreakdown()
    {
        if (weaponStats == null) return;

        // Tìm player damage components
        FindPlayerDamageComponents();

        float baseDamage = weaponStats.GetBaseDamage();
        
        Debug.Log("========== WEAPON DAMAGE BREAKDOWN ==========");
        Debug.Log($"Base Damage: {baseDamage:F1}");
        
        float totalBonus = 0f;

        // STR Bonus
        if (strengthDamage != null && weaponStats.GetStrScale() > 0)
        {
            float strDmg = strengthDamage.CalculateStrengthDamage();
            float strScale = weaponStats.GetStrScale();
            float strBonus = baseDamage * strScale * (strDmg / 100f);
            totalBonus += strBonus;
            Debug.Log($"+ STR Bonus: {strBonus:F1} (Scale: {strScale:F2} × Stat Dmg: {strDmg:F1})");
        }

        // DEX Bonus
        if (dexterityDamage != null && weaponStats.GetDexScale() > 0)
        {
            float dexDmg = dexterityDamage.CalculateDexterityDamage();
            float dexScale = weaponStats.GetDexScale();
            float dexBonus = baseDamage * dexScale * (dexDmg / 100f);
            totalBonus += dexBonus;
            Debug.Log($"+ DEX Bonus: {dexBonus:F1} (Scale: {dexScale:F2} × Stat Dmg: {dexDmg:F1})");
        }

        // INT Bonus
        if (magicDamage != null && weaponStats.GetIntScale() > 0)
        {
            float intDmg = magicDamage.CalculateMagicDamage();
            float intScale = weaponStats.GetIntScale();
            float intBonus = baseDamage * intScale * (intDmg / 100f);
            totalBonus += intBonus;
            Debug.Log($"+ INT Bonus: {intBonus:F1} (Scale: {intScale:F2} × Stat Dmg: {intDmg:F1})");
        }

        Debug.Log($"= TOTAL DAMAGE: {CalculateWeaponDamage():F1} ({baseDamage:F1} + {totalBonus:F1})");
        Debug.Log("=============================================");
    }

    /// <summary>
    /// Lấy damage với modifier (critical, buff, etc.)
    /// </summary>
    public float GetDamageWithModifier(float modifier = 1f)
    {
        return CalculateWeaponDamage() * modifier;
    }
}