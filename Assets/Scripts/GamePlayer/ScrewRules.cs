using UnityEngine;

public class ScrewRules : MonoBehaviour
{
    // Kiểm tra bolt target có thể nhận screw không
    public bool CanAccept(BotlBase target, ScrewBase screw)
    {
        if (target == null || screw == null) return false;

        // ✅ FIX: Chỉ kiểm tra đầy thật sự (5 screw)
        if (target.screwBases.Count >= 5) return false;

        // Bolt trống → có thể nhận
        if (target.screwBases == null || target.screwBases.Count == 0) return true;

        // ✅ FIX: Kiểm tra có thể nhận thêm screw cùng màu
        ScrewBase topScrew = target.GetTopScrew();
        if (topScrew != null && topScrew.id == screw.id)
        {
            // Nếu đã hoàn thành (3+ cùng màu) nhưng vẫn còn slot thì vẫn nhận thêm
            return target.SlotsAvailable() > 0;
        }

        return false;
    }

    // Kiểm tra có thể swap không
    public bool CanSwap(BotlBase target)
    {
        if (target == null || target.screwBases == null) return false;
        return target.screwBases.Count > 0;
    }
}