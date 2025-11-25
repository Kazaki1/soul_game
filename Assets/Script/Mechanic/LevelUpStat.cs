using UnityEngine;
using UnityEngine.UI;

public class LevelUpStat : MonoBehaviour
{
    public Slider targetSlider;    
    public float x = 1f;           

    public void IncreaseStat()
    {
        if (targetSlider == null) return;

        int currentLevel = (int)targetSlider.value;

        int cost = Mathf.RoundToInt((x + 0.1f) * Mathf.Pow(currentLevel + 81, 2) + 1);

        if (MoneyManager.Instance.SpendMoney(cost))
        {
            targetSlider.value = Mathf.Clamp(
                targetSlider.value + 1,
                targetSlider.minValue,
                targetSlider.maxValue
            );
        }
        else
        {
            Debug.Log("Không đủ tiền để nâng cấp! Cần: " + cost);
        }
    }

    public int PreviewCost()
    {
        int currentLevel = (int)targetSlider.value;
        int cost = Mathf.RoundToInt((x + 0.1f) * Mathf.Pow(currentLevel + 81, 2) + 1);
        return cost;
    }
}
