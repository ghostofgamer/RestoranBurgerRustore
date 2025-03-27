    using System;
    using System.Collections;
    using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AssemblyItemsContainer : MonoBehaviour
{
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private UnityDictionary<CutType, CutVariantData[]> _cutDatas = new(); // какие предметы во что преобразуются
    [SerializeField] private GroupsByTypeHandler groups;
    [SerializeField] private FastFood _fastFood;
    [SerializeField] private DraggerGroup _draggerGroup;
    [SerializeField] private bool _newSaveContainer;
    [SerializeField] private GameObject _smallBurgerPrefab;
    [SerializeField] private GameObject _cheeseBurgerPrefab;
    [SerializeField] private GameObject _mBurgerPrefab;
    [SerializeField] private GameObject _starBurgerPrefab;
    [SerializeField] private GameObject _bigBurgerPrefab;
    [SerializeField] private GameObject _megaBurgerPrefab;
    
    [Space]
    [SerializeField] private TutorInWorldFocus tutorFocus;

    private GameWorldInteraction gameWorldInteraction;
    private ItemsController items;
    private Coroutine _coroutine;
    
    public TutorInWorldFocus TutorFocus => tutorFocus;
    public TouchInteractive TouchInteractive => touchInteractive;

    public UnityDictionary<CutType, CutVariantData[]> CutData => _cutDatas;
    
    private CutType? currentCutType = null;

    public bool IsEmpty => currentCutType == null;
    public CutType? CurrentCutType => currentCutType;

    public GameObject InterBurger(Item item)
    {
        Debug.Log("ПЕРЕДАЧА " + item.ItemType);
        GameObject burger = new GameObject();
        
        switch (item.ItemType)
        {
          case ItemType.FinalBurger_Small:
              burger = Instantiate(_smallBurgerPrefab);
              burger.GetComponent<DraggableByPlayer>().InitGameWorldInteraction(gameWorldInteraction);
              return burger;
              break;

          case ItemType.FinalBurger_Cheeseburger:
              burger = Instantiate(_cheeseBurgerPrefab);
              burger.GetComponent<DraggableByPlayer>().InitGameWorldInteraction(gameWorldInteraction);
              return burger;
              break;
          
          case ItemType.FinalBurger_Medium:
              burger = Instantiate(_mBurgerPrefab);
              burger.GetComponent<DraggableByPlayer>().InitGameWorldInteraction(gameWorldInteraction);
              return burger;
              break;
          
          case ItemType.FinalBurger_Star:
              burger = Instantiate(_starBurgerPrefab);
              burger.GetComponent<DraggableByPlayer>().InitGameWorldInteraction(gameWorldInteraction);
              return burger;
              break;
          
          case ItemType.FinalBurger_Big:
              burger = Instantiate(_bigBurgerPrefab);
              burger.GetComponent<DraggableByPlayer>().InitGameWorldInteraction(gameWorldInteraction);
              return burger;
              break;
          
          case ItemType.FinalBurger_Mega:
              burger = Instantiate(_megaBurgerPrefab);
              burger.GetComponent<DraggableByPlayer>().InitGameWorldInteraction(gameWorldInteraction);
              return burger;
              break;
        }

        Debug.Log("ПОСЛЕ " + item.ItemType);
        return null;
        /*var burger = Instantiate(_smallBurgerPrefab);
        burger.GetComponent<DraggableByPlayer>().InitGameWorldInteraction(gameWorldInteraction);
        return burger;*/
    }
    
    private Dictionary<ItemType, DraggerGroup> GetCurrentGroups()
    {
        Dictionary<ItemType, DraggerGroup> resultGroups = new();

        if (currentCutType == null) return resultGroups;

        List<ItemType> haveItemTypesInCutVariant = new();

        var cutData = _cutDatas.Get((CutType)currentCutType);
        foreach (var cutVariant in cutData)
        {
            foreach (var element in cutVariant.To)
            {
                if (!haveItemTypesInCutVariant.Contains(element)) haveItemTypesInCutVariant.Add(element);
            }
        }
        
        foreach (var groupType in haveItemTypesInCutVariant)
        {
            resultGroups.Add(groupType, groups.GetGroupByItemType(groupType));
        }

        return resultGroups;
    }

    /// <summary>
    /// Автоматически вернуть айтем
    /// </summary>
    public Item AutoGetItem(bool assemblyStarted)
    {
        if (currentCutType == null) return null;
        var currentGroups = GetCurrentGroups();
        if (currentGroups.Count == 0) return null;

        if (currentCutType == CutType.CutBun)
        {
            if (!assemblyStarted)
            {
                var draggable = currentGroups[ItemType.BunBottom].CurrentDraggable;
                if (draggable == null) return null;
                return draggable.GetComponent<Item>();
            }
            else
            {
                var draggable = currentGroups[ItemType.BunTop].CurrentDraggable;
                if (draggable == null) return null;
                return draggable.GetComponent<Item>();
            }
        }
        else
        {
            foreach (var group in currentGroups)
            {
                if (group.Value.CurrentDraggable)
                {
                    if (currentCutType == CutType.PackingPaper)
                    {
                        return group.Value.CurrentDraggable.GetComponent<EmbeddableItem>().GetLastItem().Item;
                    }
                    else return group.Value.CurrentDraggable.GetComponent<Item>();
                }
            }
        }

        return null;
    }

    [Inject]
    protected void Construct(GameWorldInteraction gameWorldInteraction, ItemsController items)
    {
        this.gameWorldInteraction = gameWorldInteraction;
        this.items = items;
    }

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Загрузить текущий CutType исходя из содержимого в контейнере
    /// </summary>
    public void LoadCurrentCutType()
    {
        var currentItem = groups.CurrentDraggable;
        if (currentItem == null) currentCutType = null;
        else currentCutType = FindCutTypeByItemType(currentItem.GetComponent<Item>().ItemType);
    }

    private CutType? FindCutTypeByItemType(ItemType itemType)
    {
        foreach (var cutData in _cutDatas.KeyValues)
        {
            foreach (var cutVariantData in cutData.Value)
            {
                foreach (var toVariant in cutVariantData.To)
                {
                    if (toVariant == itemType) return cutData.Key;
                }
            }
        }

        return null;
    }

    private void Init()
    {
        touchInteractive.OnClickEvent += () =>
        {
            if (_newSaveContainer)
            {
                StartSaveCutlets();
                Debug.Log("ложим");
                // _fastFood.OnChangeDraggerGroup(_draggerGroup);
            }
            
            gameWorldInteraction.OnSlicedContainerClick(this);
        };

        groups.Init();

        groups.OnTotalEmpty += () =>
        {
            currentCutType = null;
        };
    }
    
    private void StartSaveCutlets()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(PauseSave());
    }
    
    private IEnumerator PauseSave()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("ложим");
        _fastFood.OnChangeDraggerGroup(_draggerGroup);
    }
    
    public void TryPlace(Item item, out bool success)
    {
        success = false;
        
        if (!CanUse(item.ItemType, out var cutType, out var cutVariant)) return;

        var fromItemType = item.ItemType;
        var toItemTypes = cutVariant.To;
        var portionsCount = items.GetItemData(fromItemType).OtherData.PortionsValue;
        item.Draggable.CurrentDragger.EndDrag();
        var previousItemPosition = item.transform.position;
        var previousItemRotation = item.transform.rotation;

        if (toItemTypes.Length == 1 && toItemTypes[0] == item.ItemType)
        {
            groups.GetGroupByItemType(item.ItemType).GetAllEmptyPlaces(out var availablePlaces);
            availablePlaces[0].StartDrag(item.Draggable);
            availablePlaces.RemoveAt(0);
        }
        else
        {
            Destroy(item.gameObject);

            foreach (var toItemType in toItemTypes)
            {
                groups.GetGroupByItemType(toItemType).GetAllEmptyPlaces(out var availablePlaces);

                for (int i = 0; i < portionsCount; i++)
                {
                    var newItem = items.CreateItem(toItemType, previousItemPosition, previousItemRotation);
                    availablePlaces[0].StartDrag(newItem.Draggable);
                    availablePlaces.RemoveAt(0);
                }
            }
        }

        currentCutType = cutType;
        success = true;
    }

    /// <summary>
    /// Можно ли расположить айтем (с учётом того, к какой нарезке сейчас привязан контейнер + с учётом заполненности драггеров)
    /// </summary>
    public bool CanUse(ItemType itemType, out CutType? cutType, out CutVariantData cutVariant)
    {
        cutType = null;
        bool checkByCutType = false;

        CutType newCutType = default;
        cutVariant = null;
        bool found = false;

        if (currentCutType == null)
        {
            // ищем newCutType
            foreach (var cutData in _cutDatas.KeyValues)
            {
                foreach (var variant in cutData.Value)
                {
                    if (variant.From == itemType)
                    {
                        newCutType = cutData.Key;
                        checkByCutType = true;
                        cutVariant = variant;
                        found = true;
                        break;
                    }
                }

                if (found) break;
            }
        }
        else
        {
            var cutVariants = _cutDatas.Get((CutType)currentCutType);

            foreach (var variant in cutVariants)
            {
                if (variant.From == itemType)
                {
                    checkByCutType = true;
                    newCutType = (CutType)currentCutType;
                    cutVariant = variant;
                    break;
                }
            }
        }

        if (!checkByCutType) return false;

        var portionsCount = items.GetItemData(itemType).OtherData.PortionsValue;
        
        foreach (var itemTypeTo in cutVariant.To)
        {
            var group = groups.GetGroupByItemType(itemTypeTo);
            if (group.GetAllEmptyPlaces(out _) < portionsCount) return false;
        }

        cutType = newCutType;

        return true;
    }
}

[Serializable]
public class CutVariantData
{
    [SerializeField] private ItemType from;
    [SerializeField] private ItemType[] to;

    public ItemType From => from;
    public ItemType[] To => to;
}

public enum CutType
{
    CutBun,
    CutCheese,
    CutTomato,
    CutOnion,
    CutCabbage,
    CutCutlet,
    PackingPaper
}