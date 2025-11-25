using System.Collections.Generic;
using UnityEngine;

public class BotlBase : MonoBehaviour
{
    public List<PostBolt> postBolts;
    public ScrewBase screwPrefab;
    public List<ScrewBase> screwBases;
    public List<int> lsId;

    public System.Action OnComplete;

    public void Init(List<int> idLs)
    {
        screwBases = new List<ScrewBase>();
        lsId = new List<int>(idLs);

        for (int i = 0; i < postBolts.Count && i < lsId.Count; i++)
        {
            if (screwPrefab == null) return;

            var screw = Instantiate(screwPrefab);
            screw.transform.position = postBolts[i].transform.position;
            screw.transform.SetParent(this.transform);
            screw.Init(lsId[i]);
            screwBases.Add(screw);
        }
    }

    public ScrewBase GetTopScrew()
    {
        if (screwBases == null || screwBases.Count == 0) return null;
        return screwBases[screwBases.Count - 1];
    }

    // ✅ FIX: Sửa logic IsFull để không chặn di chuyển khi vẫn còn slot
    public bool IsFull()
    {
        if (screwBases == null) return false;

        // Chỉ đầy khi đạt tối đa 5 screw
        return screwBases.Count >= 5;
    }

    // ✅ NEW: Thêm method kiểm tra hoàn thành riêng
    public bool IsComplete()
    {
        if (screwBases == null || screwBases.Count < 3) return false;

        // Tất cả screw phải cùng màu
        int firstId = screwBases[0].id;
        foreach (var s in screwBases)
        {
            if (s == null || s.id != firstId) return false;
        }

        return true; // Hoàn thành khi có ít nhất 3 screw cùng màu
    }

    public int SlotsAvailable()
    {
        if (screwBases == null) return 5;
        return Mathf.Max(0, 5 - screwBases.Count);
    }

    public void AddScrew(ScrewBase screw)
    {
        if (screwBases == null) screwBases = new List<ScrewBase>();
        if (screw != null && !screwBases.Contains(screw))
        {
            screwBases.Add(screw);
            if (lsId != null) lsId.Add(screw.id);
        }
    }

    public void RemoveScrew(ScrewBase screw)
    {
        if (screwBases == null || screw == null) return;

        if (screwBases.Contains(screw))
        {
            screwBases.Remove(screw);
            if (lsId != null && lsId.Contains(screw.id))
            {
                lsId.Remove(screw.id);
            }
        }
    }
}