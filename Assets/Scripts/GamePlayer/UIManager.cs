using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("🎮 Start Menu UI")]
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private Button playButton;
    [SerializeField] private bool showStartMenuOnStart = true;

    [Header("🎊 Complete UI")]
    [SerializeField] private GameObject completeUI;
    [SerializeField] private Button nextButton;
    [SerializeField] private CoinManager coinManager;

    [Header("🛒 Buy Tool UI")]
    [SerializeField] private GameObject buyToolUI;
    [SerializeField] private Button closeBuyButton;
    [SerializeField] private Button backBuyButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI toolNameText;
    [SerializeField] private TextMeshProUGUI toolTitleText;
    [SerializeField] private Image toolImageDisplay;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("💰 Not Enough Money UI")]
    [SerializeField] private GameObject notEnoughMoneyUI;
    [SerializeField] private Button notEnoughMoneyBackButton;

    private ToolData currentToolToBuy;
    private GameScene gameScene;

    void Start()
    {
        gameScene = FindObjectOfType<GameScene>();
        SetupStartMenuUI();
        SetupCompleteUI();
        SetupBuyToolUI();
        SetupNotEnoughMoneyUI();

        if (showStartMenuOnStart)
        {
            ShowStartMenu();
        }
    }

    // ===== START MENU UI =====
    private void SetupStartMenuUI()
    {
        if (startMenuUI != null)
        {
            startMenuUI.SetActive(false);

            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayButtonClicked);
            }
        }
    }

    public void ShowStartMenu()
    {
        if (startMenuUI != null)
        {
            startMenuUI.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("🎮 Start Menu hiển thị");
        }
    }

    public void HideStartMenu()
    {
        if (startMenuUI != null)
        {
            startMenuUI.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("🎮 Start Menu đã ẩn - Game bắt đầu!");
        }
    }

    private void OnPlayButtonClicked()
    {
        // ✅ PHÁT ÂM THANH BUTTON
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        Debug.Log("▶️ Nút Play được nhấn!");
        HideStartMenu();

        if (gameScene != null)
        {
            gameScene.ReloadCurrentLevel();
        }
    }

    public void ReturnToStartMenu()
    {
        ShowStartMenu();
    }

    // ===== COMPLETE UI =====
    private void SetupCompleteUI()
    {
        completeUI.SetActive(false);
        nextButton.onClick.AddListener(OnNext);
    }

    public void ShowComplete()
    {
        completeUI.SetActive(true);
    }

    private void OnNext()
    {
        // ✅ PHÁT ÂM THANH BUTTON
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        coinManager.AddLevelReward();
        completeUI.SetActive(false);

        var gameScene = FindObjectOfType<GameScene>();
        gameScene.LoadNextLevel();
    }

    // ===== BUY TOOL UI =====
    private void SetupBuyToolUI()
    {
        if (buyToolUI != null) buyToolUI.SetActive(false);
        if (closeBuyButton != null) closeBuyButton.onClick.AddListener(HideBuyToolUI);
        if (backBuyButton != null) backBuyButton.onClick.AddListener(HideBuyToolUI);
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyTool);
    }

    public void ShowBuyToolUI(ToolData toolData)
    {
        currentToolToBuy = toolData;

        if (toolNameText != null) toolNameText.text = toolData.nameTools;
        if (toolTitleText != null) toolTitleText.text = toolData.titleTools;
        if (toolImageDisplay != null) toolImageDisplay.sprite = toolData.imageTools;
        if (priceText != null) priceText.text = toolData.price.ToString();

        if (buyToolUI != null) buyToolUI.SetActive(true);
    }

    public void HideBuyToolUI()
    {
        if (buyToolUI != null) buyToolUI.SetActive(false);
        currentToolToBuy = null;
    }

    private void OnBuyTool()
    {
        // ✅ PHÁT ÂM THANH BUTTON
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        if (currentToolToBuy == null) return;

        var toolManager = FindObjectOfType<ToolManager>();
        if (toolManager != null)
        {
            toolManager.BuyTool(currentToolToBuy);
        }
    }

    // ===== 💰 NOT ENOUGH MONEY UI - ĐƠN GIẢN =====
    private void SetupNotEnoughMoneyUI()
    {
        if (notEnoughMoneyUI != null)
        {
            notEnoughMoneyUI.SetActive(false);
        }

        if (notEnoughMoneyBackButton != null)
        {
            notEnoughMoneyBackButton.onClick.AddListener(HideNotEnoughMoneyUI);
        }
    }

    public void ShowNotEnoughMoneyUI()
    {
        if (notEnoughMoneyUI == null) return;

        // ✅ ẨN BUY TOOL UI VÀ HIỆN NOT ENOUGH MONEY UI
        HideBuyToolUI();
        notEnoughMoneyUI.SetActive(true);

        Debug.Log("💰 Showing not enough money UI");
    }

    public void HideNotEnoughMoneyUI()
    {
        // ✅ PHÁT ÂM THANH BUTTON
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        if (notEnoughMoneyUI != null)
        {
            notEnoughMoneyUI.SetActive(false);
        }

        Debug.Log("💰 Hidden not enough money UI");
    }

    // ===== PUBLIC METHODS =====
    public bool IsStartMenuActive()
    {
        return startMenuUI != null && startMenuUI.activeSelf;
    }

    public void DisableStartMenuOnStart()
    {
        showStartMenuOnStart = false;
    }

    public bool IsAnyUIActive()
    {
        return (startMenuUI != null && startMenuUI.activeSelf) ||
               (completeUI != null && completeUI.activeSelf) ||
               (buyToolUI != null && buyToolUI.activeSelf) ||
               (notEnoughMoneyUI != null && notEnoughMoneyUI.activeSelf);
    }
}