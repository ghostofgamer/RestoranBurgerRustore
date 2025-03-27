using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderItemElement : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Set(Sprite icon, int value)
    {
        iconImg.sprite = icon;
        valueText.text = $"x{value}";
    }
}