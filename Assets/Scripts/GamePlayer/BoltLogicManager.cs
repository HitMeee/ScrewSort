using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoltLogicManager : MonoBehaviour
{
    public float moveDuration = 0.1f; // Giảm từ 0.2f
    public float uniformLiftHeight = 1.5f;
    public float liftDuration = 0.2f; // Giảm từ 0.4f để nhanh hơn
    public List<BotlBase> allBolts;

    // State
    private ScrewBase currentLiftedScrew;
    private BotlBase currentSourceBolt;
    private Queue<BotlBase> clickQueue = new Queue<BotlBase>();
    private bool isProcessing = false;
    private bool isAnimating = false; // Track animation state để block input
    private Dictionary<BotlBase, bool> boltLockStatus = new Dictionary<BotlBase, bool>();

    public void Init()
    {
        SetUp();
    }
    public void SetUp()
    {
        // Setup bolts
        if (allBolts == null || allBolts.Count == 0)
            allBolts = GamePlayerController.Instance?.gameContaint?.levelController?.GetAllBolts() ?? new List<BotlBase>();

        // Initialize lock status
        boltLockStatus.Clear();
        allBolts.ForEach(bolt => { if (bolt != null) boltLockStatus[bolt] = false; });
    }

    public void OnBoltClicked(BotlBase clickedBolt)
    {
        if (clickedBolt == null || IsBoltLocked(clickedBolt) || clickQueue.Contains(clickedBolt))
            return;

        clickQueue.Enqueue(clickedBolt);
        if (!isProcessing)
            StartCoroutine(ProcessClickQueue());
    }

    private IEnumerator ProcessClickQueue()
    {
        isProcessing = true;

        while (clickQueue.Count > 0)
        {
            var bolt = clickQueue.Dequeue();
            if (bolt != null && !IsBoltLocked(bolt))
            {
                yield return ProcessBoltClick(bolt);
                yield return new WaitForSeconds(0.1f);
            }
        }

        isProcessing = false;
    }

    private IEnumerator ProcessBoltClick(BotlBase clickedBolt)
    {
        var sourceBolt = currentSourceBolt;

        // Main logic decision
        if (HasLiftedScrew())
            yield return HandleLiftedScrewClick(clickedBolt);
        else
            yield return LiftScrewFromBolt(clickedBolt);

        // Post-processing
        if (sourceBolt != null && sourceBolt != clickedBolt)
            CheckGameCompletion();
        UpdateBoltLockStatus();
    }

    private IEnumerator HandleLiftedScrewClick(BotlBase targetBolt)
    {
        // Drop back if same bolt
        if (targetBolt == currentSourceBolt)
        {
            yield return DropScrew();
            yield break;
        }

        var targetTopScrew = targetBolt.GetTopScrew();

        // Determine action: Move or Swap
        if (ShouldMoveScrews(targetBolt, targetTopScrew))
            yield return MoveScrewToBolt(targetBolt);
        else
            yield return SwapScrews(targetBolt, targetTopScrew);
    }

    private bool ShouldMoveScrews(BotlBase targetBolt, ScrewBase targetTopScrew)
    {
        // Empty target or same color with space
        return targetTopScrew == null ||
               (currentLiftedScrew.id == targetTopScrew.id && targetBolt.SlotsAvailable() > 0);
    }

    private IEnumerator LiftScrewFromBolt(BotlBase sourceBolt)
    {
        if (sourceBolt?.screwBases?.Count <= 0) yield break;

        var topScrew = sourceBolt.GetTopScrew();
        if (topScrew == null) yield break;

        currentLiftedScrew = topScrew;
        currentSourceBolt = sourceBolt;

        isAnimating = true;
        yield return WaitForAnimation(topScrew.LiftUp, uniformLiftHeight, liftDuration);
        isAnimating = false;
    }

    private IEnumerator DropScrew()
    {
        isAnimating = true;
        yield return WaitForAnimation(currentLiftedScrew.DropToOriginal, moveDuration);
        isAnimating = false;
        ResetLiftedScrew();
    }

    private IEnumerator MoveScrewToBolt(BotlBase targetBolt)
    {
        var sortScrew = GamePlayerController.Instance?.gameContaint?.sortScrew;
        if (sortScrew != null)
        {
            isAnimating = true;
            
            // Tính số lượng ốc sẽ di chuyển để tính thời gian đợi chính xác
            int moveCount = CalculateMoveCount(currentSourceBolt, targetBolt, currentLiftedScrew.id);
            float totalWaitTime = moveCount * moveDuration + (moveCount - 1) * 0.02f + 0.1f;
            
            sortScrew.HandleScrewMovement(currentLiftedScrew, currentSourceBolt, targetBolt);
            yield return new WaitForSeconds(totalWaitTime);
            isAnimating = false;
            ResetLiftedScrew();
        }
    }
    
    private int CalculateMoveCount(BotlBase source, BotlBase target, int screwId)
    {
        if (source == null || target == null) return 0;
        
        // Đếm số ốc cùng màu liên tiếp từ trên xuống
        int consecutive = 0;
        if (source.screwBases != null)
        {
            for (int i = source.screwBases.Count - 1; i >= 0; i--)
            {
                if (source.screwBases[i]?.id == screwId)
                    consecutive++;
                else
                    break;
            }
        }
        
        // Tính số slot khả dụng
        int available = target.SlotsAvailable();
        return Mathf.Min(consecutive, available);
    }

    private IEnumerator SwapScrews(BotlBase targetBolt, ScrewBase targetTopScrew)
    {
        // Drop current
        yield return DropScrew();

        // Lift target
        if (targetTopScrew != null && targetBolt != null)
        {
            currentLiftedScrew = targetTopScrew;
            currentSourceBolt = targetBolt;
            isAnimating = true;
            yield return WaitForAnimation(targetTopScrew.LiftUp, uniformLiftHeight, liftDuration);
            isAnimating = false;
        }
    }

    // ✅ HELPER: Unified animation waiting
    private IEnumerator WaitForAnimation(System.Action<float, System.Action> animationMethod, float duration, System.Action onComplete = null)
    {
        bool completed = false;
        animationMethod(duration, () => { onComplete?.Invoke(); completed = true; });
        yield return new WaitUntil(() => completed);
    }

    private IEnumerator WaitForAnimation(System.Action<float, float, System.Action> animationMethod, float param1, float param2)
    {
        bool completed = false;
        animationMethod(param1, param2, () => completed = true);
        yield return new WaitUntil(() => completed);
    }

    private void CheckGameCompletion()
    {
        GamePlayerController.Instance?.gameContaint?.sortScrew?.checker?.CheckAfterMove(currentSourceBolt, null);

        if (IsGameComplete())
        {
            SoundManager.Instance?.PlayLevelComplete();
            GamePlayerController.Instance?.gameScene?.OnLevelComplete();
            ForceResetState();
        }
    }

    public bool IsGameComplete()
    {
        if (allBolts?.Count == 0) return false;

        var activeBolts = allBolts.FindAll(b => b?.screwBases?.Count > 0);
        return activeBolts.Count > 0 && activeBolts.All(IsBoltComplete);
    }

    private void UpdateBoltLockStatus()
    {
        allBolts.ForEach(bolt => { if (bolt != null) boltLockStatus[bolt] = IsBoltComplete(bolt); });
    }

    private bool IsBoltComplete(BotlBase bolt)
    {
        if (bolt?.screwBases?.Count != 5) return false;
        return bolt.screwBases.All(screw => screw?.id == bolt.screwBases[0].id);
    }

    private bool IsBoltLocked(BotlBase bolt) => boltLockStatus.GetValueOrDefault(bolt, false);

    private void ResetLiftedScrew()
    {
        currentLiftedScrew = null;
        currentSourceBolt = null;
    }

    // Public API
    public void ForceResetState()
    {
        StopAllCoroutines();
        ResetLiftedScrew();
        clickQueue.Clear();
        isProcessing = false;
        isAnimating = false;
    }

    public bool HasLiftedScrew() => currentLiftedScrew != null && currentSourceBolt != null;
    public ScrewBase GetCurrentLiftedScrew() => currentLiftedScrew;
    public BotlBase GetCurrentSourceBolt() => currentSourceBolt;
    public int GetQueueSize() => clickQueue.Count;
    public bool IsCurrentlyProcessing() => isProcessing;
    public bool IsCurrentlyAnimating() => isAnimating;

    public void SetLiftedScrew(ScrewBase screw, BotlBase sourceBolt)
    {
        currentLiftedScrew = screw;
        currentSourceBolt = sourceBolt;
    }
}