using UnityEngine;
using System.Collections;

public class SlimeAtk : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float attackDelay = 0.3f;
    public int damage = 10;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale;
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            player = obj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Lật hướng
    // Lật hướng theo vị trí player, nhưng vẫn giữ nguyên scale từ Inspector
    if (player.position.x < transform.position.x)
        transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
    else
        transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Nếu trong tầm đánh
        if (distance <= attackRange && !isAttacking && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
        }

        // Animation di chuyển (chỉ khi có vận tốc thực tế)
        if (!isAttacking)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            anim.SetBool("isMoving", isMoving);
        }
    }

    IEnumerator Attack()
    {
        // Nếu đã tấn công thì không làm lại
        if (isAttacking) yield break;

        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);
        anim.SetBool("isAttacking", true);

        // Đợi tới frame đánh thật (khớp với animation)
        yield return new WaitForSeconds(attackDelay);

        // Gây damage đúng 1 lần
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                pc.TakeDamage(damage);
        }

        // 🔧 Đợi cho đến khi animation Attack thực sự kết thúc
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        // Reset trạng thái animation và flag
        anim.SetBool("isAttacking", false);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
