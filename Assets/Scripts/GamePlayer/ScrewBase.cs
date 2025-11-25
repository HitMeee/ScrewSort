using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScrewBase : MonoBehaviour
{
    public int id;
    public List<Material> materials;
    public MeshRenderer meshRenderer;

    [HideInInspector] public bool isLifted = false;
    [HideInInspector] public Vector3 originalPosition;

    public void Init(int id)
    {
        this.id = id;
        originalPosition = transform.position;
        if (meshRenderer != null)
        {
            Material mat = GetMaterialById(id);
            if (mat != null)
                meshRenderer.material = mat;
        }
    }

    // ✅ FIX: Nâng screw lên độ cao đồng nhất và mượt mà
    public void LiftUp(float upOffset, float duration, System.Action onComplete = null)
    {
        if (originalPosition == Vector3.zero)
            originalPosition = transform.position;

        // Kill tween cũ để tránh conflict
        transform.DOKill();

        // ✅ Tính độ cao đồng nhất dựa trên bolt parent
        Vector3 uniformPos = CalculateUniformLiftPosition(upOffset);

        // ✅ Animation mượt mà với ease curve
        transform.DOMove(uniformPos, duration)
            .SetEase(Ease.OutBack, 1.2f) // Ease mượt mà với hiệu ứng bounce nhẹ
            .OnComplete(() =>
            {
                isLifted = true;
                onComplete?.Invoke();
            });
    }

    // ✅ NEW: Tính vị trí nâng lên đồng nhất
    private Vector3 CalculateUniformLiftPosition(float upOffset)
    {
        Vector3 uniformPos = originalPosition;

        // Tìm bolt parent để làm reference
        BotlBase parentBolt = GetComponentInParent<BotlBase>();
        if (parentBolt != null)
        {
            // Sử dụng vị trí bolt + offset cố định để tất cả screw có cùng độ cao
            float boltBaseY = parentBolt.transform.position.y;
            uniformPos.y = boltBaseY + upOffset;
        }
        else
        {
            // Fallback: dùng world Y + offset
            uniformPos.y = originalPosition.y + upOffset;
        }

        return uniformPos;
    }

    public void MoveTo(Vector3 targetPosition, float duration, System.Action onComplete = null)
    {
        transform.DOKill();

        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.InOutQuart) // Ease mượt mà cho di chuyển
            .OnComplete(() =>
            {
                originalPosition = targetPosition;
                isLifted = false;
                onComplete?.Invoke();
            });
    }

    public void DropToOriginal(float duration, System.Action onComplete = null)
    {
        transform.DOKill();

        transform.DOMove(originalPosition, duration)
            .SetEase(Ease.OutBounce) // Ease với bounce nhẹ khi thả xuống
            .OnComplete(() =>
            {
                isLifted = false;
                onComplete?.Invoke();
            });
    }

    Material GetMaterialById(int id)
    {
        if (materials != null && id >= 0 && id < materials.Count)
            return materials[id];
        return null;
    }
}