using UnityEngine;
using TMPro;
using DG.Tweening;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    private int coins = 0;

    void Start()
    {
        LoadCoins();
        UpdateUI();
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
    }

    public void AddCoins(int amount)
    {
        coins += amount; // Có thể âm để trừ xu
        if (coins < 0) coins = 0; // Không cho âm
        SaveCoins();
        UpdateUI();
    }

    /// <summary>
    /// Thêm coin với hiệu ứng tăng dần (counter animation)
    /// </summary>
    public void AddCoinsWithAnimation(int amount, float duration = 1.5f)
    {
        int startValue = coins;
        int targetValue = coins + amount;
        if (targetValue < 0) targetValue = 0;

        // Tăng giá trị thật ngay (để save)
        coins = targetValue;
        SaveCoins();

        // Animation UI counter
        DOTween.To(
            () => startValue,
            x => 
            {
                // Cập nhật UI từ từ
                if (coinText != null)
                    coinText.text = "" + Mathf.RoundToInt(x);
            },
            targetValue,
            duration
        ).SetEase(Ease.OutCubic);

        Debug.Log($"💰 Coin tăng từ {startValue} → {targetValue} trong {duration}s");
    }

    public void AddLevelReward(int stars = 1)
    {
        // Tính coin dựa trên số sao
        int reward = 0;
        switch (stars)
        {
            case 1:
                reward = 30;
                break;
            case 2:
                reward = 45;
                break;
            case 3:
                reward = 60;
                break;
            default:
                reward = 30; 
                break;
        }
        
        Debug.Log($"💰 Nhận {reward} coins cho {stars} sao");
        
        // ✅ SỬ DỤNG ANIMATION THAY VÌ CỘNG TRỰC TIẾP
        AddCoinsWithAnimation(reward, 0.5f);
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "" + coins;
    }

    public int GetCoins() => coins;
}