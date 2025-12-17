using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerController : MonoBehaviour
{
    public static GamePlayerController Instance;

    public GameScene gameScene;
    public GameContaint gameContaint;
    public UIManager uiManager;

    private void Awake()
    {
        Instance = this;

    }

    void Start()
    {
        gameScene.Init();
        gameContaint.Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleStartMenu();
        }
    }



    public void ToggleStartMenu()
    {
        if (uiManager != null)
        {
            if (uiManager.IsStartMenuActive())
            {
                uiManager.HideStartMenu();
            }
            else
            {
                uiManager.ShowStartMenu();
            }
        }
    }

    public void StartGame()
    {
        if (uiManager != null)
        {
            uiManager.HideStartMenu();
        }

        Debug.Log("🚀 Game Started!");
    }

    public void ReturnToMenu()
    {
        if (uiManager != null)
        {
            uiManager.ReturnToStartMenu();
        }
    }
}