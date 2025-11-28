using System.Collections.Generic;
using UnityEngine;

public class SortScrew : MonoBehaviour
{
    public ScrewMover mover;
    public ScrewRules rules;
    public ScrewBatchMover batcher;
    public BoltChecker checker;

    public void Init()
    {
        mover ??= gameObject.AddComponent<ScrewMover>();
        rules ??= gameObject.AddComponent<ScrewRules>();
        batcher ??= gameObject.AddComponent<ScrewBatchMover>();
        checker ??= gameObject.AddComponent<BoltChecker>();
    }

    // ✅ MAIN LOGIC HANDLER - Không có callback
    public void HandleScrewMovement(ScrewBase lifted, BotlBase source, BotlBase target)
    {
        if (lifted == null || source == null || target == null)
        {
            Debug.LogError("❌ HandleScrewMovement: tham số null");
            return;
        }

        Debug.Log($"=== XỬ LÝ DI CHUYỂN SCREW ===");
        Debug.Log($"Screw: ID {lifted.id} từ {source.name} → {target.name}");

        // TH1: Cùng bolt - trả screw xuống
        if (source == target)
        {
            HandleSameBolt(lifted, source);
            return;
        }

        // TH2: Bolt khác - xử lý theo tình huống
        if (target.screwBases.Count == 0)
        {
            // TH2a: Bolt đích trống
            HandleEmptyTarget(lifted, source, target);
        }
        else
        {
            var topTarget = target.GetTopScrew();
            if (topTarget == null)
            {
                Debug.LogError($"❌ TopTarget null cho bolt {target.name}");
                HandleSameBolt(lifted, source);
                return;
            }

            if (topTarget.id != lifted.id)
            {
                // TH2b: Bolt đích có screw khác màu - swap
                HandleDifferentColor(lifted, source, target, topTarget);
            }
            else
            {
                // TH2c: Bolt đích có screw cùng màu
                HandleSameColor(lifted, source, target);
            }
        }

        // Kiểm tra hoàn thành sau mỗi thao tác
        checker.CheckAfterMove(source, target);
    }

    // ✅ TH1: CÙNG BOLT - Trả screw xuống
    private void HandleSameBolt(ScrewBase lifted, BotlBase source)
    {
        Debug.Log($"📍 TH1: Trả screw {lifted.id} xuống {source.name}");

        // Animation thả xuống (không callback logic)
        TriggerDropAnimation(lifted);
    }

    // ✅ TH2A: BOLT TRỐNG - Di chuyển batch
    private void HandleEmptyTarget(ScrewBase lifted, BotlBase source, BotlBase target)
    {
        Debug.Log($"📦 TH2a: Bolt đích {target.name} trống");

        // Đếm số screw liên tiếp cùng màu
        int consecutiveCount = CountConsecutiveScrews(source, lifted.id);
        int availableSlots = target.SlotsAvailable();
        int moveCount = Mathf.Min(consecutiveCount, availableSlots);

        Debug.Log($"Consecutive: {consecutiveCount}, Available: {availableSlots}, Move: {moveCount}");

        if (moveCount <= 0)
        {
            Debug.Log("❌ Không thể di chuyển - trả screw xuống");
            HandleSameBolt(lifted, source);
            return;
        }

        // Thực hiện logic di chuyển ngay lập tức
        ExecuteBatchMove(source, target, moveCount, lifted.id);
    }

    // ✅ TH2B: KHÁC MÀU - Swap
    private void HandleDifferentColor(ScrewBase lifted, BotlBase source, BotlBase target, ScrewBase topTarget)
    {
        Debug.Log($"🔄 TH2b: Swap {lifted.id} ⟷ {topTarget.id}");

        // BƯỚC 1: Thả screw hiện tại xuống source
        TriggerDropAnimation(lifted);

        // BƯỚC 2: Nâng screw từ target (logic ngay lập tức)
        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager != null)
        {
            boltManager.SetLiftedScrew(topTarget, target);

            // Trigger animation nâng lên
            TriggerLiftAnimation(topTarget, boltManager.uniformLiftHeight, boltManager.liftDuration);
        }

        Debug.Log($"✅ Swap completed: {lifted.id} → {source.name}, {topTarget.id} → lifted");
    }

    // ✅ TH2C: CÙNG MÀU - Di chuyển batch có kiểm tra
    private void HandleSameColor(ScrewBase lifted, BotlBase source, BotlBase target)
    {
        Debug.Log($"🎯 TH2c: Cùng màu {lifted.id}");

        int availableSlots = target.SlotsAvailable();
        if (availableSlots <= 0)
        {
            Debug.Log("❌ Bolt đích đầy - trả screw xuống");
            HandleSameBolt(lifted, source);
            return;
        }

        // Đếm số screw có thể di chuyển
        int consecutiveCount = CountConsecutiveScrews(source, lifted.id);
        int moveCount = Mathf.Min(consecutiveCount, availableSlots);

        Debug.Log($"Cùng màu - Move count: {moveCount}");

        if (moveCount <= 0)
        {
            HandleSameBolt(lifted, source);
            return;
        }

        // Thực hiện logic di chuyển
        ExecuteBatchMove(source, target, moveCount, lifted.id);
    }

    // ✅ ĐẾM SCREW LIÊN TIẾP CÙNG MÀU
    private int CountConsecutiveScrews(BotlBase bolt, int targetId)
    {
        if (bolt?.screwBases == null || bolt.screwBases.Count == 0)
            return 0;

        int count = 0;
        for (int i = bolt.screwBases.Count - 1; i >= 0; i--)
        {
            if (bolt.screwBases[i]?.id == targetId)
            {
                count++;
            }
            else
            {
                break; // Dừng khi gặp screw khác màu
            }
        }

        return count;
    }

    // ✅ THỰC HIỆN LOGIC DI CHUYỂN BATCH
    private void ExecuteBatchMove(BotlBase source, BotlBase target, int moveCount, int screwId)
    {
        Debug.Log($"🚚 ExecuteBatchMove: {moveCount} screw màu {screwId} từ {source.name} → {target.name}");

        List<ScrewBase> screwsToMove = new List<ScrewBase>();

        // Lấy danh sách screw cần di chuyển (từ top xuống)
        for (int i = 0; i < moveCount && source.screwBases.Count > 0; i++)
        {
            var topScrew = source.GetTopScrew();
            if (topScrew != null && topScrew.id == screwId)
            {
                screwsToMove.Add(topScrew);

                // Cập nhật logic state ngay lập tức
                source.RemoveScrew(topScrew);
                target.AddScrew(topScrew);
                topScrew.transform.SetParent(target.transform);
            }
            else
            {
                break;
            }
        }

        Debug.Log($"✅ Logic completed: Moved {screwsToMove.Count} screws");

        // Trigger animation cho tất cả screw đã di chuyển
        TriggerBatchMoveAnimation(screwsToMove, target);
    }

    // ✅ ANIMATION TRIGGERS - Không có callback logic
    private void TriggerDropAnimation(ScrewBase screw)
    {
        screw.DropToOriginal(mover.moveDuration, () =>
        {
            Debug.Log($"✨ Drop animation completed for screw {screw.id}");
        });
    }

    private void TriggerLiftAnimation(ScrewBase screw, float height, float duration)
    {
        screw.LiftUp(height, duration, () =>
        {
            Debug.Log($"✨ Lift animation completed for screw {screw.id}");
        });
    }

    private void TriggerBatchMoveAnimation(List<ScrewBase> screws, BotlBase target)
    {
        for (int i = 0; i < screws.Count; i++)
        {
            var screw = screws[i];
            Vector3 targetPos = GetCorrectPosition(target, target.screwBases.IndexOf(screw));

            screw.MoveTo(targetPos, mover.moveDuration, () =>
            {
                Debug.Log($"✨ Move animation completed for screw {screw.id}");
                screw.originalPosition = targetPos;
            });
        }
    }

    // ✅ TÍNH VỊ TRÍ CHÍNH XÁC
    private Vector3 GetCorrectPosition(BotlBase bolt, int screwIndex)
    {
        if (bolt?.postBolts != null && screwIndex >= 0 && screwIndex < bolt.postBolts.Count && bolt.postBolts[screwIndex] != null)
        {
            return bolt.postBolts[screwIndex].transform.position;
        }

        // Fallback
        return bolt.transform.position + Vector3.up * (screwIndex * 0.3f + 0.2f);
    }

    // ✅ PUBLIC METHODS
    public bool IsGameComplete(List<BotlBase> allBolts)
    {
        return checker.IsGameComplete(allBolts);
    }
}