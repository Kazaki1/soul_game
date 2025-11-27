using UnityEngine;

public class ArmorStats : MonoBehaviour
{
    public enum ArmorType
    {
        Head,      
        Chest,     
        Legs,       
        Boots       
    }

    [Header("Armor Category")]
    [SerializeField] private ArmorType armorType;

    [Header("Armor Defense")]
    [SerializeField] private float armorDefense = 10f;

    [Header("Equipment Load")]
    [SerializeField] private float armorWeight = 5f;


    // ===== Getters =====
    public ArmorType GetArmorType() => armorType;
    public float GetArmorDefense() => armorDefense;
    public float GetArmorWeight() => armorWeight;

    // ===== Setters =====
    public void SetArmorType(ArmorType type) => armorType = type;
    public void SetArmorDefense(float value) => armorDefense = Mathf.Max(0f, value);
    public void SetArmorWeight(float value) => armorWeight = Mathf.Max(0f, value);
}
