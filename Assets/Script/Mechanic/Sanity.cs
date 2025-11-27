using UnityEngine;
using UnityEngine.UI;

public class Sanity : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;

    [Header("Base Stats")]
    public int baseSanity = 100;
    public int maxSanity;
    public float currentSanity;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
        }
    }

    private void Start()
    {
        UpdateMaxSanityFromIntelligence();
    }

    /// <summary>
    /// Cập nhật Max Sanity dựa trên Intelligence
    /// Level 1-15: +3 to +4 (avg: +3.21)
    /// Level 16-35: +5 to +6 (avg: +5.25)
    /// Level 36-50: +7 to +6 (avg: +6.67)
    /// Level 51-60: +6 to +4 (avg: +5.00)
    /// Level 61-99: +2 to +3 (avg: +2.56)
    /// </summary>
    public void UpdateMaxSanityFromIntelligence()
    {
        if (playerStats == null) return;

        maxSanity = CalculateSanityFromIntelligence(playerStats.intelligence);

        // Khởi tạo current sanity
        if (currentSanity == 0)
        {
            currentSanity = maxSanity;
        }

        // Clamp current sanity
        if (currentSanity > maxSanity)
        {
            currentSanity = maxSanity;
        }

        UpdateUI();

        Debug.Log($"🧠 Sanity Updated | INT: {playerStats.intelligence} | Max Sanity: {maxSanity}");
    }

    /// <summary>
    /// Tính Max Sanity từ Intelligence level
    /// </summary>
    private int CalculateSanityFromIntelligence(int intLevel)
    {
        int sanity = baseSanity;

        for (int lv = 1; lv <= intLevel; lv++)
        {
            int gain = 0;

            if (lv <= 15)
            {
                gain = (lv % 2 == 1) ? 3 : 4;
            }
            else if (lv <= 35)
            {
                gain = (lv % 2 == 0) ? 5 : 6;
            }
            else if (lv <= 50)
            {
                float t = (lv - 36) / 14f;
                gain = Mathf.RoundToInt(Mathf.Lerp(7, 6, t));
            }
            else if (lv <= 60)
            {
                float t = (lv - 51) / 9f;
                gain = Mathf.RoundToInt(Mathf.Lerp(6, 4, t));
            }
            else if (lv <= 99)
            {
                gain = (lv % 2 == 1) ? 2 : 3;
            }

            sanity += gain;
        }

        return sanity;
    }

    /// <summary>
    /// Giảm Sanity
    /// </summary>
    public void DecreaseSanity(float amount)
    {
        currentSanity -= amount;

        if (currentSanity < 0)
        {
            currentSanity = 0;
        }

        UpdateUI();
    }

    /// <summary>
    /// Tăng Sanity
    /// </summary>
    public void IncreaseSanity(float amount)
    {
        currentSanity += amount;

        if (currentSanity > maxSanity)
        {
            currentSanity = maxSanity;
        }

        UpdateUI();
    }

    /// <summary>
    /// Update UI slider
    /// </summary>
    private void UpdateUI()
    {
        if (slider != null)
        {
            slider.maxValue = maxSanity;
            slider.value = currentSanity;
        }
    }

    /// <summary>
    /// Kiểm tra có đủ sanity không
    /// </summary>
    public bool HasEnoughSanity(float amount)
    {
        return currentSanity >= amount;
    }

    /// <summary>
    /// Lấy % Sanity hiện tại (0-1)
    /// </summary>
    public float GetSanityPercentage()
    {
        if (maxSanity <= 0) return 0f;
        return currentSanity / maxSanity;
    }

    // Getters
    public float GetCurrentSanity() => currentSanity;
    public int GetMaxSanity() => maxSanity;
    public int GetBaseSanity() => baseSanity;

    // Setter
    public void SetBaseSanity(int value)
    {
        baseSanity = value;
        UpdateMaxSanityFromIntelligence();
    }
}