using UnityEngine;

public class PlayerDefend : MonoBehaviour
{
    private PlayerStats playerStats;
    private ItemController itemController; // Thêm reference đến ItemController

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        itemController = GetComponent<ItemController>(); // Thêm để lấy buffs

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
        }

        if (itemController == null)
        {
            Debug.LogWarning("ItemController không tìm thấy! Không có armor buff.");
        }
    }

    /// <summary>
    /// Tính Base Defense từ Soul Level (Rune Level)
    /// If rune level < 72: 40 + (rune level + 78) ÷ 2.483
    /// If 72 ≤ rune level < 92: 29 + rune level
    /// If 92 ≤ rune level < 161: 120 + (rune level - 91) ÷ 4.667
    /// If 161 ≤ rune level: 135 + (rune level - 161) ÷ 27.6
    /// </summary>
    private float CalculateBaseDefenseFromSoulLevel()
    {
        if (playerStats == null) return 0f;

        int soulLevel = playerStats.soulLevel;
        float baseDefense = 0f;

        if (soulLevel < 72)
        {
            baseDefense = 40f + (soulLevel + 78f) / 2.483f;
        }
        else if (soulLevel < 92)
        {
            baseDefense = 29f + soulLevel;
        }
        else if (soulLevel < 161)
        {
            baseDefense = 120f + (soulLevel - 91f) / 4.667f;
        }
        else // soulLevel >= 161
        {
            baseDefense = 135f + (soulLevel - 161f) / 27.6f;
        }

        return baseDefense;
    }

    /// <summary>
    /// Tính Strength Bonus Defense
    /// If Strength < 30: Strength ÷ 3
    /// If 30 ≤ Strength < 40: 10 + (Strength - 30) ÷ 2
    /// If 40 ≤ Strength < 60: 15 + (Strength - 40) ÷ 1.333
    /// If 60 ≤ Strength: 30 + (Strength - 60) ÷ 3.9
    /// </summary>
    private float CalculateStrengthBonus()
    {
        if (playerStats == null) return 0f;

        int strength = playerStats.strength;
        float bonus = 0f;

        if (strength < 30)
        {
            bonus = strength / 3f;
        }
        else if (strength < 40)
        {
            bonus = 10f + (strength - 30f) / 2f;
        }
        else if (strength < 60)
        {
            bonus = 15f + (strength - 40f) / 1.333f;
        }
        else // strength >= 60
        {
            bonus = 30f + (strength - 60f) / 3.9f;
        }

        return bonus;
    }

    /// <summary>
    /// Tính tổng Defense = Base Defense + Strength Bonus + Armor + Buffs
    /// </summary>
    public float CalculateDefense()
    {
        float baseDefense = CalculateBaseDefenseFromSoulLevel();
        float strBonus = CalculateStrengthBonus();
        float armorDefense = GetComponent<ArmorController>().GetTotalArmorDefense();

        // Áp dụng armor buff từ items
        float buffMultiplier = 1f;
        if (itemController != null)
        {
            float buffPercent = itemController.GetTotalArmorBuffPercent();
            buffMultiplier += buffPercent / 100f;
        }

        float totalDefense = (baseDefense + strBonus + armorDefense) * buffMultiplier;

        return totalDefense;
    }

    /// <summary>
    /// Lấy defense với modifier bổ sung (armor, buffs)
    /// </summary>
    public float GetDefenseWithModifier(float modifier = 1f)
    {
        return CalculateDefense() * modifier;
    }

    /// <summary>
    /// Lấy defense stat đã tính (alias cho CalculateDefense)
    /// </summary>
    public float GetDefenseStat()
    {
        return CalculateDefense();
    }

    /// <summary>
    /// Debug info - hiển thị breakdown defense
    /// </summary>
    public void DisplayDefenseBreakdown()
    {
        if (playerStats == null) return;

        float baseDefense = CalculateBaseDefenseFromSoulLevel();
        float strBonus = CalculateStrengthBonus();
        float total = CalculateDefense();

        Debug.Log("========== DEFENSE BREAKDOWN ==========");
        Debug.Log($"Soul Level: {playerStats.soulLevel}");
        Debug.Log($"Strength: {playerStats.strength}");
        Debug.Log($"Base Defense (from Soul Level): {baseDefense:F1}");
        Debug.Log($"Strength Bonus: {strBonus:F1}");
        Debug.Log($"TOTAL DEFENSE: {total:F1}");
        Debug.Log("=======================================");
    }
}