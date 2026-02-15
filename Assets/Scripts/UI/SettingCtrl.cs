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
        SoundOnIcon.onClick.AddListener(() => ToggleSound(false));
        SoundOffIcon.onClick.AddListener(() => ToggleSound(true));

        MusicOnIcon.onClick.AddListener(() => ToggleMusic(false)); 
        MusicOffIcon.onClick.AddListener(() => ToggleMusic(true));

        VibrateOnIcon.onClick.AddListener(() => ToggleVibrate(false)); 
        VibrateOffIcon.onClick.AddListener(() => ToggleVibrate(true)); 

        ClosePanel.onClick.AddListener(CloseSettingPanel);
        OpenPanel.onClick.AddListener(OpenSettingPanel);
        ToggleSound(PlayerPrefs.GetInt("Sound", 1) == 1);
        ToggleMusic(PlayerPrefs.GetInt("Music", 1) == 1);
        ToggleVibrate(PlayerPrefs.GetInt("Vibrate", 1) == 1);
    }

    private void ToggleSound(bool isOn)
    {

        SoundOnIcon.gameObject.SetActive(isOn);
        SoundOffIcon.gameObject.SetActive(!isOn);

        PlayerPrefs.SetInt("Sound", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ToggleMusic(bool isOn)
    {
        MusicOnIcon.gameObject.SetActive(isOn);
        MusicOffIcon.gameObject.SetActive(!isOn);

        // Xử lý logic nhạc nền ở đây...

        PlayerPrefs.SetInt("Music", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ToggleVibrate(bool isOn)
    {
        VibrateOnIcon.gameObject.SetActive(isOn);
        VibrateOffIcon.gameObject.SetActive(!isOn);

        // Xử lý logic rung ở đây...

        PlayerPrefs.SetInt("Vibrate", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void CloseSettingPanel()
    {
        settingBoard.localScale = Vector3.one;
        settingBoard.DOKill();
        settingBoard.DOScale(0.3f, animationDuration).SetEase(Ease.InBack)
        .OnComplete(() => SettingPanel.SetActive(false));
    }
    
    private void OpenSettingPanel()
    {
        SettingPanel.SetActive(true);
        settingBoard.localScale = Vector3.one * 0.3f; 
        settingBoard.DOKill();
        settingBoard.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
    }
}