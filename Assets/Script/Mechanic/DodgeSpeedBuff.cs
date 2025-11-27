using UnityEngine;

public class DodgeSpeedBuff : MonoBehaviour
{
    [Header("Dodge Speed Buff Settings")]
    [Tooltip("Phần trăm tăng tốc độ dodge (ví dụ: 10 = +10%)")]
    [SerializeField] private float buffPercent = 10f;

    public float GetBuffPercent()
    {
        return buffPercent;
    }
}