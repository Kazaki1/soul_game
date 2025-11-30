using UnityEngine;

public static class SaveSystem
{
    private const string SAVE_KEY = "GAME_SNAPSHOT";

    public static void SaveSnapshot(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static SaveData LoadSnapshot()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return null;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}
