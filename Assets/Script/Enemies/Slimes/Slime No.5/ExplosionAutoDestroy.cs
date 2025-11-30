using UnityEngine;

public class ExplosionAutoDestroy : MonoBehaviour
{
    private Animator anim;
    private float animLength;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Lấy thời lượng animation đầu tiên
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            var clip = anim.runtimeAnimatorController.animationClips[0];
            animLength = clip.length;
        }
        else
        {
            animLength = 0.5f; // fallback nếu không tìm thấy animation
        }

        // Hủy sau khi phát xong animation
        Destroy(gameObject, animLength);
    }
}
