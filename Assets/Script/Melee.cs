using UnityEngine;

public class Melee : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 5f;

    private Transform player;
    private PlayerWeaponController weaponController;
    private ItemController itemController; // Thêm reference đến ItemController

    private void Start()
    {
        player = transform.root;

        // Lấy PlayerWeaponController và ItemController từ root (Player)
        weaponController = player.GetComponent<PlayerWeaponController>();
        itemController = player.GetComponent<ItemController>();

        if (weaponController == null)
        {
            Debug.LogError("PlayerWeaponController không tìm thấy trên Player!");
        }

        if (itemController == null)
        {
            Debug.LogWarning("ItemController không tìm thấy trên Player! Không có buff item.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            EnemyKnockback enemyKB = other.GetComponent<EnemyKnockback>();

            // Lấy damage từ weapon
            float finalDamage = CalculateFinalDamage();

            // Debug log damage
            Debug.Log($"[MELEE] Hit {other.name} for {finalDamage:F1} damage!");

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage((int)finalDamage);
            }

            if (enemyKB != null)
            {
                Vector2 dir = (other.transform.position - player.position).normalized;
                enemyKB.ApplyKnockback(dir, knockbackForce);
            }
        }
    }

    /// <summary>
    /// Tính final damage với buff từ items
    /// </summary>
    private float CalculateFinalDamage()
    {
        if (weaponController == null || !weaponController.HasWeapon())
        {
            Debug.LogWarning("[MELEE] Không có weapon! Damage = 0");
            return 0f;
        }

        float weaponDamage = weaponController.GetWeaponDamage();

        // Áp dụng damage buff từ items
        float buffMultiplier = 1f;
        if (itemController != null)
        {
            float buffPercent = itemController.GetTotalDamageBuffPercent();
            buffMultiplier += buffPercent / 100f;
        }

        float finalDamage = weaponDamage * buffMultiplier;

        Debug.Log($"[MELEE] Weapon Damage: {weaponDamage:F1} | Buff Multiplier: {buffMultiplier:F2} | Final: {finalDamage:F1}");
        return finalDamage;
    }
}