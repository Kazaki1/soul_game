using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleMask;
    public float detectRange = 8f;
    public float attackRange = 2f;
    public float attackDelay = 1.2f;
    public float moveSpeed = 3f;

    private float timeInAttackRange = 0f;
    private bool isAttacking = false;
    private int currentMoveSet = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Kiểm tra LOS (Line of Sight)
        bool hasLOS = !Physics2D.Linecast(transform.position, player.position, obstacleMask);

        if (dist <= detectRange)
        {
            if (hasLOS)
            {
                if (dist > attackRange)
                {
                    MoveToward(player.position);
                    timeInAttackRange = 0f;
                }
                else
                {
                    timeInAttackRange += Time.deltaTime;

                    if (!isAttacking && timeInAttackRange > 0.5f) // đủ lâu trong vùng attack
                    {
                        StartCoroutine(PerformAttack());
                    }
                }
            }
            else
            {
                // Nếu không có LOS thì di chuyển ngẫu nhiên để tìm đường khác
                Vector2 offset = Random.insideUnitCircle.normalized * 2f;
                MoveToward((Vector2)player.position + offset);
            }
        }
    }

    void MoveToward(Vector2 target)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }

    System.Collections.IEnumerator PerformAttack()
    {
        isAttacking = true;
        currentMoveSet = Random.Range(0, 3); // 3 kiểu tấn công

        switch (currentMoveSet)
        {
            case 0:
                yield return StartCoroutine(SlashAttack());
                break;
            case 1:
                yield return StartCoroutine(DashAttack());
                break;
            case 2:
                yield return StartCoroutine(CircleAndStrike());
                break;
        }

        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }

    System.Collections.IEnumerator SlashAttack()
    {
        // Slash cơ bản
        Debug.Log("Enemy Slash!");
        yield return new WaitForSeconds(0.3f);
    }

    System.Collections.IEnumerator DashAttack()
    {
        Debug.Log("Enemy Dash Attack!");
        Vector2 dir = (player.position - transform.position).normalized;
        float dashSpeed = moveSpeed * 5f;
        float dashTime = 0.2f;

        float t = 0;
        while (t < dashTime)
        {
            rb.MovePosition(rb.position + dir * dashSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }

    System.Collections.IEnumerator CircleAndStrike()
    {
        Debug.Log("Enemy Circle & Strike!");
        float circleTime = 1f;
        float radius = 1.5f;
        float timer = 0;

        while (timer < circleTime)
        {
            Vector2 toPlayer = player.position - transform.position;
            Vector2 perp = Vector2.Perpendicular(toPlayer).normalized;
            Vector2 circlePos = (Vector2)player.position + perp * radius;

            MoveToward(circlePos);
            timer += Time.deltaTime;
            yield return null;
        }

        // Sau khi vòng quanh xong thì chém
        yield return StartCoroutine(SlashAttack());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }


}
