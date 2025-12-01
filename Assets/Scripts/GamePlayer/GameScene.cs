using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    [Header("🎮 Game Settings")]
    [SerializeField] private float delayBeforeNext = 2f;
    [SerializeField] private bool autoNextLevel = true;

    private bool levelCompleted = false;
    private LevelController levelController;

    void Start()
    {
        Init();
        LoadFromLevel1(); // Always start from Level 1
    }

    public void Init()
    {
        levelCompleted = false;
        levelController = FindObjectOfType<LevelController>();
        Debug.Log("🎮 GameScene initialized");
    }

    // Always load from Level 1 on game start
    private void LoadFromLevel1()
    {
        // Reset to Level 1 when game starts
        LevelFileManager.SetCurrentLevelId(1);

        var level = LevelFileManager.LoadLevel(1);

        if (level != null)
        {
            Debug.Log($"🎯 Starting from Level 1: {level.levelName}");
            ApplyLevel(level);
        }
        else
        {
            Debug.Log("⚠️ Level 1 not found, creating default Level 1");
            CreateDefaultLevel(1);
        }
    }

    public void OnLevelComplete()
    {
        if (levelCompleted) return;

        levelCompleted = true;
        Debug.Log("🏆 Level Complete!");

        if (autoNextLevel)
        {
            StartCoroutine(DelayedNext());
        }
    }

    private IEnumerator DelayedNext()
    {
        Debug.Log($"⏰ Waiting {delayBeforeNext} seconds...");
        yield return new WaitForSeconds(delayBeforeNext);
        LoadNextLevel();
    }

    // Auto load next level when completed
    public void LoadNextLevel()
    {
        int nextId = LevelFileManager.GoToNextLevel();
        var level = LevelFileManager.LoadLevel(nextId);

        if (level != null)
        {
            Debug.Log($"➡️ Auto Loading Level {nextId}: {level.levelName}");
            ApplyLevel(level);
        }
        else
        {
            Debug.Log($"⚠️ Level {nextId} not found, creating default");
            CreateDefaultLevel(nextId);
        }
    }

    private void ApplyLevel(SavedLevel level)
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            // Set level data using reflection
            var dataField = typeof(LevelController).GetField("levelDatas",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dataField?.SetValue(levelController, level.levelData);

            levelController.ForceReinit();
            levelCompleted = false;

            Debug.Log($"✅ Applied Level {level.levelId}: {level.levelName}");
        }
    }

    private void CreateDefaultLevel(int levelId)
    {
        var data = new LevelData();

        // Create 3 default bolts
        for (int i = 0; i < 3; i++)
        {
            data.lsDataBolt.Add(new DataBolt
            {
                idBolt = i + 1,
                lsIdScrew = new System.Collections.Generic.List<int> { 1, 2, 1, 2, 3 }
            });
        }

        var level = new SavedLevel
        {
            levelId = levelId,
            levelName = $"Default Level {levelId}",
            levelData = data
        };

        ApplyLevel(level);

        // Save default level to PlayerPrefs for future use
        LevelFileManager.SaveLevel(levelId, level.levelName, level.levelData);
        Debug.Log($"💾 Auto-saved default Level {levelId}");
    }

    // Manual level loading from Level Editor
    public void LoadLevelById(int levelId)
    {
        LevelFileManager.SetCurrentLevelId(levelId);
        var level = LevelFileManager.LoadLevel(levelId);

        if (level != null)
        {
            Debug.Log($"🎯 Manually Loading Level {levelId}: {level.levelName}");
            ApplyLevel(level);
        }
        else
        {
            Debug.Log($"⚠️ Manual load failed, creating default Level {levelId}");
            CreateDefaultLevel(levelId);
        }
    }

    // UI Button Methods
    public void OnReplayClicked() => ReloadCurrentLevel();
    public void OnNextClicked() => LoadNextLevel();
    public void OnMenuClicked() => SceneManager.LoadScene(0);

    // Restart from Level 1
    public void RestartFromLevel1()
    {
        Debug.Log("🔄 Restarting from Level 1");
        LoadFromLevel1();
    }

    public void ReloadCurrentLevel()
    {
        int currentId = LevelFileManager.GetCurrentLevelId();
        var level = LevelFileManager.LoadLevel(currentId);

        if (level != null)
        {
            Debug.Log($"🔄 Reloading Level {currentId}: {level.levelName}");
            ApplyLevel(level);
        }
        else
        {
            CreateDefaultLevel(currentId);
        }
    }

    // Getters
    public int GetCurrentLevelId() => LevelFileManager.GetCurrentLevelId();
    public bool IsLevelCompleted() => levelCompleted;
}