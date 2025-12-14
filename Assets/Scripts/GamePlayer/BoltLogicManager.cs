using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltLogicManager : MonoBehaviour
{
    [Header("Settings")]
    public float moveDuration = 0.3f;
    public float uniformLiftHeight = 1.5f;
    public float liftDuration = 0.4f;

    [Header("References")]
    public List<BotlBase> allBolts;

    // Private state
    private ScrewBase currentLiftedScrew;
    private BotlBase currentSourceBolt;
    private Queue<BotlBase> clickQueue = new Queue<BotlBase>();
    private bool isProcessing = false;
    private Dictionary<BotlBase, bool> boltLockStatus = new Dictionary<BotlBase, bool>();

    public void Init()
    {
        SetupLiftHeight();
        SetupBolts();
        InitializeLockStatus();
        Debug.Log("🎮 BoltLogicManager initialized");
    }

    private void SetupLiftHeight()
    {
        if (uniformLiftHeight <= 0)
            uniformLiftHeight = CalculateOptimalLiftHeight();
    }

    private void SetupBolts()
    {
        if (allBolts == null || allBolts.Count == 0)
        {
            var levelController = GamePlayerController.Instance?.gameContaint?.levelController;
            allBolts = levelController?.GetAllBolts() ?? new List<BotlBase>();
        }
    }

    private void InitializeLockStatus()
    {
        boltLockStatus.Clear();
        foreach (var bolt in allBolts)
        {
            if (bolt != null) boltLockStatus[bolt] = false;
        }
    }

    private float CalculateOptimalLiftHeight()
    {
        float maxHeight = 0f;
        foreach (var bolt in allBolts)
        {
            if (bolt?.postBolts != null)
            {
                foreach (var post in bolt.postBolts)
                {
                    if (post != null)
                        maxHeight = Mathf.Max(maxHeight, post.transform.position.y - bolt.transform.position.y);
                }
            }
        }
        return maxHeight + 0.8f;
    }

    public void OnBoltClicked(BotlBase clickedBolt)
    {
        if (clickedBolt == null || IsBoltLocked(clickedBolt) || clickQueue.Contains(clickedBolt))
            return;

        clickQueue.Enqueue(clickedBolt);
        Debug.Log($"🎯 Added {clickedBolt.name} to queue. Size: {clickQueue.Count}");

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
                yield return StartCoroutine(ProcessBoltClick(bolt));
                yield return new WaitForSeconds(0.1f);
            }
        }

        isProcessing = false;
    }

    private IEnumerator ProcessBoltClick(BotlBase clickedBolt)
    {
        var sourceBolt = currentSourceBolt; // Store for completion check

        if (HasLiftedScrew())
            yield return StartCoroutine(HandleLiftedScrewClick(clickedBolt));
        else
            yield return StartCoroutine(LiftScrewFromBolt(clickedBolt));

        // Check completion after moves
        if (sourceBolt != null && sourceBolt != clickedBolt)
            CheckGameCompletion(sourceBolt, clickedBolt);

        UpdateBoltLockStatus();
    }

    private IEnumerator HandleLiftedScrewClick(BotlBase targetBolt)
    {
        // Same bolt - drop back
        if (targetBolt == currentSourceBolt)
        {
            yield return StartCoroutine(DropScrewBack());
            yield break;
        }

        var targetTopScrew = targetBolt.GetTopScrew();

        // Can move - same color or empty
        if (targetTopScrew == null || currentLiftedScrew.id == targetTopScrew.id)
            yield return StartCoroutine(MoveScrewToBolt(targetBolt));
        else
            yield return StartCoroutine(SwapScrews(targetBolt, targetTopScrew));
    }

    private IEnumerator DropScrewBack()
    {
        bool completed = false;
        currentLiftedScrew.DropToOriginal(moveDuration, () =>
        {
            ResetLiftedScrew();
            completed = true;
        });
        yield return new WaitUntil(() => completed);
    }

    private IEnumerator MoveScrewToBolt(BotlBase targetBolt)
    {
        var sortScrew = GamePlayerController.Instance?.gameContaint?.sortScrew;
        if (sortScrew != null)
        {
            sortScrew.HandleScrewMovement(currentLiftedScrew, currentSourceBolt, targetBolt);
            yield return new WaitForSeconds(moveDuration + 0.1f);
            ResetLiftedScrew();
        }
    }

    private IEnumerator SwapScrews(BotlBase targetBolt, ScrewBase targetTopScrew)
    {
        // Drop current
        yield return StartCoroutine(DropScrewBack());

        // Lift new
        currentLiftedScrew = targetTopScrew;
        currentSourceBolt = targetBolt;

        bool completed = false;
        targetTopScrew.LiftUp(uniformLiftHeight, liftDuration, () => completed = true);
        yield return new WaitUntil(() => completed);
    }

    private IEnumerator LiftScrewFromBolt(BotlBase sourceBolt)
    {
        if (sourceBolt?.screwBases?.Count > 0)
        {
            var topScrew = sourceBolt.GetTopScrew();
            if (topScrew != null)
            {
                currentLiftedScrew = topScrew;
                currentSourceBolt = sourceBolt;

                bool completed = false;
                topScrew.LiftUp(uniformLiftHeight, liftDuration, () => completed = true);
                yield return new WaitUntil(() => completed);
            }
        }
    }

    private void CheckGameCompletion(BotlBase sourceBolt, BotlBase targetBolt)
    {
        var boltChecker = GamePlayerController.Instance?.gameContaint?.sortScrew?.checker;
        boltChecker?.CheckAfterMove(sourceBolt, targetBolt);

        if (IsGameComplete())
        {
            Debug.Log("🏆 GAME COMPLETED!");
            GamePlayerController.Instance?.gameScene?.OnLevelComplete();
            ForceResetState();
        }
    }

    public bool IsGameComplete()
    {
        if (allBolts == null || allBolts.Count == 0) return false;

        int completed = 0, total = 0;

        foreach (var bolt in allBolts)
        {
            if (bolt?.screwBases?.Count > 0)
            {
                total++;
                if (IsBoltComplete(bolt)) completed++;
            }
        }

        return total > 0 && completed == total;
    }

    private void UpdateBoltLockStatus()
    {
        foreach (var bolt in allBolts)
        {
            if (bolt != null)
                boltLockStatus[bolt] = IsBoltComplete(bolt);
        }
    }

    private bool IsBoltComplete(BotlBase bolt)
    {
        if (bolt?.screwBases == null || bolt.screwBases.Count != 5) return false;

        int firstId = bolt.screwBases[0].id;
        foreach (var screw in bolt.screwBases)
        {
            if (screw?.id != firstId) return false;
        }
        return true;
    }

    private bool IsBoltLocked(BotlBase bolt) =>
        boltLockStatus.ContainsKey(bolt) && boltLockStatus[bolt];

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
    }

    public bool HasLiftedScrew() => currentLiftedScrew != null && currentSourceBolt != null;
    public ScrewBase GetCurrentLiftedScrew() => currentLiftedScrew;
    public BotlBase GetCurrentSourceBolt() => currentSourceBolt;
    public int GetQueueSize() => clickQueue.Count;
    public bool IsCurrentlyProcessing() => isProcessing;
    public bool IsCurrentlyAnimating() => isProcessing; // Simplified - same as processing

    public void SetLiftedScrew(ScrewBase screw, BotlBase sourceBolt)
    {
        currentLiftedScrew = screw;
        currentSourceBolt = sourceBolt;
    }
}