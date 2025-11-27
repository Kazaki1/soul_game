using UnityEngine;

public class ItemStats : MonoBehaviour
{
    [SerializeField] private float bonus = 10f;
    [SerializeField] private float weight = 2f;

    public float GetItemBonus() => bonus;
    public float GetItemWeight() => weight;
}