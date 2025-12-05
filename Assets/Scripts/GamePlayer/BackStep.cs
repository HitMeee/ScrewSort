using System.Collections.Generic;
using UnityEngine;

public class BackStep : MonoBehaviour
{
    [Header("⏪ Cài đặt BackStep")]
    [SerializeField] private int maxSteps = 10;

    public List<DataBackMove> lsDataBackMove = new List<DataBackMove>();

    // ✅ LƯU TRẠNG THÁI HIỆN TẠI CỦA TẤT CẢ BOLT
    public void SaveCurrentState()
    {
        var levelController = FindObjectOfType<LevelController>();
        if (levelController == null) return;

        var allBolts = levelController.GetAllBolts();
        if (allBolts == null || allBolts.Count == 0) return;

        // Tạo ảnh chụp của toàn bộ trạng thái
        DataBackMove snapshot = new DataBackMove();

        foreach (var bolt in allBolts)
        {
            if (bolt == null) continue;

            BoltSnapshot boltData = new BoltSnapshot
            {
                bolt = bolt,
                screwsInBolt = new List<ScrewBase>()
            };

            // Lưu tất cả vít trong bolt này
            foreach (var screw in bolt.screwBases)
            {
                if (screw != null)
                {
                    boltData.screwsInBolt.Add(screw);
                }
            }

            snapshot.trangThaiBolts.Add(boltData);
        }

        lsDataBackMove.Add(snapshot);

        // Giới hạn số bước
        if (lsDataBackMove.Count > maxSteps)
        {
            lsDataBackMove.RemoveAt(0);
        }

        Debug.Log($"📝 Đã lưu trạng thái: {allBolts.Count} bolt");
    }

    // ✅ QUAY LẠI TRẠNG THÁI TRƯỚC ĐÓ
    public void GoBack()
    {
        if (lsDataBackMove.Count == 0)
        {
            Debug.Log("❌ Không có trạng thái nào để quay lại!");
            return;
        }

        // Lấy trạng thái trước đó
        var previousState = lsDataBackMove[lsDataBackMove.Count - 1];
        lsDataBackMove.RemoveAt(lsDataBackMove.Count - 1);

        Debug.Log("⏪ Đang khôi phục trạng thái...");

        // ✅ KHÔI PHỤC TOÀN BỘ TRẠNG THÁI - KHÔNG CÓ ANIMATION
        RestoreState(previousState);

        Debug.Log("✅ Đã khôi phục về trạng thái trước!");
    }

    // ✅ KHÔI PHỤC TRẠNG THÁI (DỊCH CHUYỂN TỨC THỜI)
    private void RestoreState(DataBackMove state)
    {
        foreach (var boltSnapshot in state.trangThaiBolts)
        {
            if (boltSnapshot.bolt == null) continue;

            var bolt = boltSnapshot.bolt;

            // Xóa các vít hiện tại trong bolt
            bolt.screwBases.Clear();

            // Khôi phục vít theo đúng thứ tự
            foreach (var screw in boltSnapshot.screwsInBolt)
            {
                if (screw != null && screw.gameObject != null)
                {
                    // Thêm vào bolt
                    bolt.screwBases.Add(screw);

                    // Đặt parent
                    screw.transform.SetParent(bolt.transform);

                    // ✅ DỊCH CHUYỂN TỨC THỜI - KHÔNG CÓ ANIMATION
                    Vector3 correctPos = GetCorrectPosition(bolt, bolt.screwBases.Count - 1);
                    screw.transform.position = correctPos;
                    screw.originalPosition = correctPos;

                    // Đảm bảo vít không bị nâng lên
                    screw.isLifted = false;
                }
            }
        }

        // ✅ RESET TRẠNG THÁI VÍT BỊ NÂNG
        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager != null)
        {
            boltManager.ForceResetState();
        }
    }

    // Tính toán vị trí chính xác trong bolt
    private Vector3 GetCorrectPosition(BotlBase bolt, int index)
    {
        if (bolt?.postBolts != null && index >= 0 && index < bolt.postBolts.Count)
        {
            return bolt.postBolts[index].transform.position;
        }
        return bolt.transform.position + Vector3.up * (index * 0.3f + 0.2f);
    }

    // ✅ Nút UI
    public void ButtonGoBack()
    {
        GoBack();
    }

    // Xóa lịch sử
    public void ClearHistory()
    {
        lsDataBackMove.Clear();
        Debug.Log("🗑️ Đã xóa lịch sử BackStep");
    }

    // Getter
    public bool HasHistory() => lsDataBackMove.Count > 0;
    public int StepCount() => lsDataBackMove.Count;
}

// ✅ CLASS ĐỂ LƯU TOÀN BỘ TRẠNG THÁI
[System.Serializable]
public class DataBackMove
{
    public List<BoltSnapshot> trangThaiBolts = new List<BoltSnapshot>();
}

// ✅ CLASS ĐỂ LƯU TRẠNG THÁI 1 BOLT
[System.Serializable]
public class BoltSnapshot
{
    public BotlBase bolt;                    // Bolt này
    public List<ScrewBase> screwsInBolt;     // Tất cả vít trong bolt theo đúng thứ tự
}