using UnityEngine;
using TMPro;
using TheSTAR.Utility;
using Configs;

public class MiniOrderList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI orderNumberText;
    [SerializeField] private OrderItemElement[] elements;

    private ConfigHelper<ItemsConfig> itemsConfig = new();

    public void Set(ActiveOrderData orderData)
    {
        orderNumberText.text = $"{orderData.OrderIndex + 1}";

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
}