using UnityEngine;
using DG.Tweening;

public class ScrewRotate : MonoBehaviour
{
    [Header("🌀 Cài đặt xoay")]
    public float rotationSpeed = 360f; // Tốc độ xoay (độ/giây)

    private Tween rotationTween; // Lưu animation xoay hiện tại

    // 🌀 BẮT ĐẦU XOAY (gọi khi bắt đầu di chuyển)
    public void StartRotation(float duration)
    {
        // Dừng xoay cũ nếu có
        StopRotation();

        // Tính tổng độ xoay = tốc độ × thời gian
        float totalRotation = rotationSpeed * duration;

        // Bắt đầu xoay quanh trục Y
        rotationTween = transform.DORotate(
            new Vector3(0, totalRotation, 0),  // Xoay quanh trục Y
            duration,                          // Thời gian xoay
            RotateMode.LocalAxisAdd           // Cộng thêm vào rotation hiện tại
        ).SetEase(Ease.Linear);               // Xoay đều đặn

        Debug.Log($"🌀 Bắt đầu xoay ốc vít: {totalRotation}° trong {duration}s");
    }

    // 🛑 DỪNG XOAY (gọi khi hoàn thành di chuyển)
    public void StopRotation()
    {
        if (rotationTween != null)
        {
            rotationTween.Kill(); // Dừng animation
            rotationTween = null;
        }
    }

    // 🧹 TỰ ĐỘNG DỌN DẸP KHI DESTROY
    void OnDestroy()
    {
        StopRotation();
    }
}