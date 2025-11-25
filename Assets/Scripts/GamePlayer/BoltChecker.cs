using UnityEngine;
using DG.Tweening;

public class BoltChecker : MonoBehaviour
{
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
            Debug.Log($"Bolt {bolt.name} hoàn thành với {bolt.screwBases.Count} ốc loại {firstId}!");

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
}