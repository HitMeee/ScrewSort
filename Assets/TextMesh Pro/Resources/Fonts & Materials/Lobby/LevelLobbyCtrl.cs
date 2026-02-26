using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelLobbyCtrl : MonoBehaviour
{
    [Header("Map Navigation")]
    public MapNavigationCtrl mapNavigationCtrl; // Quản lý điều hướng giữa các map
    
    [Header("UI Buttons")]
    public Button btnShowAllLevel;
    
    private LevelButton[] allLevelButtons; // 🔒 Cache tất cả level buttons
    private int lastPlayedLevel = 1; // Level cuối cùng đã chơi
    
    public void Start()
    {
        if (btnShowAllLevel != null)
        {
            btnShowAllLevel.onClick.AddListener(ShowAllLevel);
        }
        
        // 🔒 Tìm tất cả level buttons trong scene
        allLevelButtons = FindObjectsOfType<LevelButton>();
        
        // 🔒 Refresh trạng thái lock/unlock khi vào lobby
        RefreshAllLevelButtons();
        
        // Tải level cuối cùng đã chơi từ PlayerPrefs
        lastPlayedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        Debug.Log($"🎮 Lobby initialized với {allLevelButtons.Length} level buttons, hiện tại ở level {lastPlayedLevel}");
    }
    
    public void ShowAllLevel()
    {
        // ✅ THÊM NULL CHECK CHO SOUNDMANAGER
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
        else
        {
            Debug.LogWarning("⚠️ SoundManager.Instance is null!");
        }
        
        if (mapNavigationCtrl != null)
        {
            mapNavigationCtrl.ShowMapForLevel(lastPlayedLevel);
        }
        else
        {
            Debug.LogWarning("⚠️ MapNavigationCtrl chưa được gán!");
        }
        
        // 🔒 Refresh lại trạng thái khi mở panel (phòng trường hợp unlock level mới)
        RefreshAllLevelButtons();
    }

    public void RefreshAllLevelButtons()
    {
        if (allLevelButtons == null || allLevelButtons.Length == 0)
        {
            allLevelButtons = FindObjectsOfType<LevelButton>();
        }
        
        foreach (var levelButton in allLevelButtons)
        {
            if (levelButton != null)
            {
                levelButton.RefreshLockState();
            }
        }
        
        Debug.Log($"🔄 Đã refresh {allLevelButtons.Length} level buttons");
    }

    public void UpdateCurrentLevel(int levelId)
    {
        lastPlayedLevel = levelId;
        PlayerPrefs.SetInt("CurrentLevel", levelId);
        PlayerPrefs.Save();
        
        Debug.Log($"📍 Cập nhật level hiện tại: {levelId}");
    }
}