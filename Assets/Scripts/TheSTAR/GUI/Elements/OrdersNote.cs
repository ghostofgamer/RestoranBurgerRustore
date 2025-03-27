using Configs;
using TheSTAR.GUI;
using TheSTAR.Utility;
using TMPro;
using UnityEngine;
using Zenject;

public class OrdersNote : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderNumberText;
    [SerializeField] private OrderItemElement[] elements;
    [SerializeField] private PointerButton previousOrderButton;
    [SerializeField] private PointerButton nextOrderButton;

    [Space]
    [SerializeField] private GameObject haveOrderContainer;
    [SerializeField] private GameObject emptyContainer;

    private OrdersManager ordersManager;

    private ConfigHelper<ItemsConfig> itemsConfig = new();

    private ActiveOrderData currentOrderData;
    public ActiveOrderData CurrentOrderData => currentOrderData;

    [Inject]
    private void Consruct(OrdersManager ordersManager)
    {
        this.ordersManager = ordersManager;
    }

    public void Init()
    {
        previousOrderButton.Init(OnPreviousClick);
        nextOrderButton.Init(OnNextClick);
    }

    public void Set(ActiveOrderData orderData, bool force)
    {
        if (!force && currentOrderData != null && currentOrderData != orderData) return;

        this.currentOrderData = orderData;

        if (orderData == null)
        {
            haveOrderContainer.SetActive(false);
            emptyContainer.SetActive(true);
            return;
        }
        
        haveOrderContainer.SetActive(true);
        emptyContainer.SetActive(false);
        orderNumberText.text = $"ORDER {orderData.OrderIndex + 1}:";

        for (int i = 0; i < elements.Length; i++)
        {
            if (i < orderData.OrderData.Items.Length)
            {
                elements[i].gameObject.SetActive(true);
                elements[i].Set(
                    itemsConfig.Get.Item(orderData.OrderData.Items[i].ItemType).MainData.IconSprite, 
                    orderData.OrderData.Items[i].Value); // orderData.OrderData.Items[i].CurrentValue >= orderData.OrderData.Items[i].Value
            }
            else
            {
                elements[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnPreviousClick()
    {
        if (currentOrderData == null) return;

        var previous = ordersManager.GetPrevious(currentOrderData.OrderIndex);
        if (previous != null) Set(previous, true);
    }

    private void OnNextClick()
    {
        if (currentOrderData == null) return;

        var next = ordersManager.GetNext(currentOrderData.OrderIndex);
        if (next != null) Set(next, true);
    }
}