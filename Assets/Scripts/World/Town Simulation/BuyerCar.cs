using UnityEngine;

public class BuyerCar : TownCar
{
    [SerializeField] private PathPoint[] buyerPoints;

    private int currentBuyersCount;
    private int maxBuyersCount;

    public int MaxBuyersCount => maxBuyersCount;
    public PathPoint[] BuyerPoints => buyerPoints;

    public void Init(int maxBuyersCount)
    {
        this.currentBuyersCount = 0;
        this.maxBuyersCount = maxBuyersCount;
    }

    public void AddBuyer(out bool readyToGoAway)
    {
        currentBuyersCount++;
        readyToGoAway = currentBuyersCount >= maxBuyersCount;
    }
}