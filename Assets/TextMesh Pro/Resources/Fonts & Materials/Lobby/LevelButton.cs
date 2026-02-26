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
    private bool isUnlocked = false;
    
    private void Start()
    {
        if (popupController == null)
        {
            popupController = FindObjectOfType<ShowPopupLevel>();
        }

        UpdateLockState(); 
    }
    private void UpdateLockState()
    {
        isUnlocked = LevelFileManager.IsLevelUnlocked(levelId);
        
        // Cập nhật UI
        if (lockObject != null)
        {
            lockObject.SetActive(!isUnlocked); 
        }
    }
    public void SetLevelId(int id)
    {
        UpdateLockState(); // 🔒 Cập nhật trạng thái lock khi set ID mới
    }
    
    public void RefreshLockState()
    {
        UpdateLockState();
    }
}
