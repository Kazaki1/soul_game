using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseSpeed = 20f;          // tốc độ cơ bản
    public float sprint_speed = 30f;       // tốc độ khi chạy
    private float currentSpeed;            // tốc độ hiện tại (bị ảnh hưởng bởi slow)

    [Header("References")]
    public Rigidbody2D rb;
    public HealthBar healthBar;

    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public float decreaseAmount;

    private Vector2 moveDirection;
    public static bool sprint_check;
    public static bool walk_check;

    private bool isSlowed = false;         // kiểm tra có đang bị làm chậm không
    private float slowTimer = 0f;          // đếm thời gian slow
    private float slowFactor = 1f;         // tỉ lệ giảm tốc độ

    void Start()
    {
        currentSpeed = baseSpeed;
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    void Update()
    {
        InputMovement();
        HandleSlowEffect();
    }

    void FixedUpdate()
    {
        Move();
        Sprint();
    }

    void InputMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY);
    }

    void Move()
    {
        rb.linearVelocity = moveDirection * currentSpeed;
    }

    void Sprint()
    {
        Stamina stamina = FindObjectOfType<Stamina>();
        bool isMoving = Mathf.Abs(moveDirection.x) > 0f || Mathf.Abs(moveDirection.y) > 0f;

        if (Input.GetKey(KeyCode.LeftShift) && isMoving && stamina != null && stamina.currentStamina > 0)
        {
            sprint_check = true;
            walk_check = false;
            currentSpeed = sprint_speed * slowFactor;  // 👈 thêm slowFactor ở đây
        }
        else
        {
            sprint_check = false;
            walk_check = true;
            currentSpeed = baseSpeed * slowFactor;     // 👈 thêm slowFactor ở đây
        }
    }

    // 🧊 Làm chậm player
    public void ApplySlow(float factor, float duration)
    {
        slowFactor = factor;         // giảm tốc theo tỉ lệ (vd: 0.5 = giảm 50%)
        slowTimer = duration;        // đếm thời gian slow
        isSlowed = true;
    }

    void HandleSlowEffect()
    {
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                slowFactor = 1f;
                isSlowed = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // Có thể thêm hiệu ứng hoặc reload scene ở đây
    }
}
