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
        if (!activateOnEnter)
        {
            SetBarriersActive(false);
        }

        bossesRemaining = bossesToDefeat.Count;
    }

    private void Update()
    {
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

    private void StartBossFight()
    {
        fightStarted = true;

        SetBarriersActive(true);

        if (showDebugLogs)
        {
            Debug.Log($"🔥 Boss Fight Started! Bosses to defeat: {bossesRemaining}");
        }

    }
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

        if (deadBosses != (bossesToDefeat.Count - bossesRemaining))
        {
            bossesRemaining = bossesToDefeat.Count - deadBosses;

            if (showDebugLogs)
            {
                Debug.Log($" Boss defeated! Remaining: {bossesRemaining}");
            }
        }

        if (bossesRemaining <= 0 && !fightCompleted)
        {
            CompleteBossFight();
        }
    }
    private void CompleteBossFight()
    {
        fightCompleted = true;

        if (showDebugLogs)
        {
            Debug.Log("🎉 Boss Fight Completed! Arena unlocked.");
        }

        if (deactivateOnComplete)
        {
            SetBarriersActive(false);
        }

    }

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
    public void AddBoss(GameObject boss)
    {
        if (!bossesToDefeat.Contains(boss))
        {
            bossesToDefeat.Add(boss);
            bossesRemaining++;
        }
    }
    public void SkipBossFight()
    {
        if (fightStarted && !fightCompleted)
        {
            Debug.Log("⚠️ Boss Fight Skipped!");
            CompleteBossFight();
        }
    }

    public void ResetArena()
    {
        fightStarted = false;
        fightCompleted = false;
        bossesRemaining = bossesToDefeat.Count;
        SetBarriersActive(false);

        Debug.Log("🔄 Arena Reset");
    }

    public bool IsFightStarted() => fightStarted;
    public bool IsFightCompleted() => fightCompleted;
    public int GetBossesRemaining() => bossesRemaining;

    private void OnDrawGizmos()
    {
        Gizmos.color = fightCompleted ? Color.green : (fightStarted ? Color.red : Color.yellow);
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);    
    }
}
