using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    public float knockbackDuration = 0.1f;
    public float flashDuration = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;
    private bool isKnocked = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (!isKnocked)
            StartCoroutine(KnockbackRoutine(direction, force));
    }

    private System.Collections.IEnumerator KnockbackRoutine(Vector2 direction, float force)
    {
        isKnocked = true;

        rb.linearVelocity = direction * force;

        sr.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        sr.color = originalColor;

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isKnocked = false;
    }
}
