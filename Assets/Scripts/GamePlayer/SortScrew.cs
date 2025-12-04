using System.Collections.Generic;
using UnityEngine;

public class SortScrew : MonoBehaviour
{
    public ScrewMover mover;
    public BoltChecker checker;
    private BackStep backStep;

    public void Init()
    {
        mover ??= gameObject.AddComponent<ScrewMover>();
        checker ??= gameObject.AddComponent<BoltChecker>();

        var levelController = FindObjectOfType<LevelController>();
        if (levelController != null)
        {
            backStep = levelController.GetBackStep();
        }
    }

    // Hàm chính xử lý di chuyển
    public void HandleScrewMovement(ScrewBase lifted, BotlBase source, BotlBase target)
    {
        if (lifted == null || source == null || target == null) return;

        // Cùng bolt - thả xuống
        if (source == target)
        {
            lifted.DropToOriginal(mover.moveDuration);
            return;
        }

        // Bolt trống
        if (target.screwBases.Count == 0)
        {
            MoveBatch(lifted, source, target);
            return;
        }

        var topTarget = target.GetTopScrew();
        if (topTarget == null)
        {
            lifted.DropToOriginal(mover.moveDuration);
            return;
        }

        // Khác màu - swap
        if (topTarget.id != lifted.id)
        {
            SwapScrews(lifted, source, target, topTarget);
        }
        // Cùng màu - di chuyển batch
        else
        {
            MoveBatch(lifted, source, target);
        }

        checker.CheckAfterMove(source, target);
    }

    // Di chuyển nhiều screw cùng màu
    private void MoveBatch(ScrewBase lifted, BotlBase source, BotlBase target)
    {
        int moveCount = GetMoveCount(source, target, lifted.id);

        if (moveCount <= 0)
        {
            lifted.DropToOriginal(mover.moveDuration);
            return;
        }

        // Ghi lại để undo
        RecordMove(source, target, moveCount, lifted.id);

        // Thực hiện di chuyển
        ExecuteMove(source, target, moveCount, lifted.id);
    }

    // Swap 2 screw khác màu
    private void SwapScrews(ScrewBase lifted, BotlBase source, BotlBase target, ScrewBase topTarget)
    {
        // Ghi lại swap
        if (backStep != null)
        {
            backStep.GhiLaiDiChuyenScrew(lifted, source, target);
        }

        // Thả screw xuống
        lifted.DropToOriginal(mover.moveDuration);

        // Nâng screw khác lên
        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager != null)
        {
            boltManager.SetLiftedScrew(topTarget, target);
            topTarget.LiftUp(boltManager.uniformLiftHeight, boltManager.liftDuration);
        }
    }

    // Tính số lượng screw có thể di chuyển
    private int GetMoveCount(BotlBase source, BotlBase target, int screwId)
    {
        int consecutive = CountConsecutive(source, screwId);
        int available = target.SlotsAvailable();
        return Mathf.Min(consecutive, available);
    }

    // Đếm screw cùng màu liên tiếp từ trên xuống
    private int CountConsecutive(BotlBase bolt, int targetId)
    {
        if (bolt?.screwBases == null) return 0;

        int count = 0;
        for (int i = bolt.screwBases.Count - 1; i >= 0; i--)
        {
            if (bolt.screwBases[i]?.id == targetId)
                count++;
            else
                break;
        }
        return count;
    }

    // Ghi lại để undo
    private void RecordMove(BotlBase source, BotlBase target, int moveCount, int screwId)
    {
        if (backStep == null) return;

        if (moveCount == 1)
        {
            var screw = source.GetTopScrew();
            backStep.GhiLaiDiChuyenScrew(screw, source, target);
        }
        else
        {
            List<ScrewBase> screws = GetScrewsToMove(source, moveCount, screwId);
            backStep.GhiLaiDiChuyenNhieuScrew(screws, source, target, screwId);
        }
    }

    // Lấy danh sách screw sẽ di chuyển
    private List<ScrewBase> GetScrewsToMove(BotlBase source, int count, int screwId)
    {
        List<ScrewBase> screws = new List<ScrewBase>();

        for (int i = source.screwBases.Count - 1; i >= 0 && screws.Count < count; i--)
        {
            var screw = source.screwBases[i];
            if (screw?.id == screwId)
                screws.Add(screw);
            else
                break;
        }

        return screws;
    }

    // ✅ FIX: Thực hiện di chuyển với bảo vệ scale
    private void ExecuteMove(BotlBase source, BotlBase target, int count, int screwId)
    {
        List<ScrewBase> movedScrews = new List<ScrewBase>();

        for (int i = 0; i < count; i++)
        {
            var screw = source.GetTopScrew();
            if (screw?.id != screwId) break;

            // ✅ FIX: Lưu scale gốc trước khi thay đổi parent
            Vector3 originalScale = screw.transform.localScale;

            // Cập nhật data
            source.RemoveScrew(screw);
            target.AddScrew(screw);
            screw.transform.SetParent(target.transform);

            // ✅ FIX: Khôi phục scale sau khi đổi parent
            screw.transform.localScale = originalScale;

            movedScrews.Add(screw);
        }

        // ✅ FIX: Animation đồng bộ không delay để tránh conflict
        TriggerSafeAnimation(movedScrews, target);
    }

    // ✅ FIX: Animation an toàn không gây scale issue
    private void TriggerSafeAnimation(List<ScrewBase> screws, BotlBase target)
    {
        for (int i = 0; i < screws.Count; i++)
        {
            var screw = screws[i];
            if (screw != null)
            {
                Vector3 targetPos = GetPosition(target, target.screwBases.IndexOf(screw));

                // ✅ FIX: Lưu scale trước animation
                Vector3 savedScale = screw.transform.localScale;

                // Di chuyển ngay không delay để tránh conflict
                screw.MoveTo(targetPos, mover.moveDuration, () => {
                    if (screw != null)
                    {
                        screw.originalPosition = targetPos;
                        // ✅ FIX: Đảm bảo scale được khôi phục
                        screw.transform.localScale = savedScale;
                    }
                });
            }
        }
    }

    // Tính vị trí đúng
    private Vector3 GetPosition(BotlBase bolt, int index)
    {
        if (bolt?.postBolts != null && index >= 0 && index < bolt.postBolts.Count)
        {
            return bolt.postBolts[index].transform.position;
        }
        return bolt.transform.position + Vector3.up * (index * 0.3f + 0.2f);
    }

    // Public methods
    public bool IsGameComplete(List<BotlBase> allBolts) => checker.IsGameComplete(allBolts);
    public void SetBackStep(BackStep backStepRef) => backStep = backStepRef;
    public BackStep GetBackStep() => backStep;
}