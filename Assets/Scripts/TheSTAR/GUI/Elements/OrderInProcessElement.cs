using Configs;
using TheSTAR.Utility;
using TMPro;
using UnityEngine;
using Zenject;

public class OrderInProcessElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderNumberText;
    [SerializeField] private OrderItemElement firstItem;
    [SerializeField] private OrderItemElement secondItem;

    private ConfigHelper<ItemsConfig> itemsConfig = new();

    public void Set(ActiveOrderData orderData)
    {
        if (orderData.OrderData.Items.Length == 1)
        {
            Set(
                orderData.Place.Index, 
                itemsConfig.Get.Item(orderData.OrderData.Items[0].ItemType).mainData.IconSprite, 
                orderData.OrderData.Items[0].Value);
        }
        else if (orderData.OrderData.Items.Length >= 2)
        {
            Set(
                orderData.Place.Index, 
                itemsConfig.Get.Item(orderData.OrderData.Items[0].ItemType).mainData.IconSprite, 
                orderData.OrderData.Items[0].Value,
                itemsConfig.Get.Item(orderData.OrderData.Items[1].ItemType).mainData.IconSprite, 
                orderData.OrderData.Items[1].Value);
        }
    }

    public void Set(int index, Sprite firstOrderIcon, int firstOrderValue)
    {
        orderNumberText.text = $"ORDER {index + 1}:";
        firstItem.Set(firstOrderIcon, firstOrderValue);
        this.secondItem.gameObject.SetActive(false);
    }

    public void Set(int index, Sprite firstOrderIcon, int firstOrderValue, Sprite secondOrderIcon, int secondOrderValue)
    {
        orderNumberText.text = $"ORDER {index + 1}:";
        firstItem.Set(firstOrderIcon, firstOrderValue);
        this.secondItem.gameObject.SetActive(true);
        secondItem.Set(secondOrderIcon, secondOrderValue);
    }
}