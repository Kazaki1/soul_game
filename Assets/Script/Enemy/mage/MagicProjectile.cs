using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float lifeTime = 4f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"🔥 Magic Projectile trúng player, gây {damage} sát thương!");
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
