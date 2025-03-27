using System;
using System.Collections.Generic;
using DayChangeContent;
using UnityEngine;
using TheSTAR.Data;
using TheSTAR.Sound;
using TheSTAR.GUI;
using TheSTAR.Utility;
using Zenject;

/// <summary>
/// Главный контроллер игры
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private GameWorld worldPrefab;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private BuyerCarsSimulation buyerCarsSimulationPrefab;
    [SerializeField] private BuyersController buyersSpawnerPrefab;
    [SerializeField] private AllBuyerPlaces allBuyerPlacesPrefab;
    [SerializeField] private PriceListInWorld priceListInWorldPrefab;
    [SerializeField] private TownSimulation townSimulationPrefab;
    [SerializeField] private DayChange  _dayChangePrefab;

    private ConfigHelper<GameConfig> gameConfig = new();

    private DataController data;
    private AnalyticsManager analytics;
    private SoundController sounds;
    private GuiController gui;
    private CameraController cameraController;
    private DiContainer diContainer;
    private CurrencyController currency;
    private Delivery delivery;
    private AllPrices allPrices;
    private BuyersController buyersController;
    private XpController xp;
    private TutorialController tutorial;
    private ItemsController items;
    private OrdersManager ordersManager;
    private DayChange  _dayChange;
    private GameWorld createdWorld;
    private Player createdPlayer;
    private AllBuyerPlaces allBuyerPlaces;
    private TownSimulation townSimulation;
    private PriceListInWorld priceListInWorld;
    private bool _startFirstTask = true;

    public bool UseCheats => gameConfig.Get.UseCheats;
    public GameWorld World => createdWorld;

    private bool inAssemblyFocus = false;
    public bool InAssemblyFocus => inAssemblyFocus;
    [Inject]
    private void Construct(
        DataController data,
        AnalyticsManager analytics,
        SoundController sounds,
        GuiController gui,
        CameraController cameraController,
        DiContainer diContainer,
        CurrencyController currency,
        Delivery delivery,
        AllPrices allPrices,
        XpController xp,
        TutorialController tutorial,
        ItemsController items,
        OrdersManager ordersManager,
        AdsManager ads)
    {
        this.data = data;
        this.analytics = analytics;
        this.sounds = sounds;
        this.gui = gui;
        this.cameraController = cameraController;
        this.diContainer = diContainer;
        this.currency = currency;
        this.delivery = delivery;
        this.allPrices = allPrices;
        this.xp = xp;
        this.tutorial = tutorial;
        this.items = items;
        this.ordersManager = ordersManager;

        ads.InitAds();

        ordersManager.OnOrderChangeEvent += (activeOrderData) =>
        {
            /*
            if (tutorial.InTutorial && tutorial.CurrentTutorial == TutorialType.CompleteOrders)
            {
                TriggerTutorial();
            }
            */
        };

        ordersManager.OnOrderCompletedEvent += (b) =>
        {
            data.gameData.levelData.completedOrdersCount++;

            if (!tutorial.IsCompleted(TutorialType.ServeTheQuests))
            {
                if (data.gameData.levelData.completedOrdersCount >= 2)
                    tutorial.CompleteTutorial(TutorialType.ServeTheQuests);
                TriggerTutorial();
            }
        };

        delivery.OnStartDeliveryEvent += (time, index) =>
        {
            if (!tutorial.IsCompleted(TutorialType.FirstDelivery))
            {
                tutorial.CompleteTutorial(TutorialType.FirstDelivery);
                TriggerTutorial();
            }
        };

        delivery.OnCompleteDeliveryEvent += () =>
        {
            if (tutorial.IsCompleted(TutorialType.FirstDelivery) &&
                !tutorial.IsCompleted(TutorialType.GetFirstDelivery))
            {
                TriggerTutorial();
            }
        };

        sounds.OnClearCurrenMusicEvent += PlayRandomMusic;
    }

    /// <summary>
    /// Главная точка входа логики в рамках всей игры
    /// </summary>
    private void Start()
    {
        PlayRandomMusic();
        data.LoadGame();
        InitGame();
        LoadGame();
    }

    private void InitGame()
    {
        data.Init(gameConfig.Get.LockData);
        sounds.Init(() => data.gameData.settingsData.isSoundsOn, () => data.gameData.settingsData.isMusicOn);
        delivery.OnDeleteBox += () =>
        {
            /*
            if (!tutorial.IsCompleted(TutorialType.ThrowBox))
            {
                tutorial.CompleteTutorial(TutorialType.ThrowBox);
                TriggerTutorial();
            }
            */
        };
        xp.OnLevelUpEvent += (level) =>
        {
            //if (!tutorial.IsCompleted(TutorialType.Expand)) TriggerTutorial();

            TriggerTutorial();
        };
    }

    private void LoadGame()
    {
        Debug.Log("StartGame");

        var load = gui.FindScreen<LoadScreen>();
        load.Init(LoadWorld, OnCompleteLoadWorld);
        gui.Show(load);
    }

    //private GameWorldInteraction worldInteraction;

    private void LoadWorld()
    {
        createdPlayer = diContainer.InstantiatePrefabForComponent<Player>(playerPrefab,
            worldPrefab.PlayerSpawnPoint.position, worldPrefab.PlayerSpawnPoint.rotation, null);
        diContainer.Bind<Player>().FromInstance(createdPlayer);

        diContainer.Bind<OrdersGenerator>().AsSingle();
        diContainer.Bind<GameWorldInteraction>().AsSingle();

        allBuyerPlaces = diContainer.InstantiatePrefabForComponent<AllBuyerPlaces>(allBuyerPlacesPrefab);
        diContainer.Bind<AllBuyerPlaces>().FromInstance(allBuyerPlaces).AsSingle();
        allBuyerPlaces.Init();

        priceListInWorld = diContainer.InstantiatePrefabForComponent<PriceListInWorld>(priceListInWorldPrefab);
        diContainer.Bind<PriceListInWorld>().FromInstance(priceListInWorld).AsSingle();

        var buyerCars = diContainer.InstantiatePrefabForComponent<BuyerCarsSimulation>(buyerCarsSimulationPrefab);
        diContainer.Bind<BuyerCarsSimulation>().FromInstance(buyerCars).AsSingle();
        buyerCars.Init();

        buyersController = diContainer.InstantiatePrefabForComponent<BuyersController>(buyersSpawnerPrefab);
        diContainer.Bind<BuyersController>().FromInstance(buyersController).AsSingle();

        createdWorld = diContainer.InstantiatePrefabForComponent<GameWorld>(worldPrefab);
        diContainer.Bind<GameWorld>().FromInstance(createdWorld).AsSingle();
        createdWorld.FastFood.OnChangeAssemblyEvent += OnChangeAssembly;
        Debug.Log("создание GAME WORLD");
        townSimulation = diContainer.InstantiatePrefabForComponent<TownSimulation>(townSimulationPrefab);
        diContainer.Bind<TownSimulation>().FromInstance(townSimulation).AsSingle();
        
        _dayChange = diContainer.InstantiatePrefabForComponent<DayChange>(_dayChangePrefab);
        diContainer.Bind<DayChange>().FromInstance(_dayChange).AsSingle();
        
        createdWorld.Init(gui.Reputation, allBuyerPlaces.BuyerPlacesFurnitureUnitTutor);
        createdWorld.Load();
        gui.InitGameWorld(createdWorld);
        buyersController.InitReputation(gui.TopUiContainer.Reputation);
        tutorial.Init(cameraController.IsObjectInView);
        Debug.Log("31");
        allPrices.Init();
        Debug.Log("35");
        cameraController.SetMainFocus(createdPlayer);

        xp.Load();
        delivery.Load();
        
        Debug.Log("Load world completed");

        // currency.AddCurrency(CurrencyType.Soft, new(10000, 0), true);
        currency.InitReputation(gui.TopUiContainer.Reputation);
    }

    private void OnCompleteLoadWorld()
    {
        gui.Show<MenuScreen>();

        if (!data.gameData.commonData.gameStarted) GiveFirstBalanceToPlayer();

        // load buyer places
        allBuyerPlaces.SetPlaces(data.gameData.levelData.activeBuyerPlaces);

        createdWorld.BakeNavigationSurface();
    }

    private void GiveFirstBalanceToPlayer()
    {
        Debug.Log("GiveFirstBalanceToPlayer");

        // dollars
        currency.AddCurrency(CurrencyType.Soft, gameConfig.Get.StartBalance);

        Debug.Log("start delivery: " + gameConfig.Get.StartDelivery.Count);
        if (gameConfig.Get.StartDelivery.Count > 0)
        {
            delivery.SpawnDeliveryBox(gameConfig.Get.StartDelivery.ToDictionary(), true);
        }

        data.gameData.levelData.activeBuyerPlaces = new();
        var allBuyerPlaceType = EnumUtility.GetValues<BuyerPlaceType>();

        foreach (var placeType in allBuyerPlaceType)
        {
            var costData = gameConfig.Get.BuyerPlaceCostData.Get(placeType);
            bool[] activity = new bool[costData.Length];

            if (gameConfig.Get.StartFurnitureUnits.ContainsKey(placeType))
            {
                for (int i = 0; i < gameConfig.Get.StartFurnitureUnits.Get(placeType); i++) activity[i] = true;
            }

            data.gameData.levelData.activeBuyerPlaces.Add(placeType, activity);
        }

        // распологаем стартовые предметы
        /*
        for (int i = 0; i < gameConfig.Get.StartCoffeeBeans; i++)
        {
            var coffeeBeans = items.CreateItem(ItemType.CoffeeBeans);
            createdWorld.StartItemsHandler.Place(coffeeBeans);
        }
        */

        data.gameData.commonData.gameStarted = true;

        data.Save(DataSectionType.Level);
        data.Save(DataSectionType.Common);
        data.Save(DataSectionType.Currency);

        analytics.Trigger(RepeatingEventType.LevelUp);
    }

    public void StartGame()
    {
        // _dayChange.StartCycle();
        analytics.Trigger(RepeatingEventType.StartSession);
        buyersController.StartSimulate();
        townSimulation.StartSimulate();

        if (!tutorial.IsCompleted(TutorialType.Congratulations))
        {
            Debug.Log("Welcome tutor");

            var dialogScreen = gui.FindScreen<DialogScreen>();
            dialogScreen.Init(() =>
            {
                tutorial.CompleteTutorial(TutorialType.Congratulations);
                gui.ShowMainScreen();
            });
            gui.Show(dialogScreen);
            return;
        }
        else gui.Show<GameScreen>();

        currency.StartSimulateIncomeOffer();
    }

    public void TryBuyPlace(BuyerPlaceType placeType, int index)
    {
        if (data.gameData.levelData.activeBuyerPlaces[placeType][index]) return;

        var cost = gameConfig.Get.BuyerPlaceCostData.Get(placeType)[index].Cost;
        currency.ReduceCurrency(CurrencyType.Soft, cost, () =>
        {
            sounds.Play(SoundType.Oplata_korsini);
            data.gameData.levelData.activeBuyerPlaces[placeType][index] = true;
            allBuyerPlaces.PlaceGroups.Get(placeType).SetPlaces(data.gameData.levelData.activeBuyerPlaces[placeType]);
            createdWorld.BakeNavigationSurface();

            if (placeType == BuyerPlaceType.Sofa)
            {
                analytics.Trigger(RepeatingEventType.Purchase_Sofa);
            }
            else if (placeType == BuyerPlaceType.SofaChair)
            {
                analytics.Trigger(RepeatingEventType.Purchase_SofaAndChair);
            }
            else if (placeType == BuyerPlaceType.SingleChair)
            {
                analytics.Trigger(RepeatingEventType.Purchase_SingleChair);

                if (!tutorial.IsCompleted(TutorialType.BuyChair))
                {
                    tutorial.CompleteTutorial(TutorialType.BuyChair);
                    TriggerTutorial();
                }
            }
            else if (placeType == BuyerPlaceType.DobbleChair)
            {
                analytics.Trigger(RepeatingEventType.Purchase_DoubleChair);
            }

            /*
            if (!tutorial.IsCompleted(TutorialType.Expand))
            {
                tutorial.CompleteTutorial(TutorialType.Expand);
                TriggerTutorial();
            }
            */
        }, () => { sounds.Play(SoundType.Oshibka); });
    }

    [ContextMenu("TriggerTutorial")]
    public void TriggerTutorial()
    {
        /*if (!tutorial.IsCompleted(TutorialType.FirstDelivery))
        {
            tutorial.TryShowInWorld(TutorialType.FirstDelivery,
                new TutorInWorldFocus[] { createdWorld.FastFood.Computer.TutorFocus }, out _);
            return;
        }*/

        if (!tutorial.IsCompleted(TutorialType.GetFirstDelivery))
        {
            var box = delivery.FindBox(ItemType.Bun);

            if (box == null)
            {
                delivery.SpawnDeliveryBox(ItemType.Bun, 8, false, true);
                box = delivery.FindBox(ItemType.Bun);
            }

            if (box) tutorial.TryShowInWorld(TutorialType.GetFirstDelivery, box.TutorFocus, out _);
            return;
        }

        /*if (!tutorial.IsCompleted(TutorialType.GetFirstDelivery))
        {
            var box = delivery.FindBox(ItemType.Bun);
            if (box) tutorial.TryShowInWorld(TutorialType.GetFirstDelivery, box.TutorFocus, out _);
            return;
        }*/

        if (!tutorial.IsCompleted(TutorialType.CutBun))
        {
            var tray = createdWorld.FastFood.TutorSlicedContainerBun;
            tutorial.TryShowInWorld(TutorialType.CutBun, tray.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.ClearTrash))
        {
            var tray = createdWorld.FastFood.Trash;
            tutorial.TryShowInWorld(TutorialType.ClearTrash, tray.TutorFocus);
            return;
        }


        if (!tutorial.IsCompleted(TutorialType.PlacePackingBoxToShelf))
        {
            var playerDraggable = createdPlayer.CurrentDraggable;

            if (playerDraggable == null)
            {
                var box = delivery.FindBox(ItemType.BurgerPackingPaper);

                if (box == null)
                {
                    delivery.SpawnDeliveryBox(ItemType.BurgerPackingPaper, 8, false, true);
                    box = delivery.FindBox(ItemType.BurgerPackingPaper);
                }

                if (box) tutorial.TryShowInWorld(TutorialType.PlacePackingBoxToShelf, box.TutorFocus);
            }
            else
            {
                var box = playerDraggable.GetComponent<Box>();

                if (box && box.ItemType == ItemType.BurgerPackingPaper)
                {
                    if (box is BoxOpenClose boxOpenClose && !boxOpenClose.IsOpen)
                    {
                        tutorial.TryShowInUI(TutorialType.PlacePackingBoxToShelf,
                            gui.FindScreen<GameScreen>().OpenButton.transform);

                        tutorial.TryShowInWorld(TutorialType.PlacePackingBoxToShelf,
                            createdWorld.FastFood.BurgerPackingPaperHandler.TutorFocus);
                    }

                    tutorial.TryShowInWorld(TutorialType.PlacePackingBoxToShelf,
                        createdWorld.FastFood.BurgerPackingPaperHandler.TutorFocus);
                    /*else
                    {
                        tutorial.TryShowInWorld(TutorialType.PlacePackingBoxToShelf,
                            createdWorld.FastFood.BurgerPackingPaperHandler.TutorFocus);
                    }*/
                }
                else
                {
                    var neededBox = delivery.FindBox(ItemType.BurgerPackingPaper);

                    if (neededBox == null)
                    {
                        delivery.SpawnDeliveryBox(ItemType.BurgerPackingPaper, 8, false, true);
                        neededBox = delivery.FindBox(ItemType.BurgerPackingPaper);
                    }

                    if (neededBox) tutorial.TryShowInWorld(TutorialType.PlacePackingBoxToShelf, neededBox.TutorFocus);
                }
            }

            return;
        }

        if (!tutorial.IsCompleted(TutorialType.FirstDelivery))
        {
            Debug.Log("FIRSTDELIVERY ");
            tutorial.TryShowInWorld(TutorialType.FirstDelivery,
                new TutorInWorldFocus[] { createdWorld.FastFood.Computer.TutorFocus }, out _);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.PlaceCutletToTray))
        {
            var playerDraggable = createdPlayer.CurrentDraggable;
            if (playerDraggable == null)
            {
                var box = delivery.FindBox(ItemType.CutletRaw);
                if (box) tutorial.TryShowInWorld(TutorialType.PlaceCutletToTray, box.TutorFocus);
            }
            else
            {
                var box = playerDraggable.GetComponent<Box>();
                if (box && box.ItemType == ItemType.CutletRaw)
                {
                    tutorial.TryShowInWorld(TutorialType.PlaceCutletToTray,
                        createdWorld.FastFood.TutorRawCutletsTray.TutorFocus);
                }
                else
                {
                    var neededBox = delivery.FindBox(ItemType.CutletRaw);
                    if (neededBox) tutorial.TryShowInWorld(TutorialType.PlaceCutletToTray, neededBox.TutorFocus);
                }
            }

            return;
        }

        if (!tutorial.IsCompleted(TutorialType.GetFourCutletsInHands))
        {
            var playerDraggable = createdPlayer.CurrentDraggable;
            if (playerDraggable)
            {
                var itemInHands = playerDraggable.GetComponent<Item>();
                if (!itemInHands || itemInHands.ItemType != ItemType.CutletRaw)
                {
                    // throw
                    var focus = gui.FindScreen<GameScreen>().ThrowButton.transform;
                    tutorial.TryShowInUI(TutorialType.GetFourCutletsInHands, focus.transform);
                    return;
                }
            }

            var tutorRawCutletsHandler = createdWorld.FastFood.TutorRawCutletsTray;
            var draggable = tutorRawCutletsHandler.CurrentDraggable;
            if (draggable != null && draggable.TryGetComponent<Item>(out var item) &&
                item.ItemType == ItemType.CutletRaw)
            {
                tutorial.TryShowInWorld(TutorialType.GetFourCutletsInHands, item.TutorFocus);
            }

            return;
        }

        if (!tutorial.IsCompleted(TutorialType.PlaceCutletToGrill))
        {
            tutorial.TryShowInWorld(TutorialType.PlaceCutletToGrill, createdWorld.FastFood.TutorGriddle.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.FryCutlet))
        {
            tutorial.TryShowInWorld(TutorialType.FryCutlet, new TutorInWorldFocus[] { }, out _);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.TakeCutlet))
        {
            var cutlet = items.FindItem(ItemType.CutletWell);
            if (cutlet == null) cutlet = items.FindItem(ItemType.CutletBurnt);
            if (cutlet != null) tutorial.TryShowInWorld(TutorialType.TakeCutlet, cutlet.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.TakeToAssemblyTable))
        {
            tutorial.TryShowInWorld(TutorialType.TakeToAssemblyTable,
                createdWorld.FastFood.CutletsContainer.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.AssemblyBurger))
        {
            // tutorial.TryShowInWorld(TutorialType.AssemblyBurger, new TutorInWorldFocus[] { }, out _);
            tutorial.TryShowInWorld(TutorialType.AssemblyBurger, createdWorld.FastFood.CuttingBoard, out _);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.SetPrice))
        {
            tutorial.TryShowInWorld(TutorialType.SetPrice,
                priceListInWorld.FindPanel(ItemType.FinalBurger_Small).TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.SetFastFoodName))
        {
            tutorial.TryShowInWorld(TutorialType.SetFastFoodName, createdWorld.FastFood.NameContainer.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.OpenFastFood))
        {
            tutorial.TryShowInWorld(TutorialType.OpenFastFood, createdWorld.FastFood.OpenClosedBoard.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.ExpectBuyers))
        {
            tutorial.BreakTutorial();
            tutorial.TryShowInWorld(TutorialType.ExpectBuyers, new TutorInWorldFocus[] { }, out _);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.AcceptOrder))
        {
            tutorial.TryShowInWorld(TutorialType.AcceptOrder, createdWorld.FastFood.OrderMonitor.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.PutBurgerOnTray))
        {
            tutorial.TryShowInWorld(TutorialType.PutBurgerOnTray, createdWorld.FastFood.OrderTrayFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.PickUpTray))
        {
            tutorial.TryShowInWorld(TutorialType.PickUpTray, createdWorld.FastFood.OrderTrayFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.ServeTheQuests) && tutorial.IsCompleted(TutorialType.AcceptOrder))
        {
            if (_startFirstTask)
            {
                data.gameData.levelData.completedOrdersCount = 0;
                _startFirstTask = false;
            }
            
            tutorial.BreakTutorial(); // чтобы обновилась TaskPanel
            tutorial.TryShowInWorld(TutorialType.ServeTheQuests, new TutorInWorldFocus[] { }, out _);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.ClearTables))
        {
            Debug.Log("1");
            tutorial.BreakTutorial();
            Debug.Log("3");
            TutorInWorldFocus placeTable = new TutorInWorldFocus();
            bool breakOuterLoop = false;
            int amount = 0;
            Debug.Log("5");

            foreach (var tableTutor in createdWorld.BuyerPlacesFurnitureUnitTutor)
            {
                Debug.Log("tableTutor " + tableTutor);

                foreach (var place in tableTutor.Places)
                {
                    Debug.Log("№ " + place.GetTrashActiveCount() + "    " + place.gameObject.name);

                    if (place.GetTrashActiveCount() > 0)
                    {
                        amount = place.GetTrashActiveCount();
                        placeTable = place.GetComponent<TutorInWorldFocus>();
                        breakOuterLoop = true;
                        break;
                    }
                }

                if (breakOuterLoop)
                    break;
            }

            if (amount > 0)
                tutorial.TryShowInWorld(TutorialType.ClearTables, placeTable, out _);

            return;
        }

        if (!tutorial.IsCompleted(TutorialType.BuyChair) && currency.Coins >= new DollarValue(30, 0))
        {
            tutorial.TryShowInWorld(TutorialType.BuyChair, createdWorld.FastFood.Computer.TutorFocus);
            return;
        }

        if (!tutorial.IsCompleted(TutorialType.BuySection) && xp.CurrentLevel >= 3 &&
            currency.Coins >= new DollarValue(100, 0))
        {
            tutorial.TryShowInWorld(TutorialType.BuySection, createdWorld.FastFood.Computer.TutorFocus);
            return;
        }

        // level ups

        if (xp.CurrentLevel == 1 && !tutorial.IsCompleted(TutorialType.UpdateMenu_Cheeseburger))
        {
            tutorial.TryShowInWorld(TutorialType.UpdateMenu_Cheeseburger,
                priceListInWorld.FindPanel(ItemType.FinalBurger_Cheeseburger).TutorFocus);
            return;
        }

        if (xp.CurrentLevel == 2 && !tutorial.IsCompleted(TutorialType.UpdateMenu_burgerM))
        {
            tutorial.TryShowInWorld(TutorialType.UpdateMenu_burgerM,
                priceListInWorld.FindPanel(ItemType.FinalBurger_Medium).TutorFocus);
            return;
        }

        /*if (xp.CurrentLevel == 3 && !tutorial.IsCompleted(TutorialType.UpdateMenu_frenchFries))
        {
            tutorial.TryShowInWorld(TutorialType.UpdateMenu_frenchFries,
                priceListInWorld.FindPanel(ItemType.FinalFrenchFries).TutorFocus);
            return;
        }*/

        if (xp.CurrentLevel == 4 && !tutorial.IsCompleted(TutorialType.UpdateMenu_starburger))
        {
            tutorial.TryShowInWorld(TutorialType.UpdateMenu_starburger,
                priceListInWorld.FindPanel(ItemType.FinalBurger_Star).TutorFocus);
            return;
        }

        if (xp.CurrentLevel == 5 && !tutorial.IsCompleted(TutorialType.UpdateMenu_soda))
        {
            //tutorial.TryShowInWorld(TutorialType.UpdateMenu_soda, priceListInWorld.FindPanel(ItemType.).TutorFocus);
            return;
        }

        if (xp.CurrentLevel == 6 && !tutorial.IsCompleted(TutorialType.UpdateMenu_mega))
        {
            tutorial.TryShowInWorld(TutorialType.UpdateMenu_mega,
                priceListInWorld.FindPanel(ItemType.FinalBurger_Mega).TutorFocus);
            return;
        }
    }

    public void OnPlayerStartDrag(Draggable draggable)
    {
        if (tutorial.InTutorial)
        {
            if (tutorial.CurrentTutorial == TutorialType.GetFirstDelivery)
            {
                var box = draggable.GetComponent<Box>();
                if (box != null && box.ItemType == ItemType.Bun)
                {
                    tutorial.CompleteTutorial(TutorialType.GetFirstDelivery);
                    TriggerTutorial();
                    return;
                }
            }
            else if (tutorial.CurrentTutorial == TutorialType.PlacePackingBoxToShelf)
            {
                var box = draggable.GetComponent<Box>();
                if (box != null && box.ItemType == ItemType.BurgerPackingPaper)
                {
                    TriggerTutorial();
                    return;
                }
            }
            else if (tutorial.CurrentTutorial == TutorialType.PlaceCutletToTray)
            {
                TriggerTutorial();
                return;
            }
            else if (tutorial.CurrentTutorial == TutorialType.PlaceCutletToGrill)
            {
                TriggerTutorial();
                return;
            }
            else if (tutorial.CurrentTutorial == TutorialType.TakeCutlet)
            {
                if (draggable.TryGetComponent<Item>(out var item) &&
                    (item.ItemType == ItemType.CutletWell || item.ItemType == ItemType.CutletBurnt))
                {
                    tutorial.CompleteCurrentTutorial();
                    TriggerTutorial();
                    return;
                }
            }
            else if (tutorial.CurrentTutorial == TutorialType.GetFourCutletsInHands)
            {
                if (draggable.TryGetComponent<Item>(out var item) && (item.ItemType == ItemType.CutletRaw) &&
                    createdPlayer.ItemsInHandsCount >= 4)
                {
                    tutorial.CompleteCurrentTutorial();
                }

                TriggerTutorial();
                return;
            }
        }
    }


    public void OnPlayerEndDrag()
    {
        if (tutorial.InTutorial)
        {
            if (tutorial.CurrentTutorial == TutorialType.PlacePackingBoxToShelf)
            {
                TriggerTutorial();
            }
            else if (tutorial.CurrentTutorial == TutorialType.PlaceCutletToGrill)
            {
                TriggerTutorial();
                return;
            }
            else if (tutorial.CurrentTutorial == TutorialType.GetFourCutletsInHands)
            {
                TriggerTutorial();
                return;
            }
        }


        /*
        if (tutorial.InTutorial && (
            tutorial.CurrentTutorial == TutorialType.PrepareCoffee ||
            tutorial.CurrentTutorial == TutorialType.CompleteOrders))
        {
            TriggerTutorial();
        }
        */
    }

    public void TryBuyCoffeeMachine()
    {
        if (data.gameData.levelData.coffeeMachinePurchased) return;

        currency.ReduceCurrency(CurrencyType.Soft, gameConfig.Get.CoffeeMachineData.Cost, () =>
        {
            sounds.Play(SoundType.Oplata_korsini);
            data.gameData.levelData.coffeeMachinePurchased = true;
            createdWorld.FastFood.UpdateApparatsActivity();
            analytics.Trigger(RepeatingEventType.Purchase_CoffeeMachine);
        }, () => { sounds.Play(SoundType.Oshibka); });
    }

    public void TryBuyDeepFlyer()
    {
        if (data.gameData.levelData.deepFryerPurchased) return;

        currency.ReduceCurrency(CurrencyType.Soft, gameConfig.Get.DeepFryerMachineData.Cost, () =>
        {
            sounds.Play(SoundType.Oplata_korsini);
            data.gameData.levelData.deepFryerPurchased = true;
            createdWorld.FastFood.UpdateApparatsActivity();
            analytics.Trigger(RepeatingEventType.Purchase_DeepFryer);
        }, () => { sounds.Play(SoundType.Oshibka); });
    }

    public void TryBuySodaMachine()
    {
        if (data.gameData.levelData.sodaMachinePurchased) return;

        currency.ReduceCurrency(CurrencyType.Soft, gameConfig.Get.SodaMachineData.Cost, () =>
        {
            sounds.Play(SoundType.Oplata_korsini);
            data.gameData.levelData.sodaMachinePurchased = true;
            createdWorld.FastFood.UpdateApparatsActivity();
            analytics.Trigger(RepeatingEventType.Purchase_SodaMachine);
        }, () => { sounds.Play(SoundType.Oshibka); });
    }

    [ContextMenu("TestBuyExpandZone")]
    private void TestBuyExpandZone()
    {
        TryBuyExpandZone(data.gameData.levelData.purchasedExpandsCount);
    }

    public void TryBuyExpandZone(int expandWallIndex)
    {
        if (expandWallIndex > data.gameData.levelData.purchasedExpandsCount) return;

        var cost = gameConfig.Get.ExpandZonesData[expandWallIndex].Cost;
        currency.ReduceCurrency(CurrencyType.Soft, cost, () =>
        {
            sounds.Play(SoundType.Oplata_korsini);
            data.gameData.levelData.purchasedExpandsCount++;
            createdWorld.FastFood.UpdateExpandWalls();

            analytics.Trigger(RepeatingEventType.Purchase_Section);

            if (!tutorial.IsCompleted(TutorialType.BuySection))
            {
                tutorial.CompleteTutorial(TutorialType.BuySection);
                TriggerTutorial();
            }
        }, () => { sounds.Play(SoundType.Oshibka); });
    }

    public void OnChangeAssembly(List<ItemType> assembly)
    {
        OnChangeAssemblyEvent?.Invoke(assembly);
    }

    public event Action<List<ItemType>> OnChangeAssemblyEvent;

    public void ResetOrderTray(int index)
    {
        createdWorld.ResetOrderTray(index);
    }

    public event Action OnStartAssemblingFocusEvent;
    public event Action OnEndAssemblingFocusEvent;

    public void OnStartAssemblingFocus()
    {
        inAssemblyFocus = true;
        OnStartAssemblingFocusEvent?.Invoke();
    }

    public void OnEndAssemblingFocus()
    {
        inAssemblyFocus = false;
        OnEndAssemblingFocusEvent?.Invoke();
    }

    public void OnWrongOrderOnTray()
    {
        gui.FindScreen<GameScreen>().OnWrongOrderOnTray();
    }

    public void PlayRandomMusic()
    {
        sounds.Play(ArrayUtility.GetRandomValue(gameConfig.Get.RandomMusicTypes));
    }
}

public enum GameVersionType
{
    // в А версии только реварды (не используется)
    VersionA,

    // в Б версии реварды, интеры и баннер
    VersionB
}

[Serializable]
public struct DollarValue
{
    public int dollars;
    public int cents;

    public DollarValue(int simpleValue)
    {
        dollars = simpleValue / 100;
        cents = simpleValue % 100;
    }

    public DollarValue(int d, int c)
    {
        dollars = d;
        cents = c;

        while (cents >= 100)
        {
            cents -= 100;
            dollars++;
        }

        while (cents < 0)
        {
            dollars--;
            cents += 100;
        }
    }

    public static DollarValue operator *(DollarValue a, int multiply)
    {
        return new DollarValue(a.dollars * multiply, a.cents * multiply);
    }

    public static DollarValue operator +(DollarValue a, DollarValue b)
    {
        return new DollarValue(a.dollars + b.dollars, a.cents + b.cents);
    }

    public static DollarValue operator -(DollarValue a, DollarValue b)
    {
        return new DollarValue(a.dollars - b.dollars, a.cents - b.cents);
    }

    public static bool operator >(DollarValue a, DollarValue b)
    {
        if (a.dollars == b.dollars) return a.cents > b.cents;
        else return a.dollars > b.dollars;
    }

    public static bool operator <(DollarValue a, DollarValue b)
    {
        if (a.dollars == b.dollars) return a.cents < b.cents;
        else return a.dollars < b.dollars;
    }

    public static bool operator >=(DollarValue a, DollarValue b)
    {
        if (a.dollars == b.dollars) return a.cents >= b.cents;
        else return a.dollars > b.dollars;
    }

    public static bool operator <=(DollarValue a, DollarValue b)
    {
        if (a.dollars == b.dollars) return a.cents <= b.cents;
        else return a.dollars < b.dollars;
    }

    public static bool operator ==(DollarValue a, DollarValue b)
    {
        return a.dollars == b.dollars && a.cents == b.cents;
    }

    public static bool operator !=(DollarValue a, DollarValue b)
    {
        return a.dollars != b.dollars || a.cents != b.cents;
    }

    public override string ToString()
    {
        return TextUtility.FormatPrice(this);
    }

    public int ToSimpleValue()
    {
        return dollars * 100 + cents;
    }
}