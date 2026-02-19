using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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
            panelAllLevel.transform.localScale = Vector3.one * 0.95f;
            panelAllLevel.transform.DOScale(1f, 0.95f).SetEase(Ease.OutBack);
        }
        
        // 🔒 Refresh lại trạng thái khi mở panel (phòng trường hợp unlock level mới)
        RefreshAllLevelButtons();
    }
    
    public void CloseAllLevel()
    {
        if (panelAllLevel != null)
        {
            panelAllLevel.transform.DOScale(0.95f, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
                panelAllLevel.SetActive(false);
            });
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
