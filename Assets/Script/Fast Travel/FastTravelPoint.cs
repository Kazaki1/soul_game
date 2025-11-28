using UnityEngine;

public class FastTravelPoint : MonoBehaviour
{
    public string pointName;        // tên điểm fast travel
    public int unlockCost = 100;    // giá mở khóa
    public bool unlocked = false;   // trạng thái mở khóa

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Gửi sự kiện đến FastTravelSystem
            FastTravelSystem.instance.PlayerEnteredPoint(this, collision.gameObject);
        }
    }
}

