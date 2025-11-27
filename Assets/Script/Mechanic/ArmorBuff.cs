using UnityEngine;

public class ArmorBuff : MonoBehaviour
{
    [Header("Armor Buff Settings")]
    [Tooltip("Phần trăm tăng giáp (ví dụ: 10 = +10%)")]
    [SerializeField] private float buffPercent = 10f;

    public float GetBuffPercent()
    {
        return buffPercent;
    }
}