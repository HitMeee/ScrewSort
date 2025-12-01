using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class LevelFileManager
{
    private const string LEVEL_KEY = "Level_";
    private const string CURRENT_KEY = "CurrentLevel";
    private const string LIST_KEY = "LevelList";

    // Current Level Management
    public static int GetCurrentLevelId() => PlayerPrefs.GetInt(CURRENT_KEY, 1);

    public static void SetCurrentLevelId(int levelId)
    {
        PlayerPrefs.SetInt(CURRENT_KEY, levelId);
        PlayerPrefs.Save();
    }

    public static int GoToNextLevel()
    {
        int next = GetCurrentLevelId() + 1;
        if (LevelExists(next))
        {
            SetCurrentLevelId(next);
            return next;
        }
        else
        {
            SetCurrentLevelId(1); // Reset về level 1
            return 1;
        }
    }

    // Save/Load Level
    public static bool SaveLevel(int levelId, string levelName, LevelData levelData)
    {
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

        Debug.Log($"💾 Saved Level {levelId}: {levelName}");
        return true;
    }

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

    // Level List Management
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

    // Utility Methods
    public static bool LevelExists(int levelId)
    {
        return PlayerPrefs.HasKey(LEVEL_KEY + levelId);
    }

    public static int GetNextAvailableLevelId()
    {
        List<int> existing = GetAllLevelIds();
        if (existing.Count == 0) return 1;

        for (int i = 1; i <= existing.Max() + 1; i++)
        {
            if (!existing.Contains(i)) return i;
        }
        return existing.Max() + 1;
    }

    public static void ListAllLevels()
    {
        List<int> levels = GetAllLevelIds();
        int current = GetCurrentLevelId();

        Debug.Log($"📋 Found {levels.Count} levels (Current: {current}):");
        foreach (int id in levels)
        {
            var level = LoadLevel(id);
            if (level != null)
            {
                string marker = (id == current) ? " ← CURRENT" : "";
                Debug.Log($"  • Level {id}: {level.levelName}{marker}");
            }
        }
    }

    // Debug Methods
    public static void DeleteLevel(int levelId)
    {
        PlayerPrefs.DeleteKey(LEVEL_KEY + levelId);
        List<int> levels = GetAllLevelIds();
        levels.Remove(levelId);
        string listString = string.Join(",", levels);
        PlayerPrefs.SetString(LIST_KEY, listString);
        PlayerPrefs.Save();
    }

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
        Debug.Log("🧹 Cleared all levels");
    }
}