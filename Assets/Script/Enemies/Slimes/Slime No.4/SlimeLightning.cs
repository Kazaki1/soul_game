using UnityEngine;

public class SlimeLightning : MonoBehaviour
{
    private Vector3 targetPosition;
    private int damage;
    private float existTime;
    private bool hasHit = false;

    public float fallSpeed = 20f;

    public void Init(Vector3 target, int dmg, float lifeTime)
    {
        targetPosition = target;
        damage = dmg;
        existTime = lifeTime;

        // Di chuyển ngay tới vị trí mục tiêu (rơi xuống)
        transform.position = new Vector3(target.x, target.y + 8f, 0);
    }

    void Update()
    {
        // Rơi dần xuống vị trí target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

        // Nếu đã gần đến đất, tự huỷ
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            Destroy(gameObject, existTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                //pc.TakeDamage(damage);

            hasHit = true;
            Destroy(gameObject);
        }
    }
}
