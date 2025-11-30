using UnityEngine;
using System.Collections;

public class SlimeAtk_Bullet : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 6f;
    public float attackRange = 5f;
    public float attackCooldown = 5f;
    public float attackDelay = 1.5f; // thời gian chờ trước khi bắn

    [Header("Prefabs")]
    public GameObject warningIconPrefab; // biểu tượng cảnh báo
    public GameObject bulletPrefab;      // prefab đạn
    public float bulletSpeed = 7f;       // tốc độ đạn

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

        // Lật hướng slime
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        if (!isAttacking && distance <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false);
        anim.SetBool("isAttacking", true);

        // Hiện biểu tượng cảnh báo
        GameObject warning = null;
        if (warningIconPrefab != null)
        {
            Vector3 warnPos = transform.position + Vector3.up * 3.5f; //chỉnh vị trí của warning
            warning = Instantiate(warningIconPrefab, warnPos, Quaternion.identity, transform);
            warning.transform.localScale = Vector3.one;
        }

        // Nhấp nháy biểu tượng
        float blinkTime = 0f;
        SpriteRenderer warnSr = warning?.GetComponent<SpriteRenderer>();
        while (blinkTime < attackDelay)
        {
            if (warnSr != null)
                warnSr.enabled = !warnSr.enabled;
            yield return new WaitForSeconds(0.15f);
            blinkTime += 0.15f;
        }

        if (warning != null)
            Destroy(warning);

        // Bắn viên đạn
        if (player != null && bulletPrefab != null)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            SlimeBulletFire b = bullet.GetComponent<SlimeBulletFire>();
            if (b != null)
                b.Init(dir, 0, bulletSpeed);
        }

        yield return new WaitForSeconds(0.3f);
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
