using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoltLogicManager : MonoBehaviour
{
    public float moveDuration = 0.1f; // Giảm từ 0.2f
    public float uniformLiftHeight = 1.5f;
    public float liftDuration = 0.2f; // Giảm từ 0.4f để nhanh hơn
    public int maxQueueSize = 5; // ✅ GIỚI HẠN số lượng click trong hàng đợi
    public List<BotlBase> allBolts;

    // State
    private ScrewBase currentLiftedScrew;
    private BotlBase currentSourceBolt;
    private Queue<BotlBase> clickQueue = new Queue<BotlBase>();
    private bool isProcessing = false;
    private bool isAnimating = false; // Track animation state để block input
    private Dictionary<BotlBase, bool> boltLockStatus = new Dictionary<BotlBase, bool>();
    private HashSet<BotlBase> animatingBolts = new HashSet<BotlBase>(); // ✅ Track các bolt đang animate

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
        // ✅ BLOCK CLICK VÀO BOLT ĐANG CÓ ANIMATION
        if (animatingBolts.Contains(clickedBolt))
        {
            Debug.Log($"⏸️ Bolt {clickedBolt.name} đang có ốc di chuyển, vui lòng đợi! [animatingBolts count: {animatingBolts.Count}]");
            return;
        }
        
        // ✅ KIỂM TRA: Bolt hợp lệ
        if (clickedBolt == null || IsBoltLocked(clickedBolt) || clickQueue.Contains(clickedBolt))
            return;
            
        // ✅ GIỚI HẠN: Chỉ chấp nhận tối đa maxQueueSize clicks
        if (clickQueue.Count >= maxQueueSize)
        {
            Debug.Log($"❌ Queue đầy! Đang xử lý {clickQueue.Count} clicks. Vui lòng đợi...");
            return;
        }

        Debug.Log($"✅ Thêm bolt {clickedBolt.name} vào queue. Queue size: {clickQueue.Count + 1}");
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
                // ✅ XỬ LÝ LOGIC của click
                yield return ProcessBoltClick(bolt);
                
                // ✅ QUAN TRỌNG: Đợi animation hoàn tất (isAnimating = false) trước khi xử lý click tiếp
                yield return new WaitUntil(() => !isAnimating);
                
                // ✅ Delay nhỏ giữa các action để mượt mà
                yield return new WaitForSeconds(0.03f);
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

        // ✅ KHÓA bolt khi bắt đầu lift
        isAnimating = true;
        animatingBolts.Add(sourceBolt);
        Debug.Log($"🔒 KHÓA Bolt {sourceBolt.name} khi lift. AnimatingBolts: {animatingBolts.Count}");
        
        yield return WaitForAnimation(topScrew.LiftUp, uniformLiftHeight, liftDuration);
        
        // ✅ MỞ KHÓA bolt sau khi lift xong (nhưng vẫn giữ isAnimating cho các action tiếp theo)
        animatingBolts.Remove(sourceBolt);
        Debug.Log($"🔓 MỞ KHÓA Bolt {sourceBolt.name} sau lift. AnimatingBolts: {animatingBolts.Count}");
        isAnimating = false;
    }

    private IEnumerator DropScrew()
    {
        BotlBase sourceBolt = currentSourceBolt;
        
        // ✅ KHÓA source bolt khi drop
        isAnimating = true;
        if (sourceBolt != null)
        {
            animatingBolts.Add(sourceBolt);
            Debug.Log($"🔒 KHÓA Bolt {sourceBolt.name} khi drop. AnimatingBolts: {animatingBolts.Count}");
        }
        
        yield return WaitForAnimation(currentLiftedScrew.DropToOriginal, moveDuration);
        
        // ✅ MỞ KHÓA bolt sau khi drop xong
        if (sourceBolt != null)
        {
            animatingBolts.Remove(sourceBolt);
            Debug.Log($"🔓 MỞ KHÓA Bolt {sourceBolt.name} sau drop. AnimatingBolts: {animatingBolts.Count}");
        }
        isAnimating = false;
        ResetLiftedScrew();
    }

    private IEnumerator MoveScrewToBolt(BotlBase targetBolt)
    {
        var sortScrew = GamePlayerController.Instance?.gameContaint?.sortScrew;
        if (sortScrew != null)
        {
            BotlBase sourceBolt = currentSourceBolt;
            
            // ✅ Khóa CÁ HAI bolt (source và target) khi bắt đầu animation
            isAnimating = true;
            if (sourceBolt != null)
            {
                animatingBolts.Add(sourceBolt);
                Debug.Log($"🔒 KHÓA Source Bolt {sourceBolt.name} khi move. AnimatingBolts: {animatingBolts.Count}");
            }
            if (targetBolt != null)
            {
                animatingBolts.Add(targetBolt);
                Debug.Log($"🔒 KHÓA Target Bolt {targetBolt.name} khi move. AnimatingBolts: {animatingBolts.Count}");
            }
            
            // ✅ ĐỢI CALLBACK THẬT SỰ thay vì ước lượng thời gian
            bool moveCompleted = false;
            sortScrew.HandleScrewMovement(currentLiftedScrew, currentSourceBolt, targetBolt, () => {
                moveCompleted = true;
                Debug.Log("✅ Animation di chuyển hoàn thành!");
            });
            
            yield return new WaitUntil(() => moveCompleted);
            
            // ✅ MỞ KHÓA CẢ 2 BOLT sau khi di chuyển xong
            if (sourceBolt != null)
            {
                animatingBolts.Remove(sourceBolt);
                Debug.Log($"🔓 MỞ KHÓA Source Bolt {sourceBolt.name} sau move. AnimatingBolts: {animatingBolts.Count}");
            }
            if (targetBolt != null)
            {
                animatingBolts.Remove(targetBolt);
                Debug.Log($"🔓 MỞ KHÓA Target Bolt {targetBolt.name} sau move. AnimatingBolts: {animatingBolts.Count}");
            }
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
            
            // ✅ KHÓA target bolt khi lift
            isAnimating = true;
            animatingBolts.Add(targetBolt);
            
            yield return WaitForAnimation(targetTopScrew.LiftUp, uniformLiftHeight, liftDuration);
            
            // ✅ MỞ KHÓA target bolt sau khi lift xong
            animatingBolts.Remove(targetBolt);
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
        animatingBolts.Clear(); // ✅ Clear tất cả bolt đang bị khóa
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