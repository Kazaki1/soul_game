using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    [Header("Weapon Base Damage")]
    [SerializeField] private float baseDamage = 50f;

    [Header("Stat Scaling (A=1.0, B=0.75, C=0.5, D=0.25, E=0.1)")]
    [SerializeField][Range(0f, 1f)] private float strScale = 0.75f;
    [SerializeField][Range(0f, 1f)] private float dexScale = 0.5f;
    [SerializeField][Range(0f, 1f)] private float intScale = 0.25f;

    [Header("Attack Properties")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private int staminaCost = 10;

    [Header("Equipment Load")]
    [SerializeField] private float weaponWeight = 5f;

    [Header("Weapon Type (Animator Parameter)")]
    [SerializeField] private int weaponType = 0;

    // Getters - Damage & Scaling
    public float GetBaseDamage() => baseDamage;
    public float GetStrScale() => strScale;
    public float GetDexScale() => dexScale;
    public float GetIntScale() => intScale;

    // Getters - Attack Properties
    public float GetAttackCooldown() => attackCooldown;
    public int GetStaminaCost() => staminaCost;

    // Getters - Weight & Type
    public float GetWeaponWeight() => weaponWeight;
    public int GetWeaponType() => weaponType;  // ← THÊM

    // Setters - Damage & Scaling
    public void SetBaseDamage(float value) => baseDamage = value;
    public void SetStrScale(float value) => strScale = Mathf.Clamp01(value);
    public void SetDexScale(float value) => dexScale = Mathf.Clamp01(value);
    public void SetIntScale(float value) => intScale = Mathf.Clamp01(value);

    // Setters - Attack Properties
    public void SetAttackCooldown(float value) => attackCooldown = Mathf.Max(0f, value);
    public void SetStaminaCost(int value) => staminaCost = Mathf.Max(0, value);

    // Setters - Weight & Type
    public void SetWeaponWeight(float value) => weaponWeight = Mathf.Max(0f, value);
    public void SetWeaponType(int value) => weaponType = Mathf.Max(0, value);
}