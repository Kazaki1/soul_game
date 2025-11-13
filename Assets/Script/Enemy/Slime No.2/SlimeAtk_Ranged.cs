using UnityEngine;
using System.Collections;

public class SlimeAtk_Ranged : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 5f;         // Phạm vi phát hiện player
    public float attackRange = 4f;        // Phạm vi tấn công (xa hơn cận chiến)
    public float attackCooldown = 2f;     // Thời gian hồi chiêu giữa 2 đòn bắn
    public float attackDelay = 0.4f;      // Thời điểm khớp frame bắn (frame "khạc")
    public int damage = 10;               // Damage gây ra
    public GameObject bulletPrefab;       // Prefab viên đạn
    public float bulletSpeed = 10f;       // Tốc độ bay của viên đạn

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

        // Lật hướng theo player, vẫn giữ nguyên scale inspector
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Trong tầm bắn
        if (distance <= attackRange && !isAttacking && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
        }

        // Animation di chuyển (chỉ khi thực sự di chuyển)
        if (!isAttacking)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            anim.SetBool("isMoving", isMoving);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        anim.SetBool("isMoving", false);
        anim.SetBool("isAttacking", true);

        // Đợi tới frame bắn (khớp với animation)
        yield return new WaitForSeconds(attackDelay);

        // Bắn đạn nếu player vẫn trong tầm
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            ShootProjectile();
        }

        // Đợi hết animation bắn
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.SetBool("isAttacking", false);
        isAttacking = false;
    }

void ShootProjectile()
{
    if (bulletPrefab == null || player == null) return;

    // Bắn liên tiếp 4 viên đạn
    StartCoroutine(ShootBurst());
}

IEnumerator ShootBurst()
{
    int bulletCount = 4;          // số viên bắn mỗi lần
    float interval = 1f;        // thời gian giữa các viên

    for (int i = 0; i < bulletCount; i++)
    {
        if (player == null) yield break;

        // Tạo viên đạn tại vị trí slime
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Gọi Init để gán player, damage, speed cho viên đạn
        SlimeBullet b = bullet.GetComponent<SlimeBullet>();
        if (b != null)
            b.Init(player, damage, bulletSpeed);

        yield return new WaitForSeconds(interval);
    }
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
