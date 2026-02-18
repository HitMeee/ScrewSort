using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;  

public class ShowPopupLevel : MonoBehaviour
{
    public GameObject popupPanel; // Panel chứa popup
    public Button btnClosePopup; // Nút đóng popup
    public Button btnShowPopup; // Nút hiển thị popup
    public Button btnPlay;
    
    private int selectedLevelId = 1; // Level được chọn

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false); 
        }
        if(btnShowPopup != null)
        {
            btnShowPopup.onClick.AddListener(ShowPopup);
        }
        if (btnClosePopup != null)
        {
            btnClosePopup.onClick.AddListener(ClosePopup);
        }
        if (btnPlay != null)
        {
            btnPlay.onClick.AddListener(PlayGame);
        }
    }
    public void ShowPopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            popupPanel.transform.localScale = Vector3.zero; 
            popupPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack); 
        }
    }
    
    // ✅ THÊM: Phương thức hiển thị popup với levelId cụ thể
    public void ShowPopupForLevel(int levelId)
    {
        selectedLevelId = levelId;
        Debug.Log($"🎯 Chọn level: {levelId}");
        ShowPopup();
    }
    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => popupPanel.SetActive(false)); 
        }
    }
    public void PlayGame()
    {
        // 🎮 Set mode = Selected (chơi level tự chọn, KHÔNG ảnh hưởng tiến độ)
        LevelFileManager.SetPlayMode(LevelFileManager.PlayMode.Selected, selectedLevelId);
        
        Debug.Log($"▶️ Chơi LEVEL TỰ CHỌN - Level {selectedLevelId} (không ảnh hưởng tiến độ)");
        
        // Chuyển sang scene GamePlay
        SceneManager.LoadScene("GamePlay");
    }

   
}
