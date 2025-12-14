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

        // ✅ CHỈ ĐẢM BẢO SOUNDMANAGER TỒN TẠI CHO SFX
        EnsureSoundManagerExists();
    }

    void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        // Khởi tạo game systems
        gameScene.Init();
        gameContaint.Init();

        Debug.Log("🎮 GamePlayerController đã khởi tạo");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleStartMenu();
        }
    }

    // ✅ CHỈ ĐẢM BẢO SOUNDMANAGER TỒN TẠI CHO SFX
    private void EnsureSoundManagerExists()
    {
        if (SoundManager.Instance == null)
        {
            GameObject soundManagerGO = new GameObject("SoundManager");
            soundManagerGO.AddComponent<SoundManager>();
            Debug.Log("🔊 Auto-created SoundManager for SFX");
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