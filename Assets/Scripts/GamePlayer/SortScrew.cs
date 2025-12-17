using System.Collections;
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

        // ✅ SAVE STATE BEFORE ANY MOVEMENT - Using new English method name
        if (backStep != null && source != target)
        {
            backStep.SaveCurrentState();
        }

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

    // ✅ ĐƠN GIẢN HÓA: Di chuyển batch (không cần ghi lại riêng)
    private void MoveBatch(ScrewBase lifted, BotlBase source, BotlBase target)
    {
        int moveCount = GetMoveCount(source, target, lifted.id);

        if (moveCount <= 0)
        {
            lifted.DropToOriginal(mover.moveDuration);
            return;
        }

        // ✅ SỬA: Sử dụng Coroutine để delay giữa các screws
        StartCoroutine(ExecuteMoveWithDelay(source, target, moveCount, lifted.id));
    }

    // ✅ ĐƠN GIẢN HÓA: Swap screws (không cần ghi lại riêng)
    private void SwapScrews(ScrewBase lifted, BotlBase source, BotlBase target, ScrewBase topTarget)
    {
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

    // ✅ MỚI: Thực hiện di chuyển với delay 0.1s giữa các screw
    private IEnumerator ExecuteMoveWithDelay(BotlBase source, BotlBase target, int count, int screwId)
    {
        for (int i = 0; i < count; i++)
        {
            var screw = source.GetTopScrew();
            if (screw?.id != screwId) break;

            // Lưu scale gốc
            Vector3 originalScale = screw.transform.localScale;

            // Cập nhật data
            source.RemoveScrew(screw);
            target.AddScrew(screw);
            screw.transform.SetParent(target.transform);

            // Khôi phục scale
            screw.transform.localScale = originalScale;

            // Animation
            Vector3 pos = GetPosition(target, target.screwBases.Count - 1);

            bool moveCompleted = false;
            screw.MoveTo(pos, mover.moveDuration, () => {
                if (screw != null)
                {
                    screw.originalPosition = pos;
                    screw.transform.localScale = originalScale;
                }
                moveCompleted = true;
            });

            // Đợi animation hoàn thành
            yield return new WaitUntil(() => moveCompleted);

            // ✅ Delay 0.1s giữa các screw (trừ screw cuối cùng)
            if (i < count - 1)
            {
                yield return new WaitForSeconds(0.02f);
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