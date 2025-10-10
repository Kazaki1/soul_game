using UnityEngine;

public class Shop_NPC : MonoBehaviour
{
    public Animator anim;
    public GameObject shopCanvas;

    private bool playerInRange = false;

    private void Start()
    {
        if (shopCanvas != null)
            shopCanvas.SetActive(false); // Ẩn ShopCanvas lúc đầu
    }

    private void Update()
    {
        // Khi người chơi ở trong vùng và nhấn E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (shopCanvas != null)
            {
                bool isActive = shopCanvas.activeSelf;
                shopCanvas.SetActive(!isActive); // Bật/tắt ShopCanvas
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem Player có tag là "Player" không
        if (collision.CompareTag("Player"))
        {
            if (anim != null)
                anim.SetBool("playerInRange", true);

            playerInRange = true;
            Debug.Log("Player đã vào vùng shop");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (anim != null)
                anim.SetBool("playerInRange", false);

            playerInRange = false;

            if (shopCanvas != null)
                shopCanvas.SetActive(false); // Ẩn shop khi người chơi rời xa
            Debug.Log("Player đã rời vùng shop");
        }
    }
}
