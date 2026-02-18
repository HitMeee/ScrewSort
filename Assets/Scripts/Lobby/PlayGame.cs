using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 🎮 NÚT "LEVEL" - Chơi tiến độ chính (level cao nhất đã mở)
/// </summary>
public class PlayGame : MonoBehaviour
{
    [SerializeField] private Button btnPlayGame;
    
    void Start()
    {
        btnPlayGame.onClick.AddListener(OnPlayButtonClicked);
    }

    public void OnPlayButtonClicked()
    {
        // 🎯 Lấy level cao nhất cần chơi (tiến độ chính)
        int progressLevel = LevelFileManager.GetProgressLevelId();
        
        // 🎮 Set mode = Progress (tiến độ chính)
        LevelFileManager.SetPlayMode(LevelFileManager.PlayMode.Progress, progressLevel);
        
        Debug.Log($"▶️ Chơi TIẾN ĐỘ CHÍNH - Level {progressLevel}");
        
        // Load game scene
        SceneManager.LoadScene("GamePlay");
    }
}
