using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoltChecker : MonoBehaviour
{
    /// ✅ GỘP TỪ SCREWRULES: Kiểm tra bolt có thể nhận screw không
    public bool CanAcceptScrew(BotlBase target, ScrewBase screw)
    {
        if (target == null || screw == null) return false;

        // Bolt đầy (5 screws) → không nhận thêm
        if (target.screwBases.Count >= 5) return false;

        // Bolt trống → có thể nhận
        if (target.screwBases.Count == 0) return true;

        // Kiểm tra screw trên cùng
        ScrewBase topScrew = target.GetTopScrew();
        if (topScrew != null && topScrew.id == screw.id)
        {
            // Cùng màu và còn slot → có thể nhận
            return target.SlotsAvailable() > 0;
        }

        return false;
    }

    ///  Kiểm tra có thể swap screws không
    public bool CanSwapScrews(BotlBase target)
    {
        return target?.screwBases?.Count > 0;
    }
    /// Kiểm tra bolt có thể tương tác không (khóa khi hoàn thành)
    public bool CanInteractWithBolt(BotlBase bolt)
    {
        if (bolt == null) return false;

        // Bolt hoàn thành (5 screws cùng màu) → khóa tương tác
        if (bolt.screwBases.Count >= 5)
        {
            return !IsAllSameColor(bolt);
        }

        return true;
    }

    /// Kiểm tra sau mỗi lần di chuyển screw
    public void CheckAfterMove(BotlBase source, BotlBase target)
    {
        Debug.Log("🔍 === CHECKING AFTER MOVE ===");

        CheckBoltCompletion(source);
        CheckBoltCompletion(target);
        CheckGameCompletion();
    }

    /// ✅ ĐƠN GIẢN HÓA: Chỉ có âm thanh khi HOÀN THÀNH đúng 5 screws
    public void CheckBoltCompletion(BotlBase bolt)
    {
        if (bolt?.screwBases == null || bolt.screwBases.Count == 0) return;

        // ✅ CHỈ KIỂM TRA KHI CÓ ĐỦ 5 SCREWS VÀ CÙNG MÀU
        if (bolt.screwBases.Count == 5 && IsAllSameColor(bolt))
        {
            // Completion effects - hiệu ứng visual
            foreach (var screw in bolt.screwBases)
            {
                screw?.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
            }

            // Gọi event completion
            bolt.OnComplete?.Invoke();

            // ✅ PHÁT ÂM THANH CHỈ KHI HOÀN THÀNH 5 SCREWS
            Debug.Log($"🎉 BOLT COMPLETED! {bolt.name} has 5 same-color screws!");

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayBoltComplete();
            }
        }
        // ✅ BỎ LOGIC "GOOD COMPLETION" - Không còn âm thanh cho 3-4 screws
    }

    /// Kiểm tra tất cả bolts hoàn thành (game finish)
    public bool IsGameComplete(List<BotlBase> allBolts)
    {
        if (allBolts == null || allBolts.Count == 0) return false;

        foreach (var bolt in allBolts)
        {
            if (bolt?.screwBases == null || bolt.screwBases.Count != 5)
                return false;

            if (!IsAllSameColor(bolt))
                return false;
        }

        Debug.Log("🏆 GAME COMPLETED - ALL BOLTS HAVE 5 SAME-COLOR SCREWS!");
        return true;
    }
    /// Kiểm tra game completion (internal)
    private void CheckGameCompletion()
    {
        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager?.allBolts != null)
        {
            if (IsGameComplete(boltManager.allBolts))
            {
                SoundManager.Instance?.PlayLevelComplete();
                GamePlayerController.Instance?.gameScene?.OnLevelComplete();
            }
        }
    }

    /// ✅ HELPER: Kiểm tra tất cả screws trong bolt cùng màu
    private bool IsAllSameColor(BotlBase bolt)
    {
        if (bolt?.screwBases == null || bolt.screwBases.Count == 0) return false;

        int firstId = bolt.screwBases[0].id;
        foreach (var screw in bolt.screwBases)
        {
            if (screw?.id != firstId) return false;
        }
        return true;
    }

    /// ✅ THÊM: Kiểm tra bolt có đầy không
    public bool IsBoltFull(BotlBase bolt)
    {
        return bolt?.screwBases?.Count >= 5;
    }
    /// ✅ THÊM: Kiểm tra bolt có trống không
    public bool IsBoltEmpty(BotlBase bolt)
    {
        return bolt?.screwBases?.Count == 0;
    }
    /// ✅ THÊM: Lấy thông tin validation cho debug
    public string GetValidationInfo(BotlBase source, BotlBase target, ScrewBase screw)
    {
        if (source == null || target == null || screw == null)
            return "❌ Invalid parameters";

        bool canAccept = CanAcceptScrew(target, screw);
        bool canSwap = CanSwapScrews(target);
        bool canInteract = CanInteractWithBolt(target);

        return $"Target {target.name}: Accept={canAccept}, Swap={canSwap}, Interact={canInteract}";
    }
}