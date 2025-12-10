using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LevelEditorOdin : MonoBehaviour
{
    #region Inspector Fields

    [Header("🎮 Level Controller")]
    [SerializeField] private LevelController levelController;

    [HorizontalGroup("Info")]
    [LabelText("ID Level")]
    [SerializeField] private int currentLevelId = 1;

    [HorizontalGroup("Info")]
    [LabelText("Tên Level")]
    [SerializeField] private string levelName = "New Level";

    [Space(10)]
    [Title("🔧 Cấu Hình Bolt")]
    [TableList(AlwaysExpanded = true)]
    [SerializeField] private List<BoltSetup> boltSetups = new List<BoltSetup>();

    #endregion

    #region Bolt Management

    [HorizontalGroup("BoltActions")]
    [Button("Add Bolt"), GUIColor(0.4f, 0.8f, 0.4f)]
    public void AddBolt()
    {
        boltSetups.Add(new BoltSetup());
        Debug.Log($"➕ Đã thêm bolt mới. Tổng số bolt: {boltSetups.Count}");
    }

    [HorizontalGroup("BoltActions")]
    [Button("Remove Last"), GUIColor(0.8f, 0.4f, 0.4f)]
    public void RemoveLastBolt()
    {
        if (boltSetups.Count > 0)
        {
            boltSetups.RemoveAt(boltSetups.Count - 1);
            Debug.Log($"➖ Đã xóa bolt cuối. Còn lại: {boltSetups.Count}");
        }
        else
        {
            Debug.LogWarning("⚠️ Không có bolt nào để xóa!");
        }
    }

    #endregion

    #region File Operations

    [HorizontalGroup("File")]
    [Button("Save Level", ButtonSizes.Large), GUIColor(0.2f, 0.8f, 0.2f)]
    public void SaveLevel()
    {
        if (!ValidateBeforeSave()) return;

        var levelData = ConvertToLevelData();
        bool success = LevelFileManager.SaveLevel(currentLevelId, levelName, levelData);

        if (success)
        {
            Debug.Log($"💾 Đã lưu Level {currentLevelId}: {levelName}");
            // ✅ KHÔNG thay đổi CurrentLevelId - giữ nguyên level đang chơi
        }
        else
        {
            Debug.LogError("❌ Lưu level thất bại!");
        }
    }

    [HorizontalGroup("File")]
    [Button("Load Level", ButtonSizes.Large), GUIColor(0.2f, 0.6f, 0.8f)]
    public void LoadLevel()
    {
        LoadAndApplyLevelData(currentLevelId); // ✅ SỬA: Áp dụng vào game scene
    }

    #endregion

    #region Navigation

    [HorizontalGroup("Navigation")]
    [Button("Prev"), GUIColor(0.8f, 0.8f, 0.6f)]
    public void PrevLevel()
    {
        var allIds = LevelFileManager.GetAllLevelIds();

        if (allIds.Count > 0)
        {
            int currentIndex = allIds.IndexOf(currentLevelId);

            if (currentIndex > 0)
            {
                currentLevelId = allIds[currentIndex - 1];
                LoadLevelDataOnly(currentLevelId); // ✅ SỬA: Chỉ load data, không áp dụng scene
                Debug.Log($"⬅️ Đã chuyển Level {currentLevelId} (chỉ hiển thị data)");
            }
            else
            {
                Debug.LogWarning("⚠️ Đã ở level đầu tiên!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Không có level nào!");
        }
    }

    [HorizontalGroup("Navigation")]
    [Button("Next"), GUIColor(0.8f, 0.8f, 0.6f)]
    public void NextLevel()
    {
        var allIds = LevelFileManager.GetAllLevelIds();

        if (allIds.Count > 0)
        {
            int currentIndex = allIds.IndexOf(currentLevelId);

            if (currentIndex >= 0 && currentIndex < allIds.Count - 1)
            {
                currentLevelId = allIds[currentIndex + 1];
                LoadLevelDataOnly(currentLevelId); // ✅ SỬA: Chỉ load data, không áp dụng scene
                Debug.Log($"➡️ Đã chuyển Level {currentLevelId} (chỉ hiển thị data)");
            }
            else
            {
                Debug.LogWarning("⚠️ Đã ở level cuối cùng!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Không có level nào!");
        }
    }

    [HorizontalGroup("Navigation")]
    [Button("New Level"), GUIColor(0.6f, 0.9f, 0.6f)]
    public void CreateNewLevel()
    {
        var allIds = LevelFileManager.GetAllLevelIds();
        int newId = allIds.Count > 0 ? allIds[allIds.Count - 1] + 1 : 1;

        currentLevelId = newId;
        levelName = $"Level {newId}";

        CreateDefaultBoltSetups();

        Debug.Log($"🆕 Tạo Level mới: {currentLevelId}");
    }

    #endregion

    #region Game Scene Management

    [Button("Delete All Bolt", ButtonSizes.Large), GUIColor(1f, 0.4f, 0.4f)]
    public void ClearGameScene()
    {
        LevelFileManager.ClearGameScene();
        Debug.Log("🗑️ Đã xóa game scene!");
    }

    [Button("Reset to Level 1", ButtonSizes.Large), GUIColor(0.2f, 0.8f, 0.9f)]
    public void ResetToLevel1()
    {
        LevelFileManager.SetCurrentLevelId(1);
        Debug.Log("🔄 Đã reset về Level 1");
    }

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        EnsureLevelController();

        if (boltSetups.Count == 0)
        {
            CreateDefaultBoltSetups();
        }
    }

    #endregion

    #region Helper Methods

    private bool EnsureLevelController()
    {
        if (levelController == null)
        {
            levelController = FindObjectOfType<LevelController>();

            if (levelController == null)
            {
                Debug.LogError("❌ Không tìm thấy LevelController trong scene!");
                return false;
            }
        }
        return true;
    }

    private bool ValidateBeforeSave()
    {
        if (boltSetups.Count == 0)
        {
            Debug.LogWarning("⚠️ Không thể lưu: Chưa có cấu hình bolt nào!");
            return false;
        }

        if (string.IsNullOrEmpty(levelName.Trim()))
        {
            Debug.LogWarning("⚠️ Không thể lưu: Tên level trống!");
            return false;
        }

        return true;
    }

    // ✅ THÊM: Chỉ load data vào editor, KHÔNG áp dụng vào game scene
    private void LoadLevelDataOnly(int levelId)
    {
        var savedLevel = LevelFileManager.LoadLevel(levelId);

        if (savedLevel != null)
        {
            // ✅ CHỈ load vào editor
            levelName = savedLevel.levelName;
            ConvertFromLevelData(savedLevel.levelData);

            Debug.Log($"📝 Đã load Level {levelId}: {levelName} với {boltSetups.Count} bolts (chỉ hiển thị data)");
        }
        else
        {
            // Nếu level không tồn tại, tạo level trống
            levelName = $"Level {levelId}";
            CreateDefaultBoltSetups();
            Debug.LogWarning($"⚠️ Level {levelId} không tồn tại, tạo level mặc định");
        }
    }

    // ✅ THÊM: Load data VÀ áp dụng vào game scene
    private void LoadAndApplyLevelData(int levelId)
    {
        var savedLevel = LevelFileManager.LoadLevel(levelId);

        if (savedLevel != null)
        {
            // Load vào editor
            levelName = savedLevel.levelName;
            ConvertFromLevelData(savedLevel.levelData);

            // ✅ ÁP DỤNG vào game để xem trước
            LevelFileManager.ApplyLevelToGame(savedLevel.levelData, levelId);

            Debug.Log($"🎮 Đã load và áp dụng Level {levelId}: {levelName} với {boltSetups.Count} bolts");
        }
        else
        {
            // Nếu level không tồn tại, tạo level trống
            levelName = $"Level {levelId}";
            CreateDefaultBoltSetups();
            Debug.LogWarning($"⚠️ Level {levelId} không tồn tại, tạo level mặc định");
        }
    }

    private LevelData ConvertToLevelData()
    {
        var levelData = new LevelData();
        levelData.lsDataBolt = new List<DataBolt>();

        for (int i = 0; i < boltSetups.Count; i++)
        {
            levelData.lsDataBolt.Add(new DataBolt
            {
                idBolt = i + 1,
                lsIdScrew = new List<int>(boltSetups[i].screwIds)
            });
        }

        return levelData;
    }

    private void ConvertFromLevelData(LevelData levelData)
    {
        boltSetups.Clear();

        if (levelData?.lsDataBolt != null)
        {
            foreach (var dataBolt in levelData.lsDataBolt)
            {
                boltSetups.Add(new BoltSetup
                {
                    screwIds = new List<int>(dataBolt.lsIdScrew ?? new List<int>())
                });
            }
        }
    }

    private void CreateDefaultBoltSetups()
    {
        boltSetups.Clear();
        for (int i = 0; i < 3; i++)
        {
            boltSetups.Add(new BoltSetup
            {
                screwIds = new List<int> { 1, 2, 1, 2, 3 }
            });
        }
        Debug.Log("🔧 Tạo cấu hình bolt mặc định");
    }

    #endregion

    #region Inner Classes

    [System.Serializable]
    public class BoltSetup
    {
        [ListDrawerSettings(ShowIndexLabels = true)]
        [LabelText("Screw IDs (1-5)")]
        public List<int> screwIds = new List<int> { 1, 1, 1, 1, 1 };
    }

    #endregion
}