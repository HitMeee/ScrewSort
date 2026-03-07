using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    [Header("Ad Unit IDs")]
    [SerializeField] private string bannerAdUnitId = "ca-app-pub-3940256099942544/6300978111"; // Test ID
    [SerializeField] private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Test ID
    [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test ID

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Initialize the Google Mobile Ads SDK
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            LoadBannerAd();
            LoadInterstitialAd();
            LoadRewardedAd();
        });
    }

    #region Banner Ads

    public void LoadBannerAd()
    {
        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at top of the screen (không bị UI đè)
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Top);

        // Listen to events
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner ad loaded.");
        };
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner ad failed to load: " + error.GetMessage());
        };

        // Create an empty ad request
        AdRequest adRequest = new AdRequest();

        // Load the banner ad
        bannerView.LoadAd(adRequest);
    }

    public void ShowBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Show();
        }
    }

    public void HideBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
        }
    }

    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    #endregion

    #region Interstitial Ads

    public void LoadInterstitialAd()
    {
        // Clean up interstitial before reusing
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        // Create the request used to load the ad
        AdRequest adRequest = new AdRequest();

        // Send the request to load the ad
        InterstitialAd.Load(interstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded.");
            interstitialAd = ad;

            // Register event handlers
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad closed.");
                LoadInterstitialAd(); // Reload for next time
            };
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to show: " + error);
                LoadInterstitialAd(); // Reload for next time
            };
        });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet.");
            LoadInterstitialAd();
        }
    }

    #endregion

    #region Rewarded Ads

    public void LoadRewardedAd()
    {
        // Clean up rewarded ad before reusing
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // Create the request used to load the ad
        AdRequest adRequest = new AdRequest();

        // Send the request to load the ad
        RewardedAd.Load(rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded.");
            rewardedAd = ad;

            // Register event handlers
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad closed.");
                LoadRewardedAd(); // Reload for next time
            };
            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to show: " + error);
                LoadRewardedAd(); // Reload for next time
            };
        });
    }

    public void ShowRewardedAd(Action<Reward> onUserEarnedReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Rewarded ad granted reward: {reward.Amount} {reward.Type}");
                onUserEarnedReward?.Invoke(reward);
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready yet.");
            LoadRewardedAd();
        }
    }

    #endregion

    private void OnDestroy()
    {
        // Clean up ads
        DestroyBannerAd();
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }
}
