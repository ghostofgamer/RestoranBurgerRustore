using System.Collections.Generic;
using TheSTAR.Utility;
using UnityEngine;
using World;
using Zenject;

/// <summary>
/// Отвечает за посадку клиентов в закусочной. Содержит все группы посадочных мест а так же очередь для ожидания
/// </summary>
public class AllBuyerPlaces : MonoBehaviour
{
    [SerializeField]
    private MembersQueue
        waitingQueue; // клиенты сперва должны вставать в WaitingQueue, после принятия заказа и оплаты должны садиться на место

    [SerializeField] private UnityDictionary<BuyerPlaceType, BuyerPlacesGroup> placeGroups;
    [SerializeField] private BuyerPlaceFurnitureUnit[] _placesFurnitureUnitTutor;

    private OrdersManager ordersManager;
    private AnalyticsManager analytics;
    private TutorialController _tutorialController;
    private GameController _gameController;

    public UnityDictionary<BuyerPlaceType, BuyerPlacesGroup> PlaceGroups => placeGroups;
    public BuyerPlaceFurnitureUnit[] BuyerPlacesFurnitureUnitTutor => _placesFurnitureUnitTutor;
    //private ItemsController items;

    public bool HaveAvailablePlaces(out int availablePlacesCount)
    {
        availablePlacesCount = 0;

        var allPlaceTypes = EnumUtility.GetValues<BuyerPlaceType>();

        foreach (var placeType in allPlaceTypes)
        {
            placeGroups.Get(placeType).HaveEmptyPlaces(out var availablePlaces);
            Debug.Log("Type " + placeType + "Count " + availablePlaces.Count);

            availablePlacesCount += availablePlaces.Count;
        }

        Debug.Log(availablePlacesCount > 0);
        return availablePlacesCount > 0;
    }

    [Inject]
    private void Construct(OrdersManager ordersManager, AnalyticsManager analytics,
        TutorialController tutorialController, GameController gameController)
    {
        this.ordersManager = ordersManager;
        this.analytics = analytics;
        _tutorialController = tutorialController;
        _gameController = gameController;
    }

    public void Init()
    {
        waitingQueue.Init();
        var allPlaceTypes = EnumUtility.GetValues<BuyerPlaceType>();
        foreach (var placeType in allPlaceTypes)
        {
            placeGroups.Get(placeType).Init(this);
        }

        ordersManager.OnOrderAcceptedEvent += (acceptedOrder) => { acceptedOrder.Place.ShowMiniList(acceptedOrder); };
        ordersManager.OnOrderChangeEvent += (acceptedOrder) => { acceptedOrder.Place.ShowMiniList(acceptedOrder); };
        ordersManager.OnOrderCompletedEvent += (buyer) => { buyer.Place.HideMiniList(); };

        //ordersManager.OnOrderCompletedEvent += RemoveBuyerFromPlace;

        ordersManager.OnOrderAcceptedEvent += OnOrderAccepted;
    }

    public void AddBuyer(Buyer buyer)
    {
        waitingQueue.AddMember(buyer, out int placeIndex);
        var place = GetAvailableBuyerPlace();
        place.SetBuyer(buyer);
        buyer.ReservePlace(place);
    }

    public BuyerPlace GetAvailableBuyerPlace()
    {
        var allPlaceTypes = EnumUtility.GetValues<BuyerPlaceType>();
        foreach (var placeType in allPlaceTypes)
        {
            if (placeGroups.Get(placeType).HaveEmptyPlaces(out var availablePlaces))
            {
                return availablePlaces[0];
            }
        }

        return null;
    }

    public void OnUpdateWaitingPlaceIndex(Buyer buyer, int waitingPlaceIndex)
    {
        if (waitingPlaceIndex <= -1) return;

        buyer.SetDestination(waitingQueue.Points[waitingPlaceIndex], (b) =>
        {
            Debug.Log("Встал в очередь");
            buyer.StartWait();

            if (waitingPlaceIndex == 0)
            {
                Debug.Log("Первый");

                if (!_tutorialController.IsCompleted(TutorialType.ExpectBuyers))
                {
                    Debug.Log("АГА");
                    _tutorialController.CompleteTutorial(TutorialType.ExpectBuyers);
                    _gameController.TriggerTutorial();
                }


                OnBuyerStayForOrder(buyer);
            }
            //buyer.Sit(place);
        });
    }

    /// <summary>
    /// Назначение места покупателю
    /// </summary>
    private void SetBuyerToPlace(Buyer buyer, BuyerPlace place)
    {
        place.SetBuyer(buyer);
        buyer.SetDestination(place.Point, (b) => { buyer.Sit(place); });
    }

    /*
    public void OnPlaceItemOnBuyerOrderPlate(Item item)
    {
        //if (item.ItemType == ItemType.CoffeeFinal) analytics.Trigger(RepeatingEventType.Give_Coffee);
        //else if (item.ItemType == ItemType.Donut) analytics.Trigger(RepeatingEventType.Give_Donut);
        //else if (item.ItemType == ItemType.Cake) analytics.Trigger(RepeatingEventType.Give_Cake);
        //else if (item.ItemType == ItemType.Waffle) analytics.Trigger(RepeatingEventType.Give_Waffle);
        //else if (item.ItemType == ItemType.IceCream) analytics.Trigger(RepeatingEventType.Give_IceCream);
    }
    */

    /*
    public void OnChangeItemsInBuyerPlace(BuyerPlace place)
    {
        var curretOrderItems = place.OrderItems;
        Dictionary<ItemType, int> currentItemsDictionary = new();

        foreach (var item in curretOrderItems)
        {
            var itemType = item.GetComponent<Item>().ItemType;
            if (currentItemsDictionary.ContainsKey(itemType)) currentItemsDictionary[itemType]++;
            else currentItemsDictionary.Add(itemType, 1);
        }

        if (place.Buyer == null) return;

        var order = place.Buyer.OrderData;

        for (int i = 0; i < order.Items.Length; i++)
        {
            if (currentItemsDictionary.ContainsKey(order.Items[i].ItemType)) order.Items[i].CurrentValue = currentItemsDictionary[order.Items[i].ItemType];
            else order.Items[i].CurrentValue = 0;
        }

        ordersManager.ChangeOrderItems(place, order);
    }
    */

    public BuyerCash FindCash()
    {
        BuyerCash cash;

        var allPlaceTypes = EnumUtility.GetValues<BuyerPlaceType>();
        foreach (var placeType in allPlaceTypes)
        {
            if (placeGroups.Get(placeType).HaveCash(out cash)) return cash;
        }

        return null;
    }

    public void SetPlaces(Dictionary<BuyerPlaceType, bool[]> data)
    {
        foreach (var element in data)
        {
            placeGroups.Get(element.Key).SetPlaces(element.Value);
        }
    }

    private void OnBuyerStayForOrder(Buyer buyer)
    {
        Debug.Log("OnBuyerStayForOrder " + buyer.gameObject.name);
        // buyer.StartWait();
        ordersManager.SetCurrentBuyerWithOrder(buyer);
    }

    private void OnOrderAccepted(ActiveOrderData orderData)
    {
        Debug.Log("OnOrderAccepted");
        orderData.Buyer.StopWaitingAnimation();
        waitingQueue.RemoveMember(orderData.Buyer);
        SetBuyerToPlace(orderData.Buyer, orderData.Buyer.Place);
    }
}

public enum BuyerPlaceType
{
    Sofa,
    SofaChair,
    SingleChair,
    DobbleChair
}