using UnityEngine;
using System.Collections;

public class SlimeAtk_Lightning : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 6f;
    public float attackRange = 5f;
    public float attackCooldown = 5f;
    public float attackDelay = 1.5f;   // Thời gian hiển thị cảnh báo

    [Header("Prefabs & Visuals")]
    public GameObject warningIconPrefab;   // Biểu tượng cảnh báo
    public GameObject lightningPrefab;     // Prefab tia sét
    public float lightningInterval = 0.5f; // Thời gian giữa các tia sét
    public int lightningCount = 3;         // Số tia sét liên tiếp
    public float lightningSpawnHeight = 8f; // Vị trí spawn trên đầu player
    public float lightningExistTime = 0.8f; // Tồn tại bao lâu trước khi biến mất

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

        // Lật hướng theo player
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Trong tầm tấn công
        if (!isAttacking && distance <= attackRange && Time.time >= nextAttackTime)
            StartCoroutine(AttackRoutine());

        // Animation di chuyển
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        anim.SetBool("isMoving", isMoving);
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
            Vector3 warnPos = transform.position + Vector3.up * 3f;
            Vector3 iconOriginalScale = warningIconPrefab.transform.localScale;
            warning = Instantiate(warningIconPrefab, warnPos, Quaternion.identity, transform);
            warning.transform.localScale = iconOriginalScale;
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

        // Gọi 3 tia sét liên tiếp
        for (int i = 0; i < lightningCount; i++)
        {
            if (player != null)
            {
                Vector3 targetPos = player.position;
                Vector3 spawnPos = targetPos + Vector3.up * lightningSpawnHeight;

                GameObject bolt = Instantiate(lightningPrefab, spawnPos, Quaternion.identity);
                SlimeLightning lightning = bolt.GetComponent<SlimeLightning>();
                if (lightning != null)
                    lightning.Init(targetPos, 0, lightningExistTime);
            }

            yield return new WaitForSeconds(lightningInterval);
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
