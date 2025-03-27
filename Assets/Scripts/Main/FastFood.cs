using TheSTAR.Data;
using UnityEngine;
using Zenject;
using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using ReputationContent;
using TheSTAR.GUI;
using TheSTAR.Sound;
using TheSTAR.Utility;
using World;

public class FastFood : MonoBehaviour
{
    [SerializeField] private Collider _monitorOrdersCollider;
    [SerializeField] private Trash _trash;
    [SerializeField] private DraggerGroup[] draggerGroupsToSaveLoad;
    [SerializeField] private Player _player;
    [Header("Objects")] [SerializeField] private OpenClosedBoard openClosedBoard;
    [SerializeField] private FastFoodNameContainer nameContainer;
    [SerializeField] private OrderMonitor orderMonitor;
    [SerializeField] private Computer computer;
    [SerializeField] private AssemblyItemsContainer burgerPackingPaperHandler;
    [SerializeField] private Griddle[] griddles;
    [SerializeField] private ItemsHandler tutorRawCutletsTray;
    [SerializeField] private AssemblyItemsContainer _cutletsContainer;
    [SerializeField] private TutorInWorldFocus _cuttingBoard;
    [SerializeField] private DraggerGroup _cutletContainer;
    [SerializeField] private TutorInWorldFocus orderTrayTutorFocus;
    [SerializeField] private GameObject[] _nightLights;

    [Header("Apparats")] [SerializeField] private CoffeeMachine coffeeMachine;

    // [SerializeField] private DeepFryer deepFryer;
    [SerializeField] private SodaMachine sodaMachine;

    [Space] [SerializeField] private GameObject[] expandWallObjects;

    [Space] [SerializeField] private AssemblingBoard assemblingBoard;
    [SerializeField] private OrderTray orderTray;
    [SerializeField] private Transform orderTrayPos;

    [SerializeField] private Transform[] _orderTrayPositions;

    [SerializeField] private AssemblyItemsContainer tutorSlicedContainerBun;
    [SerializeField] private AssemblyItemsContainer[] allAssemblyContainers;
    [SerializeField] private DraggerGroup finalBurgersGroup;
    [SerializeField] private SousContainer ketchupContainer;
    [SerializeField] private SousContainer gorchizaContainer;


    [SerializeField] private OrderTray[] _orderTrays;

    private List<OrderTray> availableTrays = new List<OrderTray>();
    private List<OrderTray> occupiedTrays = new List<OrderTray>();

    private int _currentIndexOrderTray;

    public GameObject[] NightLights => _nightLights;
    public TutorInWorldFocus OrderTrayFocus => orderTrayTutorFocus;
    public OrderMonitor OrderMonitor => orderMonitor;
    public AssemblyItemsContainer CutletsContainer => _cutletsContainer;
    public TutorInWorldFocus CuttingBoard => _cuttingBoard;
    public Trash Trash => _trash;
    public Computer Computer => computer;
    public AssemblyItemsContainer BurgerPackingPaperHandler => burgerPackingPaperHandler;
    public OpenClosedBoard OpenClosedBoard => openClosedBoard;
    public FastFoodNameContainer NameContainer => nameContainer;
    public AssemblyItemsContainer TutorSlicedContainerBun => tutorSlicedContainerBun;
    public Griddle TutorGriddle => griddles[0];
    public ItemsHandler TutorRawCutletsTray => tutorRawCutletsTray;
    public AssemblingBoard AssemblingBoard => assemblingBoard;
    public AssemblyItemsContainer[] AllAssemblyContainers => allAssemblyContainers;
    public DraggerGroup FinalBurgersGroup => finalBurgersGroup;
    public SousContainer KetchupContainer => ketchupContainer;
    public SousContainer GorchizaContainer => gorchizaContainer;

    private GameController game;
    private DataController data;
    private ItemsController items;
    private OrdersManager orders;
    private TutorialController tutorial;
    private SoundController sounds;
    private AllPrices _allPrices;

    public event Action<List<ItemType>> OnChangeAssemblyEvent;

    public event Action OrderCompleted;

    public event Action OverpaymentMaded;

    public event Action RestaurantOpened;

    private List<Griddle> griddlesInProcess = new();
    [SerializeField] private Reputation _reputation;


    private readonly ConfigHelper<ItemsConfig> itemsConfig = new();
    private ItemData itemData;

    private void Start()
    {
        availableTrays.AddRange(_orderTrays);
        _cutletContainer.OnAnyChangeEvent += (group) => OnChangeDraggerGroup(group);
        // StartCoroutine(FindReputationWithDelay(3.0f));
    }

    /*private IEnumerator FindReputationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _reputation = FindObjectOfType<Reputation>();

        if (_reputation != null)
        {
            Debug.Log("Reputation object found!");
        }
        else
        {
            Debug.Log("Reputation object not found.");
        }
    }*/

    [Inject]
    private void Construct(
        DataController data,
        GameController game,
        ItemsController items,
        TutorialController tutorial,
        BuyersController buyers,
        OrdersManager orders,
        GuiController gui,
        SoundController sounds,
        AllPrices allPrices)
    {
        this.game = game;
        this.data = data;
        this.items = items;
        this.orders = orders;
        this.tutorial = tutorial;
        this.sounds = sounds;

        _allPrices = allPrices;
        openClosedBoard.OnChangeStatusEvent += (status) =>
        {
            buyers.SetOpen(status);
            RestaurantOpened?.Invoke();

            if (!tutorial.IsCompleted(TutorialType.OpenFastFood))
            {
                tutorial.CompleteTutorial(TutorialType.OpenFastFood);
                game.TriggerTutorial();
            }
        };

        var boxOfficeScreen = gui.FindScreen<BoxOfficeScreen>();
        boxOfficeScreen.OnUpdateGivingEvent += orderMonitor.SetGiving;

        this.orders.OnOrderCompletedEvent += (buyer) =>
        {
            /*Debug.Log("заказ выполнен " + buyer.gameObject.name);
            Debug.Log("место номер " + buyer.Place.Index);

            /*foreach (var orderTyp in _orderTrays)
            {
                /*Debug.Log(orderTyp.gameObject.name);
                Debug.Log(orderTyp.NumberTable);#2#
            }#1#

            int amountActiveOrders = 0;

            foreach (var newOrderTray in _orderTrays)
            {
                Debug.Log("заказ IndeTray" + newOrderTray.IndexOrderTray);
                Debug.Log("заказ покупатель " + newOrderTray.Buyer.gameObject.name);
                Debug.Log("заказ СТОЛ " + newOrderTray.Buyer.Place.Index);

                if (newOrderTray.IndexOrderTray <= 0)
                    amountActiveOrders++;
            }

            Debug.Log("AMOUNTORDERSACTIVITY " + amountActiveOrders);*/

            int currentIndexOrderTry = -1;

            for (int i = 0; i < _orderTrays.Length; i++)
            {
                if (buyer.Place.Index == _orderTrays[i].NumberTable)
                    currentIndexOrderTry = i;
            }

            Debug.Log("currentIndexOrderTry " + currentIndexOrderTry);


            /*if (amountActiveOrders > 0)
            {
                for (int i = 0; i < this.orders.ActiveOrders.Count; i++)
                {
                    if (this.orders.ActiveOrders[i].IndexOrderTray <= 0)
                    {
                        _orderTrays[currentIndexOrderTry].SetOrder(this.orders.ActiveOrders[i]);
                        _orderTrays[currentIndexOrderTry].AddBuyer(this.orders.ActiveOrders[i].Buyer);
                        _orderTrays[currentIndexOrderTry]
                            .SetValueCurrentIndexTable(this.orders.ActiveOrders[i].Place.Index);
                        this.orders.ActiveOrders[i].SetIndexOrderTray(_orderTrays[currentIndexOrderTry].IndexOrderTray);
                        break;
                    }
                }
            }
            else
            {
                _orderTrays[currentIndexOrderTry].Clear();
                _orderTrays[currentIndexOrderTry].SetValueCurrentIndexTable(-1);
            }*/


            _orderTrays[currentIndexOrderTry].Clear();
            _orderTrays[currentIndexOrderTry].SetValueCurrentIndexTable(-1);
            _orderTrays[currentIndexOrderTry].ClearBuyer();

            Debug.Log("ПРОВЕРИМ КАК ТУТ " + currentIndexOrderTry);

            int value = 0;

            foreach (var trayOrder in _orderTrays)
            {
                if (trayOrder.Buyer == null)
                    value++;
            }

            orderMonitor.SetActiveCanvas(value > 0);
            _monitorOrdersCollider.enabled = value > 0;

            Debug.Log("Value " + value);

            /*_reputation.IncreaseReputation();*/

            OrderCompleted?.Invoke();

            /*if (this.orders.ActiveOrders.Count > 0) orderTray.SetOrder(this.orders.ActiveOrders[0]);
            else orderTray.Clear();*/
        };

        this.orders.OnOrderAcceptedEvent += (orderData) =>
        {
            orderMonitor.ClearOrder();
            Debug.Log("заказ принят " + orderData);

            foreach (var orderTray in _orderTrays)
            {
                if (orderTray.Buyer == null)
                {
                    for (int i = 0; i < this.orders.ActiveOrders.Count; i++)
                    {
                        Debug.Log(this.orders.ActiveOrders[i].IndexOrderTray);

                        if (this.orders.ActiveOrders[i].IndexOrderTray <= 0)
                        {
                            Debug.Log("Принял заказ на нужный поднос " + i + " ... " + orderTray.gameObject.name);
                            Debug.Log("IndeTray" + this.orders.ActiveOrders[i].IndexOrderTray);
                            Debug.Log("покупатель " + this.orders.ActiveOrders[i].Buyer.gameObject.name);
                            Debug.Log("СТОЛ " + this.orders.ActiveOrders[i].Buyer.Place.Index);


                            orderTray.SetOrder(this.orders.ActiveOrders[i]);
                            orderTray.AddBuyer(this.orders.ActiveOrders[i].Buyer);
                            orderTray.SetValueCurrentIndexTable(this.orders.ActiveOrders[i].Place.Index);
                            this.orders.ActiveOrders[i].SetIndexOrderTray(orderTray.IndexOrderTray);


                            itemData = itemsConfig.Get.Item(this.orders.ActiveOrders[i].OrderData.Items[0].ItemType);
                            DollarValue currentPrice =
                                _allPrices.GetPrice(this.orders.ActiveOrders[i].OrderData.Items[0].ItemType);
                            Debug.Log("За эту сумму купили!!! " + currentPrice);
                            // if(itemData.CostData.SellCostMaxRecommendation)
                            // Debug.Log("Recommended Price: " + itemData.CostData.SaleCostRec.dollars + "." + itemData.CostData.SaleCostRec.cents.ToString("D2") + " USD");
                            if (currentPrice > itemData.CostData.SellCostMaxRecommendation)
                            {
                                OverpaymentMaded?.Invoke();
                                Debug.Log("ПЕРЕПЛАТАААА");
                            }

                            Debug.Log("ЧТО Ты ЗАКАЗАЛ  " + this.orders.ActiveOrders[i].OrderData.Items[0].ItemType);
                        }
                    }

                    // var newOrder = this.orders.ActiveOrders[0];
                    /*_orders.SetOrder(this.orders.ActiveOrders[0]);
                    _orders.AddBuyer(this.orders.ActiveOrders[0].Buyer);*/
                    break;
                }
            }

            int value = 0;

            foreach (var trayOrder in _orderTrays)
            {
                if (trayOrder.Buyer == null)
                    value++;
            }

            orderMonitor.SetActiveCanvas(value > 0);
            _monitorOrdersCollider.enabled = value > 0;

            Debug.Log("Value " + value);

            // orderTray.SetOrder(this.orders.ActiveOrders[0]);
        };

        this.orders.OnSetCurrentBuyerWithOrderEvent += (buyer) => { orderMonitor.SetOrder(buyer, buyer.OrderData); };

        availableTrays.AddRange(_orderTrays);
    }

    private bool loaded = false;

    public void Init(Reputation reputation)
    {
        _reputation = reputation;

        finalBurgersGroup.Init();

        for (int i = 0; i < draggerGroupsToSaveLoad.Length; i++)
        {
            DraggerGroup itemsContainer = draggerGroupsToSaveLoad[i];
            itemsContainer.OnAnyChangeEvent += (group) => OnChangeDraggerGroup(group);
            itemsContainer.Init(i);
        }

        nameContainer.Init();

        assemblingBoard.OnChangeAssemblyEvent += (assembly) => { this.OnChangeAssemblyEvent?.Invoke(assembly); };

        //assemblingBoard.OnFinalize += () =>
        //{
        /*
        if (!tutorial.IsCompleted(TutorialType.AssemblyBurger))
        {
            tutorial.CompleteTutorial(TutorialType.AssemblyBurger);
            game.TriggerTutorial();
        }
        */
        //};

        foreach (var griddle in griddles)
        {
            griddle.OnCompleteGriddlingEvent += OnCompleteGriddling;
        }

        UpdateApparatsActivity();
        UpdateExpandWalls();

        foreach (var griddle in griddles)
        {
            griddle.OnStartGriddlingEvent += OnStartGriddling;
            griddle.OnFinishGriddlingEvent += OnFinishGriddling;
        }
    }

    private void OnStartGriddling(Griddle griddle)
    {
        if (griddlesInProcess.Contains(griddle)) return;

        griddlesInProcess.Add(griddle);

        if (griddlesInProcess.Count == 1) sounds.Play(SoundType.Jarim_myaso);
    }

    private void OnFinishGriddling(Griddle griddle)
    {
        if (!griddlesInProcess.Contains(griddle)) return;

        griddlesInProcess.Remove(griddle);

        if (griddlesInProcess.Count == 0) sounds.Stop(SoundType.Jarim_myaso);
    }

    public void LoadItems()
    {
        // load items
        var itemsData = data.gameData.levelData.itemContainers;

        for (int i = 0; i < itemsData.Count; i++)
        {
            if (i >= draggerGroupsToSaveLoad.Length)
            {
                Debug.LogError("Не удалось загрузить предмет в группу " + i);
                break;
            }

            var groupData = itemsData[i];

            foreach (var itemInGameData in groupData.items)
            {
                if (itemInGameData is EmbeddableItemInGameData embeddableItemInGameData)
                {
                    var count = embeddableItemInGameData.count;
                    for (int stackIndex = 0; stackIndex < count; stackIndex++)
                    {
                        draggerGroupsToSaveLoad[i].HavePlace(itemInGameData.itemType, out var place);
                        var item = items.CreateItem(itemInGameData.itemType, place.transform.position);
                        place.StartDrag(item.Draggable);
                    }
                }
                else
                {
                    draggerGroupsToSaveLoad[i].HavePlace(itemInGameData.itemType, out var place);
                    var item = items.CreateItem(itemInGameData.itemType, place.transform.position);
                    place.StartDrag(item.Draggable);
                }
            }
        }

        foreach (var slicedContainer in allAssemblyContainers)
        {
            slicedContainer.LoadCurrentCutType();
        }

        loaded = true;
    }

    public void SetValueNightLight(bool value)
    {
        foreach (var nightLight in _nightLights)
            nightLight.SetActive(value);
    }

    public void OnChangeDraggerGroup(DraggerGroup draggerGroup)
    {
        if (!loaded) return;

        //Debug.Log("OnChangeDraggerGroup " + draggerGroup.gameObject.name, draggerGroup.gameObject);

        if (draggerGroup.Index == -1) return;

        while (data.gameData.levelData.itemContainers.Count <= draggerGroup.Index)
            data.gameData.levelData.itemContainers.Add(new());

        List<ItemInGameData> itemsInDraggerGroup = new();
        foreach (var element in draggerGroup.AllDraggables)
        {
            var item = element.GetComponent<Item>();
            if (!item) continue;

            var embeddableItem = item.GetComponent<EmbeddableItem>();
            if (embeddableItem)
                itemsInDraggerGroup.Add(new EmbeddableItemInGameData(item.ItemType,
                    embeddableItem.GetEmbeddableItemsInStackCount));
            else itemsInDraggerGroup.Add(new ItemInGameData(item.ItemType));
        }

        //Debug.Log("Items count: " + itemsInDraggerGroup.Count);

        data.gameData.levelData.itemContainers[draggerGroup.Index].items = itemsInDraggerGroup;
        data.Save(DataSectionType.Level);
    }

    public void ResetOrderTray(int index)
    {
        _orderTrays[index - 1].Draggable.CurrentDragger.EndDrag();
        _orderTrays[index - 1].transform.position = _orderTrayPositions[index - 1].transform.position;
        _orderTrays[index - 1].transform.rotation = _orderTrayPositions[index - 1].transform.rotation;
        _orderTrays[index - 1].Clear();

        /*orderTray.Draggable.CurrentDragger.EndDrag();
        orderTray.transform.position = orderTrayPos.transform.position;
        orderTray.transform.rotation = orderTrayPos.transform.rotation;
        orderTray.Clear();*/
    }

    private void OnCompleteGriddling()
    {
        if (!tutorial.IsCompleted(TutorialType.FryCutlet))
        {
            tutorial.CompleteTutorial(TutorialType.FryCutlet);
            game.TriggerTutorial();
            return;
        }
    }

    public void UpdateApparatsActivity()
    {
        coffeeMachine.gameObject.SetActive(data.gameData.levelData.coffeeMachinePurchased);
        // deepFryer.gameObject.SetActive(data.gameData.levelData.deepFryerPurchased);
        sodaMachine.gameObject.SetActive(data.gameData.levelData.sodaMachinePurchased);
    }

    public void UpdateExpandWalls()
    {
        for (int i = 0; i < expandWallObjects.Length; i++)
        {
            expandWallObjects[i].gameObject.SetActive(i >= data.gameData.levelData.purchasedExpandsCount);
        }
    }

    /// <summary>
    /// Возвращает контейнер, в котором лежит предмет нужного типа
    /// </summary>
    public AssemblyItemsContainer FindAssemblyItemsContainerByType(ItemType neededItemType)
    {
        foreach (var container in allAssemblyContainers)
        {
            var outItem = container.AutoGetItem(assemblingBoard.Deep > 0);
            if (outItem != null && outItem.ItemType == neededItemType) return container;
        }

        return null;
    }
}