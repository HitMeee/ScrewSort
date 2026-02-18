using System.Collections.Generic;
using UnityEngine;

public static class LevelFileManager
{
    private const string LEVEL_KEY = "Level_";
    private const string CURRENT_KEY = "CurrentLevel";
    private const string LIST_KEY = "LevelList";
    private const string FIRST_TIME_KEY = "FirstTimePlaying"; // ✅ THÊM: Flag lần đầu chơi
    private const string COMPLETED_KEY = "LevelCompleted_"; // ✅ THÊM: Key lưu level đã hoàn thành
    private const string MAX_UNLOCKED_KEY = "MaxUnlockedLevel"; // ✅ THÊM: Level cao nhất đã mở
    
    // 🎮 THÊM: HỆ THỐNG 2 MODE CHƠI
    private const string PLAY_MODE_KEY = "PlayMode"; // Lưu mode đang chơi
    private const string SELECTED_LEVEL_KEY = "SelectedLevel"; // Level được chọn từ All Level
    
    public enum PlayMode
    {
        Progress,  // Chơi tiến độ chính (nút Level)
        Selected   // Chơi level tùy chọn (All Level)
    }

    // ✅ SỬA: GET CURRENT LEVEL - Ưu tiên level đã chọn
    public static int GetCurrentLevelId()
    {
        // Kiểm tra xem đã có level được chọn chưa
        if (PlayerPrefs.HasKey(CURRENT_KEY))
        {
            int currentLevel = PlayerPrefs.GetInt(CURRENT_KEY, 1);
            Debug.Log($"🔄 Load level đã chọn: Level {currentLevel}");
            return currentLevel;
        }

        // Nếu chưa có -> lần đầu chơi, set flag và trả về Level 1
        PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
        PlayerPrefs.SetInt(CURRENT_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("🆕 Lần đầu chơi - Bắt đầu từ Level 1");
        return 1;
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

    // ========== HỆ THỐNG 2 MODE CHƠI ==========
    
    /// <summary>
    /// 🎮 Lấy level cao nhất cần chơi (cho nút "Level" - Progress mode)
    /// Đây là level tiếp theo trong tiến độ chính
    /// </summary>
    public static int GetProgressLevelId()
    {
        // Lấy level cao nhất đã mở
        int maxUnlocked = GetMaxUnlockedLevel();
        
        // Tìm level đầu tiên chưa hoàn thành
        for (int i = 1; i <= maxUnlocked; i++)
        {
            if (!IsLevelCompleted(i))
            {
                Debug.Log($"🎯 Progress Level: {i} (chưa hoàn thành)");
                return i;
            }
        }
        
        // Nếu tất cả level đã unlock đều hoàn thành rồi, trả về level cao nhất
        Debug.Log($"🎯 Progress Level: {maxUnlocked} (level cao nhất)");
        return maxUnlocked;
    }
    
    /// <summary>
    /// 🎮 Set mode chơi và level trước khi vào game
    /// </summary>
    public static void SetPlayMode(PlayMode mode, int levelId)
    {
        PlayerPrefs.SetString(PLAY_MODE_KEY, mode.ToString());
        
        if (mode == PlayMode.Progress)
        {
            // Progress mode: Lưu vào CURRENT_KEY
            SetCurrentLevelId(levelId);
            Debug.Log($"🎮 Chế độ: TIẾN ĐỘ CHÍNH - Level {levelId}");
        }
        else
        {
            // Selected mode: Lưu vào SELECTED_LEVEL_KEY, không động CURRENT_KEY
            PlayerPrefs.SetInt(SELECTED_LEVEL_KEY, levelId);
            SetCurrentLevelId(levelId); // Vẫn cần set để game load đúng level
            Debug.Log($"🎮 Chế độ: CHỌN LEVEL TỰ DO - Level {levelId}");
        }
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// 🎮 Lấy mode chơi hiện tại
    /// </summary>
    public static PlayMode GetPlayMode()
    {
        string modeStr = PlayerPrefs.GetString(PLAY_MODE_KEY, PlayMode.Progress.ToString());
        
        if (System.Enum.TryParse(modeStr, out PlayMode mode))
        {
            return mode;
        }
        
        return PlayMode.Progress; // Mặc định
    }
    
    /// <summary>
    /// 🎮 Kiểm tra có đang chơi mode Selected không
    /// </summary>
    public static bool IsPlayingSelectedMode()
    {
        return GetPlayMode() == PlayMode.Selected;
    }

    // ========== HỆ THỐNG UNLOCK LEVEL ==========
    
    /// <summary>
    /// Kiểm tra xem level có được mở khóa không
    /// Level 1 luôn mở, các level khác phải hoàn thành level trước
    /// </summary>
    public static bool IsLevelUnlocked(int levelId)
    {
        if (levelId <= 1) return true; // Level 1 luôn mở
        
        int maxUnlocked = GetMaxUnlockedLevel();
        bool isUnlocked = levelId <= maxUnlocked;
        
        Debug.Log($"🔒 Check Level {levelId}: {(isUnlocked ? "Mở" : "Khóa")} (Max unlocked: {maxUnlocked})");
        return isUnlocked;
    }
    
    /// <summary>
    /// Lấy level cao nhất đã được mở
    /// </summary>
    public static int GetMaxUnlockedLevel()
    {
        return PlayerPrefs.GetInt(MAX_UNLOCKED_KEY, 1); // Mặc định chỉ mở level 1
    }
    
    /// <summary>
    /// Mở khóa một level cụ thể
    /// </summary>
    public static void UnlockLevel(int levelId)
    {
        int currentMax = GetMaxUnlockedLevel();
        if (levelId > currentMax)
        {
            PlayerPrefs.SetInt(MAX_UNLOCKED_KEY, levelId);
            PlayerPrefs.Save();
            Debug.Log($"🔓 Đã mở Level {levelId}!");
        }
    }
    
    /// <summary>
    /// Đánh dấu level đã hoàn thành và tự động mở level tiếp theo
    /// </summary>
    public static void CompleteLevel(int levelId)
    {
        // Lưu trạng thái hoàn thành
        PlayerPrefs.SetInt(COMPLETED_KEY + levelId, 1);
        
        // Tự động mở level tiếp theo
        UnlockLevel(levelId + 1);
        
        PlayerPrefs.Save();
        Debug.Log($"✅ Hoàn thành Level {levelId}, mở Level {levelId + 1}");
    }
    
    /// <summary>
    /// Kiểm tra level đã hoàn thành chưa
    /// </summary>
    public static bool IsLevelCompleted(int levelId)
    {
        return PlayerPrefs.GetInt(COMPLETED_KEY + levelId, 0) == 1;
    }
    
    /// <summary>
    /// Reset toàn bộ tiến độ - chỉ mở level 1
    /// </summary>
    public static void ResetProgress()
    {
        PlayerPrefs.SetInt(MAX_UNLOCKED_KEY, 1);
        
        // Xóa tất cả completed status
        List<int> levels = GetAllLevelIds();
        foreach (int id in levels)
        {
            PlayerPrefs.DeleteKey(COMPLETED_KEY + id);
        }
        
        PlayerPrefs.Save();
        Debug.Log("🔄 Reset tiến độ - Chỉ mở Level 1");
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

    // ✅ SỬA: DELETE ALL - Bao gồm cả flag lần đầu chơi và reset progress
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
        
        // 🔒 Reset progress (unlock level)
        ResetProgress();
        
        PlayerPrefs.Save();

        Debug.Log($"🗑️ Đã xóa tất cả {levels.Count} level và reset về trạng thái ban đầu");
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