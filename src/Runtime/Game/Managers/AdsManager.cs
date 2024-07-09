using System;
using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
using Markins.Runtime.Game.Utils;
using UnityEngine;

public class AdsManager : Singleton<AdsManager>
{
    public Action RewardedAdsOpened;
    public Action RewardedAdsRewarded;
    public Action RewardedAdsClosed;
    public Action RewardedAdsFailed;


    public Action InterstitialAdsOpened;
    public Action InterstitialAdsClosed;
    public Action InterstitialAdsFailed;

    public void Start()
    {
        Bridge.advertisement.SetMinimumDelayBetweenInterstitial(120);
        Bridge.advertisement.rewardedStateChanged += AdvertisementOnRewardedStateChanged;
        Bridge.advertisement.interstitialStateChanged += OnInterstitialStateChanged;

        Debug.Log("Storage Type:" + Bridge.storage.defaultType);

        if (Bridge.advertisement.isBannerSupported)
            Bridge.advertisement.ShowBanner();

    }

    private void OnInterstitialStateChanged(InterstitialState state)
    {
        //if (state == InterstitialState.Opened)
        //    InterstitialAdsOpened?.Invoke();
        //if (state == InterstitialState.Closed)
        //    InterstitialAdsClosed?.Invoke();
        //if (state == InterstitialState.Failed)
        //    InterstitialAdsFailed?.Invoke();
    }

    public void ShowRewarded()
    {
        Bridge.advertisement.ShowRewarded();
    }

    public void ShowInter()
    {
        Debug.Log("Show Inter");
        Bridge.advertisement.ShowInterstitial();
    }

    private void AdvertisementOnRewardedStateChanged(RewardedState rewardedState)
    {
        Debug.Log("RewardedStateChanged" + rewardedState);

        if (rewardedState == RewardedState.Opened)
            RewardedAdsOpened?.Invoke();
        if (rewardedState == RewardedState.Closed)
            RewardedAdsClosed?.Invoke();
        if (rewardedState == RewardedState.Rewarded)
            RewardedAdsRewarded?.Invoke();
    }


}
