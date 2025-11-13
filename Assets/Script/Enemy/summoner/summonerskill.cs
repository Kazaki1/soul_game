using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class summonerskill : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject minionPrefab;
    public Transform summonPointLeft;
    public Transform summonPointRight;

    [Header("Settings")]
    public float detectRange = 15f;
    public float attackCooldown = 5f;
    public float summonDelay = 1.5f;
    public float minDistance = 7f;
    public float retreatDistance = 8f;
    public float retreatSpeed = 10f;
    public int maxMinions = 3; // 🧠 Giới hạn số minion tối đa

    private bool canSummon = true;
    private bool isSummoning = false;
    private bool isRetreating = false;

    private SpriteRenderer sprite;
    private Vector3 startScale;
    private AIPath aiPath;

    // 🧩 Danh sách lưu minion đang tồn tại
    private List<GameObject> activeMinions = new List<GameObject>();

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startScale = transform.localScale;
        aiPath = GetComponent<AIPath>();

        if (summonPointLeft == null) summonPointLeft = transform;
        if (summonPointRight == null) summonPointRight = transform;
    }

    void Update()
    {
        if (!player) return;

        // 🧹 Xóa minion đã chết khỏi danh sách
        activeMinions.RemoveAll(m => m == null);

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectRange && !isSummoning)
        {
            if (aiPath != null)
                aiPath.canMove = true;

            if (distance < minDistance * 1.2f && !isRetreating)
                StartCoroutine(RetreatFromPlayer());
            else if (distance >= minDistance && canSummon && activeMinions.Count < maxMinions)
                StartCoroutine(SummonMinions());

            Vector3 dir = player.position - transform.position;
            if (dir.x != 0)
                transform.localScale = new Vector3(Mathf.Sign(dir.x) * Mathf.Abs(startScale.x), startScale.y, startScale.z);
        }
        else if (aiPath != null)
        {
            aiPath.canMove = false;
        }
    }

    IEnumerator RetreatFromPlayer()
    {
        isRetreating = true;
        if (aiPath != null) aiPath.canMove = false;

        while (Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            Vector2 dir = (transform.position - player.position).normalized;
            transform.position += (Vector3)(dir * retreatSpeed * Time.deltaTime);
            yield return null;
        }

        if (aiPath != null) aiPath.canMove = true;
        isRetreating = false;
    }

    IEnumerator SummonMinions()
    {
        isSummoning = true;
        canSummon = false;

        if (aiPath != null)
            aiPath.canMove = false;

        Debug.Log("🔮 Mage bắt đầu triệu hồi minion...");
        yield return new WaitForSeconds(summonDelay);

        // 🧟‍♂️ Tạo 2 minion mỗi lần
        SpawnMinion(summonPointLeft.position);
        SpawnMinion(summonPointRight.position);

        Debug.Log($"✨ Mage triệu hồi minion! (Tổng: {activeMinions.Count})");

        yield return new WaitForSeconds(attackCooldown);
        canSummon = true;
        isSummoning = false;

        if (aiPath != null)
            aiPath.canMove = true;
    }

    void SpawnMinion(Vector3 position)
    {
        if (minionPrefab == null) return;
        if (activeMinions.Count >= maxMinions) return; // 🚫 Giới hạn số lượng

        GameObject minion = Instantiate(minionPrefab, position, Quaternion.identity);
        activeMinions.Add(minion);
    }
}
