using UnityEngine;

public class SlimeBulletFire : MonoBehaviour
{
    private Vector2 direction;
    private int damage;
    private float speed;
    private bool hasExploded = false;

    public GameObject explosionPrefab; // Gán prefab vụ nổ trong Inspector
    public float lifetime = 5f;        // Thời gian sống nếu không trúng gì
    public float knockbackForce = 8f;  // Lực đẩy player

    private Rigidbody2D rb;

    public void Init(Vector2 dir, int dmg, float spd)
    {
        direction = dir;
        damage = dmg;
        speed = spd;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;

        // Xoay viên đạn theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;

        if (collision.CompareTag("Player"))
        {
            hasExploded = true;
            rb.linearVelocity = Vector2.zero;

            // Gây damage
            PlayerController pc = collision.GetComponent<PlayerController>();
            if (pc != null)
                //pc.TakeDamage(damage);



            // Gọi hiệu ứng nổ
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
            {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
