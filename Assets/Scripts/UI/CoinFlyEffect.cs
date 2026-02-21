using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CoinFlyEffect : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private RectTransform coinSpawnPoint;
    [SerializeField] private RectTransform coinTargetPosition;
    [SerializeField] private int numberOfCoins = 10;
    
    [Header("Animation Settings")]
    [SerializeField] private float spreadRadius = 150f;
    [SerializeField] private float explosionDuration = 0.5f;
    [SerializeField] private float flyDuration = 0.8f;
    [SerializeField] private float delayBetweenCoins = 0.05f;
    [SerializeField] private Ease flyEase = Ease.InBack;

    public void PlayCoinFlyEffect()
    {      
        StartCoroutine(SpawnAndFlyCoins());
    }

    public float GetTotalAnimationTime()
    {
        return explosionDuration + flyDuration + (numberOfCoins * delayBetweenCoins) + 0.2f;
    }

    private IEnumerator SpawnAndFlyCoins()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        
        for (int i = 0; i < numberOfCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, canvas.transform);
            RectTransform coinRect = coin.GetComponent<RectTransform>();
            
            coinRect.position = coinSpawnPoint.position;
            coinRect.localScale = Vector3.zero;
            
            Vector3 randomOffset = Random.insideUnitCircle * spreadRadius;
            Vector3 explosionPos = coinSpawnPoint.position + randomOffset;
            
            // Animation
            coinRect.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            
            Sequence coinSequence = DOTween.Sequence();
            coinSequence.Append(coinRect.DOMove(explosionPos, explosionDuration).SetEase(Ease.OutQuad));
            coinSequence.Append(coinRect.DOMove(coinTargetPosition.position, flyDuration).SetEase(flyEase));
            coinSequence.Join(coinRect.DOScale(0.5f, flyDuration * 0.5f).SetDelay(flyDuration * 0.5f));
            
            coinRect.DORotate(new Vector3(0, 0, 360), flyDuration + explosionDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear);
            
            coinSequence.OnComplete(() => Destroy(coin));
            
            yield return new WaitForSeconds(delayBetweenCoins);
        }
    }
}