using System.Collections.Generic;
using UnityEngine;

public class BoltLogicManager : MonoBehaviour
{
    public List<BotlBase> allBolts;
    public float moveDuration = 0.3f;

    [Header("Lift Settings - Đảm bảo độ cao đồng nhất")]
    [Tooltip("Độ cao nâng screw từ base của bolt (đồng nhất cho tất cả)")]
    public float uniformLiftHeight = 1.5f;

    [Tooltip("Thời gian animation nâng lên")]
    public float liftDuration = 0.4f;

    private ScrewBase currentLiftedScrew;
    private BotlBase currentSourceBolt;

    public void Init()
    {
        // Auto-calculate optimal lift height nếu chưa set
        if (uniformLiftHeight <= 0)
        {
            uniformLiftHeight = CalculateOptimalLiftHeight();
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
                        float postY = post.transform.position.y;
                        float boltY = bolt.transform.position.y;
                        float relativeHeight = postY - boltY;

                        if (relativeHeight > maxPostHeight)
                        {
                            maxPostHeight = relativeHeight;
                        }
                    }
                }
            }
        }

        return maxPostHeight + 0.8f;
    }

    public void OnBoltClicked(BotlBase clickedBolt)
    {
        if (clickedBolt == null) return;

        // ✅ FIX: LẤY BOLTCHECKER ĐỂ KIỂM TRA
        var boltChecker = GamePlayerController.Instance?.gameContaint?.sortScrew?.checker;

        if (currentLiftedScrew != null && currentSourceBolt != null)
        {
            // ✅ FIX: KIỂM TRA CÓ THỂ TƯƠNG TÁC VỚI BOLT ĐÍCH KHÔNG
            if (boltChecker != null && !boltChecker.CanInteractWithBolt(clickedBolt))
            {
                Debug.Log($"❌ Không thể tương tác với bolt {clickedBolt.name} - đã bị khóa!");
                return; // KHÓA - không cho tương tác
            }

            // Có screw đang lift → xử lý di chuyển
            if (GamePlayerController.Instance?.gameContaint?.sortScrew != null)
            {
                GamePlayerController.Instance.gameContaint.sortScrew.HandleScrewMovement(
                    currentLiftedScrew, currentSourceBolt, clickedBolt, () =>
                    {
                        ResetCurrentScrew();
                    });
            }
        }
        else
        {
            // ✅ FIX: KIỂM TRA CÓ THỂ NÂNG SCREW TỪ BOLT NÀY KHÔNG
            if (boltChecker != null && !boltChecker.CanInteractWithBolt(clickedBolt))
            {
                Debug.Log($"❌ Không thể nâng screw từ bolt {clickedBolt.name} - đã bị khóa!");
                return; // KHÓA - không cho nâng screw
            }

            // Không có screw nào lift → nâng screw từ bolt được click
            if (clickedBolt.screwBases.Count > 0)
            {
                ScrewBase topScrew = clickedBolt.GetTopScrew();
                if (topScrew != null)
                {
                    LiftScrew(topScrew, clickedBolt);
                }
            }
        }
    }

    void LiftScrew(ScrewBase screw, BotlBase sourceBolt)
    {
        screw.LiftUp(uniformLiftHeight, liftDuration, () =>
        {
            currentLiftedScrew = screw;
            currentSourceBolt = sourceBolt;
        });
    }

    public void LiftScrewFromExternal(ScrewBase screw, BotlBase sourceBolt)
    {
        LiftScrew(screw, sourceBolt);
    }

    void ResetCurrentScrew()
    {
        currentLiftedScrew = null;
        currentSourceBolt = null;
    }

    public bool IsGameComplete()
    {
        if (GamePlayerController.Instance?.gameContaint?.sortScrew != null)
        {
            return GamePlayerController.Instance.gameContaint.sortScrew.IsGameComplete(allBolts);
        }
        return false;
    }

    [ContextMenu("Test Uniform Lift Height")]
    public void TestUniformLiftHeight()
    {
        Debug.Log($"Current uniformLiftHeight: {uniformLiftHeight}");
        Debug.Log($"Calculated optimal: {CalculateOptimalLiftHeight()}");

        if (allBolts.Count > 0 && allBolts[0] != null)
        {
            var testScrew = allBolts[0].GetTopScrew();
            if (testScrew != null)
            {
                testScrew.LiftUp(uniformLiftHeight, liftDuration, () =>
                {
                    Debug.Log("Test lift completed");
                });
            }
        }
    }
}