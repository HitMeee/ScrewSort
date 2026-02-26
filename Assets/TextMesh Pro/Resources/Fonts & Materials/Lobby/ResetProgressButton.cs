using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 🔄 NÚT RESET TIẾN ĐỘ - Reset về Level 1, khóa tất cả level khác
/// </summary>
public class ResetProgressButton : MonoBehaviour
{
    [SerializeField] private Button btnReset;
    
    void Start()
    {
        if (btnReset == null)
        {
            btnReset = GetComponent<Button>();
        }
        
        if (btnReset != null)
        {
            btnReset.onClick.AddListener(OnResetButtonClicked);
        }
    }

    public void OnResetButtonClicked()
    {
        // 🔄 Reset toàn bộ tiến độ
        LevelFileManager.ResetProgress();
        
        // 🔄 Set current level về 1
        LevelFileManager.SetCurrentLevelId(1);
        
        Debug.Log("🔄 Đã reset về Level 1 - Chỉ mở Level 1, tất cả level khác bị khóa");
        
        // 🔄 Refresh UI (tìm LevelLobbyCtrl và refresh)
        var lobbyCtrl = FindObjectOfType<LevelLobbyCtrl>();
        if (lobbyCtrl != null)
        {
            lobbyCtrl.RefreshAllLevelButtons();
        }
        else
        {
            // 🔄 Nếu không tìm thấy LevelLobbyCtrl, refresh thủ công tất cả LevelButton
            var allButtons = FindObjectsOfType<LevelButton>();
            foreach (var btn in allButtons)
            {
                btn.RefreshLockState();
            }
        }
        
        Debug.Log("✅ Đã refresh UI - Level 1 mở, còn lại khóa");
    }
}
