using UnityEngine;
using System.Collections;

public class Slime_Ice_attack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 6f;         // Phạm vi phát hiện player
    public float attackRange = 5f;        // Phạm vi tấn công
    public float attackCooldown = 3f;     // Thời gian giữa các lần tấn công
    public float attackDelay = 0.8f;      // Delay khớp animation (thời điểm tạo cột băng)
    public int damage = 15;               // Sát thương gây ra

    [Header("Ice Pillar Settings")]
    public GameObject icePillarPrefab;    // Prefab cột băng (animation 3 frame)
    public float pillarDuration = 5f;     // Thời gian tồn tại của cột băng

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

        // Lật hướng theo player (giữ nguyên tỉ lệ inspector)
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Nếu player trong tầm tấn công
        if (distance <= attackRange && !isAttacking && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
        }

        // Cập nhật animation di chuyển
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

        // Đợi tới frame phù hợp trong animation
        yield return new WaitForSeconds(attackDelay);

        // Triệu hồi cột băng tại vị trí player
        if (player != null && icePillarPrefab != null)
        {
            Vector3 spawnPos = player.position;
            GameObject pillar = Instantiate(icePillarPrefab, spawnPos, Quaternion.identity);

            // Gửi thông tin player vào cột băng
            IcePillar ice = pillar.GetComponent<IcePillar>();
            if (ice != null)
                ice.Init( damage, pillarDuration);

            Destroy(pillar, pillarDuration);
        }
        

        // Chờ animation kết thúc
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
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
