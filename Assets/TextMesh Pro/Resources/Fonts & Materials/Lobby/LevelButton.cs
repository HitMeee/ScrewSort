using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelId = 1; 
    
    [Header("References")]
    [SerializeField] private ShowPopupLevel popupController;
    [SerializeField] private GameObject lockObject;
    private Button button;
    private bool isUnlocked = false;
    
    private void Start()
    {
        if (popupController == null)
        {
            popupController = FindObjectOfType<ShowPopupLevel>();
        }

        // Tự động lấy Button component
        button = GetComponent<Button>();
        
        // Gán sự kiện click
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }

        UpdateLockState(); 
    }
    
    private void UpdateLockState()
    {
        isUnlocked = LevelFileManager.IsLevelUnlocked(levelId);
        
        if (lockObject != null)
        {
            lockObject.SetActive(!isUnlocked); 
        }

        if (button != null)
        {
            button.interactable = isUnlocked;
        }
    }
    
    // ✅ Method xử lý click
    public void OnButtonClick()
    {
        if (!isUnlocked) return;

        if (popupController != null)
        {
            popupController.ShowPopupForLevel(levelId);
            Debug.Log($"🎯 Chọn Level {levelId}");
        }
    }
    
    public void SetLevelId(int id)
    {
        levelId = id; // ✅ QUAN TRỌNG: Phải set levelId
        UpdateLockState();
    }
    
    public void RefreshLockState()
    {
        UpdateLockState();
    }
}
