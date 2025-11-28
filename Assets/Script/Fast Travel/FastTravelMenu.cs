using UnityEngine;
using UnityEngine.UI;
using TMPro; // nếu dùng TextMeshPro

public class FastTravelMenu : MonoBehaviour
{
    public GameObject menuUI;        // gán FastTravelMenu
    public Transform buttonParent;   // gán Content 
    public Button buttonPrefab;      // gán TravelButtonPrefab (prefab)
    public GameObject player;        // gán player hoặc để null để tìm tự động

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        menuUI.SetActive(false);
    }

    // Mở menu — mỗi lần mở sẽ rebuild danh sách (ngăn duplicate)
    public void OpenMenu()
    {
        RefreshMenu();
        menuUI.SetActive(true);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
        ClearContainer();
    }

    void RefreshMenu()
    {
        ClearContainer();

        if (FastTravelSystem.instance == null)
        {
            Debug.LogError("FastTravelSystem.instance == null");
            return;
        }

        foreach (FastTravelPoint p in FastTravelSystem.instance.travelPoints)
        {
            // Nếu muốn chỉ hiện điểm đã unlock, uncomment:
            // if (!p.unlocked) continue;

            var point = p; // tạo bản copy local để tránh lỗi closure trong vòng lặp
            Button btn = Instantiate(buttonPrefab, buttonParent);
            // set tên (hỗ trợ TMP hoặc legacy Text)
            TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
            if (tmp != null) tmp.text = point.pointName;
            else
            {
                Text legacy = btn.GetComponentInChildren<Text>();
                if (legacy != null) legacy.text = point.pointName;
            }

            // nếu muốn ẩn / disable nút khi chưa mở khóa:
            btn.interactable = point.unlocked;
            Transform lockIcon = btn.transform.Find("LockIcon");
            if (lockIcon != null) lockIcon.gameObject.SetActive(!point.unlocked);

            if (point.unlocked)
            {
                btn.onClick.AddListener(() =>
                {
                    FastTravelSystem.instance.PlayerEnteredPoint(point, player);
                    CloseMenu();
                });
            }
        }
    }

    void ClearContainer()
    {
        for (int i = buttonParent.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonParent.GetChild(i).gameObject);
        }
    }
}
