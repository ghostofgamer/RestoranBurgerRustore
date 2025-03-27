using System;
using TheSTAR.Data;
using TheSTAR.Utility;
using UnityEngine;
using Zenject;

public class AdsManager
{
    private readonly ResourceHelper<AdsConfig> adsConfig = new("Configs/AdsConfig");
    private DataController data;
    private AnalyticsManager analytics;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private bool NeedShowBanner => initialized && adsConfig.Get.UseBanner && !AdsFree;

    public bool AdsFree =>
        false; // data.gameData.inappsData.adsFreeSmallPurchased || data.gameData.inappsData.adsFreeBigPurchased;

    public bool FreeRewards => false; // data.gameData.inappsData.adsFreeBigPurchased;
    private bool initialized = false;
    public const bool ShowDebugs = true;

    public static bool Connection => Application.internetReachability != NetworkReachability.NotReachable;

    [Inject]
    private void Consruct(DataController data, AnalyticsManager analytics)
    {
        this.data = data;
        this.analytics = analytics;
    }

    public void InitAds()
    {
        interstitialAd = new(adsConfig.Get.InterAdUnitID, adsConfig.Get.FirstInterDelaySeconds,
            adsConfig.Get.InterDelaySeconds, analytics);
        interstitialAd.Init();
        rewardedAd = new(adsConfig.Get.RewardedAdUnitID, analytics);
        rewardedAd.Init();

        MaxSdkCallbacks.OnSdkInitializedEvent += OnAdsInitialized;
        if (adsConfig.Get.TestAd) MaxSdk.SetUserId("TEST_USER");
        MaxSdk.InitializeSdk();
    }

    private void OnAdsInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
    {
        initialized = true;

        if (ShowDebugs) Debug.Log("AppLovin successfully initialized");
        UpdateBanner();

        rewardedAd.LoadRewarded();
    }

    public void UpdateBanner(string placement = "bottom")
    {
        if (NeedShowBanner)
        {
            // AppLovin SDK is initialized, start loading ads
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(adsConfig.Get.BannerAdUnitID, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(adsConfig.Get.BannerAdUnitID, Color.black);
            MaxSdk.ShowBanner(adsConfig.Get.BannerAdUnitID);

            if (ShowDebugs) Debug.Log($"Show banner with ID \"{adsConfig.Get.BannerAdUnitID}\"");

            analytics.Log(RepeatingEventType.ShowBannerAd, placement);
        }
        else
        {
            MaxSdk.HideBanner(adsConfig.Get.BannerAdUnitID);
        }
    }

    public void TriggerInterstitial(string placement)
    {
        if (!initialized)
        {
            //analytics.Log(RepeatingEventType.ShowInterstitialAd, placement);
            //analytics.LogAds(AdLogType.Attempt, new AdAnalyticData(AdType.Interstitial, placement, false, Connection));
            return;
        }

        int AdsValue = PlayerPrefs.GetInt("removeADS", 0);

        if (AdsValue > 0)
            return;

        interstitialAd.LoadAndShowInterstitial(placement);
    }

    public void ShowRewarded(string placement, Action<bool> completeAction)
    {
        //Debug.Log("Try show rewarded...");
        if (FreeRewards)
        {
            if (ShowDebugs) Debug.Log("Ads Free");
            completeAction?.Invoke(true);
            return;
        }

        if (!initialized)
        {
            if (ShowDebugs) Debug.Log("Break Ad: AdsManager not initialized");
            completeAction?.Invoke(false);
            return;
        }

        rewardedAd.TryShowRewarded(placement, completeAction);
    }

    #region Sub Classes

    private class InterstitialAd
    {
        private string placement;
        private readonly string adUnitId;
        private readonly float firstInterDelaySeconds;
        private readonly float interDelaySeconds;
        private DateTime previousShowInterstitialDateTime;
        private bool inTryToShowInterstitial;
        private AnalyticsManager analytics;
        private bool firstInterstitialCompleted = false;

        public InterstitialAd(string adUnitId, float firstDelaySeconds, float interDelaySeconds,
            AnalyticsManager analytics)
        {
            this.adUnitId = adUnitId;
            this.firstInterDelaySeconds = firstDelaySeconds;
            this.interDelaySeconds = interDelaySeconds;
            this.analytics = analytics;
            previousShowInterstitialDateTime = DateTime.Now; // нужно, чтобы началась задержка на первый интерстишл
        }

        public void Init()
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        }

        public void LoadAndShowInterstitial(string placement)
        {
            if (inTryToShowInterstitial || (DateTime.Now - previousShowInterstitialDateTime).TotalSeconds <
                (firstInterstitialCompleted ? interDelaySeconds : firstInterDelaySeconds))
            {
                if (!firstInterstitialCompleted) Debug.Log("Первое ожидание (6m)");
                else Debug.Log("Обычное ожидание (3m)");

                //analytics.LogAds(AdLogType.Attempt, new AdAnalyticData(AdType.Interstitial, placement, false, Connection));
                return;
            }

            //analytics.LogAds(AdLogType.Attempt, new AdAnalyticData(AdType.Interstitial, placement, true, Connection));

            inTryToShowInterstitial = true;
            this.placement = placement;
            MaxSdk.LoadInterstitial(adUnitId);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            //analytics.LogAds(AdLogType.Load, new AdAnalyticData(AdType.Interstitial, placement, true, Connection));
            MaxSdk.ShowInterstitial(adUnitId);
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            //analytics.LogAds(AdLogType.Load, new AdAnalyticData(AdType.Interstitial, placement, false, Connection));
            inTryToShowInterstitial = false;
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            analytics.Log(RepeatingEventType.ShowInterstitialAd, placement);
            inTryToShowInterstitial = false;
            previousShowInterstitialDateTime = DateTime.Now;
            if (!firstInterstitialCompleted) firstInterstitialCompleted = true;
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            inTryToShowInterstitial = false;
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
        }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
        }
    }

    private class RewardedAd
    {
        private string placement;
        private readonly string adUnitId;
        private bool inTryToShowRewarded = false;
        private bool needGiveReward = false;
        private Action<bool> completeAction;
        private AnalyticsManager analytics;
        private AdLoadStatus adLoadStatus = AdLoadStatus.None;

        public RewardedAd(string adUnitId, AnalyticsManager analytics)
        {
            this.adUnitId = adUnitId;
            this.analytics = analytics;
        }

        public void Init()
        {
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        public void TryShowRewarded(string placement, Action<bool> completeAction)
        {
            if (ShowDebugs) Debug.Log("TryShowRewarded...");

            if (inTryToShowRewarded)
            {
                if (ShowDebugs) Debug.Log("Break, already in loading process");
                return;
            }

            this.completeAction = completeAction;

            needGiveReward = false;
            inTryToShowRewarded = true;
            this.placement = placement;

            if (adLoadStatus == AdLoadStatus.None) LoadRewarded(); // запрашиваем рекламу
            else if (adLoadStatus == AdLoadStatus.Loading) return; // ожидаем окончания загрузки
            else if (adLoadStatus == AdLoadStatus.Loaded)
                MaxSdk.ShowRewardedAd(adUnitId); // есть загруженная реклама, показываем её
        }

        public void LoadRewarded()
        {
            if (adLoadStatus != AdLoadStatus.None) return;

            if (ShowDebugs) Debug.Log("Load rewarded...");
            adLoadStatus = AdLoadStatus.Loading;
            MaxSdk.LoadRewardedAd(adUnitId);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            adLoadStatus = AdLoadStatus.Loaded;
            if (ShowDebugs) Debug.Log("Rewarded loaded");

            if (inTryToShowRewarded)
            {
                if (ShowDebugs) Debug.Log("Show rewarded...");
                MaxSdk.ShowRewardedAd(adUnitId);
            }
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            adLoadStatus = AdLoadStatus.None;
            if (ShowDebugs) Debug.LogError("Rewarded load fail");
            if (ShowDebugs) Debug.LogError(errorInfo.Message);
            //analytics.LogAds(AdLogType.Load, new AdAnalyticData(AdType.Rewarded, placement, false, Connection));
            inTryToShowRewarded = false;
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            adLoadStatus = AdLoadStatus.None;
            if (ShowDebugs) Debug.Log("Rewarded shown successfully");
            analytics.Log(RepeatingEventType.ShowRewardedAd, placement);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            if (ShowDebugs) Debug.LogError("Rewarded display fail");
            inTryToShowRewarded = false;
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
        }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden.
            inTryToShowRewarded = false;
            completeAction.Invoke(needGiveReward);
            LoadRewarded(); // Pre-load the next ad
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            needGiveReward = true;
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
        }
    }

    #endregion
}

public enum AdLoadStatus
{
    /// <summary>
    /// Нет загруженной рекламы, нет текущго запроса на загрузку
    /// </summary>
    None,

    /// <summary>
    /// Рекламу запросили, она загружается
    /// </summary>
    Loading,

    /// <summary>
    /// Реклама загружена, её можно либо придержать либо показать моментально
    /// </summary>
    Loaded
}