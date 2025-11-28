using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FastTravelSystem : MonoBehaviour
{
    public static FastTravelSystem instance;

    [Header("Fast Travel Points")]
    public List<FastTravelPoint> travelPoints = new List<FastTravelPoint>();

    [Header("UI & Confirm")]
    public ConfirmUI confirmUI;

    private FastTravelPoint selectedPoint;
    private GameObject currentPlayer;

    private bool isTraveling = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    /// <summary>
    /// Gọi khi player đi vào collider điểm Fast Travel
    /// </summary>
    public void PlayerEnteredPoint(FastTravelPoint point, GameObject player)
    {
        currentPlayer = player;
        selectedPoint = point;

        if (point.unlocked)
        {
            StartCoroutine(TravelRoutine(point));
        }
        else
        {
            // Hiển thị Confirm Panel
            confirmUI.Show(
                $"Bạn có muốn mở khóa {point.pointName} với giá {point.unlockCost} tiền không?",
                OnConfirmYes,
                OnConfirmNo
            );
        }
    }

    private void OnConfirmYes()
    {
        if (selectedPoint == null) return;

        // DÙNG MoneyManager THAY PlayerStats
        if (MoneyManager.Instance.SpendMoney(selectedPoint.unlockCost))
        {
            selectedPoint.unlocked = true;
            Debug.Log($"Mở khóa {selectedPoint.pointName} - Trừ {selectedPoint.unlockCost} tiền!");

            StartCoroutine(TravelRoutine(selectedPoint));
        }
        else
        {
            Debug.Log("Không đủ tiền để mở " + selectedPoint.pointName);
        }
    }

    private void OnConfirmNo()
    {
        Debug.Log("Người chơi từ chối mở khóa Fast Travel.");
    }

    private IEnumerator TravelRoutine(FastTravelPoint point)
    {
        if (isTraveling) yield break;
        isTraveling = true;

        // Reset alpha & bật fade UI
        FastTravelTransition.instance.fadeUI.alpha = 0.001f;
        FastTravelTransition.instance.fadeUI.gameObject.SetActive(true);

        // Fade màn hình vào tối
        yield return FastTravelTransition.instance.FadeIn();

        if (currentPlayer != null)
        {
            currentPlayer.transform.position = point.transform.position;
            Debug.Log("Dịch chuyển tới: " + point.pointName);
        }

        yield return null;

        // Fade trở lại sáng
        yield return FastTravelTransition.instance.FadeOut();

        isTraveling = false;
    }
}
