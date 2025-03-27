using UnityEngine;
using World;
using TheSTAR.Utility;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

[Obsolete]
public class SalesController
{
    /*
    private LevelController level;
    private int minAvailableOrders; // без учёта покупателей "для рабочих"
    private int maxAvailableOrders; // без учёта покупателей "для рабочих"
    private GameTimeSpan ordersPeriodMin;
    private GameTimeSpan ordersPeriodMax;

    private int CurrentMinAvailableOrders => minAvailableOrders;
    private int CurrentMaxAvailableOrders => maxAvailableOrders;

    private const float MinOrderCallDelay = 1f; // какое минимальное количество времени должно произойти между ордер-колами (в секундах)

    public void Init(int minAvailableOrders, int maxAvailableOrders, GameTimeSpan ordersPeriodMin, GameTimeSpan ordersPeriodMax)
    {
        //this.level = level;
        this.minAvailableOrders = minAvailableOrders;
        this.maxAvailableOrders = maxAvailableOrders;
        this.ordersPeriodMin = ordersPeriodMin;
        this.ordersPeriodMax = ordersPeriodMax;
    }

    public void StartSimulate()
    {
        if (OrdersLessThenMin) TryCallOrder();
        else if (OrdersLessThenMax) WaitToTryCall(DefaultWaitSeconds);
    }

    private Tweener waitTweener;

    private void WaitToTryCall(float timeWaitSeconds)
    {
        if (waitTweener != null)
        {
            waitTweener.Kill();
            waitTweener = null;
        }

        waitTweener =
        DOVirtual.Float(0, 1, timeWaitSeconds, (value) => { }).OnComplete(() =>
        {
            waitTweener = null;
            TryCallOrder();
        }).SetEase(Ease.Linear);
    }

    private bool inWaitForBuyer => waitTweener != null;
    public bool OrdersLessThenMin => level.GetOrderLevelEventsInProcess() < CurrentMinAvailableOrders;
    private bool OrdersLessThenMax => level.GetOrderLevelEventsInProcess() < CurrentMaxAvailableOrders;
    private float DefaultWaitSeconds => Random.Range(ordersPeriodMin.TotalSeconds, ordersPeriodMax.TotalSeconds);

    public void ForceCallOrder()
    {
        TryCallOrder();
    }

    private void TryCallOrder()
    {
        if (!OrdersLessThenMax) return;

        level.CallBuyerToQueue();

        float waitSeconds;

        if (OrdersLessThenMin) waitSeconds = MinOrderCallDelay;
        else waitSeconds = DefaultWaitSeconds;

        WaitToTryCall(waitSeconds);
    }

    public void CompleteOrder(Buyer buyer)
    {
        var queue = buyer.Queue;
        queue.RemoveBuyerFromQueue(buyer);

        //GameController.Instance.Sounds.PlaySound(TheSTAR.Sound.SoundType.Selling);

        level.OnCompleteOrder(buyer);

        if (OrdersLessThenMin) TryCallOrder();
        else if (!inWaitForBuyer) WaitToTryCall(DefaultWaitSeconds);

        return;
    }

    #region Reacts

    #endregion

    private Tweener incomeBonusTweener;

    public void PrepareForGoToNextLevel()
    {
        if (incomeBonusTweener != null)
        {
            incomeBonusTweener.Kill();
            incomeBonusTweener = null;
        }
    }

    private int[] haveDollarsVariants = new int[] {1, 5, 10, 20, 50};

    public void CalculateHaveCash(DollarValue needHaveValue, out DollarValue have)
    {
        int haveDollars = 0;
        int haveCents = 0;
        
        while (haveDollars < needHaveValue.dollars) haveDollars += ArrayUtility.GetRandomValue(haveDollarsVariants);

        if (needHaveValue.cents > 0 && haveDollars == needHaveValue.dollars) haveDollars++;

        have = new (haveDollars, haveCents);
    }
    */
}