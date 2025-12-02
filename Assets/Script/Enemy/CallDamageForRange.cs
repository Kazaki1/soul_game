using UnityEngine;

public class CallDamageForRange : MonoBehaviour
{
    public EnemyDamageDealRange damageDealer;

    private void Start()
    {
        if (damageDealer == null)
        {
            damageDealer = GetComponent<EnemyDamageDealRange>();
            if (damageDealer == null)
            {
                Debug.LogError($"❌ EnemyDamageDeal component not found on {gameObject.name}! Please add it.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageDealer != null)
        {
            if (other.CompareTag("Player"))
            {
                damageDealer.DealDamageOnTrigger(other);
                Destroy(gameObject);
            }
        }
    }
}
