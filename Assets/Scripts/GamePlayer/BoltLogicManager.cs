using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltLogicManager : MonoBehaviour
{
    public List<BotlBase> allBolts;
    public float moveDuration = 0.3f;
    public float uniformLiftHeight = 1.5f;
    public float liftDuration = 0.4f;

    private ScrewBase currentLiftedScrew;
    private BotlBase currentSourceBolt;
    private Queue<BotlBase> clickQueue = new Queue<BotlBase>();
    private bool isProcessing = false;
    private Dictionary<BotlBase, bool> boltLockStatus = new Dictionary<BotlBase, bool>();

    public void Init()
    {
        // Auto-calculate lift height if not set
        if (uniformLiftHeight <= 0)
        {
            uniformLiftHeight = CalculateOptimalLiftHeight();
        }

        // Get bolts from LevelController if not assigned
        if (allBolts == null || allBolts.Count == 0)
        {
            var levelController = GamePlayerController.Instance?.gameContaint?.levelController;
            if (levelController != null)
            {
                allBolts = levelController.GetAllBolts();
            }
        }

        // Initialize bolt lock status
        boltLockStatus.Clear();
        foreach (var bolt in allBolts)
        {
            if (bolt != null) boltLockStatus[bolt] = false;
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
                        float relativeHeight = post.transform.position.y - bolt.transform.position.y;
                        if (relativeHeight > maxPostHeight)
                            maxPostHeight = relativeHeight;
                    }
                }
            }
        }
        return maxPostHeight + 0.8f;
    }

    // Main click handler - adds click to queue
    public void OnBoltClicked(BotlBase clickedBolt)
    {
        if (clickedBolt == null) return;

        clickQueue.Enqueue(clickedBolt);

        if (!isProcessing)
        {
            StartCoroutine(ProcessClickQueue());
        }
    }

    // Process clicks one by one
    private IEnumerator ProcessClickQueue()
    {
        isProcessing = true;

        while (clickQueue.Count > 0)
        {
            var clickedBolt = clickQueue.Dequeue();
            ProcessSingleClick(clickedBolt);
            yield return null;
        }

        isProcessing = false;
    }

    // Handle single bolt click
    private void ProcessSingleClick(BotlBase clickedBolt)
    {
        if (IsBoltLocked(clickedBolt)) return;

        if (currentLiftedScrew != null && currentSourceBolt != null)
        {
            if (clickedBolt == currentSourceBolt)
            {
                // Drop screw back to original position
                currentLiftedScrew.DropToOriginal(moveDuration, null);
                ResetCurrentScrew();
                return;
            }

            ScrewBase targetTopScrew = clickedBolt.GetTopScrew();

            if (targetTopScrew == null || currentLiftedScrew.id == targetTopScrew.id)
            {
                // Move screw to target bolt
                HandleScrewMovement(clickedBolt);
            }
            else
            {
                // Different color: drop old, lift new
                currentLiftedScrew.DropToOriginal(moveDuration, null);
                ResetCurrentScrew();
                HandleLiftScrew(clickedBolt);
            }
        }
        else
        {
            // First click: lift screw from bolt
            HandleLiftScrew(clickedBolt);
        }

        UpdateBoltLockStatus();
    }

    // Handle screw movement between bolts
    private void HandleScrewMovement(BotlBase targetBolt)
    {
        var sortScrew = GamePlayerController.Instance?.gameContaint?.sortScrew;
        if (sortScrew != null)
        {
            sortScrew.HandleScrewMovement(currentLiftedScrew, currentSourceBolt, targetBolt);
            ResetCurrentScrew();
        }
    }

    // Lift top screw from bolt
    private void HandleLiftScrew(BotlBase sourceBolt)
    {
        if (sourceBolt.screwBases.Count > 0)
        {
            ScrewBase topScrew = sourceBolt.GetTopScrew();
            if (topScrew != null)
            {
                currentLiftedScrew = topScrew;
                currentSourceBolt = sourceBolt;
                topScrew.LiftUp(uniformLiftHeight, liftDuration, null);
            }
        }
    }

    // Check if bolt is locked (completed)
    private bool IsBoltLocked(BotlBase bolt)
    {
        return boltLockStatus.ContainsKey(bolt) && boltLockStatus[bolt];
    }

    // Update bolt lock status based on completion
    private void UpdateBoltLockStatus()
    {
        foreach (var bolt in allBolts)
        {
            if (bolt != null)
            {
                boltLockStatus[bolt] = IsBoltComplete(bolt);
            }
        }
    }

    // Check if bolt has 5 screws of same color
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

    // Reset lifted screw state
    private void ResetCurrentScrew()
    {
        currentLiftedScrew = null;
        currentSourceBolt = null;
    }

    // Check if all bolts are completed
    public bool IsGameComplete()
    {
        if (allBolts == null || allBolts.Count == 0) return false;

        int completedBolts = 0;
        int totalBoltsWithScrews = 0;

        foreach (var bolt in allBolts)
        {
            if (bolt?.screwBases != null && bolt.screwBases.Count > 0)
            {
                totalBoltsWithScrews++;
                if (IsBoltComplete(bolt))
                    completedBolts++;
            }
        }

        return totalBoltsWithScrews > 0 && completedBolts == totalBoltsWithScrews;
    }

    // Public utility methods
    public void SetLiftedScrew(ScrewBase screw, BotlBase sourceBolt)
    {
        currentLiftedScrew = screw;
        currentSourceBolt = sourceBolt;
    }

    public void ForceResetState()
    {
        ResetCurrentScrew();
        clickQueue.Clear();
        isProcessing = false;
    }

    public bool HasLiftedScrew() => currentLiftedScrew != null && currentSourceBolt != null;
    public ScrewBase GetCurrentLiftedScrew() => currentLiftedScrew;
    public BotlBase GetCurrentSourceBolt() => currentSourceBolt;
}