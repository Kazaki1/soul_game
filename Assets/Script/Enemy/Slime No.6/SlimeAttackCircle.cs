using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlimeAttackCircle : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 6f;         // Phạm vi phát hiện player
    public float attackRange = 4f;        // Trong tầm này thì bắn
    public float attackCooldown = 4f;     // Thời gian giữa các lần tấn công
    public int orbCount = 12;             // Số lượng viên đạn mỗi vòng
    public float orbSpeed = 4f;           // Tốc độ bay của viên đạn
    public float orbLifetime = 3f;        // Thời gian tồn tại của đạn
    public int damage = 8;                // Sát thương mỗi viên
    public GameObject orbPrefab;          // Prefab của viên đạn

    [Header("Pattern Settings")]
    public float delayBetweenWaves = 0.8f; // Thời gian giữa 2 đợt bắn

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

        // Lật hướng slime (chỉ để animation đúng hướng)
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Nếu player trong tầm và chưa bắn
        if (distance <= attackRange && !isAttacking && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackPattern());
        }

        // Animation di chuyển
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        anim.SetBool("isMoving", isMoving);
    }

    IEnumerator AttackPattern()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);
        anim.SetBool("isAttacking", true);

        // Bắn 2 đợt liên tiếp
        yield return StartCoroutine(ShootCircle());
        yield return new WaitForSeconds(delayBetweenWaves);
        yield return StartCoroutine(ShootCircle());

        // Đợi animation kết thúc
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("isAttacking", false);
        isAttacking = false;
    }

    IEnumerator ShootCircle()
    {
        if (orbPrefab == null) yield break;

        Vector3 spawnCenter = transform.position;

        for (int i = 0; i < orbCount; i++)
        {
            float angle = i * Mathf.PI * 2f / orbCount;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject orb = Instantiate(orbPrefab, spawnCenter, Quaternion.identity);
            Rigidbody2D rbOrb = orb.GetComponent<Rigidbody2D>();

            if (rbOrb != null)
            {
                rbOrb.linearVelocity = dir * orbSpeed;
            }

            // Xoay sprite đạn theo hướng bay
            orb.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            // Nếu có script gây damage
            SlimeCircleBullet bullet = orb.GetComponent<SlimeCircleBullet>();
            if (bullet != null)
                bullet.damage = damage;

            Destroy(orb, orbLifetime);
        }

        yield return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
