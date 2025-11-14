using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Melee;              
    public float attackCooldown = 0.5f;   
    public int staminaCost = 10;

    private bool canAttack = true;
    private float attackTimer = 0f;

    private Animator anim;
    private PlayerController playerController;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        if (Melee != null)
            Melee.SetActive(false); 
    }

    void Update()
    {
        HandleAttackInput();

        if (!canAttack)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                canAttack = true;
        }
    }

    void HandleAttackInput()
    {
        Stamina stamina = FindObjectOfType<Stamina>();

        if (!canAttack) return;
        if (stamina != null && stamina.currentStamina < staminaCost) return;

        if (Input.GetMouseButtonDown(0))
        {
            canAttack = false;
            attackTimer = attackCooldown;

            if (stamina != null)
                stamina.currentStamina -= staminaCost;

            anim.SetTrigger("IsAttack");

            if (playerController != null)
            {
                Vector2 dir = playerController.GetLastMoveDirection();
                anim.SetFloat("AttackX", dir.x);
                anim.SetFloat("AttackY", dir.y);
            }
        }
    }

    public void EnableMelee()
    {
        if (Melee != null)
            Melee.SetActive(true);
    }

    public void DisableMelee()
    {
        if (Melee != null)
            Melee.SetActive(false);
    }
}
