using UnityEngine;

public class HealthBuff : MonoBehaviour
{
    [Header("Health Buff Settings")]
    [Tooltip("Phần trăm tăng máu (ví dụ: 10 = +10%)")]
    [SerializeField] private float buffPercent = 10f;

    public float GetBuffPercent()
    {
        return buffPercent;
    }
}