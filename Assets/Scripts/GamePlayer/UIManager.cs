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

    private ToolData currentToolToBuy;
    private GameScene gameScene;

    void Start()
    {
        gameScene = FindObjectOfType<GameScene>();

        SetupStartMenuUI();
        SetupCompleteUI();
        SetupBuyToolUI();

        // Hiển thị Start Menu khi bắt đầu game
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

            // Tạm dừng game khi hiển thị Start Menu
            Time.timeScale = 0f;

            Debug.Log("🎮 Start Menu hiển thị");
        }
    }

    public void HideStartMenu()
    {
        if (startMenuUI != null)
        {
            startMenuUI.SetActive(false);

            // Tiếp tục game khi ẩn Start Menu
            Time.timeScale = 1f;

            Debug.Log("🎮 Start Menu đã ẩn - Game bắt đầu!");
        }
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("▶️ Nút Play được nhấn!");

        // Ẩn Start Menu
        HideStartMenu();

        // Bắt đầu game từ level hiện tại hoặc level 1
        if (gameScene != null)
        {
            // Có thể load level từ đầu hoặc tiếp tục level hiện tại
            // gameScene.RestartFromLevel1(); // Nếu muốn bắt đầu từ Level 1
            gameScene.ReloadCurrentLevel(); // Hoặc tiếp tục level hiện tại
        }
    }

    // Phương thức để show Start Menu từ ngoài (ví dụ từ Pause menu)
    public void ReturnToStartMenu()
    {
        ShowStartMenu();
    }

    private void SetupCompleteUI()
    {
        completeUI.SetActive(false);
        nextButton.onClick.AddListener(OnNext);
    }

    private void SetupBuyToolUI()
    {
        if (buyToolUI != null) buyToolUI.SetActive(false);
        if (closeBuyButton != null) closeBuyButton.onClick.AddListener(HideBuyToolUI);
        if (backBuyButton != null) backBuyButton.onClick.AddListener(HideBuyToolUI);
        if (buyButton != null) buyButton.onClick.AddListener(OnBuyTool);
    }

    // ===== COMPLETE UI =====
    public void ShowComplete()
    {
        completeUI.SetActive(true);
    }

    private void OnNext()
    {
        coinManager.AddLevelReward();
        completeUI.SetActive(false);

        var gameScene = FindObjectOfType<GameScene>();
        gameScene.LoadNextLevel();
    }

    // ===== BUY TOOL UI =====
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
        if (currentToolToBuy == null) return;

        var toolManager = FindObjectOfType<ToolManager>();
        if (toolManager != null)
        {
            toolManager.BuyTool(currentToolToBuy);
        }
    }

    // ===== PUBLIC METHODS =====
    public bool IsStartMenuActive()
    {
        return startMenuUI != null && startMenuUI.activeSelf;
    }

    // Phương thức để tắt Start Menu từ code khác
    public void DisableStartMenuOnStart()
    {
        showStartMenuOnStart = false;
    }
}