using UnityEngine;

public class EnemyAttack1 : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 3f;
    public float attackCooldown = 2f;
    public float jumpForce = 6f;

    private Rigidbody2D rb;
    private Transform player;
    private float nextAttackTime = 0f;
    private EnemyDamageDeal damageDeal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        damageDeal = GetComponent<EnemyDamageDeal>();

        if (damageDeal == null)
        {
            Debug.LogError($"❌ EnemyDamageDeal component not found on {gameObject.name}! Please add it.");
        }

        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            player = obj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            JumpAttack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void JumpAttack()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sử dụng EnemyDamageDeal để gây damage
        if (damageDeal != null)
        {
            damageDeal.DealDamageOnCollision(collision);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}