using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public Slider slider;

    public int maxStamina;
    public float currentStamina;

    [Header("Base Stats")]
    public int baseStamina = 100;

    [Header("Regen Settings")]
    public float baseAutoFillTime = 6f;
    public float autoFillTimePerStamina = 0.06f;

    private float autoFillTime;
    private float regenDelay = 1f;
    private float timeSinceStaminaDrop = 0f;
    private bool staminaJustDropped = false;

    private PlayerStats stats;
    private PlayerWeaponController weaponController;
    private PlayerCapacity playerCapacity;
    private ItemController itemController; // Thêm reference đến ItemController

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        weaponController = GetComponent<PlayerWeaponController>();
        playerCapacity = GetComponent<PlayerCapacity>();
        itemController = GetComponent<ItemController>(); // Thêm để lấy buffs

        if (weaponController == null)
        {
            Debug.LogWarning("⚠️ PlayerWeaponController not found! Load system won't affect stamina regen.");
        }

        if (playerCapacity == null)
        {
            Debug.LogWarning("⚠️ PlayerCapacity not found! Load system won't affect stamina regen.");
        }

        if (itemController == null)
        {
            Debug.LogWarning("⚠️ ItemController not found! Không có stamina buff.");
        }
    }

    private void Start()
    {
        if (stats != null)
            UpdateMaxStaminaFromEndurance();
    }

    private void Update()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        float previousStamina = currentStamina;

        // Sprint
        if (PlayerController.sprint_check)
        {
            if (currentStamina > 0)
            {
                currentStamina -= playerController.decreaseAmount * Time.deltaTime;

                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    PlayerController.sprint_check = false;
                    PlayerController.walk_check = true;
                }
            }
        }

        // Track giảm stamina
        if (currentStamina < previousStamina)
        {
            staminaJustDropped = true;
            timeSinceStaminaDrop = 0f;
        }

        if (staminaJustDropped)
        {
            timeSinceStaminaDrop += Time.deltaTime;
            if (timeSinceStaminaDrop >= regenDelay)
                staminaJustDropped = false;
        }

        // Regen với Equipment Load modifier
        if (!staminaJustDropped && currentStamina < maxStamina)
        {
            float modifiedAutoFillTime = GetModifiedAutoFillTime();
            currentStamina += (maxStamina / modifiedAutoFillTime) * Time.deltaTime;

            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        slider.value = currentStamina;
    }

    /// <summary>
    /// Tính auto fill time sau khi áp dụng Equipment Load modifier
    /// < 50%: -10% (regen nhanh hơn)
    /// 50-75%: Normal
    /// 75-100%: +10% (regen chậm hơn)
    /// 100-110%: +25% (regen chậm nhiều)
    /// > 110%: +40% (regen rất chậm)
    /// </summary>
    private float GetModifiedAutoFillTime()
    {
        float loadModifier = GetEquipLoadModifier();

        // autoFillTime càng cao = regen càng chậm
        // Nên modifier > 1 = chậm hơn, modifier < 1 = nhanh hơn
        return autoFillTime * loadModifier;
    }

    /// <summary>
    /// Lấy Equipment Load Modifier dựa trên % tải trọng
    /// </summary>
    private float GetEquipLoadModifier()
    {
        if (playerCapacity == null) return 1.0f;

        float currentLoad = playerCapacity.GetCurrentLoad();
        float maxLoad = playerCapacity.GetMaxEquipLoad();
        float loadPercentage = (currentLoad / maxLoad) * 100f;

        if (loadPercentage < 50f)
        {
            return 0.8f; // Giảm 10% time → regen nhanh hơn
        }
        else if (loadPercentage < 75f)
        {
            return 1.0f; // Giữ nguyên
        }
        else if (loadPercentage < 100f)
        {
            return 1.3f; // Tăng 10% time → regen chậm hơn
        }
        else if (loadPercentage < 110f)
        {
            return 1.5f; // Tăng 25% time → regen chậm nhiều
        }
        else
        {
            return 2f; // Tăng 40% time → regen rất chậm
        }
    }

    /// <summary>
    /// Lấy load tier (để debug/UI)
    /// </summary>
    public string GetStaminaRegenTier()
    {
        if (playerCapacity == null) return "Unknown";

        float currentLoad = playerCapacity.GetCurrentLoad();
        float maxLoad = playerCapacity.GetMaxEquipLoad();
        float loadPercentage = (currentLoad / maxLoad) * 100f;

        if (loadPercentage < 50f)
            return "Light Load (Regen +10% faster)";
        else if (loadPercentage < 75f)
            return "Medium Load (Normal Regen)";
        else if (loadPercentage < 100f)
            return "Heavy Load (Regen -10% slower)";
        else if (loadPercentage < 110f)
            return "Overloaded (Regen -25% slower)";
        else
            return "Severely Overloaded (Regen -40% slower)";
    }

    public void OnStaminaUsed()
    {
        staminaJustDropped = true;
        timeSinceStaminaDrop = 0f;
    }

    public void UpdateMaxStaminaFromEndurance()
    {
        if (stats == null) return;

        int baseMaxStamina = CalculateStaminaFromEndurance(stats.endurance);

        // Áp dụng stamina buff từ items
        float buffMultiplier = 1f;
        if (itemController != null)
        {
            float buffPercent = itemController.GetTotalStaminaBuffPercent();
            buffMultiplier += buffPercent / 100f;
        }

        maxStamina = Mathf.RoundToInt(baseMaxStamina * buffMultiplier);

        int bonusStamina = maxStamina - baseStamina;
        autoFillTime = baseAutoFillTime + (bonusStamina * autoFillTimePerStamina);

        if (currentStamina > maxStamina)
            currentStamina = maxStamina;

        if (currentStamina == 0)
            currentStamina = maxStamina;

        if (slider == null) return;

        slider.maxValue = maxStamina;
        slider.value = currentStamina;

        // Debug info
        float modifiedTime = GetModifiedAutoFillTime();
        float regenRate = maxStamina / modifiedTime;
        Debug.Log($"💚 Stamina Regen | Base Time: {autoFillTime:F2}s | Modified: {modifiedTime:F2}s | Rate: {regenRate:F1}/s | Buff Multiplier: {buffMultiplier:F2}");
    }

    public int CalculateStaminaFromEndurance(int end)
    {
        int stamina = baseStamina;

        for (int lv = 1; lv <= end; lv++)
        {
            int gain = 0;

            if (lv <= 15)
            {
                float t = (lv - 1) / 14f;
                gain = Mathf.RoundToInt(Mathf.Lerp(2, 1, t));
            }
            else if (lv <= 30)
            {
                float t = (lv - 16) / 14f;
                gain = Mathf.RoundToInt(Mathf.Lerp(2, 1, t));
            }
            else if (lv <= 50)
            {
                float t = (lv - 31) / 19f;
                gain = Mathf.RoundToInt(Mathf.Lerp(2, 1, t));
            }
            else if (lv <= 99)
            {
                float t = (lv - 51) / 48f;
                gain = Mathf.RoundToInt(Mathf.Lerp(1, 0, t));
            }

            stamina += gain;
        }

        return stamina;
    }

    /// <summary>
    /// Lấy stamina regen rate hiện tại (stamina/second)
    /// </summary>
    public float GetCurrentRegenRate()
    {
        float modifiedTime = GetModifiedAutoFillTime();
        return maxStamina / modifiedTime;
    }
}