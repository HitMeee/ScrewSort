using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapNavigationCtrl : MonoBehaviour
{
    [Header("Map Panels")]
    public List<GameObject> mapPanels; // List các map panel (Map1, Map2, Map3...)
    
    [Header("Navigation Buttons")]
    public Button btnPrevMap;
    public Button btnNextMap;
    public Button btnCloseMap; // ✨ Thêm nút close
    
    [Header("Settings")]
    public int levelsPerMap = 9; // Số level trên mỗi map (mặc định 9)
    
    private int currentMapIndex = 0; // Index map đang hiển thị
    
    private void Start()
    {
        // Ẩn tất cả nút khi khởi động
        HideNavigationButtons();
        
        if (mapPanels == null || mapPanels.Count == 0)
        {
            Debug.LogError("❌ MapPanels chưa được gán!");
            return;
        }
        
        // Ẩn tất cả map khi start
        foreach (var map in mapPanels)
        {
            if (map != null)
            {
                map.SetActive(false);
            }
        }
        
        // Đăng ký sự kiện cho các nút
        if (btnPrevMap != null)
        {
            btnPrevMap.onClick.AddListener(ShowPreviousMap);
        }
        if (btnNextMap != null)
        {
            btnNextMap.onClick.AddListener(ShowNextMap);
        }
        if (btnCloseMap != null)
        {
            btnCloseMap.onClick.AddListener(CloseAllMaps);
        }
    }
    
    /// <summary>
    /// Hiển thị map tương ứng với level ID
    /// </summary>
    public void ShowMapForLevel(int levelId)
    {
        int mapIndex = GetMapIndexFromLevel(levelId);
        ShowMap(mapIndex);
        
        // ✨ Hiển thị các nút khi mở map
        ShowNavigationButtons();
    }
    
    /// <summary>
    /// Tính toán index của map dựa trên level ID
    /// </summary>
    private int GetMapIndexFromLevel(int levelId)
    {
        return (levelId - 1) / levelsPerMap;
    }
    
    /// <summary>
    /// Hiển thị map theo index
    /// </summary>
    private void ShowMap(int mapIndex)
    {
        if (mapIndex < 0 || mapIndex >= mapPanels.Count)
        {
            Debug.LogWarning($"⚠️ Map index {mapIndex} out of range!");
            return;
        }
        
        // Ẩn map hiện tại với animation
        if (currentMapIndex >= 0 && currentMapIndex < mapPanels.Count)
        {
            GameObject currentMap = mapPanels[currentMapIndex];
            if (currentMap != null && currentMap.activeSelf)
            {
                currentMap.transform.DOScale(0.8f, 0.2f).SetEase(Ease.InBack).OnComplete(() => {
                    currentMap.SetActive(false);
                });
            }
        }
        
        // Hiển thị map mới với animation
        currentMapIndex = mapIndex;
        GameObject newMap = mapPanels[currentMapIndex];
        if (newMap != null)
        {
            newMap.SetActive(true);
            newMap.transform.localScale = Vector3.zero;
            newMap.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
        
        // Cập nhật trạng thái nút Previous/Next
        UpdateNavigationButtons();
        
        Debug.Log($"🗺️ Hiển thị Map {currentMapIndex + 1}/{mapPanels.Count}");
    }
    
    /// <summary>
    /// Hiển thị map trước đó
    /// </summary>
    public void ShowPreviousMap()
    {
        if (currentMapIndex > 0)
        {
            ShowMap(currentMapIndex - 1);
        }
    }
    
    /// <summary>
    /// Hiển thị map tiếp theo
    /// </summary>
    public void ShowNextMap()
    {
        if (currentMapIndex < mapPanels.Count - 1)
        {
            ShowMap(currentMapIndex + 1);
        }
    }
    
    /// <summary>
    /// ✨ Đóng tất cả map và ẩn các nút
    /// </summary>
    public void CloseAllMaps()
    {
        // Đóng map hiện tại với animation
        if (currentMapIndex >= 0 && currentMapIndex < mapPanels.Count)
        {
            GameObject currentMap = mapPanels[currentMapIndex];
            if (currentMap != null && currentMap.activeSelf)
            {
                currentMap.transform.DOScale(0.8f, 0.2f).SetEase(Ease.InBack).OnComplete(() => {
                    currentMap.SetActive(false);
                    // ✨ Ẩn các nút sau khi đóng animation xong
                    HideNavigationButtons();
                });
            }
        }
        else
        {
            // ✨ Nếu không có animation, ẩn ngay
            HideNavigationButtons();
        }
        
        Debug.Log("🚪 Đóng tất cả map");
    }
    
    /// <summary>
    /// Cập nhật trạng thái enable/disable của nút navigation
    /// </summary>
    private void UpdateNavigationButtons()
    {
        if (btnPrevMap != null)
        {
            btnPrevMap.interactable = (currentMapIndex > 0);
        }
        if (btnNextMap != null)
        {
            btnNextMap.interactable = (currentMapIndex < mapPanels.Count - 1);
        }
    }
    
    /// <summary>
    /// ✨ Hiển thị các nút navigation
    /// </summary>
    private void ShowNavigationButtons()
    {
        if (btnPrevMap != null) btnPrevMap.gameObject.SetActive(true);
        if (btnNextMap != null) btnNextMap.gameObject.SetActive(true);
        if (btnCloseMap != null) btnCloseMap.gameObject.SetActive(true);
        
        UpdateNavigationButtons(); // Cập nhật trạng thái interactable
    }
    
    /// <summary>
    /// ✨ Ẩn các nút navigation
    /// </summary>
    private void HideNavigationButtons()
    {
        if (btnPrevMap != null) btnPrevMap.gameObject.SetActive(false);
        if (btnNextMap != null) btnNextMap.gameObject.SetActive(false);
        if (btnCloseMap != null) btnCloseMap.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Lấy index của map đang hiển thị
    /// </summary>
    public int GetCurrentMapIndex()
    {
        return currentMapIndex;
    }
}