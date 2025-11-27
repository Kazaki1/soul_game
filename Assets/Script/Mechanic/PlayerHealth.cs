using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    int maxHealth;
    int currentHealth;

    [Header("Base Stats")]
    public int baseHealth = 100;

    public HealthBar healthBar;

    private PlayerStats stats;
    private ItemController itemController; // Thêm reference đến ItemController

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        itemController = GetComponent<ItemController>(); // Thêm để lấy buffs

        if (itemController == null)
        {
            Debug.LogWarning("ItemController không tìm thấy! Không có health buff.");
        }
    }

    private void Start()
    {
        if (stats != null)
            UpdateMaxHealthFromVigor(true);
    }

    public void UpdateMaxHealthFromVigor(bool fullHeal = true)
    {
        if (stats == null)
        {
            return;
        }

        int baseMaxHealth = CalculateHealthFromVigor(stats.vigor);

        // Áp dụng health buff từ items
        float buffMultiplier = 1f;
        if (itemController != null)
        {
            float buffPercent = itemController.GetTotalHealthBuffPercent();
            buffMultiplier += buffPercent / 100f;
        }

        maxHealth = Mathf.RoundToInt(baseMaxHealth * buffMultiplier);

        if (fullHeal)
            currentHealth = maxHealth;
        else
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log($"Updated Max Health: {maxHealth} (Base: {baseMaxHealth} | Buff Multiplier: {buffMultiplier:F2})");
    }

    public int CalculateHealthFromVigor(int vigor)
    {
        int hp = baseHealth;

        for (int lv = 1; lv <= vigor; lv++)
        {
            int gain;

            if (lv <= 40)
            {
                float t = (lv - 1) / 39f;
                gain = Mathf.RoundToInt(Mathf.Lerp(48, 4, t));  // +48 → +4
            }
            else if (lv <= 60)
            {
                float t = (lv - 41) / 19f;
                gain = Mathf.RoundToInt(Mathf.Lerp(26, 13, t)); // +26 → +13
            }
            else
            {
                float t = (lv - 61) / 38f;
                gain = Mathf.RoundToInt(Mathf.Lerp(6, 3, t));   // +6 → +3
            }

            hp += gain;
        }

        return hp;
    }

    public void TakeDamage(int damage)
    {
        int previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.SetHealth(currentHealth);

        // Debug info
        Debug.Log($"💔 Player took {damage} damage! ({previousHealth} → {currentHealth}/{maxHealth})");

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.SetHealth(currentHealth);

        Debug.Log($"💚 Player healed {amount} HP! ({previousHealth} → {currentHealth}/{maxHealth})");
    }

    private void Die()
    {
        Debug.Log("💀 Player died!");
        // TODO: Implement death logic
    }

    // Getters (để EnemyDamageDeal có thể lấy max HP)
    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}