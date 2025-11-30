using UnityEngine;

public class EnemyDamageDealRange : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int baseDamage = 100;
    [SerializeField][Range(0f, 1f)] private float playerHpPercentage = 0.2f; // 20% = 0.2

    [Header("Debug Info (Read Only)")]
    [SerializeField] private int lastCalculatedDamage;
    [SerializeField] private int lastBaseDamage;
    [SerializeField] private int lastPercentageDamage;
    [SerializeField] private float lastPlayerDefense;
    [SerializeField] private int lastFinalDamage;

    /// <summary>
    /// Tính toán damage dựa trên: (Base Damage + % Player Max HP) - Defense Reduction
    /// Formula: total_damage = totalDamage - (totalDamage * (PlayerDefend/20) / 100)
    /// 20 Defense = 1% reduction
    /// </summary>
    public int CalculateDamage(PlayerHealth playerHealth, PlayerDefend playerDefend = null)
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth is null! Returning base damage only.");
            return baseDamage;
        }

        // Lấy max HP của player
        int playerMaxHP = playerHealth.GetMaxHealth();

        // Tính % damage
        int percentageDamage = Mathf.RoundToInt(playerMaxHP * playerHpPercentage);

        // Tổng damage trước khi trừ defense
        int totalDamage = baseDamage + percentageDamage;

        // Áp dụng defense reduction
        int finalDamage = totalDamage;
        float defenseValue = 0f;

        if (playerDefend != null)
        {
            defenseValue = playerDefend.CalculateDefense();

            // Formula: Reduction% = (PlayerDefend / 20) / 100
            // Example: 20 Defense = (20/20)/100 = 0.01 = 1%
            //          40 Defense = (40/20)/100 = 0.02 = 2%
            //          100 Defense = (100/20)/100 = 0.05 = 5%
            float reductionPercentage = (defenseValue / 20f) / 100f;
            float damageReduction = totalDamage * reductionPercentage;
            finalDamage = Mathf.CeilToInt(totalDamage - damageReduction);

            // Đảm bảo damage tối thiểu là 1
            finalDamage = Mathf.Max(1, finalDamage);
        }

        // Cache để debug
        lastBaseDamage = baseDamage;
        lastPercentageDamage = percentageDamage;
        lastCalculatedDamage = totalDamage;
        lastPlayerDefense = defenseValue;
        lastFinalDamage = finalDamage;

        float reductionPercent = (defenseValue / 20f);
        Debug.Log($"⚔️ Enemy Damage: {totalDamage} - Defense: {defenseValue:F1} ({reductionPercent:F2}%) → Final: {finalDamage}");

        return finalDamage;
    }

    /// <summary>
    /// Gây damage cho player (gọi hàm này khi enemy attack hit)
    /// </summary>
    public void DealDamageTo(PlayerHealth playerHealth)
    {
        if (playerHealth == null) return;

        // Lấy PlayerDefend component
        PlayerDefend playerDefend = playerHealth.GetComponent<PlayerDefend>();

        int damage = CalculateDamage(playerHealth, playerDefend);
        playerHealth.TakeDamage(damage);
    }

    /// <summary>
    /// Gây damage cho player thông qua collision
    /// </summary>
    public void DealDamageOnTrigger(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                DealDamageTo(playerHealth);
            }
        }
    }

    // Getters
    public int GetBaseDamage() => baseDamage;
    public float GetPlayerHpPercentage() => playerHpPercentage;
    public int GetLastCalculatedDamage() => lastCalculatedDamage;
    public int GetLastFinalDamage() => lastFinalDamage;

    // Setters
    public void SetBaseDamage(int value) => baseDamage = Mathf.Max(0, value);
    public void SetPlayerHpPercentage(float value) => playerHpPercentage = Mathf.Clamp01(value);

    /// <summary>
    /// Hiển thị thông tin damage trong console
    /// </summary>
    public void DisplayDamageInfo(PlayerHealth playerHealth)
    {
        if (playerHealth == null) return;

        PlayerDefend playerDefend = playerHealth.GetComponent<PlayerDefend>();
        int damage = CalculateDamage(playerHealth, playerDefend);

        float reductionPercent = (lastPlayerDefense / 20f);

        Debug.Log("========== ENEMY DAMAGE INFO ==========");
        Debug.Log($"Base Damage: {baseDamage}");
        Debug.Log($"HP% Scaling: {playerHpPercentage * 100}%");
        Debug.Log($"Player Max HP: {playerHealth.GetMaxHealth()}");
        Debug.Log($"Percentage Damage: {lastPercentageDamage}");
        Debug.Log($"Raw Damage: {lastCalculatedDamage}");
        Debug.Log($"Player Defense: {lastPlayerDefense:F1}");
        Debug.Log($"Damage Reduction: {reductionPercent:F2}% ({lastCalculatedDamage * (lastPlayerDefense / 20f) / 100f:F1} damage)");
        Debug.Log($"FINAL DAMAGE: {damage}");
        Debug.Log("=======================================");
    }
}