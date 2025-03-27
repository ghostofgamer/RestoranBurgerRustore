using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Объединяет драггеры, чтобы поочерёдно заполнять их или быстро проверять наличие предмета
/// </summary>
public class DraggerGroup : MonoBehaviour
{
    [SerializeField] private Dragger[] draggers;

    public event Action<DraggerGroup> OnAnyChangeEvent;
    public event Action<DraggerGroup, Dragger, Draggable> OnSetItemEvent; // оба эти ивента вызывать когда ставим стакан в стакан
    public event Action<DraggerGroup, Dragger, Draggable> OnEndDragItemEvent; // (или достаём стакан из стакана)
    public event Action<DraggerGroup> OnEmptyGroup; // группа опустошена

    private int index = -1;
    public int Index => index;

    public void Init(int index)
    {
        this.index = index;
    }

    /*
    private void Awake()
    {
        Init();
    }
    */

    public void Init()
    {
        foreach (var dragger in draggers)
        {
            dragger.SetGroup(this);
            dragger.OnStartDragEvent += (dragger, d) =>
            {
                OnSetItemEvent?.Invoke(this, dragger, d);
                OnAnyChangeEvent?.Invoke(this);
            };
            dragger.OnChangeStackEvent += () => OnAnyChangeEvent?.Invoke(this);
            dragger.OnEndDragEvent += OnEndDrag;
        }
    }

    public Draggable CurrentDraggable
    {
        get
        {
            for (int i = draggers.Length - 1; i >= 0; i--)
            {
                Dragger dragger = draggers[i];
                if (dragger.CurrentDraggable) return dragger.CurrentDraggable;
            }

            return null;
        }
    }

    public bool IsEmpty => CurrentDraggable == null;

    public List<Draggable> AllDraggables
    {
        get
        {
            List<Draggable> allDraggables = new();
            for (int i = draggers.Length - 1; i >= 0; i--)
            {
                Dragger dragger = draggers[i];
                if (dragger.CurrentDraggable) allDraggables.Add(dragger.CurrentDraggable);
            }

            return allDraggables;
        }
    }

    public bool HavePlace(ItemType forItemType, out Dragger availablePlace)
    {
        foreach (var dragger in draggers)
        {
            if (dragger.IsEmpty)
            {
                availablePlace = dragger;
                return true;
            }
            else
            {
                var embeddableItem = dragger.CurrentDraggable.GetComponent<EmbeddableItem>();
                if (embeddableItem == null) continue;
                if (embeddableItem.Item.ItemType != forItemType) continue;
                if (embeddableItem.CanAddItem(out availablePlace)) return true;
            }
        }

        availablePlace = null;
        return false;
    }
    
    public bool HavePlaceCoffee(ItemType forItemType, out Dragger availablePlace)
    {
        foreach (var dragger in draggers)
        {
            if (dragger.IsEmpty)
            {
                Debug.Log("dragger.IsEmpty " + dragger.gameObject.name);
                availablePlace = dragger;
                return true;
            }
        }

        availablePlace = null;
        return false;
    }

    public int GetAllEmptyPlaces(out List<Dragger> allAvailablePlaces)
    {
        allAvailablePlaces = new();

        foreach (var dragger in draggers)
        {
            if (dragger.IsEmpty) allAvailablePlaces.Add(dragger);
        }

        return allAvailablePlaces.Count;
    }

    private void OnEndDrag(Dragger dragger, Draggable draggable)
    {
        OnEndDragItemEvent?.Invoke(this, dragger, draggable);
        OnAnyChangeEvent?.Invoke(this);

        if (AllDraggables.Count == 0) OnEmptyGroup?.Invoke(this);
    }

    public void ForceActivateColForDraggables()
    {
        var allDraggables = AllDraggables;

        foreach (var draggable in allDraggables)
        {
            draggable.ForceActivateCol();
        }
    }

    public void ForceDeactivateColForDraggables()
    {
        var allDraggables = AllDraggables;

        foreach (var draggable in allDraggables)
        {
            draggable.ForceDeactivateCol();
        }
    }
}