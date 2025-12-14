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

    // ✅ THÊM: Độ cao nâng đồng nhất cho tất cả screws
    [Header("Lift Settings")]
    public static float UNIFORM_LIFT_HEIGHT = 2.0f; // Độ cao cố định từ mặt đất

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

    public void LiftUp(float upOffset, float duration, System.Action onComplete = null)
    {
        if (originalPosition == Vector3.zero)
            originalPosition = transform.position;

        transform.DOKill();

        // ✅ SỬA: Sử dụng độ cao đồng nhất
        Vector3 uniformPos = CalculateUniformLiftPosition(upOffset);

        transform.DOMove(uniformPos, duration)
            .SetEase(Ease.OutBack, 1.2f)
            .OnComplete(() =>
            {
                isLifted = true;
                onComplete?.Invoke();
            });
    }

    public void MoveTo(Vector3 targetPosition, float duration, System.Action onComplete = null)
    {
        transform.DOKill();

        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.InOutQuart)
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
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                isLifted = false;
                onComplete?.Invoke();
            });
    }

    // ✅ SỬA: Tính toán độ cao nâng đồng nhất
    private Vector3 CalculateUniformLiftPosition(float upOffset)
    {
        Vector3 uniformPos = originalPosition;

        // ✅ CÁCH 1: Sử dụng độ cao cố định từ world origin
        uniformPos.y = UNIFORM_LIFT_HEIGHT;

        // ✅ CÁCH 2: Hoặc nếu muốn relative với upOffset
        // uniformPos.y = UNIFORM_LIFT_HEIGHT + upOffset;

        return uniformPos;
    }

    // ✅ THÊM: Phương thức để set độ cao đồng nhất
    public static void SetUniformLiftHeight(float height)
    {
        UNIFORM_LIFT_HEIGHT = height;
    }

    Material GetMaterialById(int id)
    {
        if (materials != null && id >= 0 && id < materials.Count)
            return materials[id];
        return null;
    }
}