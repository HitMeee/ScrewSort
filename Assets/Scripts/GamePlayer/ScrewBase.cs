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

    public void LiftUp(float upOffset, float duration, System.Action onComplete = null)
    {
        if (originalPosition == Vector3.zero)
            originalPosition = transform.position;

        transform.DOKill();

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

    private Vector3 CalculateUniformLiftPosition(float upOffset)
    {
        Vector3 uniformPos = originalPosition;

        BotlBase parentBolt = GetComponentInParent<BotlBase>();
        if (parentBolt != null)
        {
            float boltBaseY = parentBolt.transform.position.y;
            uniformPos.y = boltBaseY + upOffset;
        }
        else
        {
            uniformPos.y = originalPosition.y + upOffset;
        }

        return uniformPos;
    }

    Material GetMaterialById(int id)
    {
        if (materials != null && id >= 0 && id < materials.Count)
            return materials[id];
        return null;
    }
}