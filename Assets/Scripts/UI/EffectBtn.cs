using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using DG.Tweening;

public class EffectBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Cài đặt hiệu ứng")]
    public float scalePress = 0.8f; 
    public float duration = 0.1f;   

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * scalePress, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        transform.DOKill();

        transform.DOScale(originalScale, duration).SetEase(Ease.OutBack); // OutBack tạo cảm giác nảy nhẹ
    }
    
}