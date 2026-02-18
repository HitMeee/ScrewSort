using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelId = 1; // ID của level này
    
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private ShowPopupLevel popupController;
    [SerializeField] private TextMeshProUGUI levelText; // Text hiển thị số level (optional)
    [SerializeField] private GameObject lockObject; // 🔒 GameObject hiện khi level bị khóa (LVBlock)
    
    private bool isUnlocked = false;
    
    private void Start()
    {
        // Tự động tìm button nếu chưa assign
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        
        // Tự động tìm ShowPopupLevel nếu chưa assign
        if (popupController == null)
        {
            popupController = FindObjectOfType<ShowPopupLevel>();
        }
        
        // Setup button click
        if (button != null)
        {
            button.onClick.AddListener(OnLevelButtonClicked);
        }
        
        // Cập nhật text nếu có
        if (levelText != null)
        {
            levelText.text = levelId.ToString();
        }
        
        
        // 🔒 Kiểm tra và cập nhật trạng thái lock/unlock
        UpdateLockState();
        
        Debug.Log($"🎮 LevelButton {levelId} đã được khởi tạo - {(isUnlocked ? "Mở" : "Khóa")}");
    }
    
    private void OnLevelButtonClicked()
    {
        // 🔒 Kiểm tra level có bị khóa không
        if (!isUnlocked)
        {
            Debug.Log($"🔒 Level {levelId} đang bị khóa! Hoàn thành level trước đó để mở.");
            // TODO: Có thể thêm animation hoặc sound effect ở đây
            return;
        }
        
        Debug.Log($"🖱️ Click vào Level {levelId}");
        
        if (popupController != null)
        {
            popupController.ShowPopupForLevel(levelId);
        }
        else
        {
            Debug.LogError("❌ ShowPopupLevel controller chưa được assign!");
        }
    }
    
    /// <summary>
    /// Cập nhật trạng thái lock/unlock của level button
    /// </summary>
    private void UpdateLockState()
    {
        isUnlocked = LevelFileManager.IsLevelUnlocked(levelId);
        
        // Cập nhật UI
        if (lockObject != null)
        {
            lockObject.SetActive(!isUnlocked); // Hiện lock nếu bị khóa
        }
        
        // Cập nhật button interactable
        if (button != null)
        {
            button.interactable = isUnlocked;
        }
        
        Debug.Log($"🔒 Level {levelId}: {(isUnlocked ? "✅ Mở" : "🔒 Khóa")}");
    }
    
    // Method public để set levelId từ code (nếu dùng dynamic generation)
    public void SetLevelId(int id)
    {
        levelId = id;
        if (levelText != null)
        {
            levelText.text = id.ToString();
        }
        UpdateLockState(); // 🔒 Cập nhật trạng thái lock khi set ID mới
    }
    
    /// <summary>
    /// Method public để refresh trạng thái lock/unlock (gọi sau khi hoàn thành level)
    /// </summary>
    public void RefreshLockState()
    {
        UpdateLockState();
    }
}
