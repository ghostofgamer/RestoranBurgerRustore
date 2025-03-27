using System;
using TheSTAR.GUI;
using TheSTAR.Sound;
using TheSTAR.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartElementSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private ValueSwitcher valueSwitcher;

    private ItemType itemType;
    private Action<ItemType, int> onChangeItemsCountAction;

    public void Init(SoundController sounds, ItemType itemType, Action<ItemType, int> onChangeItemsCountAction, ScrollRect scrollRect)
    {
        this.itemType = itemType;
        this.onChangeItemsCountAction = onChangeItemsCountAction;

        valueSwitcher.Init(sounds);
        valueSwitcher.Init(OnChangeValue, scrollRect);
    }

    public void SetVisual(string itemName, DollarValue price, int value)
    {
        nameText.text = itemName;
        priceText.text = TextUtility.FormatPrice(price);
        valueSwitcher.SetValueWithoutNotify(value);
    }

    private void OnChangeValue(int value)
    {
        this.onChangeItemsCountAction?.Invoke(itemType, value);
    }
}