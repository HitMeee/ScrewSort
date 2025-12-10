using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("🎮 Thiết lập Level")]
    public List<Transform> PostCreateBolts;
    [SerializeField] BotlBase botlBase;
    public LevelData levelDatas; // ✅ ĐỔI THÀNH PUBLIC để LevelFileManager truy cập

    [Header("🛠️ Công cụ hỗ trợ")]
    [SerializeField] private AddBolt addBolt;
    [SerializeField] private BackStep backStep;

    private List<BotlBase> botlBases = new List<BotlBase>();
    public bool gameCompleted = false; // ✅ ĐỔI THÀNH PUBLIC để LevelFileManager truy cập
    public bool isInitialized = false; // ✅ ĐỔI THÀNH PUBLIC để LevelFileManager truy cập

    void Update()
    {
        if (!isInitialized || gameCompleted) return;

        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager?.IsGameComplete() == true)
        {
            CompleteLevel();
        }
    }

    // ✅ GỘP: Xử lý hoàn thành level
    private void CompleteLevel()
    {
        gameCompleted = true;

        backStep?.ClearHistory();
        GamePlayerController.Instance?.gameScene?.OnLevelComplete();
    }

    public void ButtonThemBolt() => addBolt?.ButtonAddBolt();
    public void ButtonQuayLai() => backStep?.ButtonGoBack();

    public void Init()
    {
        if (isInitialized) return;

        ResetLevel();
        CreateLevelFromData();
        isInitialized = true;

        Debug.Log($"✅ Level khởi tạo với {botlBases.Count} bolts");
    }

    // ✅ GỘP: Reset level state
    private void ResetLevel()
    {
        gameCompleted = false;
        ClearBolts();
        backStep?.ClearHistory();
    }

    // ✅ GỘP: Tạo level từ data
    private void CreateLevelFromData()
    {
        if (levelDatas?.lsDataBolt == null || levelDatas.lsDataBolt.Count == 0)
        {
            CreateDefaultLevel();
        }

        if (PostCreateBolts != null && levelDatas?.lsDataBolt != null)
        {
            int count = Mathf.Min(levelDatas.lsDataBolt.Count, PostCreateBolts.Count);
            for (int i = 0; i < count; i++)
            {
                if (PostCreateBolts[i] != null)
                {
                    CreateBolt(levelDatas.lsDataBolt[i], PostCreateBolts[i].position);
                }
            }
        }
    }

    private void CreateBolt(DataBolt dataBolt, Vector3 position)
    {
        if (botlBase == null || dataBolt == null) return;

        var bolt = Instantiate(botlBase, position, Quaternion.identity);
        bolt.Init(dataBolt.lsIdScrew ?? new List<int>());
        bolt.name = $"Bolt_{dataBolt.idBolt}";
        botlBases.Add(bolt);
    }

    // ✅ SỬA: Xóa cả bolt và tất cả screws
    private void ClearBolts()
    {
        foreach (var bolt in botlBases)
        {
            if (bolt != null)
            {
                // ✅ XÓA TỪNG SCREW TRƯỚC KHI XÓA BOLT
                if (bolt.screwBases != null)
                {
                    foreach (var screw in bolt.screwBases)
                    {
                        if (screw != null && screw.gameObject != null)
                        {
                            if (Application.isPlaying)
                                Destroy(screw.gameObject);
                            else
                                DestroyImmediate(screw.gameObject);
                        }
                    }
                    bolt.screwBases.Clear(); // Clear list
                }

                // ✅ SAU ĐÓ MỚI XÓA BOLT
                if (bolt.gameObject != null)
                {
                    if (Application.isPlaying)
                        Destroy(bolt.gameObject);
                    else
                        DestroyImmediate(bolt.gameObject);
                }
            }
        }
        botlBases.Clear();
        Debug.Log("🗑️ Đã xóa tất cả bolt và screw");
    }

    private void CreateDefaultLevel()
    {
        levelDatas = new LevelData();
        for (int i = 0; i < 3; i++)
        {
            levelDatas.lsDataBolt.Add(new DataBolt
            {
                idBolt = i + 1,
                lsIdScrew = new List<int> { 1, 2, 1, 2, 3 }
            });
        }
        Debug.Log("🔧 Tạo level mặc định");
    }

    // ✅ ĐƠN GIẢN: ForceReinit gọi lại Init
    public void ForceReinit()
    {
        isInitialized = false;
        Init();
    }

    // ✅ ĐƠN GIẢN: Getters
    public AddBolt GetAddBolt() => addBolt;
    public BackStep GetBackStep() => backStep;
    public List<BotlBase> GetAllBolts() => botlBases ?? new List<BotlBase>();
    public int GetCurrentLevelId()
    {
        try { return LevelFileManager.GetCurrentLevelId(); }
        catch { return 1; }
    }
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