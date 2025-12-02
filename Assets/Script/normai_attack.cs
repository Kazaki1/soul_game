using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyAnimation))]
public class NormalAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.3f;
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;

    [Header("Layer Settings")]
    public LayerMask playerLayer;

    private float nextAttackTime = 0f;
    private EnemyAnimation anim;
    private bool isAttacking;

    void Start()
    {
        anim = GetComponent<EnemyAnimation>();
    }

    // TryAttack() mặc định dùng attackRange nội bộ
    public void TryAttack()
    {
        TryAttack(attackRange);
    }

    // Cho phép BaseEnemy truyền vào range (để đồng bộ với attackRange của AI)
    public void TryAttack(float range)
    {
        if (Time.time < nextAttackTime || isAttacking) return;

        // Tìm player trong range
        Collider2D hitCheck = Physics2D.OverlapCircle(transform.position, range, playerLayer);
        if (hitCheck == null) return;

        PlayerHealth playerHealth = hitCheck.GetComponent<PlayerHealth>();
        EnemyDamageDeal damage = GetComponent<EnemyDamageDeal>();

        if (playerHealth != null && damage != null)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(PerformAttackAfterDelay(0.25f, playerHealth, damage));
        }
    }


    private IEnumerator PerformAttackAfterDelay(float delay, PlayerHealth playerHealth, EnemyDamageDeal damage)
    {
        isAttacking = true;

        yield return new WaitForSeconds(delay);

        // Sau delay, gây damage cho player
        if (playerHealth != null && damage != null)
        {
            damage.DealDamageTo(playerHealth);
        }

        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
