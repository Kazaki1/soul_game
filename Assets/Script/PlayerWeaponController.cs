using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Equipped Weapon")]
    [SerializeField] private GameObject equippedWeapon;

    private WeaponDamage weaponDamage;
    private WeaponStats weaponStats;
    private float cachedWeaponDamage;

    private PlayerCapacity playerCapacity;

    private void Awake()
    {
        playerCapacity = GetComponent<PlayerCapacity>();

        if (playerCapacity == null)
        {
            Debug.LogWarning("PlayerCapacity component not found!");
        }
    }

    private void Start()
    {
        if (equippedWeapon != null)
        {
            LoadWeapon();
        }
    }

    /// <summary>
    /// Load weapon damage component từ equipped weapon
    /// </summary>
    private void LoadWeapon()
    {
        if (equippedWeapon == null)
        {
            weaponDamage = null;
            weaponStats = null;
            cachedWeaponDamage = 0f;
            Debug.LogWarning("Không có weapon được equipped!");
            return;
        }

        weaponDamage = equippedWeapon.GetComponent<WeaponDamage>();
        weaponStats = equippedWeapon.GetComponent<WeaponStats>();

        if (weaponDamage == null)
        {
            Debug.LogError($"Weapon {equippedWeapon.name} không có WeaponDamage component!");
            cachedWeaponDamage = 0f;
            return;
        }

        if (weaponStats == null)
        {
            Debug.LogError($"Weapon {equippedWeapon.name} không có WeaponStats component!");
        }

        // Cache damage
        UpdateWeaponDamage();
        Debug.Log($"Loaded weapon: {equippedWeapon.name} - Damage: {cachedWeaponDamage:F1}, Weight: {GetCurrentWeaponWeight():F1}");
    }

    /// <summary>
    /// Cập nhật weapon damage (gọi khi stat thay đổi)
    /// </summary>
    public void UpdateWeaponDamage()
    {
        if (weaponDamage != null)
        {
            cachedWeaponDamage = weaponDamage.CalculateWeaponDamage();
        }
        else
        {
            cachedWeaponDamage = 0f;
        }
    }

    /// <summary>
    /// Lấy weapon damage hiện tại (đã cache)
    /// </summary>
    public float GetWeaponDamage()
    {
        return cachedWeaponDamage;
    }

    /// <summary>
    /// Equip weapon mới (dùng khi có hệ thống nhặt weapon sau này)
    /// </summary>
    public void EquipWeapon(GameObject newWeapon)
    {
        // Unequip weapon cũ nếu có
        if (equippedWeapon != null)
        {
            Debug.Log($"Unequipped {equippedWeapon.name}");
        }

        // Equip weapon mới
        equippedWeapon = newWeapon;
        LoadWeapon();
    }

    /// <summary>
    /// Unequip weapon hiện tại
    /// </summary>
    public void UnequipWeapon()
    {
        if (equippedWeapon != null)
        {
            Debug.Log($"Unequipped {equippedWeapon.name}");
        }

        equippedWeapon = null;
        weaponDamage = null;
        weaponStats = null;
        cachedWeaponDamage = 0f;
    }

    /// <summary>
    /// Kiểm tra có weapon không
    /// </summary>
    public bool HasWeapon()
    {
        return weaponDamage != null;
    }

    /// <summary>
    /// Hiển thị thông tin weapon
    /// </summary>
    public void DisplayWeaponInfo()
    {
        if (weaponDamage != null)
        {
            weaponDamage.DisplayDamageBreakdown();

            if (weaponStats != null)
            {
                Debug.Log($"Weapon Weight: {weaponStats.GetWeaponWeight():F1}");
            }

            // Hiển thị capacity info
            DisplayCapacityInfo();
        }
        else
        {
            Debug.Log("Không có weapon!");
        }
    }

    /// <summary>
    /// Hiển thị thông tin capacity với current equip load
    /// </summary>
    public void DisplayCapacityInfo()
    {
        if (playerCapacity == null) return;

        float currentLoad = GetCurrentEquipLoad();
        float maxLoad = playerCapacity.GetMaxEquipLoad();
        float loadPercentage = GetLoadPercentage();

        Debug.Log("========== CAPACITY INFO ==========");
        Debug.Log($"Current Load: {currentLoad:F1}");
        Debug.Log($"Max Load: {maxLoad:F1}");
        Debug.Log($"Load %: {loadPercentage:F1}%");
        Debug.Log("===================================");
    }

    /// <summary>
    /// Lấy tổng current equip load
    /// </summary>
    public float GetCurrentEquipLoad()
    {
        float total = 0f;

        // Tính weapon weight
        if (weaponStats != null)
        {
            total += weaponStats.GetWeaponWeight();
        }

        // TODO: Thêm armor, shield, etc.

        return total;
    }

    /// <summary>
    /// Lấy % tải trọng hiện tại
    /// </summary>
    public float GetLoadPercentage()
    {
        if (playerCapacity == null) return 0f;

        float maxLoad = playerCapacity.GetMaxEquipLoad();
        if (maxLoad <= 0) return 0f;

        return (GetCurrentEquipLoad() / maxLoad) * 100f;
    }

    /// <summary>
    /// Kiểm tra có bị overload không (> 100%)
    /// </summary>
    public bool IsOverloaded()
    {
        return GetLoadPercentage() > 100f;
    }

    /// <summary>
    /// Lấy weapon weight hiện tại
    /// </summary>
    public float GetCurrentWeaponWeight()
    {
        if (weaponStats != null)
            return weaponStats.GetWeaponWeight();
        return 0f;
    }

    /// <summary>
    /// Lấy weapon damage component (để custom damage calculation)
    /// </summary>
    public WeaponDamage GetWeaponDamageComponent()
    {
        return weaponDamage;
    }

    /// <summary>
    /// Lấy weapon stats component
    /// </summary>
    public WeaponStats GetWeaponStats()
    {
        return weaponStats;
    }
}