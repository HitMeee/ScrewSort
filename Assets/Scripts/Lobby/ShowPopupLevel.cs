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
    public GameObject PanelBlack;

    private int selectedLevelId = 1; // Level được chọn

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        if (btnShowPopup != null)
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
        PanelBlack.SetActive(true);
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            popupPanel.transform.localScale = Vector3.one * 0.5f;
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
            popupPanel.transform.DOScale(0.5f, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => {
                    popupPanel.SetActive(false);
                    PanelBlack.SetActive(false);
                });
        }
    }
    public void PlayGame()
    {
        // ✅ ĐÓNG POPUP TRƯỚC (với animation)
        ClosePopup();

        // ✅ CHỜ ANIMATION ĐÓNG XONG RỒI MỚI CHUYỂN SCENE
        DOVirtual.DelayedCall(0.5f, () =>
        {
            // 🎮 Set mode = Selected
            LevelFileManager.SetPlayMode(LevelFileManager.PlayMode.Selected, selectedLevelId);

            Debug.Log($"▶️ Chơi LEVEL TỰ CHỌN - Level {selectedLevelId}");

            // Chuyển sang scene GamePlay
            SceneManager.LoadScene("GamePlay");
        });
    }

}
