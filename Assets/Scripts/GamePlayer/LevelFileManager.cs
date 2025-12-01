using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class LevelFileManager
{
    private static string LevelsPath => Path.Combine(Application.streamingAssetsPath, "Levels");

    static LevelFileManager()
    {
        if (!Directory.Exists(LevelsPath))
            Directory.CreateDirectory(LevelsPath);
    }

    public static bool SaveLevel(int levelId, string levelName, LevelData levelData)
    {
        try
        {
            var saveData = new SavedLevel
            {
                levelId = levelId,
                levelName = levelName,
                createdDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                levelData = levelData
            };

            string fileName = $"level_{levelId:D3}.json";
            string filePath = Path.Combine(LevelsPath, fileName);
            string json = JsonUtility.ToJson(saveData, true);

            File.WriteAllText(filePath, json);
            Debug.Log($"✅ Saved Level {levelId}: {levelName}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Save failed: {e.Message}");
            return false;
        }
    }

    public static SavedLevel LoadLevel(int levelId)
    {
        try
        {
            string fileName = $"level_{levelId:D3}.json";
            string filePath = Path.Combine(LevelsPath, fileName);

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var result = JsonUtility.FromJson<SavedLevel>(json);
                Debug.Log($"📂 Loaded Level {levelId}: {result.levelName}");
                return result;
            }

            Debug.LogWarning($"⚠️ Level {levelId} not found");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Load failed: {e.Message}");
            return null;
        }
    }

    public static List<int> GetAllLevelIds()
    {
        var levelIds = new List<int>();

        if (!Directory.Exists(LevelsPath))
            return levelIds;

        var files = Directory.GetFiles(LevelsPath, "level_*.json");

        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (fileName.StartsWith("level_"))
            {
                string idStr = fileName.Substring(6);
                if (int.TryParse(idStr, out int id))
                {
                    levelIds.Add(id);
                }
            }
        }

        levelIds.Sort();
        return levelIds;
    }

    public static void ListAllLevels()
    {
        var levels = GetAllLevelIds();
        Debug.Log($"📋 Found {levels.Count} levels:");

        foreach (int id in levels)
        {
            var level = LoadLevel(id);
            if (level != null)
            {
                Debug.Log($"  • Level {level.levelId}: {level.levelName} ({level.createdDate}) - {level.levelData.lsDataBolt.Count} bolts");
            }
        }
    }

    public static int GetNextAvailableLevelId()
    {
        var existingIds = GetAllLevelIds();

        if (existingIds.Count == 0)
            return 1;

        for (int i = 1; i <= existingIds.Max() + 1; i++)
        {
            if (!existingIds.Contains(i))
                return i;
        }

        return existingIds.Max() + 1;
    }
}