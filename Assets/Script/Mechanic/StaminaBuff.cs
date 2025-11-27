using UnityEngine;

public class StaminaBuff : MonoBehaviour
{
    [Header("Stamina Buff Settings")]
    [Tooltip("Phần trăm tăng stamina (ví dụ: 10 = +10%)")]
    [SerializeField] private float buffPercent = 10f;

    public float GetBuffPercent()
    {
        return buffPercent;
    }
}