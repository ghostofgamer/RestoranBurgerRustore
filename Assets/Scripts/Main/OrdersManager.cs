using System;
using System.Collections.Generic;
using UnityEngine;
using World;
using Zenject;

public class OrdersManager
{
    private List<ActiveOrderData> activeOrders = new();
    public List<ActiveOrderData> ActiveOrders => activeOrders;

    public event Action<ActiveOrderData> OnOrderAcceptedEvent; // заказ принят

    public event Action<ActiveOrderData>
        OnOrderChangeEvent; // произошли какие-то изменения по заказу (добавился/удалился товар к выдачеы)

    public event Action<Buyer> OnOrderCompletedEvent; // выполнили заказ
    public event Action<Buyer> OnSetCurrentBuyerWithOrderEvent; // клиент готов сделать заказ

    private AnalyticsManager analytics;

    [Inject]
    private void Construct(AnalyticsManager analytics)
    {
        this.analytics = analytics;
    }

    public void TryAcceptOrder(Buyer buyer, BuyerPlace place, OrderData orderData)
    {
        foreach (var element in activeOrders)
        {
            if (element.Buyer == buyer) return;
        }

        var acceptedOrderData = new ActiveOrderData(place.Index, buyer, place, orderData);
        activeOrders.Add(acceptedOrderData);
        /*
        Debug.Log("добавили заказ " + acceptedOrderData.OrderIndex);

        foreach (var element in activeOrders)
        {
            Debug.Log("заказ покупатель " + element.Buyer.gameObject.name);
            Debug.Log("заказ СТОЛ " + element.Buyer.Place.Index);
        }
        */

        analytics.Trigger(RepeatingEventType.OrderAccepted);

        OnOrderAcceptedEvent?.Invoke(acceptedOrderData); // todo покупатель должен пойти сесть на место
    }

    public ActiveOrderData GetNext(int fromNumber)
    {
        for (int i = 0; i < activeOrders.Count; i++)
        {
            if (activeOrders[i].OrderIndex > fromNumber) return activeOrders[i];
        }

        return null;
    }

    public ActiveOrderData GetPrevious(int fromNumber)
    {
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            if (activeOrders[i].OrderIndex < fromNumber) return activeOrders[i];
        }

        return null;
    }

    public ActiveOrderData FindOrder(int index)
    {
        foreach (var order in activeOrders)
        {
            if (order.OrderIndex == index) return order;
        }

        return null;
    }

    public void CompleteOrder(int index)
    {
        Debug.Log("Удаленние " + index);
        var order = FindOrder(index);
        if (order == null) return;

        var buyer = order.Buyer;

        Debug.Log("Удаленние " + order.Buyer.gameObject.name);
        Debug.Log("Удаленние " + order.Buyer.Place.Index);

        activeOrders.Remove(order);

        foreach (var orderNew in activeOrders)
        {
            Debug.Log("Что осталось ? " + orderNew.Buyer.gameObject.name);
        }


        order = null;

        analytics.Trigger(RepeatingEventType.OrderCompleted);

        OnOrderCompletedEvent?.Invoke(buyer);
    }

    public void SetCurrentBuyerWithOrder(Buyer buyer)
    {
        this.OnSetCurrentBuyerWithOrderEvent?.Invoke(buyer);
    }
}

public class ActiveOrderData
{
    private int orderIndex;
    private Buyer buyer;
    private BuyerPlace place;
    private OrderData orderData;
    private int _indexOrderTray;

    public int IndexOrderTray => this._indexOrderTray;

    public int OrderIndex => orderIndex;
    public Buyer Buyer => buyer;
    public BuyerPlace Place => place;
    public OrderData OrderData => orderData;

    public void SetIndexOrderTray(int index)
    {
        _indexOrderTray = index;
    }

    public void ClearIndexOrderTray()
    {
        _indexOrderTray = 0;
    }

    public ActiveOrderData(int orderIndex, Buyer buyer, BuyerPlace place, OrderData orderData)
    {
        this.orderIndex = orderIndex;
        this.buyer = buyer;
        this.place = place;
        this.orderData = orderData;
    }

    public void ChangeOrderItems(OrderData orderData)
    {
        this.orderData = orderData;
    }

    public int AllItemsCount
    {
        get
        {
            int result = 0;

            foreach (var element in orderData.Items)
            {
                result += element.Value;
            }

            return result;
        }
    }
}