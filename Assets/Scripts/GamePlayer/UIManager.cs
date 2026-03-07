using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button ClosePoppup;
    [SerializeField] private CoinFlyEffect coinFlyEffect; // ✅ Hiệu ứng coin bay

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
    [SerializeField] private Button watchAdsButton;
    [SerializeField] private GameObject PanelBlack;

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
        ClosePoppup.onClick.AddListener(ClosePopupComplete);
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
    public void ClosePopupComplete()
    {
        SceneManager.LoadScene("LobbyScene");
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
        if (completeUI != null)
        {
            nextButton.interactable = true; // Đảm bảo nút Next có thể nhấn được khi hiển thị popup
            ClosePoppup.interactable = true; // Đảm bảo nút Close có thể
                                             // Lưu số sao để tính coin reward sau
            currentStars = stars;
            PanelBlack.SetActive(true);
            // Hiển thị popup ngay lập tức
            completeUI.SetActive(true);
            completeUI.transform.localScale = Vector3.zero;
            completeUI.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            // Cập nhật reward text theo số sao
            UpdateRewardText(stars);

            // Ẩn tất cả sao trước
            if (star1 != null) star1.SetActive(false);
            if (star2 != null) star2.SetActive(false);
            if (star3 != null) star3.SetActive(false);

            // Bắt đầu coroutine để hiển thị sao với animation
            StartCoroutine(ShowStarsWithAnimation(stars));

            Debug.Log($"⭐ Sẽ hiển thị {stars} sao sau 0.5s với animation");
        }
    }

    private void UpdateRewardText(int stars)
    {
        if (rewardText == null) return;

        int reward = 0;
        switch (stars)
        {
            case 1: reward = 30; break;
            case 2: reward = 45; break;
            case 3: reward = 60; break;
            default: reward = 30; break;
        }

        rewardText.text = "+" + reward;
        Debug.Log($"💰 Cập nhật reward text: +{reward}");
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
        PanelBlack.SetActive(false);
        // ✅ PHÁT ÂM THANH BUTTON
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        // ✅ GỌI ADS FREQUENCY MANAGER KHI THẮNG
        if (AdsFrequencyManager.Instance != null)
        {
            AdsFrequencyManager.Instance.OnLevelWin();
        }

        DOVirtual.DelayedCall(0.8f, () =>
        {
            SoundManager.Instance.PlayCollectCoin();
        });
        nextButton.interactable = false;
        ClosePoppup.interactable = false;

        // ✅ CHẠY HIỆU ỨNG COIN BAY
        if (coinFlyEffect != null)
        {
            coinFlyEffect.PlayCoinFlyEffect();

            // Delay để chờ coin bay xong rồi mới thực hiện logic tiếp
            float totalDelay = coinFlyEffect.GetTotalAnimationTime();
            DOVirtual.DelayedCall(totalDelay, () =>
            {
                // Tính coin reward theo số sao
                coinManager.AddLevelReward(currentStars);
                completeUI.SetActive(false);

                var gameScene = FindObjectOfType<GameScene>();
                gameScene.LoadNextLevel();
            });
        }
        else
        {
            // Nếu không có effect thì chạy logic cũ
            coinManager.AddLevelReward(currentStars);
            completeUI.SetActive(false);

            nextButton.interactable = true;
            ClosePoppup.interactable = true;// Reset trạng thái button cho lần sau

            var gameScene = FindObjectOfType<GameScene>();
            gameScene.LoadNextLevel();
        }
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

        PanelBlack.SetActive(true);
        if (buyToolUI != null)
        {
            buyToolUI.SetActive(true);
            buyToolUI.transform.localScale = Vector3.zero;
            buyToolUI.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        Time.timeScale = 0f;
    }

    public void HideBuyToolUI()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        if (buyToolUI != null)
        {
            buyToolUI.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() => buyToolUI.SetActive(false));
        }
        PanelBlack.SetActive(false);
        buyToolUI.SetActive(false);
        currentToolToBuy = null;
        Time.timeScale = 1f;
    }

    private void OnBuyTool()
    {
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

        if (watchAdsButton != null)
        {
            watchAdsButton.onClick.AddListener(OnWatchAds);
        }
    }

    public void ShowNotEnoughMoneyUI()
    {
        if (notEnoughMoneyUI == null) return;

        // ✅ ẨN BUY TOOL UI VÀ HIỆN NOT ENOUGH MONEY UI
        HideBuyToolUI();
        PanelBlack.SetActive(true);
        if (notEnoughMoneyUI != null)
        {
            notEnoughMoneyUI.SetActive(true);
            notEnoughMoneyUI.transform.localScale = Vector3.zero;
            notEnoughMoneyUI.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        Time.timeScale = 0f; // Tạm dừng game khi mở UI

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
            PanelBlack.SetActive(false);
            notEnoughMoneyUI.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).SetUpdate(true)
                .OnComplete(() => notEnoughMoneyUI.SetActive(false));
        }
        Time.timeScale = 1f; // Resume game when closing UI
        Debug.Log("💰 Hidden not enough money UI");
    }

    // ===== 📺 WATCH ADS FOR COINS =====
    private void OnWatchAds()
    {
        // ✅ PHÁT ÂM THANH BUTTON
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        Debug.Log("📺 Người dùng nhấn Watch Ads");

        // ✅ ẨN POPUP NGAY KHI BẮT ĐẦU XEM ADS
        HideNotEnoughMoneyUI();

        // Tạm dừng time scale về 1 để ads có thể hiển thị
        Time.timeScale = 1f;

        // Gọi Rewarded Ad
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowRewardedAd((reward) =>
            {
                // ✅ Callback này CHỈ ĐƯỢC GỌI KHI NGƯỜI DÙNG ĐÓNG ADS
                Debug.Log($"✅ Người dùng đã ĐÓNG ads! Nhận thưởng: {reward.Amount} {reward.Type}");

                // ✅ CỘNG COIN NGAY KHI ĐÓNG ADS (không cần delay nữa)
                if (coinManager != null)
                {
                    coinManager.AddCoinsWithAnimation(100, 1.5f);
                    Debug.Log("💰 Đang cộng 100 coin với animation!");
                    
                    // ✅ PHÁT ÂM THANH COIN
                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlayCollectCoin();
                    }
                }

                // Có thể thử mua lại tool nếu đủ tiền
                if (currentToolToBuy != null)
                {
                    var toolManager = FindObjectOfType<ToolManager>();
                    if (toolManager != null && coinManager != null)
                    {
                        // Delay nhỏ để người dùng nhìn thấy coin tăng
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            // Kiểm tra lại xem đã đủ tiền chưa
                            if (coinManager.GetCoins() >= currentToolToBuy.price)
                            {
                                Debug.Log("✅ Đã đủ tiền! Hiển thị lại popup mua tool.");
                                ShowBuyToolUI(currentToolToBuy);
                            }
                            else
                            {
                                Debug.Log("⚠️ Vẫn chưa đủ tiền để mua tool.");
                            }
                        });
                    }
                }

                // Resume time scale nếu không có UI nào hiển thị
                if (!IsAnyUIActive())
                {
                    Time.timeScale = 1f;
                }
            });
        }
        else
        {
            Debug.LogError("❌ AdsManager không tồn tại!");
            Time.timeScale = 0f; // Quay lại time scale 0 nếu không có ads
        }
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