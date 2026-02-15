using UnityEngine;
using TMPro;

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
        AddCoins(reward);
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "" + coins;
    }

    public int GetCoins() => coins;
}