using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltLogicManager : MonoBehaviour
{
    public List<BotlBase> allBolts;
    public float moveDuration = 0.3f;

    [Header("Lift Settings - Đảm bảo độ cao đồng nhất")]
    [Tooltip("Độ cao nâng screw từ base của bolt (đồng nhất cho tất cả)")]
    public float uniformLiftHeight = 1.5f;

    [Tooltip("Thời gian animation nâng lên")]
    public float liftDuration = 0.4f;

    // ✅ LOGIC STATE - Không phụ thuộc animation
    private ScrewBase currentLiftedScrew;
    private BotlBase currentSourceBolt;

    // ✅ CLICK QUEUE SYSTEM
    private Queue<BotlBase> clickQueue = new Queue<BotlBase>();
    private bool isProcessing = false;

    // ✅ BOLT LOCK SYSTEM
    private Dictionary<BotlBase, bool> boltLockStatus = new Dictionary<BotlBase, bool>();

    public void Init()
    {
        // Auto-calculate optimal lift height nếu chưa set
        if (uniformLiftHeight <= 0)
        {
            uniformLiftHeight = CalculateOptimalLiftHeight();
        }

        // Khởi tạo lock status cho tất cả bolt
        InitializeBoltLockStatus();
    }

    private void InitializeBoltLockStatus()
    {
        boltLockStatus.Clear();
        foreach (var bolt in allBolts)
        {
            if (bolt != null)
            {
                boltLockStatus[bolt] = false;
            }
        }
    }

    private float CalculateOptimalLiftHeight()
    {
        float maxPostHeight = 0f;

        foreach (var bolt in allBolts)
        {
            if (bolt?.postBolts != null)
            {
                foreach (var post in bolt.postBolts)
                {
                    if (post != null)
                    {
                        float postY = post.transform.position.y;
                        float boltY = bolt.transform.position.y;
                        float relativeHeight = postY - boltY;

                        if (relativeHeight > maxPostHeight)
                        {
                            maxPostHeight = relativeHeight;
                        }
                    }
                }
            }
        }

        return maxPostHeight + 0.8f;
    }

    // ✅ MAIN CLICK HANDLER - Chỉ enqueue và bắt đầu xử lý
    public void OnBoltClicked(BotlBase clickedBolt)
    {
        if (clickedBolt == null) return;

        Debug.Log($"👆 Click enqueued: {clickedBolt.name} (Queue size: {clickQueue.Count})");

        // Enqueue click
        clickQueue.Enqueue(clickedBolt);

        // Bắt đầu xử lý nếu chưa đang xử lý
        if (!isProcessing)
        {
            StartCoroutine(ProcessClickQueue());
        }
    }

    // ✅ QUEUE PROCESSOR - Xử lý tuần tự từng click
    private IEnumerator ProcessClickQueue()
    {
        isProcessing = true;
        Debug.Log("🔄 Bắt đầu xử lý click queue");

        while (clickQueue.Count > 0)
        {
            var clickedBolt = clickQueue.Dequeue();
            Debug.Log($"⚡ Xử lý click: {clickedBolt.name} (Còn lại: {clickQueue.Count})");

            // Xử lý logic ngay lập tức (không chờ animation)
            ProcessSingleClick(clickedBolt);

            // Yield để không block frame
            yield return null;
        }

        isProcessing = false;
        Debug.Log("✅ Hoàn thành xử lý click queue");
    }

    // ✅ LOGIC XỬ LÝ MỖI CLICK - Độc lập với animation
    private void ProcessSingleClick(BotlBase clickedBolt)
    {
        // Kiểm tra bolt có bị khóa không
        if (IsBoltLocked(clickedBolt))
        {
            Debug.Log($"🔒 Bolt {clickedBolt.name} đã hoàn thành - bỏ qua click");
            return;
        }

        if (currentLiftedScrew != null && currentSourceBolt != null)
        {
            // Có screw đang lift → xử lý di chuyển
            HandleScrewMovement(clickedBolt);
        }
        else
        {
            // Không có screw nào lift → nâng screw từ bolt được click
            HandleLiftScrew(clickedBolt);
        }

        // Cập nhật lock status sau mỗi thao tác
        UpdateBoltLockStatus();
    }

    // ✅ XỬ LÝ DI CHUYỂN SCREW
    private void HandleScrewMovement(BotlBase targetBolt)
    {
        if (GamePlayerController.Instance?.gameContaint?.sortScrew != null)
        {
            Debug.Log($"🔄 Xử lý di chuyển screw từ {currentSourceBolt.name} đến {targetBolt.name}");

            // Gọi SortScrew xử lý logic di chuyển (không callback)
            GamePlayerController.Instance.gameContaint.sortScrew.HandleScrewMovement(
                currentLiftedScrew, currentSourceBolt, targetBolt);

            // Logic đã được xử lý, reset state
            ResetCurrentScrew();
        }
    }

    // ✅ XỬ LÝ NÂNG SCREW
    private void HandleLiftScrew(BotlBase sourceBolt)
    {
        if (sourceBolt.screwBases.Count > 0)
        {
            ScrewBase topScrew = sourceBolt.GetTopScrew();
            if (topScrew != null)
            {
                Debug.Log($"🔼 Nâng screw ID {topScrew.id} từ {sourceBolt.name}");

                // Cập nhật logic state ngay lập tức
                currentLiftedScrew = topScrew;
                currentSourceBolt = sourceBolt;

                // Trigger animation (không callback)
                TriggerLiftAnimation(topScrew);
            }
        }
        else
        {
            Debug.Log($"⚠️ Bolt {sourceBolt.name} trống - không có screw để nâng");
        }
    }

    // ✅ TRIGGER ANIMATION - Không có callback logic
    private void TriggerLiftAnimation(ScrewBase screw)
    {
        screw.LiftUp(uniformLiftHeight, liftDuration, () =>
        {
            Debug.Log($"✨ Animation hoàn thành: screw {screw.id} đã nâng lên");
            // Chỉ là thông báo animation xong, không ảnh hưởng logic
        });
    }

    // ✅ KIỂM TRA BOLT KHÓA
    private bool IsBoltLocked(BotlBase bolt)
    {
        return boltLockStatus.ContainsKey(bolt) && boltLockStatus[bolt];
    }

    // ✅ CẬP NHẬT TRẠNG THÁI KHÓA BOLT
    private void UpdateBoltLockStatus()
    {
        foreach (var bolt in allBolts)
        {
            if (bolt != null)
            {
                bool wasLocked = IsBoltLocked(bolt);
                bool isNowComplete = IsBoltComplete(bolt);

                boltLockStatus[bolt] = isNowComplete;

                if (!wasLocked && isNowComplete)
                {
                    Debug.Log($"🏆 Bolt {bolt.name} vừa hoàn thành - đã khóa!");
                }
            }
        }
    }

    // ✅ KIỂM TRA BOLT HOÀN THÀNH
    private bool IsBoltComplete(BotlBase bolt)
    {
        if (bolt?.screwBases == null || bolt.screwBases.Count != 5)
            return false;

        int firstId = bolt.screwBases[0].id;
        foreach (var screw in bolt.screwBases)
        {
            if (screw == null || screw.id != firstId)
                return false;
        }

        return true;
    }

    // ✅ RESET STATE
    private void ResetCurrentScrew()
    {
        Debug.Log("🔄 Reset lifted screw state");
        currentLiftedScrew = null;
        currentSourceBolt = null;
    }

    // ✅ PUBLIC METHODS
    public void SetLiftedScrew(ScrewBase screw, BotlBase sourceBolt)
    {
        currentLiftedScrew = screw;
        currentSourceBolt = sourceBolt;
        Debug.Log($"📌 Set lifted screw: {screw.id} từ {sourceBolt.name}");
    }

    public bool IsGameComplete()
    {
        foreach (var bolt in allBolts)
        {
            if (bolt != null && !IsBoltComplete(bolt))
                return false;
        }

        Debug.Log("🏆 GAME HOÀN THÀNH - TẤT CẢ BOLT ĐỀU CÓ 5/5 SCREW CÙNG MÀU!");
        return allBolts.Count > 0;
    }

    // ✅ DEBUG METHODS
    [ContextMenu("Debug Current State")]
    public void DebugCurrentState()
    {
        Debug.Log($"=== BOLT LOGIC MANAGER STATE ===");
        Debug.Log($"isProcessing: {isProcessing}");
        Debug.Log($"clickQueue.Count: {clickQueue.Count}");
        Debug.Log($"currentLiftedScrew: {(currentLiftedScrew ? $"ID {currentLiftedScrew.id}" : "null")}");
        Debug.Log($"currentSourceBolt: {(currentSourceBolt ? currentSourceBolt.name : "null")}");

        Debug.Log("=== BOLT LOCK STATUS ===");
        foreach (var kvp in boltLockStatus)
        {
            Debug.Log($"{kvp.Key.name}: {(kvp.Value ? "LOCKED" : "UNLOCKED")}");
        }
    }

    [ContextMenu("Test Uniform Lift Height")]
    public void TestUniformLiftHeight()
    {
        Debug.Log($"Current uniformLiftHeight: {uniformLiftHeight}");
        Debug.Log($"Calculated optimal: {CalculateOptimalLiftHeight()}");
    }
}