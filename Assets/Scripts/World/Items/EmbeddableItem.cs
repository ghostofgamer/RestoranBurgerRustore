using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// чтобы всё корректно работало корень должен сообщать о любом изменении драггеру 

/// <summary>
/// EmbeddableItems одного типа могут складываться друг в друга, пока не достигнут лимита 
/// </summary>
public class EmbeddableItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private Dragger dragger;
    [SerializeField] private TouchInteractive touchInteractive;
    
    public Dragger Dragger => dragger;
    public Item Item => item;

    [SerializeField] private int maxItemsInStackCount = 8;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        dragger.OnStartDragEvent += (dragger, draggable) =>
        {
            touchInteractive.Deactivate();
            OnChangeStack();
        };

        dragger.OnEndDragEvent += (dragger, draggable) =>
        {
            touchInteractive.Activate();
            OnChangeStack();
        };

        dragger.OnChangeStackEvent += () =>
        {
            OnChangeStack();
        };
    }

    public int GetEmbeddableItemsInStackCount
    {
        get
        {
            GetLastItem(1, out int itemsInStackCount);
            return itemsInStackCount;
        }
    }

    public bool CanAddItem(out Dragger to)
    {
        var lastEmbeddableItem = GetLastItem(1, out int itemsInStackCount);

        if (itemsInStackCount >= maxItemsInStackCount)
        {
            to = null;
            return false;
        }

        if (lastEmbeddableItem.dragger.CurrentDraggable == null)
        {
            to = lastEmbeddableItem.dragger;
            return true;
        }
        else
        {
            to = null;
            return false;
        }
    }

    // возвращает верхний предмет в стопке
    public EmbeddableItem GetLastItem() => GetLastItem(1, out _);

    public EmbeddableItem GetLastItem(int checkedNumber, out int itemsInStaskCount)
    {        
        if (dragger.CurrentDraggable == null)
        {
            itemsInStaskCount = checkedNumber;
            return this;
        }
        else
        {
            var next = dragger.CurrentDraggable.GetComponent<EmbeddableItem>();
            if (next == null)
            {
                itemsInStaskCount = checkedNumber;
                return this;
            }
            else
            {
                return next.GetLastItem(checkedNumber + 1, out itemsInStaskCount);
            }
        }
    }

    /// <summary>
    /// Изменилась вложенность (положили или убрали какой-то элемент)
    /// </summary>
    public void OnChangeStack()
    {
        //Debug.Log("Смена стака: информация дошла до " + gameObject.name, gameObject);
        var previousDragger = item.Draggable.CurrentDragger;
        if (previousDragger == null) return;

        previousDragger.OnChangeStack();
    }
}