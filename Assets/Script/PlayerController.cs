using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float sprint_speed;
    public Rigidbody2D rb;

    private Vector2 moveDirection;

    public static bool sprint_check;
    public static bool walk_check;

    public float decreaseAmount;

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public Animator anim;

    public Transform Aim;
    private Vector2 lastMoveDirection;
    bool isMoving = false;

    void Update()
    {
        HandleInput();
        Animate();
    }

    void FixedUpdate()
    {
        Move();
        Sprint();

        if (isMoving)
        {
            Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
    }

    void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveX != 0f || moveY != 0f)
        {
            isMoving = true;
            lastMoveDirection = moveDirection;

            Vector3 vector3 = Vector3.left * lastMoveDirection.x + Vector3.down * lastMoveDirection.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, vector3);
        }
        else
        {
            isMoving = false;
        }

        if (moveDirection != Vector2.zero)
        {
            lastMoveDirection = moveDirection;
        }

    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    void Sprint()
    {
        Stamina stamina = FindObjectOfType<Stamina>();
        bool isMoving = Mathf.Abs(moveDirection.x) > 0f || Mathf.Abs(moveDirection.y) > 0f;

        if (Input.GetKey(KeyCode.LeftShift) && isMoving && stamina != null && stamina.currentStamina > 0)
        {
            sprint_check = true;
            walk_check = false;
            speed = sprint_speed;
        }
        else
        {
            speed = 5f;
            sprint_check = false;
            walk_check = true;
        }
    }

    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }

    void Doughing()
    {

    }

    void Animate()
    {
        anim.SetFloat("AnimMoveX", moveDirection.x);
        anim.SetFloat("AnimMoveY", moveDirection.y);
        anim.SetFloat("AnimMoveMagnitude", moveDirection.magnitude);

        anim.SetFloat("AnimLastMoveX", lastMoveDirection.x);
        anim.SetFloat("AnimLastMoveY", lastMoveDirection.y);
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
        // Thêm logic chết ở đây (ví dụ: reload scene, hiển thị menu game over, v.v.)
    }
}
