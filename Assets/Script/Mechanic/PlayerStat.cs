using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Attributes")]
    public int vigor = 1;
    public int strength = 1;
    public int dexterity = 1;
    public int intelligence = 1;
    public int endurance = 1;

    public PlayerHealth playerHealth;
    public Stamina stamina;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        stamina = GetComponent<Stamina>();

        // Cập nhật health và stamina ban đầu
        playerHealth.UpdateMaxHealthFromVigor();
        if (stamina != null)
            stamina.UpdateMaxStaminaFromEndurance();
    }

    public void IncreaseStat(string statName, int amount)
    {
        switch (statName.ToLower())
        {
            case "vigor":
            case "vig":
                vigor += amount;
                playerHealth.UpdateMaxHealthFromVigor();
                break;

            case "str":
            case "strength":
                strength += amount;
                break;

            case "dex":
            case "dexterity":
                dexterity += amount;
                break;

            case "int":
            case "intelligence":
                intelligence += amount;
                break;

            case "end":
            case "endurance":
                endurance += amount;
                if (stamina != null)
                    stamina.UpdateMaxStaminaFromEndurance(); // 🔥 Cập nhật stamina khi tăng endurance
                break;

            default:
                Debug.LogWarning("Tên stat không hợp lệ: " + statName);
                break;
        }
    }
}