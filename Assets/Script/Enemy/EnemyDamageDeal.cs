using UnityEngine;

public class EnemyDamageDeal : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int baseDamage = 100;
    [SerializeField][Range(0f, 1f)] private float playerHpPercentage = 0.2f; // 20% = 0.2

    [Header("Debug Info (Read Only)")]
    [SerializeField] private int lastCalculatedDamage;
    [SerializeField] private int lastBaseDamage;
    [SerializeField] private int lastPercentageDamage;

    /// <summary>
    /// Tính toán damage dựa trên: Base Damage + % Player Max HP
    /// </summary>
    public int CalculateDamage(PlayerHealth playerHealth)
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

        // Tổng damage
        int totalDamage = baseDamage + percentageDamage;

        // Cache để debug
        lastBaseDamage = baseDamage;
        lastPercentageDamage = percentageDamage;
        lastCalculatedDamage = totalDamage;

        Debug.Log($"⚔️ Enemy Damage: {totalDamage} (Base: {baseDamage} + {playerHpPercentage * 100}% of {playerMaxHP} = {percentageDamage})");

        return totalDamage;
    }

    /// <summary>
    /// Gây damage cho player (gọi hàm này khi enemy attack hit)
    /// </summary>
    public void DealDamageTo(PlayerHealth playerHealth)
    {
        if (playerHealth == null) return;

        int damage = CalculateDamage(playerHealth);
        playerHealth.TakeDamage(damage);
    }

    /// <summary>
    /// Gây damage cho player thông qua collision
    /// </summary>
    public void DealDamageOnCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
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

    // Setters
    public void SetBaseDamage(int value) => baseDamage = Mathf.Max(0, value);
    public void SetPlayerHpPercentage(float value) => playerHpPercentage = Mathf.Clamp01(value);

    /// <summary>
    /// Hiển thị thông tin damage trong console
    /// </summary>
    public void DisplayDamageInfo(PlayerHealth playerHealth)
    {
        if (playerHealth == null) return;

        int damage = CalculateDamage(playerHealth);

        Debug.Log("========== ENEMY DAMAGE INFO ==========");
        Debug.Log($"Base Damage: {baseDamage}");
        Debug.Log($"HP% Scaling: {playerHpPercentage * 100}%");
        Debug.Log($"Player Max HP: {playerHealth.GetMaxHealth()}");
        Debug.Log($"Percentage Damage: {lastPercentageDamage}");
        Debug.Log($"Total Damage: {damage}");
        Debug.Log("=======================================");
    }
}