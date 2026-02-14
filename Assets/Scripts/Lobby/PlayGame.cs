using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{
    [SerializeField] private Button btnPlayGame;
    void Start()
    {
        btnPlayGame.onClick.AddListener(OnPlayButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
