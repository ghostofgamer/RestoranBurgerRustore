using UnityEngine;
using TheSTAR.Utility;

[CreateAssetMenu(menuName = "Data/BuyersConfig", fileName = "BuyersConfig")]
public class BuyersConfig : ScriptableObject
{
    [SerializeField] private int minBuyersCount; 
    [SerializeField] private int maxBuyersCount;

    [Space]
    [SerializeField] private GameTimeSpan ordersPeriodMin;
    [SerializeField] private GameTimeSpan ordersPeriodMax;
    
    [Space]
    [SerializeField] private ItemType[] orderItems;

    [Space]
    [SerializeField] private GameTimeSpan minEatingDuration; 
    [SerializeField] private GameTimeSpan maxEatingDuration; 

    public int MinBuyersCount => minBuyersCount;
    public int MaxBuyersCount => maxBuyersCount;
    public GameTimeSpan OrdersPeriodMin => ordersPeriodMin;
    public GameTimeSpan OrdersPeriodMax => ordersPeriodMax;
    public ItemType[] OrderItems => orderItems;
    public GameTimeSpan MinEatingDuration => minEatingDuration;
    public GameTimeSpan MaxEatingDuration => maxEatingDuration;
}