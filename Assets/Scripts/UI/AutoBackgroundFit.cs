
using UnityEditor.Playables;
using UnityEngine;

namespace CoreGame
{
  [RequireComponent(typeof(SpriteRenderer))]
  public class AutoBackgroundFit : MonoBehaviour
  {
    void Start()
    {
      FitBackground();
    }

    void FitBackground()
    {
      SpriteRenderer sr = GetComponent<SpriteRenderer>();

      if (sr == null || sr.sprite == null)
      {
        // Utility.LogWarning("AutoBackgroundFit: Missing SpriteRenderer or Sprite.");
        return;
      }

      var worldScreenHeight = Camera.main.orthographicSize * 2f;
      var worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

      Vector3 spriteSize = sr.sprite.bounds.size;

      var scaleX = worldScreenWidth / spriteSize.x;
      var scaleY = worldScreenHeight / spriteSize.y;

      var finalScale = Mathf.Max(scaleX, scaleY); // fit full screen, giữ nguyên tỷ lệ

      transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
  }
}
