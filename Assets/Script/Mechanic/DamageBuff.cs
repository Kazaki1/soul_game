using UnityEngine;

public class DamageBuff : MonoBehaviour
{
    [Header("Damage Buff Settings")]
    [Tooltip("Phần trăm tăng sát thương (ví dụ: 10 = +10%)")]
    [SerializeField] private float buffPercent = 10f;

    public float GetBuffPercent()
    {
        return buffPercent;
    }
}