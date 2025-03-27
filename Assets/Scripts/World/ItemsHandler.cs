using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Представляет некий контейнер предметов, куда игрок может положить предметы
/// </summary>
public class ItemsHandler : MonoBehaviour
{
    [field:SerializeField]public bool IsAssemblyTable { get; private set; }
    [field:SerializeField]public bool IsCutletRawContainer { get; private set; }
    
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private bool useSplitter = true;
    [SerializeField] protected DraggerSplitter splitter;
    [SerializeField] private DraggerGroup group;

    [Space]
    [SerializeField] private bool useSectionFilter;
    [SerializeField] private ItemSectionType[] sectionFilters;

    [Space]
    [SerializeField] private TutorInWorldFocus tutorFocus;

    public event Action OnUpdateDraggablesEvent; // изменилось расположение предметов
    public event Action<Draggable> OnPlaceEvent;
    public event Action OnEndDragEvent;

    public List<Draggable> AllDraggables
    {
        get
        {
            if (useSplitter) return splitter.AllDraggables;
            else return group.AllDraggables;
        }   
    }

    public Draggable CurrentDraggable
    {
        get
        {
            if (useSplitter) return splitter.CurrentDraggable;
            else return group.CurrentDraggable;
        }
    }

    public bool UseSectionFilter => useSectionFilter;
    public ItemSectionType[] SectionFilters => sectionFilters;
    public TutorInWorldFocus TutorFocus => tutorFocus;

    protected GameWorldInteraction gameWorldInteraction;
 
    [Inject]
    protected void Construct(GameWorldInteraction gameWorldInteraction)
    {
        this.gameWorldInteraction = gameWorldInteraction;
    }

    public bool HavePlace(ItemType forItemType) => HavePlace(forItemType, out _);
    public bool HavePlace(ItemType forItemType, out Dragger availablePlace)
    {
        bool result;
        
        if (useSplitter)
        {
            Debug.Log("use Splitter");
            result = splitter.HavePlace(forItemType, out availablePlace);
            
        }
        else
        {
            Debug.Log(" DONT use Splitter");
            result = group.HavePlace(forItemType, out availablePlace);
        } 
        
        return result;
        
        /*if (useSplitter) result = splitter.HavePlace(forItemType, out availablePlace);
        else result = group.HavePlace(forItemType, out availablePlace);
        return result;*/
    }
    
    public bool HavePlaceCoffeeCup(ItemType forItemType, out Dragger availablePlace)
    {
        bool result;

        if (useSplitter)
        {
            Debug.Log("use Splitter");
            result = splitter.HavePlace(forItemType, out availablePlace);
            
        }
        else
        {
            Debug.Log(" DONT use Splitter " ) ;
            
            if (forItemType == ItemType.CoffeeCup||forItemType == ItemType.SodaCup)
            {
                result = group.HavePlaceCoffee(forItemType, out availablePlace);
            }
            else
            {
                
                result = group.HavePlace(forItemType, out availablePlace);
            }
        } 
        
        return result;
    }

    public bool CanPlaceBySectionType(ItemSectionType sectionType)
    {
        if (!useSectionFilter) return true;
        
        foreach (var filter in sectionFilters)
        {
            if (filter == sectionType) return true;
        }
        
        return false;
    }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (useSplitter) splitter.Init();
        else group.Init();

        touchInteractive.OnClickEvent += () =>
        {
            gameWorldInteraction.OnItemsHandlerClick(this);
        };

        if (useSplitter)
        {
            splitter.OnAnyChangeEvent += () =>
            {
                OnUpdateDraggablesEvent?.Invoke();
            };
            splitter.OnEndDragEvent += (e) => OnEndDrag();
            splitter.OnSetItemEvent += (dragger, e) => OnPlaceEvent?.Invoke(e);
        }
        else
        {
            group.OnAnyChangeEvent += (group) =>
            {
                OnUpdateDraggablesEvent?.Invoke();
            };
            group.OnEndDragItemEvent += (group, dragger, e) => OnEndDrag();
            group.OnSetItemEvent += (group, dragger, d) => OnPlaceEvent?.Invoke(d);
        }
    }

    private void OnEndDrag()
    {
        OnEndDragEvent?.Invoke();
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        touchInteractive.Activate();

        if (useSplitter) splitter.ForceActivateColForDraggables();
        else group.ForceActivateColForDraggables();
    }

    [ContextMenu("Deactivate")]
    public void Deactivate()
    {
        touchInteractive.Deactivate();

        if (useSplitter) splitter.ForceDeactivateColForDraggables();
        else group.ForceDeactivateColForDraggables();
    }
}