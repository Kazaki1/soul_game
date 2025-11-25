using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int currentMoney = 0;

    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
        SaveMoney();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney < amount)
        {
            Debug.Log("Không đủ tiền!");
            return false;
        }

        currentMoney -= amount;
        SaveMoney();
        UpdateMoneyUI();
        return true;
    }

    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }
    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = currentMoney.ToString();
    }

    private void SaveMoney()
    {
        //PlayerPrefs.SetInt("PlayerMoney", currentMoney);
        //PlayerPrefs.Save();
    }
}