using System.Collections.Generic;
using UnityEngine;

public class AddBolt : MonoBehaviour
{
    [Header("🔧 Thêm Bolt Trống")]
    [SerializeField] private LevelController levelController;
    [SerializeField] private BotlBase boltPrefab;
    [SerializeField] private int maxBolts = 6;

    void Start()
    {
        Debug.Log("🔧 AddBolt Start() - Component initialized");

        if (levelController == null)
        {
            levelController = FindObjectOfType<LevelController>();
            if (levelController != null)
                Debug.Log("✅ Found LevelController automatically");
            else
                Debug.LogError("❌ LevelController not found!");
        }

        if (boltPrefab == null)
        {
            var existingBolt = FindObjectOfType<BotlBase>();
            if (existingBolt != null)
            {
                boltPrefab = existingBolt;
                Debug.Log("✅ Found BotlBase prefab automatically");
            }
            else
            {
                Debug.LogError("❌ BotlBase prefab not found!");
            }
        }
    }

    public void ThemBoltTrong()
    {
        Debug.Log("🔧 === THÊM BOLT TRỐNG ===");

        if (!CoTheThemBolt())
        {
            Debug.LogWarning("❌ Không thể thêm bolt - đã đạt tối đa hoặc lỗi!");
            return;
        }

        Vector3 viTriMoi = TimViTriTiepTheo();
        Debug.Log($"📍 Vị trí mới: {viTriMoi}");

        BotlBase boltMoi = TaoBoltTrong(viTriMoi);

        if (boltMoi != null)
        {
            ThemVaoDanhSach(boltMoi);

            // ✅ GHI LẠI CHO BACKSTEP
            var backStep = levelController.GetBackStep();
            if (backStep != null)
            {
                backStep.GhiLaiThemBolt(boltMoi);
                Debug.Log("📝 Đã ghi lại action cho BackStep");
            }

            Debug.Log("✅ ĐÃ THÊM BOLT THÀNH CÔNG!");
        }
        else
        {
            Debug.LogError("❌ Không thể tạo bolt mới!");
        }
    }

    private bool CoTheThemBolt()
    {
        if (levelController == null)
        {
            Debug.LogError("❌ LevelController is null!");
            return false;
        }

        var danhSachBolt = levelController.GetAllBolts();
        if (danhSachBolt == null)
        {
            Debug.LogError("❌ GetAllBolts returned null!");
            return false;
        }

        // ✅ SỬA: Kiểm tra cả PostCreateBolts
        var postCreateBolts = GetPostCreateBolts();
        if (postCreateBolts == null || postCreateBolts.Count == 0)
        {
            Debug.LogError("❌ PostCreateBolts không có sẵn!");
            return false;
        }

        bool canAdd = danhSachBolt.Count < postCreateBolts.Count && danhSachBolt.Count < maxBolts;
        Debug.Log($"📊 Bolts hiện tại: {danhSachBolt.Count}/{postCreateBolts.Count} PostCreateBolts, Max: {maxBolts} - Có thể thêm: {canAdd}");
        return canAdd;
    }

    // ✅ SỬA: Sử dụng PostCreateBolts từ LevelController
    private Vector3 TimViTriTiepTheo()
    {
        var danhSachBolt = levelController.GetAllBolts();
        var postCreateBolts = GetPostCreateBolts();

        if (postCreateBolts != null && danhSachBolt.Count < postCreateBolts.Count)
        {
            // Sử dụng vị trí tiếp theo từ PostCreateBolts
            var nextTransform = postCreateBolts[danhSachBolt.Count];
            if (nextTransform != null)
            {
                Debug.Log($"📍 Sử dụng PostCreateBolts[{danhSachBolt.Count}]: {nextTransform.position}");
                return nextTransform.position;
            }
        }

        // Fallback: tính toán vị trí dự phòng
        if (danhSachBolt.Count > 0)
        {
            var boltCuoi = danhSachBolt[danhSachBolt.Count - 1];
            Vector3 newPos = boltCuoi.transform.position + Vector3.right * 2f;
            Debug.LogWarning($"⚠️ Fallback position: {newPos}");
            return newPos;
        }

        Debug.LogWarning("⚠️ Using Vector3.zero as fallback");
        return Vector3.zero;
    }

    // ✅ THÊM: Method để lấy PostCreateBolts từ LevelController
    private List<Transform> GetPostCreateBolts()
    {
        if (levelController == null) return null;

        // Sử dụng reflection để truy cập PostCreateBolts private field
        var field = typeof(LevelController).GetField("PostCreateBolts",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            return field.GetValue(levelController) as List<Transform>;
        }

        return null;
    }

    private BotlBase TaoBoltTrong(Vector3 viTri)
    {
        if (boltPrefab == null)
        {
            Debug.LogError("❌ BoltPrefab is null - cannot create bolt!");
            return null;
        }

        try
        {
            BotlBase boltMoi = Instantiate(boltPrefab, viTri, Quaternion.identity);
            List<int> danhSachTrong = new List<int>(); // Bolt trống
            boltMoi.Init(danhSachTrong);
            boltMoi.name = "BoltTrong_" + (levelController.GetAllBolts().Count + 1);

            Debug.Log($"✅ Đã tạo bolt: {boltMoi.name} tại {viTri}");
            return boltMoi;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Lỗi khi tạo bolt: {ex.Message}");
            return null;
        }
    }

    private void ThemVaoDanhSach(BotlBase boltMoi)
    {
        var danhSach = levelController.GetAllBolts();
        danhSach.Add(boltMoi);
        Debug.Log($"📋 Đã thêm vào danh sách. Tổng: {danhSach.Count}");
    }

    // Hàm cho Button UI
    public void NutThemBolt()
    {
        Debug.Log("🔘 === NUT THÊM BOLT ĐƯỢC NHẤN ===");
        ThemBoltTrong();
    }

    // Getters
    public bool DaDayBolt()
    {
        var postCreateBolts = GetPostCreateBolts();
        var currentCount = levelController?.GetAllBolts()?.Count ?? 0;
        var maxPossible = postCreateBolts?.Count ?? maxBolts;
        return currentCount >= maxPossible || currentCount >= maxBolts;
    }

    public int SoBoltHienTai() => levelController?.GetAllBolts()?.Count ?? 0;
}