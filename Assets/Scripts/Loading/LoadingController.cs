using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class LoadingController : MonoBehaviour
{
    public Image FillImage; 
    
    private float _timer;

    void Start()
    {
        if (FillImage != null)
        {
            FillImage.fillAmount = 0f;
        }
        _timer = 0f;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (FillImage != null)
        {

            float tiLeFill = _timer / 3f;
            FillImage.fillAmount = tiLeFill;
        }

        if (_timer >= 3f)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
}