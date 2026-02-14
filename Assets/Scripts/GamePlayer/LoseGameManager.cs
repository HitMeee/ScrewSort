using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoseGameManager : MonoBehaviour
{
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button btnRetry;
    [SerializeField] private Button btnExit;

    void Awake()
    {
        btnRetry.onClick.AddListener(OnRetry);
        btnExit.onClick.AddListener(OnExit);
    }
    private void OnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnExit()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
