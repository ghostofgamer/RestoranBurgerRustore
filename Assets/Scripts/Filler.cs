using System;
using UnityEngine;

/// <summary>
/// Представляет некий контейнер, который можно заполнять определённым расходником от 0% до 100% (или лучше смотреть в литрах)
/// </summary>
public class Filler : MonoBehaviour
{
    [SerializeField] private ItemType itemFilter; // тип расходника

    private float value = 0;
    [SerializeField] private float MaxValue = 100;
    [SerializeField] private float ValueForOneFill = 25;
    [SerializeField] private float ValueForOneUse = 5;

    public bool CanFill(ItemType itemType) => itemType == itemFilter && value < MaxValue;
    public bool CanUse => value >= ValueForOneUse;

    public event Action<float> OnChangeFillPercentEvent;

    public void Fill(Item item)
    {
        if (item.ItemType != itemFilter) return;
        if (item.Draggable.CurrentDragger) item.Draggable.CurrentDragger.EndDrag();
        Destroy(item.gameObject);

        value += ValueForOneFill;
        if (value > MaxValue) value = MaxValue;

        Debug.Log($"{(float)value}/{MaxValue}");

        OnChangeFillPercentEvent?.Invoke((float)value / MaxValue);
    }

    public void Use(Action completeUseAction)
    {
        if (value < ValueForOneUse) return;
        
        value -= ValueForOneUse;
        OnChangeFillPercentEvent?.Invoke((float)value / MaxValue);

        Debug.Log($"{(float)value}/{MaxValue}");

        completeUseAction?.Invoke();
    }
}