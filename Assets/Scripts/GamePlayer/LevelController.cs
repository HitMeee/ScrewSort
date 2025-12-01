using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("🎮 Level Setup")]
    [SerializeField] List<Transform> PostCreateBolts;
    [SerializeField] BotlBase botlBase;
    [SerializeField] LevelData levelDatas;

    [Header("📂 File Loading")]
    [SerializeField] private bool loadFromFile = false;
    [SerializeField] private int levelIdToLoad = 1;

    private List<BotlBase> botlBases = new List<BotlBase>();
    private bool gameCompleted = false;
    private bool isInitialized = false;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (!isInitialized || gameCompleted) return;

        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager != null && boltManager.IsGameComplete())
        {
            gameCompleted = true;
            Debug.Log("🏆 LEVEL COMPLETE!");

            if (GamePlayerController.Instance?.gameScene != null)
            {
                GamePlayerController.Instance.gameScene.OnLevelComplete();
            }
        }
    }

    public void Init()
    {
        if (isInitialized) return;

        gameCompleted = false;

        if (loadFromFile)
        {
            LoadLevelFromFile(levelIdToLoad);
        }

        ClearBolts();

        if (levelDatas?.lsDataBolt == null || levelDatas.lsDataBolt.Count == 0)
        {
            CreateDefaultLevel();
        }

        for (int i = 0; i < levelDatas.lsDataBolt.Count && i < PostCreateBolts.Count; i++)
        {
            CreateBolt(levelDatas.lsDataBolt[i], PostCreateBolts[i].position);
        }

        isInitialized = true;
        Debug.Log($"✅ Level initialized with {botlBases.Count} bolts");
    }

    public void LoadLevelFromFile(int levelId)
    {
        try
        {
            var savedLevel = LevelFileManager.LoadLevel(levelId);
            if (savedLevel != null)
            {
                levelDatas = savedLevel.levelData;
                levelIdToLoad = levelId;
                Debug.Log($"📂 Loaded level: {savedLevel.levelName}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"⚠️ Load error: {e.Message}");
        }
    }

    private void CreateBolt(DataBolt dataBolt, Vector3 position)
    {
        if (botlBase == null) return;

        var bolt = Instantiate(botlBase, position, Quaternion.identity);
        bolt.Init(dataBolt.lsIdScrew);
        bolt.name = $"Bolt_{dataBolt.idBolt}";
        botlBases.Add(bolt);
    }

    private void ClearBolts()
    {
        foreach (var bolt in botlBases)
        {
            if (bolt != null)
            {
                if (Application.isPlaying)
                    Destroy(bolt.gameObject);
                else
                    DestroyImmediate(bolt.gameObject);
            }
        }
        botlBases.Clear();
    }

    private void CreateDefaultLevel()
    {
        levelDatas = new LevelData { lsDataBolt = new List<DataBolt>() };

        for (int i = 0; i < 3; i++)
        {
            levelDatas.lsDataBolt.Add(new DataBolt
            {
                idBolt = i + 1,
                lsIdScrew = new List<int> { 1, 2, 1, 2, 3 }
            });
        }

        Debug.Log("🔧 Created default level");
    }

    public void ForceReinit()
    {
        isInitialized = false;
        gameCompleted = false;
        Init();
    }

    public List<BotlBase> GetAllBolts() => botlBases ?? new List<BotlBase>();
    public int GetCurrentLevelId() => levelIdToLoad;
    public bool IsInitialized() => isInitialized;
}

[System.Serializable]
public class LevelData
{
    public List<DataBolt> lsDataBolt = new List<DataBolt>();
}

[System.Serializable]
public class DataBolt
{
    public int idBolt;
    public List<int> lsIdScrew = new List<int>();
}

[System.Serializable]
public class SavedLevel
{
    public int levelId;
    public string levelName;
    public string createdDate;
    public LevelData levelData;
}