using System.Collections;
using UnityEngine;
using Pathfinding;

public class mageskill : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject magicProjectilePrefab;
    public Transform firePoint;

    [Header("Settings")]
    public float detectRange = 15f;
    public float attackCooldown = 2.5f;
    public float castTime = 1.2f;
    public float projectileSpeed = 8f;
    public float minDistance = 7f;
    public float retreatDistance = 7f;
    public float retreatSpeed = 10f;

    private bool canAttack = true;
    private bool isCasting = false;
    private bool isRetreating = false;

    private SpriteRenderer sprite;
    private Vector3 startScale;
    private AIPath aiPath;
    private Seeker seeker;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
        aiPath = GetComponent<AIPath>();
        seeker = GetComponent<Seeker>();

        if (firePoint == null)
            firePoint = transform;
    }

void Update()
{
    if (!player) return;

    float distance = Vector2.Distance(transform.position, player.position);

    // 🟢 Trong tầm phát hiện
    if (distance < detectRange && !isCasting)
    {
        if (aiPath != null)
            aiPath.canMove = true; // 🔥 Cho phép di chuyển khi phát hiện player

        // 🟡 Nếu player quá gần → lùi lại
        if (distance < minDistance * 1.3f && !isRetreating)
        {
            StartCoroutine(RetreatFromPlayer());
        }
        // 🔵 Nếu đủ xa → tấn công
        else if (distance >= minDistance && canAttack)
        {
            StartCoroutine(CastAndShoot());
        }

        // 🔄 Quay mặt về phía player
        Vector3 dir = player.position - transform.position;
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(startScale.x), startScale.y, startScale.z);
    }
    else if (aiPath != null)
    {
        aiPath.canMove = false; // 💤 Ngoài tầm thì đứng yên
    }
}


IEnumerator RetreatFromPlayer()
{
    isRetreating = true;

    if (aiPath != null)
        aiPath.canMove = false; // Tắt AIPath để lùi thủ công

    // 🔥 Lùi cho đến khi đủ xa
    while (Vector2.Distance(transform.position, player.position) < retreatDistance)
    {
        Vector2 dir = (transform.position - player.position).normalized;
        transform.position += (Vector3)(dir * retreatSpeed * Time.deltaTime);
        yield return null;
    }

    if (aiPath != null)
        aiPath.canMove = true; // Bật lại Pathfinding

    isRetreating = false;
}


    IEnumerator CastAndShoot()
    {
        isCasting = true;
        canAttack = false;

        if (aiPath != null)
            aiPath.canMove = false;

        Debug.Log("🔮 Mage bắt đầu niệm phép...");
        yield return StartCoroutine(CastEffect());

        ShootMagic();

        Debug.Log("✨ Mage bắn ra luồng phép!");
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        isCasting = false;

        if (aiPath != null)
            aiPath.canMove = true;
    }

    void ShootMagic()
    {
        if (!magicProjectilePrefab || !firePoint || !player) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject magic = Instantiate(magicProjectilePrefab, firePoint.position, Quaternion.identity);
        magic.transform.right = dir;

        Rigidbody2D rb = magic.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;

        Destroy(magic, 5f);
    }

    IEnumerator CastEffect()
    {
        float time = 0f;
        while (time < castTime)
        {
            float scale = Mathf.Lerp(1f, 1.2f, Mathf.PingPong(time * 2f, 1));
            transform.localScale = startScale * scale;
            sprite.color = new Color(1f, 0.8f + Mathf.Sin(time * 10f) * 0.2f, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = startScale;
        sprite.color = Color.white;
    }
}
