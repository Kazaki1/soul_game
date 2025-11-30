using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InventorySampleScene;

public class PlayerHealth : MonoBehaviour
{
    int maxHealth;
    int currentHealth;

    [Header("Base Stats")]
    public int baseHealth = 100;
    public HealthBar healthBar;

    private PlayerStats stats;
    private Stamina playerStamina;

    public Image blackScreen;
    public float respawnDelay = 5f;

    private bool isDead = false;
    private bool isRespawning = false;

    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerStamina = GetComponent<Stamina>();
        Instance = this;
    }

    private void Start()
    {
        if (stats != null)
            UpdateMaxHealthFromVigor(true);

        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0;
            blackScreen.color = c;
            blackScreen.gameObject.SetActive(true);
        }
    }

    public void UpdateMaxHealthFromVigor(bool fullHeal = true)
    {
        if (stats == null) return;

        maxHealth = CalculateHealthFromVigor(stats.vigor);
        currentHealth = fullHeal ? maxHealth : Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    public int CalculateHealthFromVigor(int vigor)
    {
        int hp = baseHealth;
        for (int lv = 1; lv <= vigor; lv++)
        {
            int gain;
            if (lv <= 40)
                gain = Mathf.RoundToInt(Mathf.Lerp(48, 4, (lv - 1) / 39f));
            else if (lv <= 60)
                gain = Mathf.RoundToInt(Mathf.Lerp(26, 13, (lv - 41) / 19f));
            else
                gain = Mathf.RoundToInt(Mathf.Lerp(6, 3, (lv - 61) / 38f));
            hp += gain;
        }
        return hp;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateHealthBar();
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(DeathAndRespawn());
        }
    }

    private IEnumerator DeathAndRespawn()
    {
        if (blackScreen != null)
        {
            float t = 0;
            Color c = blackScreen.color;
            while (t < 1f)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0, 1, t);
                blackScreen.color = c;
                yield return null;
            }
        }

        yield return new WaitForSeconds(respawnDelay);

        RespawnAtCheckpoint();

        if (blackScreen != null)
        {
            float t = 0;
            Color c = blackScreen.color;
            while (t < 1f)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(1, 0, t);
                blackScreen.color = c;
                yield return null;
            }
        }
    }

    private void RespawnAtCheckpoint()
    {
        if (isRespawning) return;
        isRespawning = true;

        if (!SaveSystem.HasSave())
        {
            SetFullHealth();
            if (playerStamina != null)
                playerStamina.currentStamina = playerStamina.maxStamina;
            FinishRespawn();
            return;
        }

        SaveData snapshot = SaveSystem.LoadSnapshot();
        if (snapshot != null)
        {
            transform.position = snapshot.playerPos;
            transform.eulerAngles = snapshot.playerRot;
            currentHealth = Mathf.Clamp(Mathf.RoundToInt(snapshot.health), 0, maxHealth);
            if (playerStamina != null)
                playerStamina.currentStamina = snapshot.stamina;
            UpdateHealthBar();
        }
        else
        {
            SetFullHealth();
            if (playerStamina != null)
                playerStamina.currentStamina = playerStamina.maxStamina;
        }

        FinishRespawn();
    }

    private void FinishRespawn()
    {
        isDead = false;
        isRespawning = false;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar();
    }

    public int GetMaxHealth() => maxHealth;
    public int GetCurrentHealth() => currentHealth;

    public void SetFullHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}
