using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlimeAtk_Orb : MonoBehaviour
{
    [Header("Attack Settings")]
    public float chaseRange = 5f;           
    public float attackRange = 3f;          
    public float orbRadius = 2f;            
    public int orbCount = 6;                
    public float rotateSpeed = 100f;        
    public float respawnDelay = 5f;                        
    public GameObject orbPrefab;            

    [Header("Orb Behavior")]
    public float orbFlyOutTime = 0.6f;      // thời gian orb bay ra từ slime
    public float orbVisibleDuration = 5f;   // thời gian orb tồn tại trước khi ẩn đi

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private Transform orbParent;            
    private List<GameObject> activeOrbs = new List<GameObject>();
    private bool isRespawning = false;
    private bool isIdle = true;             // xác định trạng thái idle

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
            player = obj.transform;

        orbParent = new GameObject("OrbParent").transform;
        orbParent.position = transform.position;
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

        // Idle nếu không di chuyển và không tấn công
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        anim.SetBool("isMoving", isMoving);
        isIdle = !isMoving && distance > attackRange;

        // Nếu idle → ẩn orb
        if (isIdle && activeOrbs.Count > 0)
        {
            HideAllOrbs();
        }
        // Nếu không idle → đảm bảo orb tồn tại
        else if (!isIdle && activeOrbs.Count == 0 && !isRespawning)
        {
            StartCoroutine(SpawnAndCycleOrbs());
        }

        // Quay orb quanh slime
        RotateOrbs();
    }

    IEnumerator SpawnAndCycleOrbs()
    {
        isRespawning = true;
        yield return StartCoroutine(SpawnOrbsSmooth()); // bay ra từ slime
        yield return new WaitForSeconds(orbVisibleDuration);

        // ẩn orb đi
        HideAllOrbs();
        yield return new WaitForSeconds(respawnDelay);

        isRespawning = false;
    }

IEnumerator SpawnOrbsSmooth()
{
    if (orbPrefab == null) yield break;

    activeOrbs.Clear();

    foreach (Transform child in orbParent)
        Destroy(child.gameObject);

    // 🔥 Đảm bảo orbParent không bị xoay khi spawn
    orbParent.position = transform.position;
    orbParent.rotation = Quaternion.identity;

    for (int i = 0; i < orbCount; i++)
    {
        GameObject orb = Instantiate(orbPrefab, transform.position, Quaternion.identity, orbParent);
        SlimeOrb slimeOrb = orb.GetComponent<SlimeOrb>();
        if (slimeOrb != null)
            slimeOrb.Init(this, 0);
        activeOrbs.Add(orb);

        // Vị trí chuẩn quanh slime (đối xứng 360°)
        float angle = i * Mathf.PI * 2f / orbCount;
        Vector3 targetPos = orbParent.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * orbRadius;

        StartCoroutine(MoveOrbOutward(orb.transform, targetPos));
    }
}


    IEnumerator MoveOrbOutward(Transform orb, Vector3 targetPos)
    {
        float t = 0;
        Vector3 startPos = transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime / orbFlyOutTime;
            if (orb != null)
                orb.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    void HideAllOrbs()
    {
        foreach (GameObject orb in activeOrbs)
        {
            if (orb != null)
                Destroy(orb);
        }
        activeOrbs.Clear();
    }

void RotateOrbs()
{
    if (orbParent == null) return;

    // pivot luôn trùng tâm slime
    orbParent.position = transform.position;

        // reset scale âm do flip
    orbParent.localScale = Vector3.one; 


    orbParent.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

    foreach (Transform orb in orbParent)
    {
        if (orb != null)
            orb.rotation = Quaternion.identity;
    }
}

    public void NotifyOrbDestroyed(GameObject orb)
    {
        if (activeOrbs.Contains(orb))
            activeOrbs.Remove(orb);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, orbRadius);
    }
}
