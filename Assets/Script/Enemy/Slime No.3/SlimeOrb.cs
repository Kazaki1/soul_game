using UnityEngine;

public class SlimeOrb : MonoBehaviour
{
    private SlimeAtk_Orb owner;
    private int damage;

    public void Init(SlimeAtk_Orb slime, int dmg)
    {
        owner = slime;
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.TakeDamage(damage);

            if (owner != null)
                owner.NotifyOrbDestroyed(gameObject);

            Destroy(gameObject);
        }
    }
}
