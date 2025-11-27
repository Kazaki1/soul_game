using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Base Movement Stats")]
    public float baseSpeed = 8f;
    public float baseSprintSpeed = 15f;
    public float baseDoughSpeed = 20f;
    public float baseDoughDuration = 0.25f;
    public float baseDoughStaminaCost = 20f;

    [Header("Runtime Values (Auto-calculated)")]
    public float speed;
    public float sprint_speed;
    public float dough_speed;
    public float dough_duration;
    public float doughStaminaCost;

    public Rigidbody2D rb;

    private Vector2 moveDirection;

    public static bool sprint_check;
    public static bool walk_check;

    public float decreaseAmount;

    public Animator anim;

    public Transform Aim;
    private Vector2 lastMoveDirection;
    private bool isMoving = false;

    public float dough_cooldown = 1f;
    bool isDoughing = false;
    bool canDough = true;
    bool isInvincible = false;

    private PlayerWeaponController weaponController;
    private PlayerCapacity playerCapacity;
    private ItemController itemController; // Thêm reference đến ItemController

    private void Awake()
    {
        weaponController = GetComponent<PlayerWeaponController>();
        playerCapacity = GetComponent<PlayerCapacity>();
        itemController = GetComponent<ItemController>(); // Thêm để lấy buffs

        if (weaponController == null)
        {
            Debug.LogWarning("⚠️ PlayerWeaponController not found! Load system won't affect movement.");
        }

        if (playerCapacity == null)
        {
            Debug.LogWarning("⚠️ PlayerCapacity not found! Load system won't affect movement.");
        }

        if (itemController == null)
        {
            Debug.LogWarning("⚠️ ItemController not found! Không có buff item cho movement.");
        }
    }

    private void Start()
    {
        canDough = true;
        UpdateMovementStats();
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

        // Debug key - xem stats hiện tại
        if (Input.GetKeyDown(KeyCode.M))
        {
            DisplayMovementStats();
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
            // Update speed với load modifier
            UpdateMovementStats();
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

        // Update dough stats trước khi dodge
        UpdateMovementStats();

        if (stamina != null)
            stamina.currentStamina -= doughStaminaCost;
        stamina.OnStaminaUsed();

        canDough = false;
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

    /// <summary>
    /// Cập nhật tất cả movement stats dựa trên Equipment Load và buffs
    /// </summary>
    public void UpdateMovementStats()
    {
        EquipLoadModifiers modifiers = GetEquipLoadModifiers();

        // Áp dụng dodge speed buff từ items
        float dodgeBuffMultiplier = 1f;
        if (itemController != null)
        {
            float buffPercent = itemController.GetTotalDodgeSpeedBuffPercent();
            dodgeBuffMultiplier += buffPercent / 100f;
        }

        speed = baseSpeed * modifiers.speedMultiplier;
        sprint_speed = baseSprintSpeed * modifiers.sprintSpeedMultiplier;
        dough_speed = baseDoughSpeed * modifiers.doughSpeedMultiplier * dodgeBuffMultiplier;
        dough_duration = baseDoughDuration * modifiers.doughDurationMultiplier;
        doughStaminaCost = baseDoughStaminaCost * modifiers.doughCostMultiplier;
    }

    /// <summary>
    /// Lấy tất cả modifiers dựa trên Equipment Load %
    /// </summary>
    private EquipLoadModifiers GetEquipLoadModifiers()
    {
        if (playerCapacity == null)
            return new EquipLoadModifiers(); // Return default (1.0x tất cả)

        float currentLoad = playerCapacity.GetCurrentLoad();
        float maxLoad = playerCapacity.GetMaxEquipLoad();
        float loadPercentage = (currentLoad / maxLoad) * 100f;

        if (loadPercentage < 50f)
        {
            // Light Load: Bonus movement
            return new EquipLoadModifiers
            {
                speedMultiplier = 1.2f,          // +20%
                sprintSpeedMultiplier = 1.3f,    // +30%
                doughSpeedMultiplier = 1.2f,     // +20%
                doughDurationMultiplier = 1f,  // -10% (faster dodge)
                doughCostMultiplier = 0.8f       // -20%
            };
        }
        else if (loadPercentage < 75f)
        {
            return new EquipLoadModifiers
            {
                speedMultiplier = 1f,
                sprintSpeedMultiplier = 1f,
                doughSpeedMultiplier = 1f,
                doughDurationMultiplier = 1f,
                doughCostMultiplier = 1f
            };
        }
        else if (loadPercentage < 100f)
        {
            // Heavy Load: Slight penalties
            return new EquipLoadModifiers
            {
                speedMultiplier = 0.9f,          // -10%
                sprintSpeedMultiplier = 0.8f,    // -20%
                doughSpeedMultiplier = 0.8f,     // -20%
                doughDurationMultiplier = 1f,  // +10% (slower dodge)
                doughCostMultiplier = 1.1f       // +10%
            };
        }
        else if (loadPercentage < 110f)
        {
            // Overloaded: Heavy penalties
            return new EquipLoadModifiers
            {
                speedMultiplier = 0.8f,          // -20%
                sprintSpeedMultiplier = 0.6f,    // -40%
                doughSpeedMultiplier = 0.5f,     // -50%
                doughDurationMultiplier = 1f,  // +20%
                doughCostMultiplier = 1.2f       // +20%
            };
        }
        else
        {
            // Severely Overloaded: Extreme penalties
            return new EquipLoadModifiers
            {
                speedMultiplier = 0.6f,          // -40%
                sprintSpeedMultiplier = 0.2f,    // -80%
                doughSpeedMultiplier = 0.0f,     // -100% (cannot dodge!)
                doughDurationMultiplier = 1.5f,  // +50%
            };
        }
    }

    /// <summary>
    /// Lấy load tier hiện tại
    /// </summary>
    public string GetMovementLoadTier()
    {
        if (playerCapacity == null) return "Unknown";

        float currentLoad = playerCapacity.GetCurrentLoad();
        float maxLoad = playerCapacity.GetMaxEquipLoad();
        float loadPercentage = (currentLoad / maxLoad) * 100f;

        if (loadPercentage < 50f)
            return "Light Load (Fast Roll)";
        else if (loadPercentage < 75f)
            return "Medium Load (Mid Roll)";
        else if (loadPercentage < 100f)
            return "Heavy Load (Slow Roll)";
        else if (loadPercentage < 110f)
            return "Overloaded (Fat Roll)";
        else
            return "Severely Overloaded (No Roll!)";
    }

    /// <summary>
    /// Hiển thị thông tin movement stats
    /// </summary>
    public void DisplayMovementStats()
    {
        UpdateMovementStats();

        Debug.Log("========== MOVEMENT STATS ==========");
        Debug.Log($"Load Tier: {GetMovementLoadTier()}");
        Debug.Log($"Speed: {speed:F2} (base: {baseSpeed})");
        Debug.Log($"Sprint Speed: {sprint_speed:F2} (base: {baseSprintSpeed})");
        Debug.Log($"Dodge Speed: {dough_speed:F2} (base: {baseDoughSpeed})");
        Debug.Log($"Dodge Duration: {dough_duration:F2}s (base: {baseDoughDuration}s)");
        Debug.Log($"Dodge Cost: {doughStaminaCost:F0} (base: {baseDoughStaminaCost})");
        Debug.Log("====================================");
    }

    /// <summary>
    /// Kiểm tra có thể dodge không
    /// </summary>
    public bool CanDodge()
    {
        if (!canDough) return false;

        Stamina stamina = FindObjectOfType<Stamina>();
        if (stamina == null) return true;

        UpdateMovementStats();
        return stamina.currentStamina >= doughStaminaCost;
    }
}

/// <summary>
/// Struct chứa tất cả modifiers cho movement
/// </summary>
[System.Serializable]
public struct EquipLoadModifiers
{
    public float speedMultiplier;
    public float sprintSpeedMultiplier;
    public float doughSpeedMultiplier;
    public float doughDurationMultiplier;
    public float doughCostMultiplier;

    // Constructor với default values (1.0 = normal)
    public EquipLoadModifiers(float defaultValue = 1.0f)
    {
        speedMultiplier = defaultValue;
        sprintSpeedMultiplier = defaultValue;
        doughSpeedMultiplier = defaultValue;
        doughDurationMultiplier = defaultValue;
        doughCostMultiplier = defaultValue;
    }
}