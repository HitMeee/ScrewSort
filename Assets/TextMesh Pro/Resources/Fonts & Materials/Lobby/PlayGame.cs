using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 🎮 NÚT "LEVEL" - Chơi tiến độ chính (level cao nhất đã mở)
/// </summary>
public class PlayGame : MonoBehaviour
{
    [SerializeField] private Button btnPlayGame;
    [SerializeField] private TextMeshProUGUI tmpLevelText;
    
    void Start()
    {
        btnPlayGame.onClick.AddListener(OnPlayButtonClicked);
        UpdateTMPLevel();
    }

    public void OnPlayButtonClicked()
    {
        SoundManager.Instance.PlayButtonClick();
        
        // 🎯 Lấy level cao nhất cần chơi (tiến độ chính)
        int progressLevel = LevelFileManager.GetProgressLevelId();
        
        // 🎮 Set mode = Progress (tiến độ chính)
        LevelFileManager.SetPlayMode(LevelFileManager.PlayMode.Progress, progressLevel);
        
        Debug.Log($"▶️ Chơi TIẾN ĐỘ CHÍNH - Level {progressLevel}");
        
        // ✅ SỬ DỤNG SCENE TRANSITION
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadSceneWithFade("GamePlay");
        }
        else
        {
            // Fallback nếu không có SceneTransition
            SceneManager.LoadScene("GamePlay");
        }
    }
    public void UpdateTMPLevel()
    {
        int currentLevel = LevelFileManager.GetProgressLevelId();
        
        // Cập nhật text
        if (tmpLevelText != null)
        {
            tmpLevelText.text = $"Level {currentLevel}";
        }
    }
}
