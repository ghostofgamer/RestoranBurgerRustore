using UnityEngine;
using Zenject;

public class OrdersInProcessMonitor : MonoBehaviour
{
    [SerializeField] private OrderInProcessElement[] elements;
    [SerializeField] private GameObject emptyObject;

    private OrdersManager ordersManager;

    [Inject]
    private void Construct(OrdersManager ordersManager)
    {
        this.ordersManager = ordersManager;
        ordersManager.OnOrderAcceptedEvent += (acceptedOrder) =>
        {
            UpdateOrdersList();
        };

        ordersManager.OnOrderCompletedEvent += (buyer) =>
        {
            UpdateOrdersList();
        };
    }

    private void UpdateOrdersList()
    {
        foreach (var element in elements) element.gameObject.SetActive(false);

        var activeOrders = ordersManager.ActiveOrders;
        for (int i = 0; i < activeOrders.Count && i < elements.Length; i++)
        {
            elements[i].Set(activeOrders[i]);
            elements[i].gameObject.SetActive(true);
        }

        emptyObject.SetActive(activeOrders.Count == 0);
    }
}