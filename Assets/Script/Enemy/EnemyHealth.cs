using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Money Reward")]
    public int rewardMoney = 10;
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        EnemyDefense defense = GetComponent<EnemyDefense>();
        if (defense != null)
        {
            damage = defense.ModifyDamage(damage);
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(rewardMoney);
        }
        Destroy(gameObject);
    }
}