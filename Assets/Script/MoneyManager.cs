using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int currentMoney = 100;
    public TextMeshProUGUI moneyText;
    private const string MoneyKey = "PlayerMoney";

    private void Awake()
    {
        // Singleton đơn giản
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadMoney();
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        currentMoney += amount;
        UpdateMoneyUI();
        SaveMoney();
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0) return false;
        if (currentMoney < amount)
        {
            Debug.Log("MoneyManager: Không đủ tiền!");
            return false;
        }
        currentMoney -= amount;
        UpdateMoneyUI();
        SaveMoney();
        return true;
    }

    public bool HasEnoughMoney(int amount) => currentMoney >= amount;

    // Thêm method này để lấy số tiền hiện tại
    public int GetCurrentMoney() => currentMoney;

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = currentMoney.ToString();
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt(MoneyKey, currentMoney);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        currentMoney = PlayerPrefs.GetInt(MoneyKey, currentMoney);
    }

    [ContextMenu("ResetMoney")]
    public void ResetMoney()
    {
        currentMoney = 0;
        SaveMoney();
        UpdateMoneyUI();
    }
}