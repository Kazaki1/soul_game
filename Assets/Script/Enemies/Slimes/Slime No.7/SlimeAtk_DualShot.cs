using UnityEngine;
using System.Collections;

public class SlimeAtk_DualShot : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 6f;
    public float attackRange = 4f;
    public float attackCooldown = 3f;
    public float attackDelay = 0.4f;
    public float bulletSpeed = 8f;
    public GameObject bulletPrefab;

    [Header("Slow Effect Settings")]
    public float slowDuration = 2f;
    public float slowMultiplier = 0.5f;  // 50% speed during slow

    [Header("Effect Prefabs")]
    public GameObject sakuraEffectPrefab;     // 🌸 hiệu ứng hoa anh đào xoay trên đầu quái
    public Transform effectPoint;             // vị trí hiển thị (thường là empty object trên đầu quái)

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    private GameObject currentSakuraEffect;   // để xoá sau khi tấn công

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

        // Lật hướng theo player
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Khi player vào tầm
        if (distance <= attackRange && !isAttacking && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
            if (currentSakuraEffect != null)
            {
                currentSakuraEffect.transform.position = effectPoint.position;
                Vector3 iconOriginalScale = sakuraEffectPrefab.transform.localScale;
                currentSakuraEffect.transform.localScale = iconOriginalScale;
            }
        }

        // Animation di chuyển
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

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);
        anim.SetBool("isAttacking", true);

        // 🌸 Tạo hiệu ứng hoa anh đào xoay trên đầu
        if (sakuraEffectPrefab != null && effectPoint != null)
        {
            currentSakuraEffect = Instantiate(sakuraEffectPrefab, effectPoint.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(attackDelay);

        if (player != null)
        {
            ShootDualProjectiles();
        }

        yield return new WaitForSeconds(0.3f);

        // Xoá hiệu ứng hoa anh đào sau khi bắn xong
        if (currentSakuraEffect != null)
            Destroy(currentSakuraEffect);

        anim.SetBool("isAttacking", false);
        isAttacking = false;
    }

    void ShootDualProjectiles()
    {
        if (bulletPrefab == null || player == null) return;

        Vector2 baseDir = (player.position - transform.position).normalized;
        float angleOffset = 45f; // lệch 45 độ
        ShootOneBullet(baseDir, 0f); // viên giữa
        ShootOneBullet(baseDir, -angleOffset);
        ShootOneBullet(baseDir, angleOffset);
    }

    void ShootOneBullet(Vector2 baseDir, float angleOffset)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        float angle = baseAngle + angleOffset;
        Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
            rbBullet.linearVelocity = dir * bulletSpeed;

        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Gán slow parameters cho viên đạn
        SlimeDualBullet sb = bullet.GetComponent<SlimeDualBullet>();
        if (sb != null)
        {
            sb.slowDuration = slowDuration;
            sb.slowMultiplier = slowMultiplier;
        }

        Destroy(bullet, 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}