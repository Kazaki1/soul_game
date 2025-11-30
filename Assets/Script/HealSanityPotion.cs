using UnityEngine;

public class HealSanityPotion : MonoBehaviour
{
    [Header("Potion Settings")]
    public float sanityHealAmount = 30f;
    public int potionCount = 3;
    public float useCooldown = 1f;

    private float lastUseTime = -999f;
    private Sanity targetSanity;

    public void SetSanityReference(Sanity sanity)
    {
        targetSanity = sanity;
    }

    public void UsePotion()
    {
        if (targetSanity == null) return;
        if (Time.time - lastUseTime < useCooldown) return;
        if (potionCount <= 0) return;

        targetSanity.IncreaseSanity(sanityHealAmount);
        potionCount--;
        lastUseTime = Time.time;
    }
}
