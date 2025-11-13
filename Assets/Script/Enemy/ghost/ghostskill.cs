using System.Collections;
using UnityEngine;
using Pathfinding; // 💡 Quan trọng: để dùng AIPath

public class ghostskill : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private SpriteRenderer sprite;
    private AIPath aiPath;
    private Collider2D col;

    [Header("Settings")]
    public LayerMask obstacleMask;
    public float detectRange = 6f;
    public float attackSpeed = 6f;
    public float attackRange = 3f;
    public float slashSpeed = 10f;
    public float slashDistance = 6f;
    public float teleportCooldown = 3f;
    public float fadeTime = 0.5f;

    // 🆕 Scream Settings
    [Header("Scream / Fear Effect")]
    public float screamRange = 4f;
    public float screamCooldown = 6f;
    public float fearDuration = 2f;
    private bool canScream = true;

    private bool isAttacking = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<AIPath>();
        col = GetComponent<Collider2D>();
        sprite.enabled = false;
    }

    void Update()
    {
        if (!player || isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 🧠 Nếu player nằm trong phạm vi hét (nhưng chưa quá gần)
        if (distance <= screamRange && distance > 1.5f && canScream)
        {
            StartCoroutine(ScreamFear());
            return;
        }

        // 🗡 Nếu player trong phạm vi phát hiện (dùng tấn công khác)
        if (distance < detectRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            float dist = distance;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);

            if (hit.collider == null)
            {
                StartCoroutine(TeleportAttack());
            }
            else
            {
                StartCoroutine(PhantomSlash());
            }
        }
    }


    IEnumerator TeleportAttack()
    {
        isAttacking = true;

        // Tạm tắt di chuyển tự động
        if (aiPath) aiPath.canMove = false;

        Vector2 teleportPos = player.position + (Vector3)(Random.insideUnitCircle.normalized * attackRange);
        transform.position = teleportPos;

        yield return StartCoroutine(FadeIn());

        Vector2 dir = (player.position - transform.position).normalized;
        float elapsed = 0f, attackTime = 0.4f;

        while (elapsed < attackTime)
        {
            transform.position += (Vector3)(dir * attackSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Ghost dùng Teleport Attack trúng player!");

        yield return StartCoroutine(FadeOut());

        if (aiPath) aiPath.canMove = true;
        yield return new WaitForSeconds(teleportCooldown);
        isAttacking = false;
    }

    IEnumerator PhantomSlash()
    {
        isAttacking = true;

        // 🔒 Tắt pathfinding và collider để xuyên tường
        if (aiPath) aiPath.canMove = false;
        if (col) col.enabled = false;

        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        Vector2 teleportPos = (Vector2)player.position - dirToPlayer * 3f;
        transform.position = teleportPos;

        yield return StartCoroutine(FadeIn());

        // Di chuyển xuyên qua
        Vector2 dir = (player.position - transform.position).normalized;
        float moved = 0f;
        while (moved < slashDistance)
        {
            transform.position += (Vector3)(dir * slashSpeed * Time.deltaTime);
            moved += slashSpeed * Time.deltaTime;
            yield return null;
        }

        if (Vector2.Distance(transform.position, player.position) < 1.5f)
            Debug.Log("Ghost dùng Phantom Slash xuyên qua player!");

        yield return StartCoroutine(FadeOut());

        // ✅ Bật lại pathfinding + collider
        if (col) col.enabled = true;
        if (aiPath) aiPath.canMove = true;

        yield return new WaitForSeconds(teleportCooldown);
        isAttacking = false;
    }

    IEnumerator ScreamFear()
    {
        canScream = false;
        isAttacking = true;
        if (aiPath) aiPath.canMove = false;

        Debug.Log("Ghost hét lên! Gây hiệu ứng Fear!");

        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(0.3f);

        // 🌀 Giả lập Fear effect — player tạm bị "sợ" (chỉ log)
        Debug.Log($"Player bị Fear trong {fearDuration} giây!");

        yield return new WaitForSeconds(fearDuration);
        yield return StartCoroutine(FadeOut());

        if (aiPath) aiPath.canMove = true;
        isAttacking = false;

        // Hồi chiêu
        yield return new WaitForSeconds(screamCooldown);
        canScream = true;
    }

    IEnumerator FadeIn()
    {
        sprite.enabled = true;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float a = Mathf.Lerp(0, 1, t / fadeTime);
            sprite.color = new Color(1, 1, 1, a);
            yield return null;
        }
        sprite.color = new Color(1, 1, 1, 1);
    }

    IEnumerator FadeOut()
    {
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float a = Mathf.Lerp(1, 0, t / fadeTime);
            sprite.color = new Color(1, 1, 1, a);
            yield return null;
        }
        sprite.enabled = false;
    }
}
