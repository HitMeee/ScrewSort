using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReLoadGame : MonoBehaviour
{
    [SerializeField] private Button reloadButton;

    private void Start()
    {
        if (reloadButton != null)
        {
            reloadButton.onClick.AddListener(OnReloadButtonClicked);
        }
    }

    private void OnReloadButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Tải lại scene hiện tại
    }
}
