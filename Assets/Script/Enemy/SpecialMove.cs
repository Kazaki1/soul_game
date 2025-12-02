using UnityEngine;

public class SpecialMove : MonoBehaviour
{
    [Header("Special Move Settings")]
    public int hitsBeforeSpecial = 2;      // Sau 5 hit thường thì hit thứ 6 là đặc biệt
    public float specialDamageMultiplier = 5f;
    public float specialRangeMultiplier = 3f;

    private int currentHitCount = 0;
    private MediumEnemy enemy;

    void Awake()
    {
        enemy = GetComponent<MediumEnemy>();
    }

    /// <summary>
    /// Gọi hàm này mỗi khi enemy chuẩn bị đánh melee.
    /// </summary>
    public bool IsSpecialAttack()
    {
        if (currentHitCount >= hitsBeforeSpecial)
        {
            currentHitCount = 0; // reset sau special attack
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gọi khi đánh thường thành công.
    /// </summary>
    public void RegisterNormalHit()
    {
        currentHitCount++;
    }
}
