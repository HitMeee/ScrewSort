using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoltChecker : MonoBehaviour
{
    // Kiểm tra bolt có thể tương tác không (khóa khi 5 screw cùng màu)
    public bool CanInteractWithBolt(BotlBase bolt)
    {
        if (bolt == null)
        {
            Debug.Log("❌ CanInteractWithBolt: bolt null");
            return false;
        }

        if (bolt.screwBases.Count >= 5)
        {
            bool allSameColor = true;
            int firstId = bolt.screwBases[0].id;

            foreach (var screw in bolt.screwBases)
            {
                if (screw == null || screw.id != firstId)
                {
                    allSameColor = false;
                    break;
                }
            }

            if (allSameColor)
            {
                Debug.Log($"🔒 Bolt {bolt.name} HOÀN THÀNH (5/5 screw cùng màu {firstId}) - KHÓA, KHÔNG THỂ TƯƠNG TÁC!");
                return false;
            }
            else
            {
                Debug.Log($"⚠️ Bolt {bolt.name} đầy (5/5 screw) nhưng KHÁC MÀU - vẫn có thể tương tác");
                return true;
            }
        }

        return true;
    }

    // Kiểm tra sau mỗi lần di chuyển screw
    public void CheckAfterMove(BotlBase source, BotlBase target)
    {
        Debug.Log("🔍 === CHECKING AFTER MOVE ===");

        IsBoltComplete(source);
        IsBoltComplete(target);
        CheckGameComplete();
    }

    // Kiểm tra bolt có hoàn thành không và hiệu ứng
    public void IsBoltComplete(BotlBase bolt)
    {
        if (bolt == null || bolt.screwBases == null)
        {
            Debug.Log("BoltChecker: bolt hoặc screwBases null");
            return;
        }

        if (bolt.screwBases.Count == 0)
        {
            Debug.Log($"Bolt {bolt.name} trống - chưa hoàn thành");
            return;
        }

        int firstId = bolt.screwBases[0].id;
        bool allSameType = true;

        foreach (var screw in bolt.screwBases)
        {
            if (screw == null || screw.id != firstId)
            {
                allSameType = false;
                break;
            }
        }

        if (allSameType && bolt.screwBases.Count >= 3)
        {
            if (bolt.screwBases.Count == 5)
            {
                Debug.Log($"🎉 PERFECT! Bolt {bolt.name} hoàn thành TUYỆT ĐỐI với 5/5 ốc cùng màu {firstId}!");
            }
            else
            {
                Debug.Log($"✅ Bolt {bolt.name} hoàn thành với {bolt.screwBases.Count} ốc cùng màu {firstId}!");
            }

            foreach (var screw in bolt.screwBases)
            {
                if (screw != null)
                {
                    screw.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
                }
            }

            bolt.OnComplete?.Invoke();
        }
        else
        {
            Debug.Log($"Bolt {bolt.name} chưa hoàn thành - AllSame: {allSameType}, Count: {bolt.screwBases.Count}");
        }
    }

    // Kiểm tra tất cả bolt hoàn thành để kết thúc game
    private void CheckGameComplete()
    {
        var boltManager = GamePlayerController.Instance?.gameContaint?.boltLogicManager;
        if (boltManager?.allBolts != null)
        {
            bool allBoltsComplete = true;
            int completeBolts = 0;
            int totalBolts = boltManager.allBolts.Count;

            foreach (var bolt in boltManager.allBolts)
            {
                if (bolt?.screwBases != null)
                {
                    if (bolt.screwBases.Count == 5)
                    {
                        bool allSameColor = true;
                        int firstId = bolt.screwBases[0].id;

                        foreach (var screw in bolt.screwBases)
                        {
                            if (screw == null || screw.id != firstId)
                            {
                                allSameColor = false;
                                break;
                            }
                        }

                        if (allSameColor)
                        {
                            completeBolts++;
                        }
                        else
                        {
                            allBoltsComplete = false;
                        }
                    }
                    else
                    {
                        allBoltsComplete = false;
                    }
                }
                else
                {
                    allBoltsComplete = false;
                }
            }

            Debug.Log($"🔍 Kiểm tra game: {completeBolts}/{totalBolts} bolt hoàn thành (5/5 cùng màu)");

            if (allBoltsComplete && totalBolts > 0)
            {
                Debug.Log("🏆🎊 HOÀN THÀNH GAME! TẤT CẢ BOLT ĐỀU CÓ 5/5 SCREW CÙNG MÀU! 🎊🏆");
            }
        }
    }

    // Kiểm tra game hoàn thành (method public cho SortScrew gọi)
    public bool IsGameComplete(List<BotlBase> allBolts)
    {
        if (allBolts == null || allBolts.Count == 0) return false;

        foreach (var bolt in allBolts)
        {
            if (bolt?.screwBases == null || bolt.screwBases.Count != 5)
            {
                return false;
            }

            int firstId = bolt.screwBases[0].id;
            foreach (var screw in bolt.screwBases)
            {
                if (screw == null || screw.id != firstId)
                {
                    return false;
                }
            }
        }

        Debug.Log("🏆 GAME HOÀN THÀNH - TẤT CẢ BOLT ĐỀU CÓ 5/5 SCREW CÙNG MÀU!");
        return true;
    }
}