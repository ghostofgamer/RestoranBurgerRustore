using System;
using UnityEngine;

/// <summary>
/// Соотносит ItemType с DraggerGroup
/// </summary>
public class GroupsByTypeHandler : MonoBehaviour
{
    [SerializeField] private DraggerGroup defaultGroup;
    [SerializeField] private UnityDictionary<ItemType, DraggerGroup> specialGroups;

    public DraggerGroup Default => defaultGroup;
    public UnityDictionary<ItemType, DraggerGroup> Special => specialGroups; 

    public DraggerGroup GetGroupByItemType(ItemType itemType)
    {
        if (specialGroups != null && specialGroups.Count > 0)
        {
            foreach (var element in specialGroups.KeyValues)
            {
                if (element.Key == itemType) return element.Value;
            }
        }
        
        return defaultGroup;
    }

    public Action OnTotalEmpty;

    public bool IsTotalEmpty
    {
        get
        {
            if (!defaultGroup.IsEmpty) return false;

            foreach (var group in specialGroups.KeyValues)
            {
                if (!group.Value.IsEmpty) return false;
            }

            return true;
        }
    }

    public Draggable CurrentDraggable
    {
        get
        {
            if (defaultGroup.CurrentDraggable) return defaultGroup.CurrentDraggable;

            foreach (var group in specialGroups.KeyValues)
            {
                if (group.Value.CurrentDraggable) return group.Value.CurrentDraggable;
            }

            return null;
        }
    }

    public void Init()
    {
        defaultGroup.OnEmptyGroup += OnEmptyGroup;
        foreach (var element in specialGroups.KeyValues)
        {
            element.Value.Init();
            element.Value.OnEmptyGroup += OnEmptyGroup;
        }
    }

    private void OnEmptyGroup(DraggerGroup group)
    {
        if (IsTotalEmpty) OnTotalEmpty?.Invoke();
    }
}