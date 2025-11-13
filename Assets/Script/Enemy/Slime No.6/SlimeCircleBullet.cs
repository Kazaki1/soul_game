using UnityEngine;

public class SlimeCircleBullet : MonoBehaviour
{
    public int damage = 10;
    public float knockbackForce = 2f; // lực đẩy player khi trúng

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Gây sát thương
                player.TakeDamage(damage);

                // Đẩy player ra xa
                Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();
                if (rbPlayer != null)
                {
                    Vector2 dir = (player.transform.position - transform.position).normalized;
                    rbPlayer.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
                }
            }

            Destroy(gameObject); // huỷ đạn khi trúng player
        }

        // Nếu chạm vật thể cản (tường, chướng ngại vật)
        if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
