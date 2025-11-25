using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float sprint_speed;
    public Rigidbody2D rb;

    private Vector2 moveDirection;

    public static bool sprint_check;
    public static bool walk_check;

    public float decreaseAmount;

    public Animator anim;

    public Transform Aim;
    private Vector2 lastMoveDirection;
    private bool isMoving = false;

    public float dough_speed = 10f;
    public float dough_duration = 1f;
    public float dough_cooldown = 1f;
    public float doughStaminaCost = 20f;
    bool isDoughing = false;
    bool canDough = true;
    bool isInvincible = false;

    private void Start()
    {
        canDough = true;
    }
    void Update()
    {
        if (isDoughing)
        {
            return;
        }
        HandleInput();
        Animate();

    }
    void FixedUpdate()
    {
        if (!isDoughing)
        {
            Move();
            Sprint();
        }
        if (isDoughing)
        {
            return;
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
        if (Input.GetKeyDown(KeyCode.Space) && canDough)
        {
            StartCoroutine(Doughing());
        }

    }

    void Move()
    {
        if (isDoughing)
            return;

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


    void Animate()
    {
        anim.SetFloat("AnimMoveX", moveDirection.x);
        anim.SetFloat("AnimMoveY", moveDirection.y);
        anim.SetFloat("AnimMoveMagnitude", moveDirection.magnitude);

        anim.SetFloat("AnimLastMoveX", lastMoveDirection.x);
        anim.SetFloat("AnimLastMoveY", lastMoveDirection.y);
    }

    private IEnumerator Doughing()
    {
        Stamina stamina = FindObjectOfType<Stamina>();

        if (stamina != null)
            stamina.currentStamina -= doughStaminaCost;

        canDough = false;      // này là cooldown
        isInvincible = true;
        isDoughing = true;
        Vector2 dashDir = moveDirection != Vector2.zero ?
                          moveDirection :
                          lastMoveDirection;
        rb.linearVelocity = dashDir * dough_speed;

        yield return new WaitForSeconds(dough_duration);

        isDoughing = false;
        isInvincible = false;

        yield return new WaitForSeconds(dough_cooldown);
        canDough = true;
    }

}
