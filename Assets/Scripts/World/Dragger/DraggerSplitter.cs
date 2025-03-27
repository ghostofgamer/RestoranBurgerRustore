using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Объединяет драггер-группы, обращается к соответствующей группе по типу предмета. Если уже есть привязка к какому-то типу предмета, то другие предметы положить нельзя
/// </summary>
public class DraggerSplitter : MonoBehaviour
{
    [SerializeField] private GroupsByTypeHandler groups;

    public event Action<Draggable> OnFirstPlaceEvent;
    public event Action<Dragger, Draggable> OnSetItemEvent;
    public event Action<Draggable> OnEndDragEvent;
    public event Action OnAnyChangeEvent;
    public event Action OnEmptyEvent;

    private ItemType? currentItemType;

    public void Init()
    {        
        groups.Default.Init();
        groups.Default.OnAnyChangeEvent += (group) => OnAnyChangeEvent?.Invoke();
        groups.Default.OnSetItemEvent += OnSetDraggable;
        groups.Default.OnEmptyGroup += OnEmptyGroup;
        groups.Default.OnEndDragItemEvent += (group, dragger, d) => OnEndDragEvent?.Invoke(d);

        if (groups.Special != null && groups.Special.Count > 0)
        {
            foreach (var group in groups.Special.KeyValues) 
            {
                group.Value.Init();
                group.Value.OnAnyChangeEvent += (group) => OnAnyChangeEvent?.Invoke();
                group.Value.OnSetItemEvent += OnSetDraggable;
                group.Value.OnEmptyGroup += OnEmptyGroup;
                group.Value.OnEndDragItemEvent += (group, dragger, d) => OnEndDragEvent?.Invoke(d);
            }
        }
    }

    private void OnSetDraggable(DraggerGroup group, Dragger dragger, Draggable d)
    {        
        if (d == null) return;
        var item = d.GetComponent<Item>();
        if (item == null) return;

        currentItemType = item.ItemType;
        //Debug.Log("Назначение типа в сплиттер");

        if (group.AllDraggables.Count == 1) OnFirstPlaceEvent?.Invoke(d);

        OnSetItemEvent?.Invoke(dragger, d);
    }

    private void OnEmptyGroup(DraggerGroup group)
    {
        currentItemType = null;
        OnEmptyEvent?.Invoke();
    }

    // todo поправить, не пробросился currentItemType

    public Draggable CurrentDraggable
    {
        get
        {
            if (currentItemType == null) return null;
            else
            {
                var group = groups.GetGroupByItemType((ItemType)currentItemType);
                if (!group) return null;

                return group.CurrentDraggable;
            }
        }
    }

    public List<Draggable> AllDraggables
    {
        get
        {
            if (currentItemType == null) return new();
            else
            {
                var group = groups.GetGroupByItemType((ItemType)currentItemType);
                if (!group) return new();

                return group.AllDraggables;
            }
        }
    }
    
    public bool HavePlace(ItemType forItemType, out Dragger availablePlace)
    {
        if (currentItemType != null && currentItemType.Value != forItemType)
        {
            availablePlace = null;
            return false;
        }
        else
        {
            var group = groups.GetGroupByItemType(forItemType);
            return group.HavePlace(forItemType, out availablePlace);
        }
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