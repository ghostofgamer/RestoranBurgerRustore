using System;
using UnityEngine;
using TheSTAR.Utility;
using TheSTAR.Sound;

[CreateAssetMenu(menuName = "Data/Game", fileName = "GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private bool useCheats;
    [SerializeField] private bool lockData;
    
    [Header("Places")]
    [SerializeField] private UnityDictionary<BuyerPlaceType, Sprite> buyerPlaceIcons;
    [SerializeField] private UnityDictionary<BuyerPlaceType, PlaceData[]> buyerPlaceCostData;

    [Header("Expand")]
    [SerializeField] private ExpandZoneData[] expandData;

    [Header("Machines")]
    [SerializeField] private ApparatData coffeeMachineData;
    [SerializeField] private ApparatData deepFryerMachineData;
    [SerializeField] private ApparatData sodaMachineData;

    [Header("Start Balance")]
    [SerializeField] private DollarValue startBalance;
    [SerializeField] private UnityDictionary<ItemType, int> startDelivery;
    [SerializeField] private int startCoffeeBeans = 2;

    [Space]
    [SerializeField] private UnityDictionary<BuyerPlaceType, int> startFurnitureUnits = new();

    [Header("x2 Bonus")]
    [SerializeField] private GameTimeSpan x2BonusOfferPeriod; // с каким периодом показываем оффер
    [SerializeField] private GameTimeSpan showX2BonusOfferDuration; // сколько времени показываем оффер
    [SerializeField] private GameTimeSpan x2BonuxDuration; // сколько длится бонус на заработок

    [Space]
    [SerializeField] private MusicType[] randomMusicTypes;
    
    public bool UseCheats
    {
        get
        {
#if UNITY_EDITOR
            return useCheats;
#else
            return false;
#endif
        }
    }
    public bool LockData
    {
        get
        {
#if UNITY_EDITOR
            return lockData;
#else
            return false;
#endif
        }
    }
    public UnityDictionary<BuyerPlaceType, Sprite> BuyerPlaceIcons => buyerPlaceIcons;
    public UnityDictionary<BuyerPlaceType, PlaceData[]> BuyerPlaceCostData => buyerPlaceCostData;
    public ExpandZoneData[] ExpandZonesData => expandData;
    public ApparatData CoffeeMachineData => coffeeMachineData;
    public ApparatData DeepFryerMachineData => deepFryerMachineData;
    public ApparatData SodaMachineData => sodaMachineData;
    public DollarValue StartBalance => startBalance;
    public UnityDictionary<ItemType, int> StartDelivery => startDelivery;
    public int StartCoffeeBeans => startCoffeeBeans;
    public UnityDictionary<BuyerPlaceType, int> StartFurnitureUnits => startFurnitureUnits;
    public GameTimeSpan X2BonusOfferPeriod => x2BonusOfferPeriod;
    public GameTimeSpan ShowX2BonusOfferDuration => showX2BonusOfferDuration;
    public GameTimeSpan X2BonuxDuration => x2BonuxDuration;
    public MusicType[] RandomMusicTypes => randomMusicTypes;
}

[Serializable]
public struct PlaceData
{
    [SerializeField] private string displayName;
    [SerializeField] private DollarValue cost;
    [SerializeField] private int neededLevel;
    [SerializeField] private int placesCount;
    [SerializeField] private int neededExpandsCount; // сколько раз необходимо расширить магазин, прежде чем купить это

    public string DisplayName => displayName;
    public DollarValue Cost => cost;
    public int NeededLevel => neededLevel;
    public int PlacesCount => placesCount;
    public int NeededExpandsCount => neededExpandsCount;
}

[Serializable]
public struct ApparatData
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string displayName;
    [SerializeField] private int neededLevel;
    [SerializeField] private DollarValue cost;

    public Sprite Icon => icon;
    public string DisplayName => displayName;
    public int NeededLevel => neededLevel;
    public DollarValue Cost => cost;
}

[Serializable]
public struct ExpandZoneData
{
    [SerializeField] private string displayName;
    [SerializeField] private int neededLevel;
    [SerializeField] private int neededExpandCount;
    [SerializeField] private DollarValue cost;
    
    public string DisplayName => displayName;
    public int NeededLevel => neededLevel;
    public int NeededExpandCount => neededExpandCount;
    public DollarValue Cost => cost;
}