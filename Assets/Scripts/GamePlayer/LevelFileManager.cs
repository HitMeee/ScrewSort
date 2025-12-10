using System.Collections.Generic;
using UnityEngine;

public static class LevelFileManager
{
    private const string LEVEL_KEY = "Level_";
    private const string CURRENT_KEY = "CurrentLevel";
    private const string LIST_KEY = "LevelList";
    private const string FIRST_TIME_KEY = "FirstTimePlaying"; // ✅ THÊM: Flag lần đầu chơi

    // ✅ SỬA: GET CURRENT LEVEL - Xử lý lần đầu chơi
    public static int GetCurrentLevelId()
    {
        // Kiểm tra xem có phải lần đầu chơi không
        bool isFirstTime = !PlayerPrefs.HasKey(FIRST_TIME_KEY);

        if (isFirstTime)
        {
            // Lần đầu chơi -> set flag và return Level 1
            PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
            PlayerPrefs.SetInt(CURRENT_KEY, 1);
            PlayerPrefs.Save();
            Debug.Log("🆕 Lần đầu chơi - Bắt đầu từ Level 1");
            return 1;
        }
        else
        {
            // Đã chơi rồi -> lấy level cuối cùng từ PlayerPrefs
            int lastLevel = PlayerPrefs.GetInt(CURRENT_KEY, 1);
            Debug.Log($"🔄 Load level cuối cùng đã chơi: Level {lastLevel}");
            return lastLevel;
        }
    }

    public static void SetCurrentLevelId(int levelId)
    {
        PlayerPrefs.SetInt(CURRENT_KEY, levelId);
        PlayerPrefs.Save();
        Debug.Log($"💾 Lưu current level: {levelId}");
    }

    // ✅ THÊM: Reset về trạng thái lần đầu chơi
    public static void ResetToFirstTime()
    {
        PlayerPrefs.DeleteKey(FIRST_TIME_KEY);
        PlayerPrefs.SetInt(CURRENT_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("🔄 Reset về trạng thái lần đầu chơi");
    }

    // ✅ THÊM: Kiểm tra có phải lần đầu chơi không
    public static bool IsFirstTimePlaying()
    {
        return !PlayerPrefs.HasKey(FIRST_TIME_KEY);
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

    // ✅ SỬA: DELETE ALL - Bao gồm cả flag lần đầu chơi
    public static void ClearAllLevels()
    {
        List<int> levels = GetAllLevelIds();

        foreach (int id in levels)
        {
            PlayerPrefs.DeleteKey(LEVEL_KEY + id);
        }

        PlayerPrefs.DeleteKey(LIST_KEY);
        PlayerPrefs.DeleteKey(CURRENT_KEY);
        PlayerPrefs.DeleteKey(FIRST_TIME_KEY); // ✅ THÊM: Xóa flag lần đầu chơi
        PlayerPrefs.Save();

        Debug.Log($"🗑️ Đã xóa tất cả {levels.Count} level và reset về trạng thái lần đầu");
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

    // ✅ CHUYỂN TỪ LEVELCONTROLLER: SET LEVEL DATA
    public static void SetLevelDataToController(LevelData levelData)
    {
        var levelController = Object.FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            levelController.levelDatas = levelData;
            Debug.Log("📝 Đã set level data vào LevelController");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy LevelController trong scene!");
        }
    }

    // ✅ CHUYỂN TỪ LEVELCONTROLLER: CLEAR SCENE
    public static void ClearGameScene()
    {
        var levelController = Object.FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            // Thực hiện logic clear scene từ LevelController
            levelController.isInitialized = false;
            levelController.gameCompleted = false;

            // Gọi ClearBolts thông qua reflection vì nó là private
            var clearBoltsMethod = typeof(LevelController).GetMethod("ClearBolts",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            clearBoltsMethod?.Invoke(levelController, null);

            // Xóa levelDatas
            levelController.levelDatas = null;

            // Clear BackStep history
            var backStep = levelController.GetBackStep();
            backStep?.ClearHistory();

            Debug.Log("🧹 Scene đã được xóa hoàn toàn thông qua LevelFileManager");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy LevelController trong scene!");
        }
    }

    // ✅ FIX: APPLY LEVEL DATA - CHỈ apply vào scene, KHÔNG thay đổi current level
    public static void ApplyLevelToGame(LevelData levelData, int levelId)
    {
        var levelController = Object.FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            // Set level data
            levelController.levelDatas = levelData;

            // Force reinit
            levelController.ForceReinit();

            // ✅ BỎ: SetCurrentLevelId(levelId); - Không thay đổi current level!

            Debug.Log($"🎮 Đã áp dụng Level {levelId} vào game (không thay đổi current level)");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy LevelController trong scene!");
        }
    }

    // ✅ THÊM: Apply level VÀ set làm current level (chỉ dùng khi thực sự chơi)
    public static void ApplyLevelAndSetCurrent(LevelData levelData, int levelId)
    {
        var levelController = Object.FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            // Set level data
            levelController.levelDatas = levelData;

            // Force reinit
            levelController.ForceReinit();

            // Set current level ID
            SetCurrentLevelId(levelId);

            Debug.Log($"🎮 Đã áp dụng Level {levelId} vào game VÀ set làm current level");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy LevelController trong scene!");
        }
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