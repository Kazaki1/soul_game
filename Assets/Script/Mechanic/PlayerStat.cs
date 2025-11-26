using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Attributes")]
    public int vigor = 1;
    public int strength = 1;
    public int dexterity = 1;
    public int intelligence = 1;
    public int endurance = 1;

    [Header("Component References")]
    public PlayerHealth playerHealth;
    public Stamina stamina;
    public PlayerCapacity playerCapacity;
    public PlayerController playerController;


    [Header("Level System")]
    public int soulLevel = 5;
    private void Awake()
    {
        // Tự động lấy components
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (stamina == null)
            stamina = GetComponent<Stamina>();

        if (playerCapacity == null)
            playerCapacity = GetComponent<PlayerCapacity>();

        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        // Cập nhật tất cả stats ban đầu
        UpdateAllStats();
    }

    /// <summary>
    /// Cập nhật tất cả stats derived (Health, Stamina, Capacity, Movement)
    /// </summary>
    public void UpdateAllStats()
    {
        if (playerHealth != null)
            playerHealth.UpdateMaxHealthFromVigor();

        if (stamina != null)
            stamina.UpdateMaxStaminaFromEndurance();

        if (playerCapacity != null)
            playerCapacity.UpdateMaxEquipLoad();

        if (playerController != null)
            playerController.UpdateMovementStats();

        Debug.Log($"📊 Stats Updated | VIG:{vigor} STR:{strength} DEX:{dexterity} INT:{intelligence} END:{endurance}");
    }

    /// <summary>
    /// Tăng stat và cập nhật các derived stats
    /// </summary>
    public void IncreaseStat(string statName, int amount)
    {
        switch (statName.ToLower())
        {
            case "vigor":
            case "vig":
                vigor += amount;
                if (playerHealth != null)
                {
                    playerHealth.UpdateMaxHealthFromVigor();
                    Debug.Log($"💚 Vigor increased to {vigor}");
                }
                break;

            case "str":
            case "strength":
                strength += amount;
                Debug.Log($"💪 Strength increased to {strength}");
                // TODO: Update weapon damage if needed
                break;

            case "dex":
            case "dexterity":
                dexterity += amount;
                Debug.Log($"🎯 Dexterity increased to {dexterity}");
                // TODO: Update weapon damage if needed
                break;

            case "int":
            case "intelligence":
                intelligence += amount;
                Debug.Log($"🧠 Intelligence increased to {intelligence}");
                // TODO: Update spell power if needed
                break;

            case "end":
            case "endurance":
                endurance += amount;
                if (stamina != null)
                {
                    stamina.UpdateMaxStaminaFromEndurance();
                }
                if (playerCapacity != null)
                {
                    playerCapacity.UpdateMaxEquipLoad();
                }
                if (playerController != null)
                {
                    playerController.UpdateMovementStats();
                }
                Debug.Log($"⚡ Endurance increased to {endurance}");
                break;

            default:
                Debug.LogWarning($"⚠️ Invalid stat name: {statName}");
                break;
        }
        soulLevel += amount;
        Debug.Log($"⬆️ Soul Level increased to {soulLevel}");
    }

    /// <summary>
    /// Set stat trực tiếp (dùng cho debugging hoặc load save)
    /// </summary>
    public void SetStat(string statName, int value)
    {
        switch (statName.ToLower())
        {
            case "vigor":
            case "vig":
                vigor = Mathf.Max(1, value);
                if (playerHealth != null)
                    playerHealth.UpdateMaxHealthFromVigor();
                break;

            case "str":
            case "strength":
                strength = Mathf.Max(1, value);
                break;

            case "dex":
            case "dexterity":
                dexterity = Mathf.Max(1, value);
                break;

            case "int":
            case "intelligence":
                intelligence = Mathf.Max(1, value);
                break;

            case "end":
            case "endurance":
                endurance = Mathf.Max(1, value);
                if (stamina != null)
                    stamina.UpdateMaxStaminaFromEndurance();
                if (playerCapacity != null)
                    playerCapacity.UpdateMaxEquipLoad();
                if (playerController != null)
                    playerController.UpdateMovementStats();
                break;

            default:
                Debug.LogWarning($"⚠️ Invalid stat name: {statName}");
                break;
        }
    }

    /// <summary>
    /// Lấy giá trị stat
    /// </summary>
    public int GetStat(string statName)
    {
        switch (statName.ToLower())
        {
            case "vigor":
            case "vig":
                return vigor;
            case "str":
            case "strength":
                return strength;
            case "dex":
            case "dexterity":
                return dexterity;
            case "int":
            case "intelligence":
                return intelligence;
            case "end":
            case "endurance":
                return endurance;
            default:
                Debug.LogWarning($"⚠️ Invalid stat name: {statName}");
                return 0;
        }
    }

    public void DisplayAllStats()
    {
        Debug.Log("========== PLAYER STATS ==========");
        Debug.Log($"Vigor: {vigor}");
        Debug.Log($"Strength: {strength}");
        Debug.Log($"Dexterity: {dexterity}");
        Debug.Log($"Intelligence: {intelligence}");
        Debug.Log($"Endurance: {endurance}");

        if (playerHealth != null)
            Debug.Log($"Max HP: {playerHealth.GetMaxHealth()}");

        if (stamina != null)
            Debug.Log($"Max Stamina: {stamina.maxStamina}");

        if (playerCapacity != null)
            Debug.Log($"Max Equip Load: {playerCapacity.GetMaxEquipLoad():F1}");

        Debug.Log("==================================");
    }
}