using UnityEngine;

public class Melee : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 5f;

    private Transform player;
    private PlayerWeaponController weaponController;
    private ItemController itemController; 

    private void Start()
    {
        player = transform.root;

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

            float finalDamage = CalculateFinalDamage();

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

    private float CalculateFinalDamage()
    {
        if (weaponController == null || !weaponController.HasWeapon())
        {
            Debug.LogWarning("[MELEE] Không có weapon! Damage = 0");
            return 0f;
        }

        float weaponDamage = weaponController.GetWeaponDamage();

        float buffMultiplier = 1f;
        if (itemController != null)
        {
            float buffPercent = itemController.GetTotalDamageBuffPercent();
            buffMultiplier += buffPercent / 100f;
        }

        float finalDamage = weaponDamage * buffMultiplier;
        return finalDamage;
    }
}