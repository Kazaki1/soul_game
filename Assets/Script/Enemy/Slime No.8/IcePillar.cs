using UnityEngine;
using System.Collections;

public class IcePillar : MonoBehaviour
{
    private int damage;
    private float lifetime;
    private bool canDamage = false; // Chưa được phép gây sát thương

    public void Init(int dmg, float time)
    {
        damage = dmg;
        lifetime = time;
    }

    private void Start()
    {
        // Bắt đầu đếm thời gian kích hoạt gây damage
        StartCoroutine(ActivateDamageAfterDelay(1f));

        // Tự hủy sau thời gian tồn tại
        Destroy(gameObject, lifetime);
    }

    IEnumerator ActivateDamageAfterDelay(float delay)
    {
        // ⏳ Chờ 1 giây trước khi có thể gây sát thương
        yield return new WaitForSeconds(delay);
        canDamage = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Chỉ gây damage nếu player vẫn còn trong vùng sau 1 giây
        if (canDamage && other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage(damage);
                canDamage = false; // Chỉ gây 1 lần
            }
        }
    }
}
