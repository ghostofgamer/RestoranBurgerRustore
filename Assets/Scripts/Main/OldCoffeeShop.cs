using System;
using System.Collections.Generic;
using TheSTAR.Data;
using TheSTAR.Utility;
using UnityEngine;
using World;
using Zenject;

[Obsolete]
public class OldCoffeeShop : MonoBehaviour
{
    /*
    [SerializeField] private GameObject movableWall;
    [SerializeField] private OrderMonitor orderMonitor;
    [SerializeField] private BuyersQueue buyersQueue;
    [SerializeField] private ItemsHandler orderPlate;
    [SerializeField] private ItemsHandler cupsOrganizer;
    [SerializeField] private OldCoffeeMachine coffeeMachine;
    [SerializeField] private CoffeeGrinder coffeeGrinder;
    [SerializeField] private OpenClosedBoard openClosedBoard;
    [SerializeField] private Computer computer;

    [Space]
    [SerializeField] private ItemsHandler[] shelfsForCoffeeBeans;
    [SerializeField] private ItemsHandler[] shelfsForDessert;
    [SerializeField] private DraggerGroup[] draggerGroupsToSaveLoad;

    private bool open;
    public bool Open => open;
    public OrderMonitor OrderMonitor => orderMonitor;
    public BuyersQueue BuyersQueue => buyersQueue;
    public ItemsHandler OrderPlate => orderPlate;
    public ItemsHandler CupsOrganizer => cupsOrganizer;
    public ItemsHandler ShelfForPlaceCoffeeBeans
    {
        get
        {
            foreach (var shelf in shelfsForCoffeeBeans)
            {
                if (shelf.HavePlace(ItemType.CoffeeBeans)) return shelf;
            }

            return null;
        }
    }
    public ItemsHandler ShelfForPlaceDessert
    {
        get
        {
            foreach (var shelf in shelfsForDessert)
            {
                if (shelf.HavePlace(ItemType.ChocolateCake)) return shelf;
            }

            return null;
        }
    }
    public OldCoffeeMachine CoffeeMachine => coffeeMachine;
    public CoffeeGrinder CoffeeGrinder => coffeeGrinder;
    public OpenClosedBoard OpenClosedBoard => openClosedBoard;
    public Computer Computer => computer;

    private readonly ConfigHelper<GameConfig> gameConfig = new();

    private bool loaded = false;

    private DataController data;
    private GameController game;

    [Inject]
    private void Consruct(DataController data, GameController game)
    {
        this.data = data;
        this.game = game;
    }
    
    public void Init()
    {
        for (int i = 0; i < draggerGroupsToSaveLoad.Length; i++)
        {
            DraggerGroup itemsContainer = draggerGroupsToSaveLoad[i];
            itemsContainer.OnSetItemEvent += (group, item) => OnChangeDraggerGroup(group);
            itemsContainer.OnEndDragItemEvent += (group, item) => OnChangeDraggerGroup(group);
            itemsContainer.Init(i);
        }

        // load items

        var itemsData = data.gameData.levelData.itemContainers;
        var items = game.World.Items;
        
        for (int i = 0; i < itemsData.Count; i++)
        {
            var groupData = itemsData[i];
            foreach (var itemType in groupData.items)
            {
                var item = items.CreateItem(itemType);
                draggerGroupsToSaveLoad[i].HavePlace(itemType, out var place);
                place.StartDrag(item.Draggable);
            }
        }

        loaded = true;
    }

    public void UseSize(int sizeIndex)
    {
        var sizeData = gameConfig.Get.StoreSizeData[sizeIndex];
        movableWall.transform.localPosition = 
            new Vector3(
                movableWall.transform.localPosition.x, 
                movableWall.transform.localPosition.y, 
                sizeData.wallPosition);

        game.World.BakeNavigationSurface();
    }

    public void SetStatus(bool open)
    {
        this.open = open;
    }

    private void OnChangeDraggerGroup(DraggerGroup draggerGroup)
    {
        if (!loaded) return;

        //Debug.Log("OnChangeDraggerGroup", draggerGroup.gameObject);
        
        if (draggerGroup.Index == -1) return;

        while (data.gameData.levelData.itemContainers.Count <= draggerGroup.Index) data.gameData.levelData.itemContainers.Add(new());
        
        List<ItemType> itemsInDraggerGroup = new();
        foreach (var element in draggerGroup.AllDraggables)
        {
            var item = element.GetComponent<Item>();
            if (!item) continue;
            if (item.ItemType == ItemType.CupSmall || item.ItemType == ItemType.CupMedium || item.ItemType == ItemType.CupBig)
            {
                var cup = item.GetComponent<Cup>();
                for (int i = 0; i < cup.GetCupsInStackCount; i++)
                {
                    itemsInDraggerGroup.Add(item.ItemType);
                }
            }
            else itemsInDraggerGroup.Add(item.ItemType);
        }

        //Debug.Log("Items count: " + itemsInDraggerGroup.Count);

        data.gameData.levelData.itemContainers[draggerGroup.Index].items = itemsInDraggerGroup;
        data.Save(TheSTAR.Data.DataSectionType.Level);
    }
    */
}