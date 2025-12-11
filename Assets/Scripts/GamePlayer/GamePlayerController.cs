using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerController : MonoBehaviour
{
    public static GamePlayerController Instance;

    public GameScene gameScene;
    public GameContaint gameContaint;
    public UIManager uiManager; // ✅ THÊM: Reference đến UIManager

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Tìm UIManager nếu chưa được gán
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
        // Có thể thêm logic kiểm tra ESC để mở Start Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleStartMenu();
        }
    }

    // Phương thức để toggle Start Menu (Pause/Resume)
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

    // Phương thức để bắt đầu game
    public void StartGame()
    {
        if (uiManager != null)
        {
            uiManager.HideStartMenu();
        }

        // Có thể thêm logic khởi tạo game khác ở đây
        Debug.Log("🚀 Game Started!");
    }

    // Phương thức để quay về Start Menu
    public void ReturnToMenu()
    {
        if (uiManager != null)
        {
            uiManager.ReturnToStartMenu();
        }
    }
}