using System;
using System.Collections;
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

    public void HandleScrewMovement(ScrewBase lifted, BotlBase source, BotlBase target, Action onComplete)
    {
        if (lifted == null || source == null || target == null)
        {
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"=== HANDLE SCREW MOVEMENT ===");
        Debug.Log($"Lifted: ID {lifted.id} từ {source.name}");
        Debug.Log($"Target: {target.name} - Count: {target.screwBases.Count}, Slots: {target.SlotsAvailable()}");

        if (source == target)
        {
            Debug.Log("TH1: Cùng bolt - thả xuống");
            lifted.DropToOriginal(mover.moveDuration, onComplete);
            return;
        }

        // ✅ FIX: SỬ DỤNG BOLTCHECKER THAY VÌ KIỂM TRA TRỰC TIẾP
        if (!checker.CanInteractWithBolt(target))
        {
            // BoltChecker sẽ tự debug message "KHÓA, KHÔNG THỂ TƯƠNG TÁC"
            lifted.DropToOriginal(mover.moveDuration, onComplete);
            return;
        }

        if (target.screwBases.Count == 0)
        {
            Debug.Log("TH3: Bolt trống - di chuyển batch");
            HandleEmptyTarget(lifted, source, target, onComplete);
            return;
        }

        var topTarget = target.GetTopScrew();
        if (topTarget == null)
        {
            Debug.Log("TH4: TopTarget null - thả xuống");
            lifted.DropToOriginal(mover.moveDuration, onComplete);
            return;
        }

        Debug.Log($"TopTarget: ID {topTarget.id}");

        if (topTarget.id != lifted.id)
        {
            Debug.Log("TH4a: Screw khác màu - thực hiện swap");
            HandleSwapDifferentColor(lifted, source, target, topTarget, onComplete);
        }
        else
        {
            Debug.Log("TH4b: Screw cùng màu - kiểm tra di chuyển batch");

            if (target.SlotsAvailable() <= 0)
            {
                Debug.Log("TH4b: Không còn slot trống - thả xuống");
                lifted.DropToOriginal(mover.moveDuration, onComplete);
                return;
            }

            HandleSameColor(lifted, source, target, onComplete);
        }
    }

    private void HandleEmptyTarget(ScrewBase lifted, BotlBase source, BotlBase target, Action onComplete)
    {
        int moveCount = batcher.CountConsecutiveScrewsOfSameColor(source, lifted.id);
        moveCount = Mathf.Min(moveCount, target.SlotsAvailable());

        Debug.Log($"HandleEmptyTarget: moveCount = {moveCount}");

        if (moveCount <= 0)
        {
            lifted.DropToOriginal(mover.moveDuration, onComplete);
            return;
        }

        var batch = batcher.GetBatch(source, moveCount);
        StartCoroutine(mover.MoveBatch(batch, source, target, GetTopPos, () =>
        {
            // ✅ FIX: SỬ DỤNG BOLTCHECKER ĐỂ KIỂM TRA SAU DI CHUYỂN
            checker.CheckAfterMove(source, target);
            onComplete?.Invoke();
        }));
    }

    private void HandleSwapDifferentColor(ScrewBase lifted, BotlBase source, BotlBase target,
                                        ScrewBase topTarget, Action onComplete)
    {
        lifted.DropToOriginal(mover.moveDuration, () =>
        {
            var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
            float liftHeight = 1.5f;
            float liftDuration = 0.4f;

            if (boltManager != null)
            {
                liftHeight = boltManager.uniformLiftHeight;
                liftDuration = boltManager.liftDuration;
            }

            topTarget.LiftUp(liftHeight, liftDuration, () =>
            {
                if (boltManager != null)
                {
                    boltManager.LiftScrewFromExternal(topTarget, target);
                }
                onComplete?.Invoke();
            });
        });
    }

    private void HandleSameColor(ScrewBase lifted, BotlBase source, BotlBase target, Action onComplete)
    {
        int movable = batcher.GetMovableCount(rules, source, target, lifted);

        Debug.Log($"HandleSameColor: movable = {movable}");

        if (movable <= 0)
        {
            Debug.Log("HandleSameColor: movable <= 0 - thả xuống");
            lifted.DropToOriginal(mover.moveDuration, onComplete);
            return;
        }

        var batchSame = batcher.GetBatch(source, movable);
        Debug.Log($"Moving batch of {batchSame.Count} screws");

        StartCoroutine(mover.MoveBatch(batchSame, source, target, GetTopPos, () =>
        {
            // ✅ FIX: SỬ DỤNG BOLTCHECKER ĐỂ KIỂM TRA SAU DI CHUYỂN
            checker.CheckAfterMove(source, target);
            onComplete?.Invoke();
        }));
    }

    private Vector3 GetTopPos(BotlBase bolt)
    {
        if (bolt == null) return Vector3.zero;

        int nextIndex = bolt.screwBases.Count;

        if (bolt.postBolts != null && nextIndex < bolt.postBolts.Count && bolt.postBolts[nextIndex] != null)
        {
            return bolt.postBolts[nextIndex].transform.position;
        }

        if (bolt.screwBases.Count > 0 && bolt.screwBases[bolt.screwBases.Count - 1] != null)
        {
            return bolt.screwBases[bolt.screwBases.Count - 1].transform.position + Vector3.up * 0.3f;
        }

        return bolt.transform.position + Vector3.up * 0.2f;
    }

    // ✅ FIX: SỬ DỤNG BOLTCHECKER ĐỂ KIỂM TRA GAME HOÀN THÀNH
    public bool IsGameComplete(List<BotlBase> allBolts)
    {
        return checker.IsGameComplete(allBolts);
    }
}