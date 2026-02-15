using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

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
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;

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
    private int currentStars = 1; // Lưu số sao hiện tại

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
        
        // Mặc định ẩn tất cả sao
        if (star1 != null) star1.SetActive(false);
        if (star2 != null) star2.SetActive(false);
        if (star3 != null) star3.SetActive(false);
    }

    public void ShowComplete(int stars = 1)
    {
        // Lưu số sao để tính coin reward sau
        currentStars = stars;
        
        // Hiển thị popup ngay lập tức
        completeUI.SetActive(true);
        
        // Ẩn tất cả sao trước
        if (star1 != null) star1.SetActive(false);
        if (star2 != null) star2.SetActive(false);
        if (star3 != null) star3.SetActive(false);
        
        // Bắt đầu coroutine để hiển thị sao với animation
        StartCoroutine(ShowStarsWithAnimation(stars));
        
        Debug.Log($"⭐ Sẽ hiển thị {stars} sao sau 0.5s với animation");
    }
    
    private IEnumerator ShowStarsWithAnimation(int stars)
    {
        // Delay 0.5s trước khi hiển thị sao
        yield return new WaitForSeconds(0.5f);
        
        // Hiển thị Star 1 nếu có
        if (stars >= 1 && star1 != null)
        {
            star1.SetActive(true);
            star1.transform.localScale = Vector3.one * 0.3f;
            star1.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            
            // Delay nhỏ giữa các sao
            yield return new WaitForSeconds(0.2f);
        }
        
        // Hiển thị Star 2 nếu có
        if (stars >= 2 && star2 != null)
        {
            star2.SetActive(true);
            star2.transform.localScale = Vector3.one * 0.3f;
            star2.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            
            yield return new WaitForSeconds(0.2f);
        }
        
        // Hiển thị Star 3 nếu có
        if (stars >= 3 && star3 != null)
        {
            star3.SetActive(true);
            star3.transform.localScale = Vector3.one * 0.3f;
            star3.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
        
        Debug.Log($"✨ Đã hiển thị {stars} sao với animation");
    }

    private void OnNext()
    {
        // ✅ PHÁT ÂM THANH BUTTON
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        // Tính coin reward theo số sao
        coinManager.AddLevelReward(currentStars);
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