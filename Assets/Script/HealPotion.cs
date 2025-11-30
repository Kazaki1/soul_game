using UnityEngine;

public class HealPotion : MonoBehaviour
{
    [Header("Potion Settings")]
    public int healAmount = 50;
    public int potionCount = 3;
    public float useCooldown = 1f;

    private float lastUseTime = -999f;

    public void UsePotion()
    {
        if (Time.time - lastUseTime < useCooldown) return;
        if (potionCount <= 0) return;
        if (PlayerHealth.Instance == null) return;

        PlayerHealth.Instance.Heal(healAmount);
        potionCount--;
        lastUseTime = Time.time;
    }
}
