using UnityEngine;
using System.Collections;

public class SlimeDualBullet : MonoBehaviour
{
    public float slowDuration = 2f;
    public float slowMultiplier = 0f;  // 0 = trói chân (không di chuyển), 0.5 = giảm 50% tốc độ, 1 = bình thường

    [Header("Effect")]
    public GameObject bindEffectPrefab;  // hiệu ứng trói buộc

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                PlayerSlowEffect slowEffect = player.GetComponent<PlayerSlowEffect>();
                if (slowEffect == null)
                {
                    slowEffect = player.gameObject.AddComponent<PlayerSlowEffect>();
                }
                
                slowEffect.ApplySlow(slowDuration, slowMultiplier);

                // Tạo hiệu ứng trói buộc
                if (bindEffectPrefab != null)
                {
                    GameObject effect = Instantiate(bindEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
                    Destroy(effect, slowDuration);
                }
            }

            Destroy(gameObject);
        }

        if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}

/// <summary>
/// Component riêng để quản lý slow effect trên player
/// Tránh bị UpdateMovementStats() ghi đè
/// </summary>
public class PlayerSlowEffect : MonoBehaviour
{
    private PlayerController player;
    private Coroutine currentSlowCoroutine;
    private float slowEndTime = 0f;
    private float currentSlowMultiplier = 1f;

    public bool IsSlowed => Time.time < slowEndTime;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void ApplySlow(float duration, float multiplier)
    {
        // Nếu đang bị slow, reset timer (stack duration)
        slowEndTime = Time.time + duration;
        currentSlowMultiplier = multiplier;

        // Stop coroutine cũ nếu có
        if (currentSlowCoroutine != null)
        {
            StopCoroutine(currentSlowCoroutine);
        }

        currentSlowCoroutine = StartCoroutine(SlowCoroutine(duration));
    }

    private IEnumerator SlowCoroutine(float duration)
    {
        Debug.Log($"Slow applied! Multiplier: {currentSlowMultiplier}x for {duration}s");

        // Loop liên tục áp dụng slow effect
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Update stats từ base, sau đó nhân với slow multiplier
            player.UpdateMovementStats();
            
            player.speed *= currentSlowMultiplier;
            player.sprint_speed *= currentSlowMultiplier;
            player.dough_speed *= currentSlowMultiplier;

            // Chờ 1 frame rồi áp dụng lại (để chống UpdateMovementStats gọi từ chỗ khác)
            yield return null;
            elapsed += Time.deltaTime;
        }

        // Hết slow, restore lại stats
        player.UpdateMovementStats();
        currentSlowMultiplier = 1f;
        
        Debug.Log($"Slow ended! Speed restored to {player.speed}");
    }

    /// <summary>
    /// Gọi hàm này trong PlayerController.UpdateMovementStats() 
    /// để tự động áp dụng slow nếu đang bị slow
    /// </summary>
    public void ApplySlowMultiplierToCurrentStats()
    {
        if (IsSlowed && player != null)
        {
            player.speed *= currentSlowMultiplier;
            player.sprint_speed *= currentSlowMultiplier;
            player.dough_speed *= currentSlowMultiplier;
        }
    }
}