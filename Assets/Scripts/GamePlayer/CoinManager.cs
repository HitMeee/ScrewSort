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

    public void AddLevelReward()
    {
        AddCoins(50);
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "" + coins;
    }

    public int GetCoins() => coins;
}