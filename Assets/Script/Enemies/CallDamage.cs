using UnityEngine;

public class CallDamage : MonoBehaviour
{
    public EnemyDamageDeal damageDealer;
    private void Start()
    {
        if (damageDealer == null)
        {
            damageDealer = GetComponent<EnemyDamageDeal>();
            if (damageDealer == null)
            {
                Debug.LogError($"❌ EnemyDamageDeal component not found on {gameObject.name}! Please add it.");
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageDealer != null)
        {
            damageDealer.DealDamageOnCollision(collision);
            Destroy(gameObject);
        }
    }
}