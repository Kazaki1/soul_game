using UnityEngine;

public class SlimeBullet : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private float speed;

    [Header("Homing Settings")]
    public float turnSpeed = 5f;      // Độ nhạy khi đổi hướng đuổi
    public float lifetime = 3f;       // Thời gian sống trước khi biến mất

    public void Init(Transform target, float spd)
    {
        player = target;
        speed = spd;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime); // Tự hủy nếu không trúng
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Tính hướng từ đạn đến player
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;

        // Làm mượt hướng bay (đuổi dần)
        Vector2 newVel = Vector2.Lerp(rb.linearVelocity, dir * speed, Time.fixedDeltaTime * turnSpeed);
        rb.linearVelocity = newVel;

        // Xoay sprite đạn theo hướng bay
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
 
}
