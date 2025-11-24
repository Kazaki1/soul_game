using UnityEngine;

public class Melee : MonoBehaviour
{
    public int damage = 20;
    public float knockbackForce = 5f;   

    private Transform player;

    private void Start()
    {
        player = transform.root;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            EnemyKnockback enemyKB = other.GetComponent<EnemyKnockback>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            if (enemyKB != null)
            {
                Vector2 dir = (other.transform.position - player.position).normalized;
                enemyKB.ApplyKnockback(dir, knockbackForce);
            }
        }
    }
}
