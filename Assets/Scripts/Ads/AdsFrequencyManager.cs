using UnityEngine;

/// <summary>
/// Quản lý tần suất hiển thị Interstitial Ads
/// </summary>
public class AdsFrequencyManager : MonoBehaviour
{
    public static AdsFrequencyManager Instance { get; private set; }

    [Header("⚙️ Cấu hình Ads")]
    [SerializeField] private int winsBeforeAd = 3; // Số lần thắng trước khi hiện ads
    [SerializeField] private int losesBeforeAd = 2; // Số lần thua trước khi hiện ads
    
    private int currentWinCount = 0;
    private int currentLoseCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gọi khi người chơi THẮNG level
    /// </summary>
    public void OnLevelWin()
    {
        currentWinCount++;
        currentLoseCount = 0; // Reset lose count khi thắng
        Debug.Log($"🏆 Win count: {currentWinCount}/{winsBeforeAd} (Lose count reset)");

        // Nếu đủ số lần thắng → Hiện ads
        if (currentWinCount >= winsBeforeAd)
        {
            ShowInterstitialAd("Win " + winsBeforeAd + " levels");
            currentWinCount = 0; // Reset counter
        }
    }

    /// <summary>
    /// Gọi khi người chơi THUA (hết thời gian)
    /// </summary>
    public void OnLevelLose()
    {
        currentLoseCount++;
        Debug.Log($"❌ Lose count: {currentLoseCount}/{losesBeforeAd}");

        // Nếu đủ số lần thua → Hiện ads
        if (currentLoseCount >= losesBeforeAd)
        {
            ShowInterstitialAd("Lose " + losesBeforeAd + " times");
            currentLoseCount = 0; // Reset counter
        }
    }

    /// <summary>
    /// Gọi khi người chơi THOÁT màn hình gameplay
    /// </summary>
    public void OnExitGameplay()
    {
        Debug.Log("🚪 Player exiting gameplay - Showing ads");
        ShowInterstitialAd("Exit gameplay");
    }

    /// <summary>
    /// Hiển thị Interstitial Ad
    /// </summary>
    private void ShowInterstitialAd(string reason)
    {
        if (AdsManager.Instance != null)
        {
            Debug.Log($"📺 Showing Interstitial Ad - Reason: {reason}");
            AdsManager.Instance.ShowInterstitialAd();
        }
        else
        {
            Debug.LogWarning("⚠️ AdsManager not found!");
        }
    }

    /// <summary>
    /// Reset counter (nếu cần)
    /// </summary>
    public void ResetWinCount()
    {
        currentWinCount = 0;
        currentLoseCount = 0;
        Debug.Log("🔄 Win & Lose count reset");
    }

    /// <summary>
    /// Getter
    /// </summary>
    public int GetCurrentWinCount() => currentWinCount;
    public int GetCurrentLoseCount() => currentLoseCount;
}
