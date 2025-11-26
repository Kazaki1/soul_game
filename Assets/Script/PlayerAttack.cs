using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Hitboxes cho từng Weapon Type")]
    public GameObject[] weaponHitboxes; // Index = WeaponType


    [Header("Default Values (nếu chưa equip weapon)")]
    public float defaultAttackCooldown = 0.5f;
    public int defaultStaminaCost = 10;

    private GameObject currentHitbox;
    private bool canAttack = true;
    private float attackTimer = 0f;

    private Animator anim;
    private PlayerController playerController;
    private Stamina stamina;
    private PlayerWeaponController weaponController;
    private PlayerCapacity playerCapacity;

    // Offset cho từng weapon type và hướng
    [System.Serializable]
    public class WeaponTypeOffsets
    {
        public string weaponTypeName = "Sword";
        public Vector2 upOffset;
        public Vector2 downOffset;
        public Vector2 leftOffset;
        public Vector2 rightOffset;
    }

    [Header("Offsets cho từng Weapon Type")]
    public WeaponTypeOffsets[] weaponOffsets;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        stamina = GetComponent<Stamina>();
        weaponController = GetComponent<PlayerWeaponController>();
        playerCapacity = GetComponent<PlayerCapacity>();

        // Tắt tất cả hitboxes ban đầu
        if (weaponHitboxes != null)
        {
            foreach (GameObject hitbox in weaponHitboxes)
            {
                if (hitbox != null)
                    hitbox.SetActive(false);
            }
        }

        if (weaponController == null)
        {
            Debug.LogError("❌ PlayerWeaponController not found!");
        }

        if (playerCapacity == null)
        {
            Debug.LogError("❌ PlayerCapacity not found!");
        }

        // Set hitbox ban đầu dựa trên weapon hiện tại
        UpdateCurrentHitbox();
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

    /// <summary>
    /// Cập nhật hitbox hiện tại dựa trên weapon type
    /// </summary>
    public void UpdateCurrentHitbox()
    {
        WeaponStats weaponStats = weaponController != null ? weaponController.GetWeaponStats() : null;
        int weaponType = weaponStats != null ? weaponStats.GetWeaponType() : 0;

        // Tắt hitbox cũ
        if (currentHitbox != null)
        {
            currentHitbox.SetActive(false);
        }

        // Bật hitbox mới theo weapon type
        if (weaponHitboxes != null && weaponType < weaponHitboxes.Length && weaponHitboxes[weaponType] != null)
        {
            currentHitbox = weaponHitboxes[weaponType];
            Debug.Log($"✅ Switched to Weapon Type {weaponType} hitbox");
        }
        else
        {
            Debug.LogWarning($"⚠️ Weapon Type {weaponType} hitbox not found! Using default.");
            currentHitbox = weaponHitboxes != null && weaponHitboxes.Length > 0 ? weaponHitboxes[0] : null;
        }
    }

    void HandleAttackInput()
    {
        if (!canAttack) return;

        // Lấy weapon stats từ PlayerWeaponController
        WeaponStats weaponStats = weaponController != null ? weaponController.GetWeaponStats() : null;

        // Lấy base stats từ weapon
        float baseCooldown = weaponStats != null ? weaponStats.GetAttackCooldown() : defaultAttackCooldown;
        int baseCost = weaponStats != null ? weaponStats.GetStaminaCost() : defaultStaminaCost;
        int weaponType = weaponStats != null ? weaponStats.GetWeaponType() : 0;

        // Áp dụng Equipment Load modifier
        float loadModifier = GetEquipLoadModifier();
        float finalCooldown = baseCooldown * loadModifier;
        int finalCost = Mathf.CeilToInt(baseCost * loadModifier);

        // Check stamina
        if (stamina != null && stamina.currentStamina < finalCost) return;

        if (Input.GetMouseButtonDown(0))
        {
            canAttack = false;
            attackTimer = finalCooldown;

            // Trừ stamina
            if (stamina != null)
            {
                stamina.currentStamina -= finalCost;
                stamina.OnStaminaUsed();
            }

            // Set Animator parameters
            anim.SetTrigger("IsAttack");
            anim.SetInteger("WeaponType", weaponType); // ← SET WEAPON TYPE CHO ANIMATOR

            if (playerController != null)
            {
                Vector2 dir = SnapDirectionTo4Way(playerController.GetLastMoveDirection());
                UpdateMeleePosition(dir, weaponType);
                anim.SetFloat("AttackX", dir.x);
                anim.SetFloat("AttackY", dir.y);
            }

            // Debug info
            if (playerCapacity != null)
            {
                float loadPercentage = weaponController.GetLoadPercentage();
                Debug.Log($"⚔️ Attack | WeaponType: {weaponType} | Load: {loadPercentage:F1}% | Modifier: {loadModifier:F2}x | Cooldown: {finalCooldown:F2}s | Cost: {finalCost}");
            }
        }
    }

    /// <summary>
    /// Tính Equipment Load Modifier dựa trên % tải trọng
    /// </summary>
    private float GetEquipLoadModifier()
    {
        if (weaponController == null) return 1.0f;

        float loadPercentage = weaponController.GetLoadPercentage();

        if (loadPercentage < 50f)
            return 0.9f;
        else if (loadPercentage < 75f)
            return 1.0f;
        else if (loadPercentage < 100f)
            return 1.1f;
        else if (loadPercentage < 110f)
            return 1.25f;
        else
            return 1.4f;
    }

    /// <summary>
    /// Cập nhật vị trí hitbox dựa trên hướng và weapon type
    /// </summary>
    void UpdateMeleePosition(Vector2 dir, int weaponType)
    {
        if (currentHitbox == null) return;

        // Lấy offsets cho weapon type này
        WeaponTypeOffsets offsets = null;
        if (weaponOffsets != null && weaponType < weaponOffsets.Length)
        {
            offsets = weaponOffsets[weaponType];
        }

        // Fallback nếu không có offsets
        if (offsets == null)
        {
            Debug.LogWarning($"⚠️ No offsets defined for Weapon Type {weaponType}");
            return;
        }

        if (dir.y > 0.1f) // UP
        {
            currentHitbox.transform.localPosition = offsets.upOffset;
            currentHitbox.transform.localRotation = Quaternion.Euler(0, 0, -180);
        }
        else if (dir.y < -0.1f) // DOWN
        {
            currentHitbox.transform.localPosition = offsets.downOffset;
            currentHitbox.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dir.x > 0.1f) // RIGHT
        {
            currentHitbox.transform.localPosition = offsets.rightOffset;
            currentHitbox.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (dir.x < -0.1f) // LEFT
        {
            currentHitbox.transform.localPosition = offsets.leftOffset;
            currentHitbox.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }

    public void EnableMelee()
    {
        if (currentHitbox != null)
            currentHitbox.SetActive(true);
    }

    public void DisableMelee()
    {
        if (currentHitbox != null)
            currentHitbox.SetActive(false);
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

    public string GetEquipLoadTier()
    {
        if (weaponController == null) return "Unknown";
        float loadPercentage = weaponController.GetLoadPercentage();

        if (loadPercentage < 50f)
            return "Light Load (-10% cost/cooldown)";
        else if (loadPercentage < 75f)
            return "Medium Load (Normal)";
        else if (loadPercentage < 100f)
            return "Heavy Load (+10% cost/cooldown)";
        else if (loadPercentage < 110f)
            return "Overloaded (+25% cost/cooldown)";
        else
            return "Severely Overloaded (+40% cost/cooldown)";
    }

    public WeaponStats GetCurrentWeaponStats()
    {
        return weaponController != null ? weaponController.GetWeaponStats() : null;
    }

    public bool CanPerformAttack()
    {
        if (!canAttack) return false;

        WeaponStats weaponStats = weaponController != null ? weaponController.GetWeaponStats() : null;
        int baseCost = weaponStats != null ? weaponStats.GetStaminaCost() : defaultStaminaCost;

        float loadModifier = GetEquipLoadModifier();
        int finalCost = Mathf.CeilToInt(baseCost * loadModifier);

        return stamina == null || stamina.currentStamina >= finalCost;
    }

    public float GetFinalAttackCooldown()
    {
        WeaponStats weaponStats = weaponController != null ? weaponController.GetWeaponStats() : null;
        float baseCooldown = weaponStats != null ? weaponStats.GetAttackCooldown() : defaultAttackCooldown;
        return baseCooldown * GetEquipLoadModifier();
    }

    public int GetFinalStaminaCost()
    {
        WeaponStats weaponStats = weaponController != null ? weaponController.GetWeaponStats() : null;
        int baseCost = weaponStats != null ? weaponStats.GetStaminaCost() : defaultStaminaCost;
        return Mathf.CeilToInt(baseCost * GetEquipLoadModifier());
    }
}