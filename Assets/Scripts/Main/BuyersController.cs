using System;
using UnityEngine;
using World;
using TheSTAR.Utility;
using Zenject;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using Configs;
using DayChangeContent;
using DayChangeContent.Counters;
using ReputationContent;

/// <summary>
/// Отвечает за то, чтобы покупатели появлялись в нужный момент с нужным заказом
/// </summary>
public class BuyersController : MonoBehaviour
{
    private const int MaxBuyersInOneCarCount = 3;
    
    [SerializeField] private Buyer[] buyerVariants;

    private Reputation _reputation;
    private GameController game;
    private OrdersGenerator ordersGenerator;
    private AllBuyerPlaces allBuyerPlaces;
    private DiContainer diContainer;
    private AllPrices allPrices;
    private BuyerCarsSimulation buyerCars;
    private TutorialController tutorial;
    private XpController xp;
    private ConfigHelper<BuyersConfig> buyersConfig = new();
    private ConfigHelper<ItemsConfig> itemsConfig = new();
    private List<Buyer> createdBuyers = new();
    private bool open = false;
    private bool firstBuyersCalled = false; // истинно, если хотя бы один раз успешно вызвали клиентов
    private BuyersCounter _buyersCounter;
    
    public event Action<int> BuyersCountIncreased;
    public event Action<int> BuyersCountDecreased;
    
    public event Action OrderCompleted;
    
    public List<Buyer> CreatedBuyers => createdBuyers;

    public Buyer FirstActiveBuyer
    {
        get
        {
            if (createdBuyers.Count == 0) return null;

            foreach (var buyer in createdBuyers)
            {
                if (!buyer.Finish) return buyer;
            }

            return null;
        }
    }

    [Inject]
    private void Consruct(
        GameController game,
        OrdersGenerator ordersGenerator,
        AllBuyerPlaces allBuyerPlaces,
        OrdersManager ordersManager,
        DiContainer diContainer,
        AllPrices allPrices,
        BuyerCarsSimulation buyerCars,
        TutorialController tutorial,
        XpController xp)
    {
        this.game = game;
        this.ordersGenerator = ordersGenerator;
        this.allBuyerPlaces = allBuyerPlaces;
        this.diContainer = diContainer;
        this.allPrices = allPrices;
        this.buyerCars = buyerCars;
        this.tutorial = tutorial;
        this.xp = xp;

        ordersManager.OnOrderCompletedEvent += OnOrderComplete;
    }

    public void InitBuyersCounter(BuyersCounter buyersCounter)
    {
        _buyersCounter = buyersCounter;
    }
    
    public void StartSimulate()
    {
        TryCallBuyer();
        WaitForNextBuyer();
    }

    [ContextMenu("TryCallBuyer")]
    private void TryCallBuyer()
    {
        if (!_buyersCounter.IsAllowedSpawnNewBuyer)
            return;
        
        if (!open)
        {
            Debug.Log("Закрыто");
            return;
        }

        Debug.Log("Попытка вызвать машину с клиентами");

        if (!buyerCars.HaveAvailablePlace(out _))
        {
            Debug.Log("Нет мест на парковке");
            return;
        }

        allBuyerPlaces.HaveAvailablePlaces(out var availablePlacesCount);

        if (availablePlacesCount <= 0)
        {
            Debug.Log("Нет свободных посадочных мест");
            return;
        }

        int buyersToSpawnCount =
            MathUtility.Limit((int)Random.Range(1, MaxBuyersInOneCarCount + 1), 1, availablePlacesCount);
        List<OrderData> orders = new();

        for (int i = 0; i < buyersToSpawnCount; i++)
        {
            var order = new OrderData();
            bool success;

            order = !tutorial.IsCompleted(TutorialType.ServeTheQuests)
                ? ordersGenerator.GenerateOrder(out success, true)
                : ordersGenerator.GenerateOrder(out success, false);

            if (success) orders.Add(order);
            else
            {
                Debug.Log("Не удалось сгенерировать заказ");
                break;
            }
        }

        if (orders.Count == 0) return;

        buyerCars.GenerateCar((car) =>
        {
            for (int i = 0; i < orders.Count; i++)
            {
                var randomBuyerPrefab = ArrayUtility.GetRandomValue(buyerVariants);
                var createdBuyer = diContainer.InstantiatePrefabForComponent<Buyer>(
                    randomBuyerPrefab,
                    (car as BuyerCar).BuyerPoints[i].transform.position,
                    Quaternion.identity,
                    transform);

                createdBuyers.Add(createdBuyer);
                BuyersCountIncreased?.Invoke(createdBuyers.Count);
                Debug.Log("createdBuyers " + createdBuyers.Count);
                createdBuyer.Init((BuyerCar)car, orders[i]);
                createdBuyer.OnChangeQueueIndexEvent += allBuyerPlaces.OnUpdateWaitingPlaceIndex;
                createdBuyer.OnSitEvent += OnBuyerSit;
                allBuyerPlaces.AddBuyer(createdBuyer);
            }
        }, orders.Count);

        firstBuyersCalled = true;
    }

    private Tweener waitForNextBuyerTweener;

    public void InitReputation(Reputation reputation)
    {
        _reputation = reputation;
    }

    private void WaitForNextBuyer()
    {
        waitForNextBuyerTweener?.Kill();

        int starRestoraunt = _reputation.StarsRestaurant;
        Debug.Log("ЗХВЕЗД у заведения " + starRestoraunt);

        float baseDurationMin = buyersConfig.Get.OrdersPeriodMin.TotalSeconds;
        float baseDurationMax = buyersConfig.Get.OrdersPeriodMax.TotalSeconds;

        float durationMin = baseDurationMin;
        float durationMax = baseDurationMax;

        if (starRestoraunt < 3)
        {
            float increaseFactor = 1 + (3 - starRestoraunt) * 0.165f;
            durationMin *= increaseFactor;
            durationMax *= increaseFactor;
        }
        else if (starRestoraunt > 3)
        {
            float decreaseFactor = 1 - (starRestoraunt - 3) * 0.165f;
            durationMin *= decreaseFactor;
            durationMax *= decreaseFactor;
        }

        durationMin = Mathf.Max(durationMin, 0.1f);
        durationMax = Mathf.Max(durationMax, 0.1f);

        var duration = Random.Range(durationMin, durationMax);

        // var duration = Random.Range(buyersConfig.Get.OrdersPeriodMin.TotalSeconds, buyersConfig.Get.OrdersPeriodMax.TotalSeconds);

        waitForNextBuyerTweener =
            DOVirtual.Float(0f, 1f, duration, (value) => { }).SetEase(Ease.Linear).OnComplete(() =>
            {
                TryCallBuyer();
                WaitForNextBuyer();
            });
    }

    private void OnOrderComplete(Buyer b)
    {
        // b.Place.HideMiniList();
        b.StartEat(Random.Range(buyersConfig.Get.MinEatingDuration.TotalSeconds,
            buyersConfig.Get.MaxEatingDuration.TotalSeconds));
        
        OrderCompleted?.Invoke();
    }

    public void GoAway(Buyer b)
    {
        b.FinishSit(b.Place.Point.transform);
        _reputation.ClientView(b.RateWaitingOrder, b.PolluteRate);
        b.SetDestination(b.Car.BuyerPoints[0], (b) =>
        {
            createdBuyers.Remove(b);
            BuyersCountDecreased?.Invoke(createdBuyers.Count);
            Debug.Log("createdBuyers Rwemove" + createdBuyers.Count);
            buyerCars.BuyerBackToCar(b.Car);
            // _reputation.ClientView(b.RateWaitingOrder,b.PolluteRate);
            Destroy(b.gameObject);
        });

        // cash

        DollarValue goldReward = new(0, 0);
        int xpReward = 0;
        foreach (var element in b.OrderData.Items)
        {
            goldReward += allPrices.GetPrice(element.ItemType) * element.Value;
            xpReward += itemsConfig.Get.Item(element.ItemType).XpData.SaleXpReward * element.Value;
        }

        xp.AddXp(xpReward);

        //b.Place.ActivateCash(goldReward, xpReward);

        /*
        if (!tutorial.IsCompleted(TutorialType.CompleteOrders))
        {
            game.TriggerTutorial();
        }
        */
    }

    [ContextMenu("TestGenerateOrder")]
    private void TestGenerateOrder()
    {
        var order = ordersGenerator.GenerateOrder(out var success,false);

        if (!success)
        {
            Debug.Log("Не удалось создать заказ");
            return;
        }
        else
        {
            Debug.Log("Заказ: ");
            Debug.Log(order);
            return;
        }
    }

    public void SetOpen(bool open)
    {
        this.open = open;

        if (!firstBuyersCalled)
        {
            TryCallBuyer();
            WaitForNextBuyer();
        }
    }

    private void OnBuyerSit(Buyer buyer)
    {
        /*
        if (!tutorial.IsCompleted(TutorialType.CompleteOrders))
        {
            game.TriggerTutorial();
        }
        */
    }
}