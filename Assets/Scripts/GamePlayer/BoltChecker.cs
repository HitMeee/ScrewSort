using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoltChecker : MonoBehaviour
{
    // ✅ KIỂM TRA BOLT CÓ THỂ TƯƠNG TÁC KHÔNG
    public bool CanInteractWithBolt(BotlBase bolt)
    {
        if (bolt == null)
        {
            Debug.Log("❌ CanInteractWithBolt: bolt null");
            return false;
        }

        // ✅ LOGIC MỚI: Phải đúng 5 screw cùng màu mới khóa
        if (bolt.screwBases.Count >= 5)
        {
            // Kiểm tra tất cả 5 screw có cùng màu không
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
                return false; // Khóa vì đã hoàn thành
            }
            else
            {
                Debug.Log($"⚠️ Bolt {bolt.name} đầy (5/5 screw) nhưng KHÁC MÀU - vẫn có thể tương tác");
                return true; // Vẫn có thể tương tác vì chưa hoàn thành
            }
        }

        // Có thể tương tác
        return true;
    }

    // ✅ KIỂM TRA SAU MỖI LẦN DI CHUYỂN
    public void CheckAfterMove(BotlBase source, BotlBase target)
    {
        Debug.Log("🔍 === CHECKING AFTER MOVE ===");

        // Kiểm tra hoàn thành từng bolt
        IsBoltComplete(source);
        IsBoltComplete(target);

        // ✅ KIỂM TRA HOÀN THÀNH GAME (tất cả bolt đều 5 screw cùng màu)
        CheckGameComplete();
    }

    // Kiểm tra bolt có hoàn thành không
    public void IsBoltComplete(BotlBase bolt)
    {
        if (bolt == null || bolt.screwBases == null)
        {
            Debug.Log("BoltChecker: bolt hoặc screwBases null");
            return;
        }

        // Bolt trống → chưa hoàn thành
        if (bolt.screwBases.Count == 0)
        {
            Debug.Log($"Bolt {bolt.name} trống - chưa hoàn thành");
            return;
        }

        // Kiểm tra tất cả ốc cùng loại
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

        // Nếu tất cả ốc cùng loại và đủ số lượng → hoàn thành
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

            // Hiệu ứng hoàn thành
            foreach (var screw in bolt.screwBases)
            {
                if (screw != null)
                {
                    screw.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
                }
            }

            // Gọi event hoàn thành bolt
            bolt.OnComplete?.Invoke();
        }
        else
        {
            Debug.Log($"Bolt {bolt.name} chưa hoàn thành - AllSame: {allSameType}, Count: {bolt.screwBases.Count}");
        }
    }

    // ✅ KIỂM TRA HOÀN THÀNH GAME - TẤT CẢ BOLT ĐỀU 5 SCREW CÙNG MÀU
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
                    // Kiểm tra bolt có đúng 5 screw cùng màu không
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

            // ✅ NÚT CHÍNH: Tất cả bolt đều 5 screw cùng màu → HOÀN THÀNH GAME
            if (allBoltsComplete && totalBolts > 0)
            {
                Debug.Log("🏆🎊 HOÀN THÀNH GAME! TẤT CẢ BOLT ĐỀU CÓ 5/5 SCREW CÙNG MÀU! 🎊🏆");
            }
        }
    }

    // ✅ KIỂM TRA HOÀN THÀNH GAME (public method cho SortScrew gọi)
    public bool IsGameComplete(List<BotlBase> allBolts)
    {
        if (allBolts == null || allBolts.Count == 0) return false;

        // Kiểm tra tất cả bolt đều có đúng 5 screw cùng màu
        foreach (var bolt in allBolts)
        {
            if (bolt?.screwBases == null || bolt.screwBases.Count != 5)
            {
                return false; // Bolt chưa đủ 5 screw
            }

            // Kiểm tra 5 screw có cùng màu không
            int firstId = bolt.screwBases[0].id;
            foreach (var screw in bolt.screwBases)
            {
                if (screw == null || screw.id != firstId)
                {
                    return false; // Có screw khác màu
                }
            }
        }

        Debug.Log("🏆 GAME HOÀN THÀNH - TẤT CẢ BOLT ĐỀU CÓ 5/5 SCREW CÙNG MÀU!");
        return true;
    }
}