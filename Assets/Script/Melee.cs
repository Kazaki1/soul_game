using UnityEngine;

public class Melee : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 5f;

    private Transform player;
    private PlayerWeaponController weaponController;

    private void Start()
    {
        player = transform.root;

        // Lấy PlayerWeaponController từ root (Player)
        weaponController = player.GetComponent<PlayerWeaponController>();

        if (weaponController == null)
        {
            Debug.LogError("PlayerWeaponController không tìm thấy trên Player!");
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
    /// Tính final damage
    /// Hiện tại chỉ lấy weapon damage, sau này có thể thêm buffs, crits, etc.
    /// </summary>
    private float CalculateFinalDamage()
    {
        if (weaponController == null || !weaponController.HasWeapon())
        {
            Debug.LogWarning("[MELEE] Không có weapon! Damage = 0");
            return 0f;
        }

        float weaponDamage = weaponController.GetWeaponDamage();

        Debug.Log($"[MELEE] Weapon Damage: {weaponDamage:F1}");

        // TODO: Thêm các modifier khác ở đây
        // float critMultiplier = IsCriticalHit() ? 2f : 1f;
        // float buffBonus = GetBuffDamage();
        // float finalDamage = weaponDamage * critMultiplier + buffBonus;

        return weaponDamage;
    }
}