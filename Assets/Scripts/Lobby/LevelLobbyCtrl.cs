using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLobbyCtrl : MonoBehaviour
{
    public GameObject panelAllLevel;
    public Button btnShowAllLevel;
    public Button btnCloseAllLevel;
    
    private LevelButton[] allLevelButtons; // 🔒 Cache tất cả level buttons
    
    public void Start()
    {
        if (btnShowAllLevel != null)
        {
            btnShowAllLevel.onClick.AddListener(ShowAllLevel);
        }
        if (btnCloseAllLevel != null)
        {
            btnCloseAllLevel.onClick.AddListener(CloseAllLevel);
        }
        
        // 🔒 Tìm tất cả level buttons trong scene
        allLevelButtons = FindObjectsOfType<LevelButton>();
        
        // 🔒 Refresh trạng thái lock/unlock khi vào lobby
        RefreshAllLevelButtons();
        
        Debug.Log($"🎮 Lobby initialized với {allLevelButtons.Length} level buttons");
    }
    
    public void ShowAllLevel()
    {
        if (panelAllLevel != null)
        {
            panelAllLevel.SetActive(true);
        }
        
        // 🔒 Refresh lại trạng thái khi mở panel (phòng trường hợp unlock level mới)
        RefreshAllLevelButtons();
    }
    
    public void CloseAllLevel()
    {
        if (panelAllLevel != null)
        {
            panelAllLevel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 🔒 Refresh trạng thái lock/unlock của tất cả level buttons
    /// </summary>
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
}
