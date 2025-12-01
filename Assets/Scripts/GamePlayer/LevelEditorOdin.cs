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

    [HorizontalGroup("Info")]
    [Button("Next ID")]
    private void GetNextId()
    {
        currentLevelId = LevelFileManager.GetNextAvailableLevelId();
        levelName = $"Level {currentLevelId}";
    }

    [Space]
    [TableList(AlwaysExpanded = true)]
    [SerializeField] private List<BoltSetup> boltSetups = new List<BoltSetup>();

    [HorizontalGroup("Actions")]
    [Button("Add Bolt"), GUIColor(0.6f, 1f, 0.6f)]
    public void AddBolt() => boltSetups.Add(new BoltSetup());

    [HorizontalGroup("Actions")]
    [Button("Remove Last"), GUIColor(1f, 0.6f, 0.6f)]
    public void RemoveBolt() { if (boltSetups.Count > 0) boltSetups.RemoveAt(boltSetups.Count - 1); }

    [HorizontalGroup("Actions")]
    [Button("Random All"), GUIColor(1f, 1f, 0.6f)]
    public void RandomizeAll()
    {
        foreach (var bolt in boltSetups)
            for (int i = 0; i < bolt.screwIds.Count; i++)
                bolt.screwIds[i] = Random.Range(1, 6);
    }

    [HorizontalGroup("Templates")]
    [Button("Easy"), GUIColor(0.6f, 1f, 0.6f)]
    public void CreateEasy()
    {
        boltSetups.Clear();
        for (int i = 0; i < 3; i++)
            boltSetups.Add(new BoltSetup { screwIds = new List<int> { 1, 1, 2, 2, 3 } });
        levelName = $"Easy Level {currentLevelId}";
    }

    [HorizontalGroup("Templates")]
    [Button("Hard"), GUIColor(1f, 0.6f, 0.6f)]
    public void CreateHard()
    {
        boltSetups.Clear();
        for (int i = 0; i < 5; i++)
        {
            var setup = new BoltSetup();
            for (int j = 0; j < 5; j++) setup.screwIds.Add(Random.Range(1, 6));
            boltSetups.Add(setup);
        }
        levelName = $"Hard Level {currentLevelId}";
    }

    [HorizontalGroup("File")]
    [Button("Save Level", ButtonSizes.Large), GUIColor(0.2f, 0.8f, 0.2f)]
    public void SaveLevel()
    {
        if (boltSetups.Count == 0)
        {
            Debug.LogWarning("⚠️ No bolt setups to save!");
            return;
        }

        var gameData = new LevelData { lsDataBolt = new List<DataBolt>() };
        for (int i = 0; i < boltSetups.Count; i++)
        {
            gameData.lsDataBolt.Add(new DataBolt
            {
                idBolt = i + 1,
                lsIdScrew = new List<int>(boltSetups[i].screwIds)
            });
        }

        bool success = LevelFileManager.SaveLevel(currentLevelId, levelName, gameData);

        if (success)
        {
            Debug.Log($"✅ Successfully saved Level {currentLevelId}: {levelName}");
        }
    }

    [HorizontalGroup("File")]
    [Button("Load & Play", ButtonSizes.Large), GUIColor(0.2f, 0.6f, 0.8f)]
    public void LoadAndPlay()
    {
        var savedLevel = LevelFileManager.LoadLevel(currentLevelId);
        if (savedLevel != null)
        {
            // Load into editor
            levelName = savedLevel.levelName;
            boltSetups.Clear();
            foreach (var bolt in savedLevel.levelData.lsDataBolt)
                boltSetups.Add(new BoltSetup { screwIds = new List<int>(bolt.lsIdScrew) });

            Debug.Log($"📂 Loaded Level {currentLevelId}: {levelName} into editor");

            // Load into game and set as current level
            var gameScene = FindObjectOfType<GameScene>();
            if (gameScene != null)
            {
                gameScene.LoadLevelById(currentLevelId);
                Debug.Log($"🎮 Set Level {currentLevelId} as current game level");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Level {currentLevelId} not found!");
        }
    }

    [Button("Apply To Game", ButtonSizes.Large), GUIColor(0.2f, 0.8f, 0.2f)]
    public void ApplyToGame()
    {
        if (levelController == null) levelController = FindObjectOfType<LevelController>();

        var gameData = new LevelData { lsDataBolt = new List<DataBolt>() };
        for (int i = 0; i < boltSetups.Count; i++)
        {
            gameData.lsDataBolt.Add(new DataBolt
            {
                idBolt = i + 1,
                lsIdScrew = new List<int>(boltSetups[i].screwIds)
            });
        }

        var field = typeof(LevelController).GetField("levelDatas",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(levelController, gameData);

        levelController.ForceReinit();

        // Set as current level for auto progression
        LevelFileManager.SetCurrentLevelId(currentLevelId);

        Debug.Log($"✅ Applied Level {currentLevelId}: {levelName} to game");
    }

    [HorizontalGroup("File")]
    [Button("Delete All", ButtonSizes.Large), GUIColor(1f, 0.4f, 0.4f)]
    public void DeleteAll()
    {
        if (levelController == null) levelController = FindObjectOfType<LevelController>();

        if (levelController != null)
        {
            var clearMethod = typeof(LevelController).GetMethod("ClearBolts",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            clearMethod?.Invoke(levelController, null);

            var isInitField = typeof(LevelController).GetField("isInitialized",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            isInitField?.SetValue(levelController, false);

            Debug.Log("🗑️ Cleared all bolts from scene!");
        }
    }

    [HorizontalGroup("Nav")]
    [Button("Prev")]
    public void PreviousLevel()
    {
        var allIds = LevelFileManager.GetAllLevelIds();
        if (allIds.Count > 0)
        {
            int index = allIds.IndexOf(currentLevelId);
            if (index > 0)
            {
                currentLevelId = allIds[index - 1];
                LoadAndPlay();
            }
        }
    }

    [HorizontalGroup("Nav")]
    [Button("Next")]
    public void NextLevel()
    {
        var allIds = LevelFileManager.GetAllLevelIds();
        if (allIds.Count > 0)
        {
            int index = allIds.IndexOf(currentLevelId);
            if (index >= 0 && index < allIds.Count - 1)
            {
                currentLevelId = allIds[index + 1];
                LoadAndPlay();
            }
        }
    }

    [HorizontalGroup("Nav")]
    [Button("List All")]
    public void ListAllLevels() => LevelFileManager.ListAllLevels();

    void Start()
    {
        if (levelController == null) levelController = FindObjectOfType<LevelController>();
        if (boltSetups.Count == 0)
        {
            for (int i = 0; i < 3; i++)
                boltSetups.Add(new BoltSetup { screwIds = new List<int> { 1, 2, 1, 2, 3 } });
        }
    }

    [System.Serializable]
    public class BoltSetup
    {
        [ListDrawerSettings(ShowIndexLabels = true)]
        public List<int> screwIds = new List<int> { 1, 1, 1, 1, 1 };

        [Button("🎲")]
        private void RandomizeThis()
        {
            for (int i = 0; i < screwIds.Count; i++)
                screwIds[i] = Random.Range(1, 6);
        }
    }
}