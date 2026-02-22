using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private Image fadeImage; // Màn hình đen phủ lên
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Đảm bảo fade image trong suốt khi bắt đầu
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// Chuyển scene với hiệu ứng fade
    /// </summary>
    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // ✅ Fade TO BLACK (tối dần)
        if (fadeImage != null)
        {
            fadeImage.DOFade(1f, fadeDuration).SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(fadeDuration);

        // ✅ Load scene
        SceneManager.LoadScene(sceneName);

        yield return new WaitForSecondsRealtime(0.1f);

        // ✅ Fade FROM BLACK (sáng dần)
        if (fadeImage != null)
        {
            fadeImage.DOFade(0f, fadeDuration).SetUpdate(true);
        }
    }

    /// <summary>
    /// Fade TO BLACK (để dùng riêng nếu cần)
    /// </summary>
    public void FadeToBlack(System.Action onComplete = null)
    {
        if (fadeImage != null)
        {
            fadeImage.DOFade(1f, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }

    /// <summary>
    /// Fade FROM BLACK (để dùng riêng nếu cần)
    /// </summary>
    public void FadeFromBlack()
    {
        if (fadeImage != null)
        {
            fadeImage.DOFade(0f, fadeDuration).SetUpdate(true);
        }
    }
}