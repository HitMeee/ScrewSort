using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject completeUI;
    [SerializeField] private Button nextButton;
    [SerializeField] private CoinManager coinManager;

    void Start()
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
        coinManager.AddLevelReward();
        completeUI.SetActive(false);

        var gameScene = FindObjectOfType<GameScene>();
        gameScene.LoadNextLevel();
    }
}