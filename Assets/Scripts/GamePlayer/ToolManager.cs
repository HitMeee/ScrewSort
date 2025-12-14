using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class ToolManager : MonoBehaviour
{
    [Header("🎨 Tool Data List")]
    [SerializeField] private List<ToolData> toolDataList = new List<ToolData>();

    [Header("🛠️ Tool UI")]
    [SerializeField] private Button backStepButton;
    [SerializeField] private TextMeshProUGUI backStepCountText;
    [SerializeField] private Button addBoltButton;
    [SerializeField] private TextMeshProUGUI addBoltCountText;

    private const string TOOL_COUNT_KEY = "ToolCount_";

    void Start()
    {
        InitializeToolData();
        LoadCounts();
        SetupButtons();
        UpdateUI();
    }

    private void InitializeToolData()
    {
        if (toolDataList.Count < 2)
        {
            toolDataList.Clear();

            toolDataList.Add(new ToolData
            {
                id = 1,
                nameTools = "Quay lại",
                titleTools = "Hoàn tác bước đi",
                price = 100,
                currentCount = 3
            });

            toolDataList.Add(new ToolData
            {
                id = 2,
                nameTools = "Thêm ốc",
                titleTools = "Tạo ốc trống mới",
                price = 150,
                currentCount = 3
            });
        }
    }

    private void LoadCounts()
    {
        foreach (var tool in toolDataList)
        {
            tool.currentCount = PlayerPrefs.GetInt(TOOL_COUNT_KEY + tool.id, 3);
        }
    }

    private void SaveCounts()
    {
        foreach (var tool in toolDataList)
        {
            PlayerPrefs.SetInt(TOOL_COUNT_KEY + tool.id, tool.currentCount);
        }
        PlayerPrefs.Save();
    }

    private void SetupButtons()
    {
        if (backStepButton != null)
            backStepButton.onClick.AddListener(() => OnToolClicked(1));

        if (addBoltButton != null)
            addBoltButton.onClick.AddListener(() => OnToolClicked(2));
    }

    public void OnToolClicked(int toolId)
    {
        ToolData tool = GetToolById(toolId);
        if (tool == null) return;

        if (tool.HasCount())
        {
            UseTool(tool);
        }
        else
        {
            ShowBuyUI(tool);
        }
    }

    private ToolData GetToolById(int id)
    {
        return toolDataList.FirstOrDefault(tool => tool.id == id);
    }

    private void UseTool(ToolData tool)
    {
        if (tool.id == 1) // BackStep
        {
            var backStep = FindObjectOfType<BackStep>();
            if (backStep != null && backStep.HasHistory())
            {
                backStep.ButtonGoBack();
                tool.UseCount();
                SaveCounts();
                UpdateUI();
                Debug.Log($"⏪ Used {tool.nameTools}. Remaining: {tool.currentCount}");
            }
        }
        else if (tool.id == 2) // AddBolt
        {
            var addBolt = FindObjectOfType<AddBolt>();
            if (addBolt != null && addBolt.CanAddBolt())
            {
                addBolt.ButtonAddBolt();
                tool.UseCount();
                SaveCounts();
                UpdateUI();
                Debug.Log($"🔧 Used {tool.nameTools}. Remaining: {tool.currentCount}");
            }
        }
    }

    private void ShowBuyUI(ToolData tool)
    {
        var uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowBuyToolUI(tool);
            Debug.Log($"🛒 Show buy UI for {tool.nameTools} (ID: {tool.id})");
        }
    }

    public void BuyTool(ToolData tool)
    {
        var coinManager = FindObjectOfType<CoinManager>();
        var uiManager = FindObjectOfType<UIManager>();

        if (coinManager == null || uiManager == null) return;

        int currentCoins = coinManager.GetCoins();

        // ✅ KIỂM TRA ĐỦ TIỀN
        if (currentCoins >= tool.price)
        {
            // Đủ tiền - mua thành công
            coinManager.AddCoins(-tool.price);
            tool.AddCount(1);
            SaveCounts();
            UpdateUI();

            uiManager.HideBuyToolUI();
            Debug.Log($"✅ Bought {tool.nameTools} for {tool.price} coins!");
        }
        else
        {
            // ✅ KHÔNG ĐỦ TIỀN - HIỂN THỊ NOT ENOUGH MONEY UI ĐơN GIẢN
            uiManager.ShowNotEnoughMoneyUI();
            Debug.Log($"❌ Not enough coins! Need {tool.price}, have {currentCoins}");
        }
    }

    private void UpdateUI()
    {
        ToolData backStepTool = GetToolById(1);
        ToolData addBoltTool = GetToolById(2);

        // BackStep UI
        if (backStepCountText != null && backStepTool != null)
            backStepCountText.text = backStepTool.currentCount.ToString();

        if (backStepButton != null && backStepTool != null)
        {
            backStepButton.interactable = true;
            var canvasGroup = backStepButton.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = backStepButton.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = backStepTool.HasCount() ? 1f : 0.5f;
        }

        // AddBolt UI
        if (addBoltCountText != null && addBoltTool != null)
            addBoltCountText.text = addBoltTool.currentCount.ToString();

        if (addBoltButton != null && addBoltTool != null)
        {
            addBoltButton.interactable = true;
            var canvasGroup = addBoltButton.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = addBoltButton.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = addBoltTool.HasCount() ? 1f : 0.5f;
        }
    }

    // Public methods
    public ToolData GetToolData(int id) => GetToolById(id);

    public void ResetTools()
    {
        foreach (var tool in toolDataList)
        {
            tool.currentCount = 3;
        }
        SaveCounts();
        UpdateUI();
    }

    public void AddToolData(ToolData newTool)
    {
        if (toolDataList.Any(t => t.id == newTool.id)) return;
        toolDataList.Add(newTool);
    }
}