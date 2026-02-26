using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingCtrl : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private Transform settingBoard;

    [Header("Buttons")]
    [SerializeField] private Button SoundOnIcon;
    [SerializeField] private Button SoundOffIcon;
    
    [SerializeField] private Button MusicOnIcon;
    [SerializeField] private Button MusicOffIcon;
    
    [SerializeField] private Button VibrateOnIcon;
    [SerializeField] private Button VibrateOffIcon;
    
    [SerializeField] private Button ClosePanel;
    [SerializeField] private Button OpenPanel;

    private float animationDuration = 0.3f;

    void Start()
    {
        SoundOnIcon.onClick.AddListener(() => {
            ToggleSoundUI(false);
            PlayClickSound();
        });
        SoundOffIcon.onClick.AddListener(() => {
            ToggleSoundUI(true);
            PlayClickSound();
        });

        MusicOnIcon.onClick.AddListener(() => {
            ToggleMusicUI(false);
            PlayClickSound();
        });
        MusicOffIcon.onClick.AddListener(() => {
            ToggleMusicUI(true);
            PlayClickSound();
        });

        VibrateOnIcon.onClick.AddListener(() => {
            ToggleVibrateUI(false);
            PlayClickSound();
        });
        VibrateOffIcon.onClick.AddListener(() => {
            ToggleVibrateUI(true);
            PlayClickSound();
        });

        ClosePanel.onClick.AddListener(() => {
            CloseSettingPanel();
            PlayClickSound();
        });
        OpenPanel.onClick.AddListener(() => {
            OpenSettingPanel();
            PlayClickSound();
        });

        // ✅ Khởi tạo UI mặc định (tất cả đều ON)
        ToggleSoundUI(true);
        ToggleMusicUI(true);
        ToggleVibrateUI(true);
    }

    // ✅ CHỈ CHUYỂN ICON UI - KHÔNG LƯU, KHÔNG THAY ĐỔI ÂM THANH
    private void ToggleSoundUI(bool isOn)
    {
        SoundOnIcon.gameObject.SetActive(isOn);
        SoundOffIcon.gameObject.SetActive(!isOn);
        // ❌ KHÔNG làm gì với SoundManager
        // ❌ KHÔNG lưu PlayerPrefs
    }

    private void ToggleMusicUI(bool isOn)
    {
        MusicOnIcon.gameObject.SetActive(isOn);
        MusicOffIcon.gameObject.SetActive(!isOn);
        // ❌ KHÔNG làm gì với MusicManager
        // ❌ KHÔNG lưu PlayerPrefs
    }

    private void ToggleVibrateUI(bool isOn)
    {
        VibrateOnIcon.gameObject.SetActive(isOn);
        VibrateOffIcon.gameObject.SetActive(!isOn);
        // ❌ KHÔNG làm gì với Vibration
        // ❌ KHÔNG lưu PlayerPrefs
    }

    private void CloseSettingPanel()
    {
        Time.timeScale = 1f;
        settingBoard.localScale = Vector3.one;
        settingBoard.DOKill();
        settingBoard.DOScale(0.3f, animationDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => SettingPanel.SetActive(false));
    }
    
    private void OpenSettingPanel()
    {
        Time.timeScale = 0f;
        SettingPanel.SetActive(true);
        settingBoard.localScale = Vector3.one * 0.3f; 
        settingBoard.DOKill();
        settingBoard.DOScale(1f, animationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void PlayClickSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
    }
}