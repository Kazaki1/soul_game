using UnityEngine;

public class PlayerMovementSFX : MonoBehaviour
{
    private Rigidbody2D rb;               
    public float runThreshold = 3f;
    private float footstepTimer;

    [Header("Attack Settings")]
    public float attackRange = 1f;       // phạm vi đánh
    public LayerMask enemyLayer;         // Layer quái

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(rb == null)
            Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
    }

    void Update()
    {
        if(rb == null) return;

        HandleFootsteps();

        // Chuột trái để đánh
        if(Input.GetButtonDown("Fire1"))
        {
            SFXManager.Instance.PlayAttack();

            Vector2 attackCenter = transform.position;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                attackCenter, 
                attackRange, 
                enemyLayer
            );

            foreach(Collider2D enemy in hitEnemies)
            {
                SFXManager.Instance.PlayHitEnemy();
            }
        }
    }

    void HandleFootsteps()
    {
        float speed = rb.linearVelocity.magnitude;  

        if (speed > 0.1f)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0)
            {
                if (speed < runThreshold)         
                    SFXManager.Instance.PlayWalk();
                else                              
                    SFXManager.Instance.PlayRun();

                footstepTimer = Mathf.Max(0.1f, 0.5f / speed);  
            }
        }
        else
        {
            SFXManager.Instance.StopMovementSFX();
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 attackCenter = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCenter, attackRange);
    }
}
