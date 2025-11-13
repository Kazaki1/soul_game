using UnityEngine;
using System.Collections;

public class SlimeDualBullet : MonoBehaviour
{
    public int damage = 10;
    public float slowDuration = 2f;
    public float slowFactor = 0.5f;

    [Header("Effect")]
    public GameObject bindEffectPrefab;  // hiệu ứng trói buộc (sprite animation)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2 player = collision.GetComponent<PlayerController2>();
            if (player != null)
            {
                player.TakeDamage(damage);
                player.ApplySlow(slowFactor, slowDuration);

                // Tạo hiệu ứng trói buộc
                if (bindEffectPrefab != null)
                {
                    GameObject effect = Instantiate(bindEffectPrefab, player.transform.position, Quaternion.identity, player.transform);
                    Destroy(effect, slowDuration); // xoá sau khi hết slow
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
    