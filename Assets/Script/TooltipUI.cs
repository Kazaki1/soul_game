using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI descText;
    private Canvas canvas;
    private RectTransform panelRect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panel == null) return;

        panel.SetActive(false);

        canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (panel != null)
            panelRect = panel.GetComponent<RectTransform>();
    }

    public void ShowTooltip(Scriptable_object item, Vector2 pos)
    {
        if (item == null || panel == null) return;

        panel.SetActive(true);

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            panelRect.position = pos;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                pos,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            panelRect.localPosition = localPoint;
        }

        nameText.text = item.item_name;
        descText.text = item.description;
        statsText.text = BuildStats(item);
    }

    public void HideTooltip()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private string BuildStats(Scriptable_object item)
    {
        string stats = $"<b>Type:</b> {item.itemType}\n";

        switch (item.itemType)
        {
            case ItemType.Consumable:
                if (item.healAmount > 0)
                    stats += $"<b>Heal:</b> +{item.healAmount} HP\n";
                if (item.IsUseableAnywhere)
                    stats += $"<color=green>✓ Useable Anywhere</color>\n";
                break;

            case ItemType.Weapon:
                if (item.damage > 0)
                    stats += $"<b>Damage:</b> {item.damage}\n";
                if (item.weaponPrefab != null)
                {
                    WeaponStats weaponStats = item.weaponPrefab.GetComponent<WeaponStats>();
                    if (weaponStats != null)
                        stats += $"<b>Weight:</b> {weaponStats.GetWeaponWeight():F1}\n";
                }
                if (item.weaponAnimator != null)
                    stats += $"<color=yellow>Has Animation</color>\n";
                break;

            case ItemType.Armor:
                if (item.defense > 0)
                    stats += $"<b>Defense:</b> +{item.defense}\n";
                break;

            case ItemType.Quest:
                break;
        }

        return stats.TrimEnd('\n');
    }
}
