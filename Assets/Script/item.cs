using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    [Header("Dữ liệu item")]
    public Scriptable_object itemData;

    [Header("Pickup Hint (Optional)")]
    public GameObject pickupHintPrefab;   // KHÔNG bắt buộc phải có
    private GameObject pickupHintInstance;

    private Transform player;
    private bool isPlayerNear = false;
    private Canvas mainCanvas;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) return;

        // Nếu có gán hint thì mới tạo
        if (pickupHintPrefab != null)
        {
            mainCanvas = FindObjectOfType<Canvas>();

            if (mainCanvas != null)
            {
                pickupHintInstance = Instantiate(pickupHintPrefab, mainCanvas.transform);
                pickupHintInstance.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (player == null || itemData == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < 1.5f)
        {
            if (!isPlayerNear)
            {
                isPlayerNear = true;

                if (pickupHintInstance != null)
                    pickupHintInstance.SetActive(true);
            }

            // Cập nhật vị trí UI hint theo item
            if (pickupHintInstance != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
                pickupHintInstance.transform.position = screenPos;
            }

            // Nhấn E để nhặt item
            if (Input.GetKeyDown(KeyCode.E))
            {
                Pickup();
            }
        }
        else
        {
            if (isPlayerNear)
            {
                isPlayerNear = false;

                if (pickupHintInstance != null)
                    pickupHintInstance.SetActive(false);
            }
        }
    }

    private void Pickup()
    {
        if (itemData == null) return;

        Inventory_mananegment inv = Inventory_mananegment.Instance;
        if (inv == null) return;

        bool added = inv.Add(itemData);
        if (added)
        {
            Debug.Log($"✅ Đã nhặt item '{itemData.item_name}'");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("⚠️ Inventory đầy, không thể nhặt item");
        }
    }

    private void OnDestroy()
    {
        // Xóa hint nếu có
        if (pickupHintInstance != null)
            Destroy(pickupHintInstance);
    }
}
