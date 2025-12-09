using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LevelEditorOdin : MonoBehaviour
{
    [SerializeField] private LevelController levelController;

    [HorizontalGroup("Info")]
    [SerializeField] private int currentLevelId = 1;

    [HorizontalGroup("Info")]
    [SerializeField] private string levelName = "New Level";

    [Space]
    [TableList(AlwaysExpanded = true)]
    [SerializeField] private List<BoltSetup> boltSetups = new List<BoltSetup>();

    // ADD BOLT
    [HorizontalGroup("Actions")]
    [Button("Add Bolt"), GUIColor(0.6f, 1f, 0.6f)]
    public void AddBolt()
    {
        boltSetups.Add(new BoltSetup());
        Debug.Log($"➕ Thêm bolt mới. Tổng: {boltSetups.Count}");
    }

    // REMOVE LAST
    [HorizontalGroup("Actions")]
    [Button("Remove Last"), GUIColor(1f, 0.6f, 0.6f)]
    public void RemoveLast()
    {
        if (boltSetups.Count > 0)
        {
            boltSetups.RemoveAt(boltSetups.Count - 1);
            Debug.Log($"➖ Xóa bolt cuối. Còn lại: {boltSetups.Count}");
        }
    }

    // SAVE LEVEL
    [HorizontalGroup("File")]
    [Button("Save Level", ButtonSizes.Large), GUIColor(0.2f, 0.8f, 0.2f)]
    public void SaveLevel()
    {
        if (boltSetups.Count == 0) return;

        var levelData = ConvertToLevelData();
        bool success = LevelFileManager.SaveLevel(currentLevelId, levelName, levelData);

        if (success)
        {
            Debug.Log($"💾 Đã lưu Level {currentLevelId}: {levelName}");
        }
    }

    // LOAD LEVEL - CHỈ load vào editor và game
    [HorizontalGroup("File")]
    [Button("Load Level", ButtonSizes.Large), GUIColor(0.2f, 0.6f, 0.8f)]
    public void LoadLevel()
    {
        var savedLevel = LevelFileManager.LoadLevel(currentLevelId);

        if (savedLevel != null)
        {
            // Load vào editor
            levelName = savedLevel.levelName;
            ConvertFromLevelData(savedLevel.levelData);

            // ✅ Apply vào game khi load
            ApplyToGame();

            Debug.Log($"📂 Đã load Level {currentLevelId}: {levelName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Không tìm thấy Level {currentLevelId}!");
        }
    }

    // DELETE ALL
    [Button("Delete All", ButtonSizes.Large), GUIColor(1f, 0.4f, 0.4f)]
    public void DeleteAll()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            levelController.ClearScene();
            Debug.Log("🗑️ Đã xóa tất cả khỏi scene!");
        }
    }

    // ✅ SỬA: PREV - CHỈ đổi ID, không tự động load
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
                Debug.Log($"⬅️ Chuyển ID về Level {currentLevelId} (chưa load)");
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

    // ✅ SỬA: NEXT - CHỈ đổi ID, không tự động load
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
                Debug.Log($"➡️ Chuyển ID đến Level {currentLevelId} (chưa load)");
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

    // HELPERS
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

        foreach (var dataBolt in levelData.lsDataBolt)
        {
            boltSetups.Add(new BoltSetup { screwIds = new List<int>(dataBolt.lsIdScrew) });
        }
    }

    private void ApplyToGame()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            var levelData = ConvertToLevelData();
            levelController.SetLevelData(levelData);
            levelController.ForceReinit();
            LevelFileManager.SetCurrentLevelId(currentLevelId);
        }
    }

    void Start()
    {
        if (levelController == null)
            levelController = FindObjectOfType<LevelController>();

        if (boltSetups.Count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                boltSetups.Add(new BoltSetup { screwIds = new List<int> { 1, 2, 1, 2, 3 } });
            }
        }
    }


    [System.Serializable]
    public class BoltSetup
    {
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<int> screwIds = new List<int> { 1, 1, 1, 1, 1 };

        [Button("🎲 Random")]
        private void RandomThis()
        {
            for (int i = 0; i < screwIds.Count; i++)
            {
                screwIds[i] = Random.Range(1, 6);
            }
        }
    }
}