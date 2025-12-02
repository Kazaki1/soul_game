using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyAnimation))]
[RequireComponent(typeof(EnemyRangedAttack))]
public class MediumEnemy : MonoBehaviour
{
    [Header("Target & Detection")]
    public Transform player;
    public float detectionRange = 10f;
    public float closeAttackRange = 1.5f;
    

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
       [Header("Attack Settings")]
    public int meleeDamage = 15
        ;
    public float meleeCooldown = 1.5f;
    public float meleeAnimDuration = 0.6f;

    [Header("References")]
    public Transform firePoint;
    private Rigidbody2D rb;
    private EnemyAnimation anim;
    private EnemyRangedAttack rangedAttack;
    private EnemyDefense defense;
    private bool canMelee = true;
    private float nextRangedAttackTime = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<EnemyAnimation>();
        rangedAttack = GetComponent<EnemyRangedAttack>();
        defense = GetComponent<EnemyDefense>();
      
        anim.enemyType = EnemyAnimation.EnemyType.Medium;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }



        if (rangedAttack != null && rangedAttack.firePoint == null)
            rangedAttack.firePoint = firePoint;

        anim.SetIdle();
    }
    private void HandleFlipTowardsPlayer()
    {
        if (player == null) return;

        float vx = rb.linearVelocity.x;

        // 1️⃣ Nếu đang di chuyển → flip theo velocity
        if (Mathf.Abs(vx) > 0.05f)
        {
            bool shouldFaceRight = vx > 0f;
            anim.GetComponent<SpriteRenderer>().flipX = anim.spriteDefaultFacingLeft ? shouldFaceRight : !shouldFaceRight;
            return;
        }

        // 2️⃣ Nếu đứng yên → flip theo hướng player
        bool faceRight = player.position.x > transform.position.x;
        anim.GetComponent<SpriteRenderer>().flipX = anim.spriteDefaultFacingLeft ? faceRight : !faceRight;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // ---------- 1️⃣ Flip hướng theo player ----------
        HandleFlipTowardsPlayer();

        // ---------- 2️⃣ Cận chiến ----------
        if (distance <= closeAttackRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetSlashing();
            if (canMelee) StartCoroutine(MeleeAttack());
            return;
        }

        // ---------- 3️⃣ Bật khiên ----------
        if (defense != null && distance <= defense.blockRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBlocking();
            if (!defense.IsBlocking())
            {
                defense.StartBlock();
                defense.NextBlockTime = Time.time + defense.blockCooldown;
                StartCoroutine(StopBlockAfter(defense));
            }
            return;
        }

        // ---------- 4️⃣ Bắn tầm xa ----------
        if (rangedAttack != null && distance <= rangedAttack.attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetShooting();
            if (Time.time >= nextRangedAttackTime)
                StartCoroutine(RangedAttackRoutine());
            return;
        }

        // ---------- 5️⃣ Di chuyển theo player ----------
        if (distance <= detectionRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * moveSpeed;

            // Set walking animation
            anim.SetWalking();

            // Không reset flipX → HandleFlip() sẽ tự quản lý
            return;
        }

        // ---------- 6️⃣ Nếu ra ngoài detectionRange → đứng yên ----------
        rb.linearVelocity = Vector2.zero;
        anim.SetIdle();
    }


    // ✅ THÊM Coroutine này
    private IEnumerator StopVelocityNextFrame()
    {
        yield return null;  // Đợi 1 frame
        rb.linearVelocity = Vector2.zero;
    }

    private IEnumerator StopBlockAfter(EnemyDefense defense)
    {
        yield return new WaitForSeconds(defense.blockDuration);
        defense.StopBlock();
    }

    private IEnumerator MeleeAttack()
    {
        canMelee = false;

        SpecialMove special = GetComponent<SpecialMove>();

        bool isSpecial = false;
        float usedRange = closeAttackRange;
        int usedDamage = meleeDamage;

        // Kiểm tra special attack
        if (special != null && special.IsSpecialAttack())
        {
            isSpecial = true;
            usedRange *= special.specialRangeMultiplier;
            usedDamage = Mathf.RoundToInt(meleeDamage * special.specialDamageMultiplier);

            anim.SetSpecial();   // Blend Tree 0.8333
        }
        else
        {
            anim.SetSlashing();  // Blend Tree 0.5
        }

        yield return new WaitForSeconds(0.2f);

        // Check player trong tầm
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            usedRange,
            LayerMask.GetMask("Player")
        );

        if (hit != null)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            EnemyDamageDeal dmg = GetComponent<EnemyDamageDeal>();

            if (playerHealth != null)
            {
                if (isSpecial)
                {
                    // Special Attack: tự gây damage ×5
                    playerHealth.TakeDamage(usedDamage);
                    Debug.Log($"🔥 SPECIAL HIT! Damage = {usedDamage}");
                }
                else
                {
                    // Đánh thường → dùng hệ thống cũ
                    if (dmg != null)
                        dmg.DealDamageTo(playerHealth);

                    if (special != null)
                        special.RegisterNormalHit();  // Tăng counter
                }
            }
        }

        yield return new WaitForSeconds(meleeAnimDuration);
        yield return new WaitForSeconds(meleeCooldown);

        canMelee = true;
    }
    private IEnumerator RangedAttackRoutine()
{
    nextRangedAttackTime = Time.time + rangedAttack.attackCooldown;

    yield return new WaitForSeconds(0.2f); // delay khớp animation

    rangedAttack.AttackPlayer();           // gọi đúng logic bắn + cooldown
}


    public void FireProjectile()
    {
        if (rangedAttack != null)
            rangedAttack.SendMessage("AttackPlayer", SendMessageOptions.DontRequireReceiver);
    }
}
