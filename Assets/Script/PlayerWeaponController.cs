using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Equipped Weapon")]
    [SerializeField] private GameObject equippedWeapon;
    private WeaponDamage weaponDamage;
    private WeaponStats weaponStats;
    [Header("Equipped Weapons")]
    [SerializeField] private GameObject leftHandWeapon;   // Weapon hiện tại
    [SerializeField] private GameObject rightHandWeapon;
    [Header("Weapon Transforms")]
    public Transform leftHand;
    public Transform rightHand;  // ✅ THÊM MỚI

    private bool isSwapping = false;  
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
        if (leftHandWeapon == null)
        {
            weaponDamage = null;
            weaponStats = null;
            cachedWeaponDamage = 0f;
            Debug.LogWarning("Không có weapon được equipped!");
            return;
        }

        weaponDamage = leftHandWeapon.GetComponent<WeaponDamage>();
        weaponStats = leftHandWeapon.GetComponent<WeaponStats>();

        if (weaponDamage == null)
        {
            Debug.LogError($"Weapon {leftHandWeapon.name} không có WeaponDamage component!");
            cachedWeaponDamage = 0f;
            return;
        }

        if (weaponStats == null)
        {
            Debug.LogError($"Weapon {leftHandWeapon.name} không có WeaponStats component!");
        }

        UpdateWeaponDamage();
        Debug.Log($"Loaded weapon: {leftHandWeapon.name} - Damage: {cachedWeaponDamage:F1}");
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
        if (newWeapon == null)
        {
            Debug.LogError("❌ Cannot equip NULL weapon!");
            return;
        }

        // Nếu left hand đã có weapon → chuyển sang right hand
        if (leftHandWeapon != null)
        {
            // Destroy right hand weapon cũ nếu có
            if (rightHandWeapon != null)
            {
                Destroy(rightHandWeapon);
                Debug.Log($"🗑️ Destroyed old right hand weapon");
            }

            // Chuyển left → right
            rightHandWeapon = leftHandWeapon;
            rightHandWeapon.transform.SetParent(rightHand);
            rightHandWeapon.transform.localPosition = Vector3.zero;
            rightHandWeapon.transform.localRotation = Quaternion.identity;
            rightHandWeapon.SetActive(false); // Ẩn weapon ở right hand

            Debug.Log($"🔄 Moved left weapon to right hand: {rightHandWeapon.name}");
        }

        // Equip weapon mới vào left hand
        leftHandWeapon = newWeapon;
        leftHandWeapon.transform.SetParent(leftHand);
        leftHandWeapon.transform.localPosition = Vector3.zero;
        leftHandWeapon.transform.localRotation = Quaternion.identity;
        leftHandWeapon.SetActive(true);

        LoadWeapon();

        Debug.Log($"✅ Equipped weapon to left hand: {leftHandWeapon.name}");
    }

    /// <summary>
    /// Unequip weapon hiện tại
    /// </summary>
    public void UnequipWeapon()
    {
        if (leftHandWeapon != null)
        {
            Destroy(leftHandWeapon);
            Debug.Log($"🗑️ Unequipped left hand weapon");
        }

        leftHandWeapon = null;
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
    /// <summary>
    /// Swap weapon giữa left hand và right hand
    /// </summary>
    public void QuickSwapWeapons()
    {
        if (isSwapping)
        {
            Debug.LogWarning("⚠️ Đang trong quá trình swap, chờ xíu!");
            return;
        }

        if (leftHandWeapon == null && rightHandWeapon == null)
        {
            Debug.LogWarning("⚠️ Không có weapon nào để swap!");
            return;
        }

        isSwapping = true;

        // Swap references
        GameObject temp = leftHandWeapon;
        leftHandWeapon = rightHandWeapon;
        rightHandWeapon = temp;

        // Cập nhật transforms và visibility
        if (leftHandWeapon != null)
        {
            leftHandWeapon.transform.SetParent(leftHand);
            leftHandWeapon.transform.localPosition = Vector3.zero;
            leftHandWeapon.transform.localRotation = Quaternion.identity;
            leftHandWeapon.SetActive(true);
        }

        if (rightHandWeapon != null)
        {
            rightHandWeapon.transform.SetParent(rightHand);
            rightHandWeapon.transform.localPosition = Vector3.zero;
            rightHandWeapon.transform.localRotation = Quaternion.identity;
            rightHandWeapon.SetActive(false);
        }

        // Reload weapon stats
        LoadWeapon();

      
        Debug.Log($"🔄 Swapped weapons - Active: {leftHandWeapon?.name ?? "None"}");

        isSwapping = false;
    }

    /// <summary>
    /// Lấy weapon ở right hand (để UI sync)
    /// </summary>
    public GameObject GetRightHandWeapon()
    {
        return rightHandWeapon;
    }

    /// <summary>
    /// Lấy weapon ở left hand (để UI sync)
    /// </summary>
    public GameObject GetLeftHandWeapon()
    {
        return leftHandWeapon;
    }
    public void EquipWeaponFromInventory(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("Prefab weapon null!");
            return;
        }

        // Destroy weapon hiện tại nếu có
        if (equippedWeapon != null)
        {
            Destroy(equippedWeapon);
        }

        // Instantiate weapon mới vào leftHand
        equippedWeapon = Instantiate(weaponPrefab, leftHand);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;

        // Load stats
        LoadWeapon();
    }

}