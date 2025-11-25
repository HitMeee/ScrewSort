using System.Collections.Generic;
using UnityEngine;

public class ScrewBatchMover : MonoBehaviour
{
    // Tính số ốc có thể di chuyển từ source sang target
    public int GetMovableCount(ScrewRules rules, BotlBase source, BotlBase target, ScrewBase liftedScrew)
    {
        if (source == null || target == null || liftedScrew == null || rules == null)
            return 0;

        int consecutiveScrews = CountConsecutiveScrewsOfSameColor(source, liftedScrew.id);
        int availableSlots = target.SlotsAvailable();

        return Mathf.Min(consecutiveScrews, availableSlots);
    }

    // Đếm số ốc cùng loại liên tiếp từ trên xuống
    public int CountConsecutiveScrewsOfSameColor(BotlBase bolt, int colorId)
    {
        if (bolt == null || bolt.screwBases == null || bolt.screwBases.Count == 0)
            return 0;

        int count = 0;
        for (int i = bolt.screwBases.Count - 1; i >= 0; i--)
        {
            if (bolt.screwBases[i] != null && bolt.screwBases[i].id == colorId)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    // Lấy danh sách ốc từ trên xuống để di chuyển
    public List<ScrewBase> GetBatch(BotlBase source, int count)
    {
        List<ScrewBase> batch = new List<ScrewBase>();

        if (source == null || source.screwBases == null || count <= 0)
            return batch;

        for (int i = 0; i < count; i++)
        {
            int index = source.screwBases.Count - 1 - i;
            if (index >= 0 && source.screwBases[index] != null)
            {
                batch.Add(source.screwBases[index]);
            }
        }

        return batch;
    }
}