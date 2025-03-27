using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OrderMonitorElement : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI itemName;

    public void SetVisual(Sprite icon, string itemName)
    {
        iconImg.sprite = icon;
        this.itemName.text = itemName;
    }
}