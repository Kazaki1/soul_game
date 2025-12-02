using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawn enemies trong khu vực giới hạn
/// Tự động respawn sau khi enemy bị destroy
/// </summary>
public class SpawnEnemy : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public Vector2 spawnPosition;
        public float respawnTime = 5f;
        [HideInInspector] public GameObject spawnedEnemy;
        [HideInInspector] public bool isRespawning = false;
    }

    [Header("Spawn Settings")]
    [Tooltip("Danh sách enemies và vị trí spawn")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();

    [Header("Area Limit")]
    [Tooltip("Khu vực giới hạn spawn (tính từ transform.position)")]
    public Vector2 areaSize = new Vector2(10f, 10f);

    [Header("Auto Spawn")]
    [Tooltip("Tự động spawn khi start")]
    public bool spawnOnStart = true;

    [Header("Debug")]
    public bool showDebugLogs = true;
    public Color areaColor = Color.green;
    public Color spawnPointColor = Color.red;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnAllEnemies();
        }
    }

    private void Update()
    {
        // Check từng enemy xem có bị destroy chưa
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemySpawnData data = enemies[i];

            // Nếu enemy bị destroy và chưa respawn
            if (data.spawnedEnemy == null && !data.isRespawning)
            {
                StartCoroutine(RespawnEnemy(i));
            }
        }
    }

    /// <summary>
    /// Spawn tất cả enemies
    /// </summary>
    public void SpawnAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            SpawnEnemyAt(i);
        }
    }

    /// <summary>
    /// Spawn enemy tại index cụ thể
    /// </summary>
    public void SpawnEnemyAt(int index)
    {
        if (index < 0 || index >= enemies.Count) return;

        EnemySpawnData data = enemies[index];

        if (data.enemyPrefab == null)
        {
            Debug.LogWarning($"Enemy prefab at index {index} is null!");
            return;
        }

        // Tính vị trí spawn (relative to this transform + offset)
        Vector3 worldPosition = transform.position + (Vector3)data.spawnPosition;

        // Clamp vị trí trong area
        worldPosition = ClampPositionToArea(worldPosition);

        // Spawn enemy
        data.spawnedEnemy = Instantiate(data.enemyPrefab, worldPosition, Quaternion.identity, transform);

        if (showDebugLogs)
            Debug.Log($"✅ Spawned {data.enemyPrefab.name} at {worldPosition}");
    }

    /// <summary>
    /// Respawn enemy sau một khoảng thời gian
    /// </summary>
    private IEnumerator RespawnEnemy(int index)
    {
        if (index < 0 || index >= enemies.Count) yield break;

        EnemySpawnData data = enemies[index];
        data.isRespawning = true;

        if (showDebugLogs)
            Debug.Log($"⏳ Respawning {data.enemyPrefab.name} in {data.respawnTime}s...");

        yield return new WaitForSeconds(data.respawnTime);

        SpawnEnemyAt(index);
        data.isRespawning = false;
    }

    /// <summary>
    /// Clamp vị trí trong area giới hạn
    /// </summary>
    private Vector3 ClampPositionToArea(Vector3 position)
    {
        Vector3 center = transform.position;
        float halfWidth = areaSize.x / 2f;
        float halfHeight = areaSize.y / 2f;

        position.x = Mathf.Clamp(position.x, center.x - halfWidth, center.x + halfWidth);
        position.y = Mathf.Clamp(position.y, center.y - halfHeight, center.y + halfHeight);

        return position;
    }

    /// <summary>
    /// Spawn enemy random trong area
    /// </summary>
    public void SpawnRandomEnemy(GameObject enemyPrefab, float respawnTime = 5f)
    {
        if (enemyPrefab == null) return;

        Vector2 randomPos = new Vector2(
            Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
            Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
        );

        EnemySpawnData newData = new EnemySpawnData
        {
            enemyPrefab = enemyPrefab,
            spawnPosition = randomPos,
            respawnTime = respawnTime
        };

        enemies.Add(newData);
        SpawnEnemyAt(enemies.Count - 1);
    }

    /// <summary>
    /// Clear tất cả enemies
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (EnemySpawnData data in enemies)
        {
            if (data.spawnedEnemy != null)
            {
                Destroy(data.spawnedEnemy);
            }
        }

        StopAllCoroutines();
    }

    /// <summary>
    /// Vẽ area và spawn points trong Scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        // Vẽ area boundary
        Gizmos.color = areaColor;
        Vector3 center = transform.position;
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, areaSize.y, 0));

        // Vẽ spawn points
        if (enemies != null)
        {
            Gizmos.color = spawnPointColor;
            foreach (EnemySpawnData data in enemies)
            {
                Vector3 spawnPos = transform.position + (Vector3)data.spawnPosition;
                Gizmos.DrawWireSphere(spawnPos, 0.5f);

                // Vẽ line từ center đến spawn point
                Gizmos.DrawLine(center, spawnPos);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ chi tiết hơn khi được select
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;

        // Vẽ 4 góc
        Vector3 topLeft = center + new Vector3(-areaSize.x / 2f, areaSize.y / 2f, 0);
        Vector3 topRight = center + new Vector3(areaSize.x / 2f, areaSize.y / 2f, 0);
        Vector3 bottomLeft = center + new Vector3(-areaSize.x / 2f, -areaSize.y / 2f, 0);
        Vector3 bottomRight = center + new Vector3(areaSize.x / 2f, -areaSize.y / 2f, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}