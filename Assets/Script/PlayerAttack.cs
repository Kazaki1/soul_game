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

    public Vector2 meleeUpOffset;
    public Vector2 meleeDownOffset;
    public Vector2 meleeLeftOffset;
    public Vector2 meleeRightOffset;
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
                Vector2 dir = SnapDirectionTo4Way(playerController.GetLastMoveDirection());
                UpdateMeleePosition(dir);
                anim.SetFloat("AttackX", dir.x);
                anim.SetFloat("AttackY", dir.y);
            }
        }

    }

    void UpdateMeleePosition(Vector2 dir)
    {
        if (dir.y > 0.1f)         
            Melee.transform.localPosition = meleeUpOffset;
        else if (dir.y < -0.1f)   
            Melee.transform.localPosition = meleeDownOffset;
        else if (dir.x > 0.1f)    
            Melee.transform.localPosition = meleeRightOffset;
        else if (dir.x < -0.1f)   
            Melee.transform.localPosition = meleeLeftOffset;
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
    Vector2 SnapDirectionTo4Way(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return new Vector2(dir.x > 0 ? 1 : -1, 0);
        }
        else
        {
            return new Vector2(0, dir.y > 0 ? 1 : -1);
        }
    }
}
