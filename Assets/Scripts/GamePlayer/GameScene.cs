using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    [Header("🎮 Game Flow Settings")]
    [SerializeField] private float delayBeforeNext = 2f;
    [SerializeField] private bool autoNextLevel = true;

    [Header("📊 Level Management")]
    [SerializeField] private bool useFileSystem = true; // Dùng JSON files thay vì scenes
    [SerializeField] private int currentLevelId = 1;

    private bool levelCompleted = false;

    public void Init()
    {
        levelCompleted = false;
        Debug.Log("🎮 GameScene initialized");

        // Lấy current level từ LevelController nếu có
        var levelController = GamePlayerController.Instance?.gameContaint?.levelController;
        if (levelController != null)
        {
            currentLevelId = levelController.GetCurrentLevelId();
        }
    }

    public void OnLevelComplete()
    {
        if (levelCompleted) return; // Tránh gọi nhiều lần

        levelCompleted = true;
        Debug.Log("🏆 Level Complete! Starting next sequence...");

        ShowWinUI();

        if (autoNextLevel)
        {
            StartCoroutine(DelayedNext());
        }
    }

    private void ShowWinUI()
    {
        Debug.Log("✨ Showing Win UI...");
        // TODO: Hiển thị Win Panel, effects, etc.
    }

    private IEnumerator DelayedNext()
    {
        Debug.Log($"⏰ Waiting {delayBeforeNext} seconds before next level...");
        yield return new WaitForSeconds(delayBeforeNext);

        LoadNextLevel();
    }

    /// <summary>
    /// Load level tiếp theo
    /// </summary>
    public void LoadNextLevel()
    {
        if (useFileSystem)
        {
            LoadNextLevelFromFile();
        }
        else
        {
            LoadNextScene();
        }
    }

    /// <summary>
    /// Load level tiếp theo từ file JSON (Recommended)
    /// </summary>
    private void LoadNextLevelFromFile()
    {
        int nextLevelId = currentLevelId + 1;

        // Kiểm tra level tiếp theo có tồn tại không
        var savedLevel = LevelFileManager.LoadLevel(nextLevelId);
        if (savedLevel != null)
        {
            currentLevelId = nextLevelId;
            Debug.Log($"➡️ Loading Level {currentLevelId}: {savedLevel.levelName}");

            ApplyLevelToGame(savedLevel);
        }
        else
        {
            // Không có level tiếp theo -> restart từ level 1 hoặc về menu
            Debug.Log("🎊 All levels completed!");
            OnAllLevelsComplete();
        }
    }

    /// <summary>
    /// Áp dụng level data vào game
    /// </summary>
    private void ApplyLevelToGame(SavedLevel savedLevel)
    {
        var levelController = GamePlayerController.Instance?.gameContaint?.levelController;
        if (levelController != null)
        {
            // Set level data vào LevelController
            var field = typeof(LevelController).GetField("levelDatas",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(levelController, savedLevel.levelData);

            // Set level ID
            var idField = typeof(LevelController).GetField("levelIdToLoad",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            idField?.SetValue(levelController, currentLevelId);

            // Reinit level
            levelController.ForceReinit();

            // Reset completion flag
            levelCompleted = false;

            Debug.Log($"✅ Level {currentLevelId} loaded successfully!");
        }
        else
        {
            Debug.LogError("❌ LevelController not found!");
        }
    }

    /// <summary>
    /// Load scene tiếp theo (Fallback method)
    /// </summary>
    private void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int maxScenes = SceneManager.sceneCountInBuildSettings;

        if (currentIndex + 1 < maxScenes)
        {
            Debug.Log($"➡️ Loading next scene: Scene {currentIndex + 1}");
            SceneManager.LoadScene(currentIndex + 1);
        }
        else
        {
            Debug.Log("🎊 All scenes completed! Going to main menu...");
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Khi hoàn thành tất cả levels
    /// </summary>
    private void OnAllLevelsComplete()
    {
        Debug.Log("🏆 CONGRATULATIONS! ALL LEVELS COMPLETED!");

        // Option 1: Restart từ level 1
        RestartFromLevel1();

        // Option 2: Về menu chính
        // SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Restart từ level 1
    /// </summary>
    private void RestartFromLevel1()
    {
        Debug.Log("🔄 Restarting from Level 1...");
        currentLevelId = 1;

        var savedLevel = LevelFileManager.LoadLevel(1);
        if (savedLevel != null)
        {
            ApplyLevelToGame(savedLevel);
        }
        else
        {
            Debug.LogError("❌ Level 1 not found! Creating default level...");
            ReloadCurrentLevel(); // Fallback to reload
        }
    }

    /// <summary>
    /// Reload level hiện tại
    /// </summary>
    public void ReloadCurrentLevel()
    {
        if (useFileSystem)
        {
            Debug.Log($"🔄 Reloading Level {currentLevelId}...");
            var savedLevel = LevelFileManager.LoadLevel(currentLevelId);
            if (savedLevel != null)
            {
                ApplyLevelToGame(savedLevel);
            }
            else
            {
                Debug.Log("🔄 Level not found, reloading scene...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            Debug.Log("🔄 Reloading current scene...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// Load level theo ID cụ thể
    /// </summary>
    public void LoadLevelById(int levelId)
    {
        var savedLevel = LevelFileManager.LoadLevel(levelId);
        if (savedLevel != null)
        {
            currentLevelId = levelId;
            Debug.Log($"📂 Loading Level {levelId}: {savedLevel.levelName}");
            ApplyLevelToGame(savedLevel);
        }
        else
        {
            Debug.LogError($"❌ Level {levelId} not found!");
        }
    }

    /// <summary>
    /// Manual controls (có thể gọi từ UI buttons)
    /// </summary>
    public void OnReplayButtonClicked()
    {
        ReloadCurrentLevel();
    }

    public void OnNextButtonClicked()
    {
        LoadNextLevel();
    }

    public void OnMenuButtonClicked()
    {
        SceneManager.LoadScene(0); // Về menu chính
    }

    /// <summary>
    /// Public getters/setters
    /// </summary>
    public int GetCurrentLevelId() => currentLevelId;
    public void SetCurrentLevelId(int id) => currentLevelId = id;
    public bool IsLevelCompleted() => levelCompleted;
}