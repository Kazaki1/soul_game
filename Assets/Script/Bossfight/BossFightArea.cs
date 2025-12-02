using UnityEngine;
using System.Collections.Generic;

public class BossFightArea : MonoBehaviour
{
    [Header("Boss References")]
    [SerializeField] private List<GameObject> bossesToDefeat = new List<GameObject>();

    [Header("Arena Barriers")]
    [SerializeField] private List<GameObject> barriers = new List<GameObject>();

    [Header("Arena Settings")]
    [SerializeField] private bool activateOnEnter = true;
    [SerializeField] private bool deactivateOnComplete = true;

    [Header("Events (Optional)")]
    [SerializeField] private bool showDebugLogs = true;

    private bool fightStarted = false;
    private bool fightCompleted = false;
    private int bossesRemaining = 0;

    private void Start()
    {
        // Disable barriers ban đầu
        if (!activateOnEnter)
        {
            SetBarriersActive(false);
        }

        bossesRemaining = bossesToDefeat.Count;
    }

    private void Update()
    {
        // Kiểm tra boss còn sống không
        if (fightStarted && !fightCompleted)
        {
            CheckBossesStatus();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !fightStarted)
        {
            StartBossFight();
        }
    }

    /// <summary>
    /// Bắt đầu boss fight
    /// </summary>
    private void StartBossFight()
    {
        fightStarted = true;

        // Kích hoạt barriers
        SetBarriersActive(true);

        if (showDebugLogs)
        {
            Debug.Log($"🔥 Boss Fight Started! Bosses to defeat: {bossesRemaining}");
        }

        // TODO: Có thể thêm boss music, camera shake, etc.
    }

    /// <summary>
    /// Kiểm tra trạng thái các boss
    /// </summary>
    private void CheckBossesStatus()
    {
        int deadBosses = 0;

        foreach (GameObject boss in bossesToDefeat)
        {
            if (boss == null || !boss.activeInHierarchy)
            {
                deadBosses++;
            }
        }

        // Nếu số boss chết thay đổi
        if (deadBosses != (bossesToDefeat.Count - bossesRemaining))
        {
            bossesRemaining = bossesToDefeat.Count - deadBosses;

            if (showDebugLogs)
            {
                Debug.Log($"⚔️ Boss defeated! Remaining: {bossesRemaining}");
            }
        }

        // Nếu tất cả boss đã chết
        if (bossesRemaining <= 0 && !fightCompleted)
        {
            CompleteBossFight();
        }
    }

    /// <summary>
    /// Hoàn thành boss fight
    /// </summary>
    private void CompleteBossFight()
    {
        fightCompleted = true;

        if (showDebugLogs)
        {
            Debug.Log("🎉 Boss Fight Completed! Arena unlocked.");
        }

        // Tắt barriers
        if (deactivateOnComplete)
        {
            SetBarriersActive(false);
        }

        // TODO: Có thể thêm victory music, rewards, cutscene, etc.
    }

    /// <summary>
    /// Bật/tắt barriers
    /// </summary>
    private void SetBarriersActive(bool active)
    {
        foreach (GameObject barrier in barriers)
        {
            if (barrier != null)
            {
                barrier.SetActive(active);
            }
        }
    }

    /// <summary>
    /// Thêm boss vào danh sách (runtime)
    /// </summary>
    public void AddBoss(GameObject boss)
    {
        if (!bossesToDefeat.Contains(boss))
        {
            bossesToDefeat.Add(boss);
            bossesRemaining++;
        }
    }

    /// <summary>
    /// Bỏ qua boss fight (debug/cheat)
    /// </summary>
    public void SkipBossFight()
    {
        if (fightStarted && !fightCompleted)
        {
            Debug.Log("⚠️ Boss Fight Skipped!");
            CompleteBossFight();
        }
    }

    /// <summary>
    /// Reset arena (để test lại)
    /// </summary>
    public void ResetArena()
    {
        fightStarted = false;
        fightCompleted = false;
        bossesRemaining = bossesToDefeat.Count;
        SetBarriersActive(false);

        Debug.Log("🔄 Arena Reset");
    }

    // Getters
    public bool IsFightStarted() => fightStarted;
    public bool IsFightCompleted() => fightCompleted;
    public int GetBossesRemaining() => bossesRemaining;

    // Vẽ gizmo trong Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = fightCompleted ? Color.green : (fightStarted ? Color.red : Color.yellow);
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);    
    }
}