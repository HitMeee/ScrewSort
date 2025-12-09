using System.Collections.Generic;
using UnityEngine;

public static class LevelFileManager
{
    private const string LEVEL_KEY = "Level_";
    private const string CURRENT_KEY = "CurrentLevel";
    private const string LIST_KEY = "LevelList";

    // GET/SET CURRENT LEVEL
    public static int GetCurrentLevelId() => PlayerPrefs.GetInt(CURRENT_KEY, 1);

    public static void SetCurrentLevelId(int levelId)
    {
        PlayerPrefs.SetInt(CURRENT_KEY, levelId);
        PlayerPrefs.Save();
    }

    // ✅ THÊM: GO TO NEXT LEVEL
    public static int GoToNextLevel()
    {
        int current = GetCurrentLevelId();
        int next = current + 1;

        if (LevelExists(next))
        {
            SetCurrentLevelId(next);
            return next;
        }
        else
        {
            SetCurrentLevelId(1);
            return 1;
        }
    }

    // SAVE LEVEL - Đơn giản
    public static bool SaveLevel(int levelId, string levelName, LevelData levelData)
    {
        if (levelData == null) return false;

        var saveData = new SavedLevel
        {
            levelId = levelId,
            levelName = levelName,
            createdDate = System.DateTime.Now.ToString("dd/MM/yyyy"),
            levelData = levelData
        };

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(LEVEL_KEY + levelId, json);
        AddToLevelList(levelId);
        PlayerPrefs.Save();

        Debug.Log($"💾 Lưu Level {levelId}: {levelName}");
        return true;
    }

    // LOAD LEVEL - Đơn giản
    public static SavedLevel LoadLevel(int levelId)
    {
        string key = LEVEL_KEY + levelId;
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<SavedLevel>(json);
        }
        return null;
    }

    // DELETE ALL - Đơn giản
    public static void ClearAllLevels()
    {
        List<int> levels = GetAllLevelIds();

        foreach (int id in levels)
        {
            PlayerPrefs.DeleteKey(LEVEL_KEY + id);
        }

        PlayerPrefs.DeleteKey(LIST_KEY);
        PlayerPrefs.DeleteKey(CURRENT_KEY);
        PlayerPrefs.Save();

        Debug.Log($"🗑️ Đã xóa tất cả {levels.Count} level");
    }

    // GET ALL LEVEL IDS - Cho Prev/Next
    public static List<int> GetAllLevelIds()
    {
        string listJson = PlayerPrefs.GetString(LIST_KEY, "");
        if (string.IsNullOrEmpty(listJson)) return new List<int>();

        string[] ids = listJson.Split(',');
        List<int> result = new List<int>();

        foreach (string id in ids)
        {
            if (int.TryParse(id, out int levelId))
                result.Add(levelId);
        }

        result.Sort();
        return result;
    }

    // LEVEL EXISTS - Cho navigation
    public static bool LevelExists(int levelId)
    {
        return PlayerPrefs.HasKey(LEVEL_KEY + levelId);
    }

    // ADD TO LIST - Helper
    private static void AddToLevelList(int levelId)
    {
        List<int> levels = GetAllLevelIds();
        if (!levels.Contains(levelId))
        {
            levels.Add(levelId);
            levels.Sort();
            string listString = string.Join(",", levels);
            PlayerPrefs.SetString(LIST_KEY, listString);
        }
    }
}