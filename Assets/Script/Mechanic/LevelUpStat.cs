using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpStat : MonoBehaviour
{
    [Header("UI References")]
    public Slider targetSlider;
    public TextMeshProUGUI costText;      // Hiển thị cost (optional)

    [Header("Stat Settings")]
    public StatType statType;
    public float x = 1f;

    [Header("Player Reference")]
    public PlayerStats playerStats;

    private void Start()
    {
        // Tự động tìm PlayerStats nếu chưa assign
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();

            if (playerStats == null)
            {
                Debug.LogError($"❌ PlayerStats not found for {statType}!");
                return;
            }
        }

        // Khởi tạo slider với giá trị từ PlayerStats
        InitializeSlider();
    }

    private void Update()
    {
        // Cập nhật UI liên tục
        UpdateUI();
    }

    /// <summary>
    /// Khởi tạo slider với giá trị từ PlayerStats
    /// </summary>
    private void InitializeSlider()
    {
        if (targetSlider == null || playerStats == null) return;

        int currentStatValue = GetCurrentStatValue();
        targetSlider.value = currentStatValue;

        Debug.Log($"✅ Initialized {statType} slider to {currentStatValue}");
    }

    /// <summary>
    /// Lấy giá trị stat hiện tại từ PlayerStats
    /// </summary>
    private int GetCurrentStatValue()
    {
        if (playerStats == null) return 1;

        switch (statType)
        {
            case StatType.Vigor:
                return playerStats.vigor;
            case StatType.Strength:
                return playerStats.strength;
            case StatType.Dexterity:
                return playerStats.dexterity;
            case StatType.Intelligence:
                return playerStats.intelligence;
            case StatType.Endurance:
                return playerStats.endurance;
            default:
                return 1;
        }
    }

    /// <summary>
    /// Cập nhật UI (cost text)
    /// </summary>
    private void UpdateUI()
    {
        if (targetSlider == null) return;

        // Đồng bộ slider với PlayerStats
        int currentStatValue = GetCurrentStatValue();
        if (targetSlider.value != currentStatValue)
        {
            targetSlider.value = currentStatValue;
        }

        // Cập nhật cost text
        if (costText != null)
        {
            int cost = PreviewCost();
            costText.text = $"Cost: {cost}";
        }
    }

    /// <summary>
    /// Tăng stat (gọi khi nhấn nút Level Up)
    /// </summary>
    public void IncreaseStat()
    {
        if (targetSlider == null || playerStats == null) return;

        int currentStatValue = GetCurrentStatValue();
        int soulLevel = playerStats.soulLevel;
        int cost = CalculateCost(soulLevel); // Cost dựa trên Soul Level, không phải stat cụ thể

        // Kiểm tra có đủ tiền không
        if (MoneyManager.Instance == null || !MoneyManager.Instance.SpendMoney(cost))
        {
            Debug.Log($"❌ Not enough money! Need: {cost} (Soul Level: {soulLevel})");
            return;
        }

        // Kiểm tra có đạt max level chưa
        if (currentStatValue >= targetSlider.maxValue)
        {
            Debug.Log($"⚠️ {statType} already at max level!");
            return;
        }

        // Tăng stat trong PlayerStats
        playerStats.IncreaseStat(GetStatName(), 1);

        // Cập nhật slider (sẽ tự động sync trong Update)
        Debug.Log($"✅ Increased {statType}: {currentStatValue} → {currentStatValue + 1} | Soul Level: {soulLevel} → {soulLevel + 1} (Cost: {cost})");
    }

    /// <summary>
    /// Tính cost cho level tiếp theo dựa trên SOUL LEVEL (tổng tất cả stats)
    /// </summary>
    public int PreviewCost()
    {
        if (playerStats == null) return 0;

        // Lấy Soul Level (tổng của tất cả stats)
        int soulLevel = playerStats.soulLevel;

        return CalculateCost(soulLevel);
    }

    /// <summary>
    /// Formula tính cost: (x + 0.1) × (Soul Level + 81)² + 1
    /// </summary>
    private int CalculateCost(int soulLevel)
    {
        return Mathf.RoundToInt((x + 0.1f) * Mathf.Pow(soulLevel + 81, 2) + 1);
    }

    /// <summary>
    /// Lấy tên stat để truyền vào PlayerStats.IncreaseStat()
    /// </summary>
    private string GetStatName()
    {
        switch (statType)
        {
            case StatType.Vigor:
                return "vigor";
            case StatType.Strength:
                return "strength";
            case StatType.Dexterity:
                return "dexterity";
            case StatType.Intelligence:
                return "intelligence";
            case StatType.Endurance:
                return "endurance";
            default:
                return "";
        }
    }
}

/// <summary>
/// Enum để chọn stat type trong Inspector
/// </summary>
public enum StatType
{
    Vigor,
    Strength,
    Dexterity,
    Intelligence,
    Endurance
}