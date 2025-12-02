using UnityEngine;

public class AnimationHitboxController : MonoBehaviour
{
    public GameObject[] hitboxes;

    public bool showDebugLogs = true;

    public void EnableHitbox()
    {
        SetHitboxesActive(true);
    }

    public void DisableHitbox()
    {
        SetHitboxesActive(false);
    }
    public void EnableHitboxByIndex(int index)
    {
        if (hitboxes == null || index < 0 || index >= hitboxes.Length)
        {
            return;
        }

        if (hitboxes[index] != null)
        {
            hitboxes[index].SetActive(true);
        }
    }

    public void DisableHitboxByIndex(int index)
    {
        if (hitboxes == null || index < 0 || index >= hitboxes.Length)
        {
            return;
        }

        if (hitboxes[index] != null)
        {
            hitboxes[index].SetActive(false);
        }
    }
    private void SetHitboxesActive(bool active)
    {
        if (hitboxes == null) return;

        foreach (GameObject hitbox in hitboxes)
        {
            if (hitbox != null)
            {
                hitbox.SetActive(active);
            }
        }
    }

    private void Start()
    {
        DisableHitbox();
    }
}