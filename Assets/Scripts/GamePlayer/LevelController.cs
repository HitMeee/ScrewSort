using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("🎮 Thiết lập Level")]
    public List<Transform> PostCreateBolts;
    [SerializeField] BotlBase botlBase;
    [SerializeField] LevelData levelDatas;

    [Header("🛠️ Công cụ hỗ trợ")]
    [SerializeField] private AddBolt addBolt;
    [SerializeField] private BackStep backStep;

    private List<BotlBase> botlBases = new List<BotlBase>();
    private bool gameCompleted = false;
    private bool isInitialized = false;

    void Start()
    {
        SetupHelperTools();
        Init();
    }

    private void SetupHelperTools()
    {
        // Tự động thiết lập AddBolt
        if (addBolt == null)
        {
            addBolt = GetComponent<AddBolt>();
            if (addBolt == null)
            {
                addBolt = gameObject.AddComponent<AddBolt>();
            }
        }

        // Tự động thiết lập BackStep  
        if (backStep == null)
        {
            backStep = GetComponent<BackStep>();
            if (backStep == null)
            {
                backStep = gameObject.AddComponent<BackStep>();
            }
        }
    }

    void Update()
    {
        if (!isInitialized || gameCompleted) return;

        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager != null && boltManager.IsGameComplete())
        {
            gameCompleted = true;
            Debug.Log("🏆 HOÀN THÀNH LEVEL!");

            if (backStep != null)
            {
                backStep.ClearHistory(); // ✅ SỬA: Sử dụng đúng tên phương thức
            }

            if (GamePlayerController.Instance?.gameScene != null)
            {
                GamePlayerController.Instance.gameScene.OnLevelComplete();
            }
        }
    }

    // ✅ SỬA: THÊM 2 HÀM ĐƠN GIẢN CHO BUTTON GỌI TRỰC TIẾP

    // Hàm cho AddBolt Button
    public void ButtonThemBolt()
    {
        Debug.Log("🔘 Button Thêm Bolt được nhấn!");

        if (addBolt != null)
            addBolt.ButtonAddBolt(); // ✅ SỬA: Sử dụng đúng tên phương thức
        else
            Debug.LogError("❌ AddBolt component không tìm thấy!");
    }

    // Hàm cho BackStep Button  
    public void ButtonQuayLai()
    {
        Debug.Log("🔘 Button Quay Lại được nhấn!");

        if (backStep != null)
            backStep.ButtonGoBack(); // ✅ SỬA: Sử dụng đúng tên phương thức
        else
            Debug.LogError("❌ BackStep component không tìm thấy!");
    }

    public void Init()
    {
        if (isInitialized) return;

        gameCompleted = false;
        ClearBolts();

        if (backStep != null)
        {
            backStep.ClearHistory(); // ✅ SỬA: Sử dụng đúng tên phương thức
        }

        if (levelDatas?.lsDataBolt == null || levelDatas.lsDataBolt.Count == 0)
        {
            CreateDefaultLevel();
        }

        if (PostCreateBolts != null && levelDatas.lsDataBolt != null)
        {
            for (int i = 0; i < levelDatas.lsDataBolt.Count && i < PostCreateBolts.Count; i++)
            {
                if (PostCreateBolts[i] != null)
                {
                    CreateBolt(levelDatas.lsDataBolt[i], PostCreateBolts[i].position);
                }
            }
        }

        isInitialized = true;
        Debug.Log($"✅ Level được khởi tạo với {botlBases.Count} bolts");
    }

    private void CreateBolt(DataBolt dataBolt, Vector3 position)
    {
        if (botlBase == null)
        {
            Debug.LogError("❌ BotlBase prefab is null!");
            return;
        }

        if (dataBolt == null)
        {
            Debug.LogError("❌ DataBolt is null!");
            return;
        }

        var bolt = Instantiate(botlBase, position, Quaternion.identity);
        var screwList = dataBolt.lsIdScrew ?? new List<int>();
        bolt.Init(screwList);
        bolt.name = $"Bolt_{dataBolt.idBolt}";
        botlBases.Add(bolt);
    }

    private void ClearBolts()
    {
        if (botlBases == null) return;

        foreach (var bolt in botlBases)
        {
            if (bolt != null && bolt.gameObject != null)
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

    public void ForceReinit()
    {
        isInitialized = false;
        gameCompleted = false;

        if (backStep != null)
        {
            backStep.ClearHistory(); // ✅ SỬA: Sử dụng đúng tên phương thức
        }

        Init();
    }

    // Public accessors
    public AddBolt GetAddBolt() => addBolt;
    public BackStep GetBackStep() => backStep;
    public List<BotlBase> GetAllBolts() => botlBases ?? new List<BotlBase>();

    public int GetCurrentLevelId()
    {
        try
        {
            return LevelFileManager.GetCurrentLevelId();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Lỗi khi lấy current level ID: " + ex.Message);
            return 1;
        }
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