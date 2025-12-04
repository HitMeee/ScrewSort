using System.Collections.Generic;
using UnityEngine;

public class BackStep : MonoBehaviour
{
    [Header("⏪ Quay Lại")]
    [SerializeField] private int maxBuoc = 10;

    private List<BuocLuu> danhSachBuoc = new List<BuocLuu>();
    private bool dangQuayLai = false;

    void Update()
    {
        // Ctrl+Z để undo
        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
        {
            QuayLai();
        }
    }

    // Quay lại 1 bước
    public void QuayLai()
    {
        if (danhSachBuoc.Count == 0 || dangQuayLai)
        {
            Debug.Log("❌ Không thể quay lại!");
            return;
        }

        dangQuayLai = true;

        // Lấy bước cuối
        var buoc = danhSachBuoc[danhSachBuoc.Count - 1];
        danhSachBuoc.RemoveAt(danhSachBuoc.Count - 1);

        Debug.Log($"⏪ Quay lại: {buoc.loai}");

        // ✅ SỬA: Dùng enum thay vì string
        switch (buoc.loai)
        {
            case LoaiBuoc.DiChuyen:
                QuayLaiDiChuyen(buoc);
                break;
            case LoaiBuoc.DiChuyenNhieu:
                QuayLaiDiChuyenNhieu(buoc);
                break;
            case LoaiBuoc.ThemBolt:
                QuayLaiThemBolt(buoc);
                break;
        }

        dangQuayLai = false;
        Debug.Log("✅ Đã quay lại!");
    }

    // ✅ GHI LẠI DI CHUYỂN ĐƠN (cho swap)
    public void GhiLaiDiChuyenScrew(ScrewBase screw, BotlBase tu, BotlBase den)
    {
        if (dangQuayLai || screw == null || tu == null || den == null) return;

        var buoc = new BuocLuu
        {
            loai = LoaiBuoc.DiChuyen, // ✅ Dùng enum
            screw = screw,
            boltTu = tu,
            boltDen = den,
            viTriCu = screw.originalPosition
        };

        ThemBuoc(buoc);
        Debug.Log($"📝 Ghi lại: {screw.name} từ {tu.name} đến {den.name}");
    }

    // ✅ GHI LẠI DI CHUYỂN NHIỀU SCREW (cho batch move)
    public void GhiLaiDiChuyenNhieuScrew(List<ScrewBase> screws, BotlBase tu, BotlBase den, int mauId)
    {
        if (dangQuayLai || screws == null || screws.Count == 0 || tu == null || den == null) return;

        var buoc = new BuocLuu
        {
            loai = LoaiBuoc.DiChuyenNhieu, // ✅ Dùng enum
            danhSachScrew = new List<ScrewBase>(screws), // Copy list
            boltTu = tu,
            boltDen = den,
            mauId = mauId,
            soLuong = screws.Count
        };

        ThemBuoc(buoc);
        Debug.Log($"📝 Ghi lại: {screws.Count} screws màu {mauId} từ {tu.name} đến {den.name}");
    }

    // Ghi lại thêm bolt
    public void GhiLaiThemBolt(BotlBase bolt)
    {
        if (dangQuayLai || bolt == null) return;

        var buoc = new BuocLuu
        {
            loai = LoaiBuoc.ThemBolt, // ✅ Dùng enum
            boltMoi = bolt
        };

        ThemBuoc(buoc);
        Debug.Log($"📝 Ghi lại thêm bolt: {bolt.name}");
    }

    // Thêm bước vào danh sách
    private void ThemBuoc(BuocLuu buoc)
    {
        danhSachBuoc.Add(buoc);

        // Giới hạn số bước
        if (danhSachBuoc.Count > maxBuoc)
        {
            danhSachBuoc.RemoveAt(0);
        }
    }

    // Quay lại di chuyển đơn
    private void QuayLaiDiChuyen(BuocLuu buoc)
    {
        if (buoc.screw == null || buoc.boltTu == null || buoc.boltDen == null) return;

        // Kiểm tra screw còn tồn tại
        if (buoc.screw.gameObject == null)
        {
            Debug.Log("❌ Screw đã mất!");
            return;
        }

        Debug.Log($"🔄 Undo đơn: {buoc.screw.name} từ {buoc.boltDen.name} về {buoc.boltTu.name}");

        // Xóa khỏi bolt hiện tại
        buoc.boltDen.RemoveScrew(buoc.screw);

        // Thêm về bolt cũ
        buoc.boltTu.AddScrew(buoc.screw);
        buoc.screw.transform.SetParent(buoc.boltTu.transform);

        // Di chuyển về vị trí đúng
        Vector3 viTriMoi = LayViTriTrongBolt(buoc.boltTu, buoc.boltTu.screwBases.Count - 1);
        buoc.screw.MoveTo(viTriMoi, 0.3f, () => {
            buoc.screw.originalPosition = viTriMoi;
        });
    }

    // ✅ Quay lại di chuyển nhiều screw
    private void QuayLaiDiChuyenNhieu(BuocLuu buoc)
    {
        if (buoc.danhSachScrew == null || buoc.danhSachScrew.Count == 0 ||
            buoc.boltTu == null || buoc.boltDen == null) return;

        Debug.Log($"🔄 Undo batch: {buoc.danhSachScrew.Count} screws màu {buoc.mauId} từ {buoc.boltDen.name} về {buoc.boltTu.name}");

        // Di chuyển từng screw về bolt gốc (ngược lại thứ tự)
        for (int i = buoc.danhSachScrew.Count - 1; i >= 0; i--)
        {
            var screw = buoc.danhSachScrew[i];

            if (screw == null || screw.gameObject == null) continue;

            // Xóa khỏi bolt đích
            if (buoc.boltDen.screwBases.Contains(screw))
            {
                buoc.boltDen.RemoveScrew(screw);
            }

            // Thêm về bolt gốc
            buoc.boltTu.AddScrew(screw);
            screw.transform.SetParent(buoc.boltTu.transform);

            // Tính vị trí đúng cho screw này
            Vector3 viTriMoi = LayViTriTrongBolt(buoc.boltTu, buoc.boltTu.screwBases.Count - 1);

            // Animation di chuyển với delay nhỏ
            float delay = (buoc.danhSachScrew.Count - 1 - i) * 0.1f; // Delay tăng dần

            StartCoroutine(DiChuyenSauDelay(screw, viTriMoi, delay));
        }
    }

    // ✅ HELPER: Di chuyển sau delay
    private System.Collections.IEnumerator DiChuyenSauDelay(ScrewBase screw, Vector3 viTri, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (screw != null && screw.gameObject != null)
        {
            screw.MoveTo(viTri, 0.3f, () => {
                screw.originalPosition = viTri;
            });
        }
    }

    // Quay lại thêm bolt
    private void QuayLaiThemBolt(BuocLuu buoc)
    {
        if (buoc.boltMoi == null || buoc.boltMoi.gameObject == null) return;

        Debug.Log($"🗑️ Undo thêm bolt: {buoc.boltMoi.name}");

        // Xóa khỏi level
        var level = FindObjectOfType<LevelController>();
        if (level != null)
        {
            var bolts = level.GetAllBolts();
            bolts.Remove(buoc.boltMoi);
        }

        // Hủy bolt
        Destroy(buoc.boltMoi.gameObject);
    }

    // Lấy vị trí trong bolt
    private Vector3 LayViTriTrongBolt(BotlBase bolt, int thuTu)
    {
        if (bolt.postBolts != null && thuTu < bolt.postBolts.Count && thuTu >= 0)
        {
            return bolt.postBolts[thuTu].transform.position;
        }
        return bolt.transform.position + Vector3.up * (thuTu * 0.3f);
    }

    // Xóa lịch sử
    public void XoaLichSu()
    {
        danhSachBuoc.Clear();
        Debug.Log("🗑️ Đã xóa lịch sử");
    }

    // Button UI
    public void NutQuayLai()
    {
        QuayLai();
    }

    // Getters
    public bool CoLichSu() => danhSachBuoc.Count > 0;
    public int SoBuoc() => danhSachBuoc.Count;
}

// ✅ ENUM CHO LOẠI BƯỚC
public enum LoaiBuoc
{
    DiChuyen,       // Di chuyển 1 screw (swap)
    DiChuyenNhieu,  // Di chuyển nhiều screw (batch move)
    ThemBolt        // Thêm bolt mới
}

// ✅ CẬP NHẬT: Class lưu bước với enum
[System.Serializable]
public class BuocLuu
{
    public LoaiBuoc loai; // ✅ Dùng enum thay vì string

    // Cho di chuyển đơn
    public ScrewBase screw;
    public BotlBase boltTu;
    public BotlBase boltDen;
    public Vector3 viTriCu;

    // ✅ Cho di chuyển nhiều screw
    public List<ScrewBase> danhSachScrew;
    public int mauId;
    public int soLuong;

    // Cho thêm bolt
    public BotlBase boltMoi;
}