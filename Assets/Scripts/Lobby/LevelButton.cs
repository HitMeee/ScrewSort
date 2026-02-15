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
        
        Debug.Log($"🎮 LevelButton {levelId} đã được khởi tạo");
    }
    
    private void OnLevelButtonClicked()
    {
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
    
    // Method public để set levelId từ code (nếu dùng dynamic generation)
    public void SetLevelId(int id)
    {
        levelId = id;
        if (levelText != null)
        {
            levelText.text = id.ToString();
        }
    }
}
