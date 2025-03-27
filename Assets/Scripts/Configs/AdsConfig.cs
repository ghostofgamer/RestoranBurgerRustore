using UnityEngine;

[CreateAssetMenu(menuName = "Data/Ads", fileName = "AdsConfig")]
public class AdsConfig : ScriptableObject
{
    [Header("IDs")]
    [SerializeField] private string bannerAdUnitID = "test_banner_ad_unit_id";
    [SerializeField] private string interAdUnitID = "test_inter_ad_unit_id";
    [SerializeField] private string rewardedAdUnitID = "test_rewarded_ad_unit_id";

    [Space]
    [SerializeField] private float firstInterDelaySeconds = 60; // минимальный промежуток времени между двумя интерами
    [SerializeField] private float interDelaySeconds = 60; // минимальный промежуток времени между двумя интерами

    [Space]
    [SerializeField] private bool useBanner;
    [SerializeField] private bool testAd;

    public string BannerAdUnitID => bannerAdUnitID;
    public string InterAdUnitID => interAdUnitID;
    public string RewardedAdUnitID => rewardedAdUnitID;
    public float FirstInterDelaySeconds => firstInterDelaySeconds;
    public float InterDelaySeconds => interDelaySeconds;
    public bool UseBanner => useBanner;
    public bool TestAd => testAd;
}