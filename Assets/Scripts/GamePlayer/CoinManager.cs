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
        coins += amount;
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
            coinText.text = "Xu: " + coins;
    }

    public int GetCoins() => coins;
}