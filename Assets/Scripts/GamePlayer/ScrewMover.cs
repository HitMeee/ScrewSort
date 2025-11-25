using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScrewMover : MonoBehaviour
{
    public float moveDuration = 0.3f;
    public float delayBetweenScrews = 0.1f;

    // Thả screw về vị trí gốc
    public void DropBack(ScrewBase screw, Action onComplete = null)
    {
        if (screw == null)
        {
            onComplete?.Invoke();
            return;
        }
        screw.DropToOriginal(moveDuration, onComplete);
    }

    // Di chuyển 1 screw từ source sang target
    public void MoveSingle(ScrewBase screw, BotlBase source, BotlBase target, Vector3 targetPos, Action onComplete = null)
    {
        if (screw == null || source == null || target == null)
        {
            onComplete?.Invoke();
            return;
        }

        // ✅ FIX: Cập nhật data trước khi di chuyển
        source.RemoveScrew(screw);
        target.AddScrew(screw);
        screw.transform.SetParent(target.transform);

        // ✅ FIX: Tính lại vị trí sau khi cập nhật data
        Vector3 correctPos = GetCorrectPosition(target, target.screwBases.Count - 1);

        screw.MoveTo(correctPos, moveDuration, () =>
        {
            screw.originalPosition = correctPos; // Cập nhật vị trí gốc mới
            onComplete?.Invoke();
        });
    }

    // Di chuyển batch screws từ source sang target
    public IEnumerator MoveBatch(List<ScrewBase> screws, BotlBase source, BotlBase target,
                               Func<BotlBase, Vector3> getTopPos, Action onComplete = null)
    {
        if (screws == null || source == null || target == null || getTopPos == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        // ✅ FIX: Di chuyển tuần tự và đồng bộ
        for (int i = 0; i < screws.Count; i++)
        {
            var screw = screws[i];
            if (screw != null)
            {
                // Cập nhật data
                source.RemoveScrew(screw);
                target.AddScrew(screw);
                screw.transform.SetParent(target.transform);

                // Tính vị trí chính xác cho screw này
                Vector3 pos = GetCorrectPosition(target, target.screwBases.Count - 1);

                // Di chuyển và đợi hoàn thành
                bool moveComplete = false;
                screw.MoveTo(pos, moveDuration, () =>
                {
                    screw.originalPosition = pos; // Cập nhật vị trí gốc
                    moveComplete = true;
                });

                // Đợi di chuyển hoàn thành
                yield return new WaitUntil(() => moveComplete);

                // Delay nhỏ giữa các screw
                if (i < screws.Count - 1)
                    yield return new WaitForSeconds(delayBetweenScrews);
            }
        }

        onComplete?.Invoke();
    }

    // ✅ FIX: Hàm tính vị trí chính xác
    private Vector3 GetCorrectPosition(BotlBase bolt, int screwIndex)
    {
        if (bolt == null || bolt.postBolts == null) return Vector3.zero;

        // Sử dụng postBolt nếu có
        if (screwIndex >= 0 && screwIndex < bolt.postBolts.Count && bolt.postBolts[screwIndex] != null)
        {
            return bolt.postBolts[screwIndex].transform.position;
        }

        // Fallback: tính toán dựa trên bolt base
        return bolt.transform.position + Vector3.up * (screwIndex * 0.3f + 0.2f);
    }
}