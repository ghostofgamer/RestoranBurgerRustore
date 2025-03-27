using System;
using TheSTAR.GUI;
using UnityEngine;
using World;
using Configs;
using TheSTAR.Data;
using UnityEngine.EventSystems;
using TheSTAR.Utility;
using Zenject;

/// <summary>
/// Главный контроллер уровня
/// </summary>
// LevelController не используем в новом проекте
[Obsolete]
public class LevelController : MonoBehaviour
{    
    /*
    private readonly ConfigHelper<LevelsConfig> levelsConfig = new();
    private readonly ConfigHelper<ItemsConfig> itemsConfig = new();

    private LevelData currentLevelConfigData;
    public ItemsConfig ItemsConfig => itemsConfig.Get;
    public LevelsConfig LevelsConfig => levelsConfig.Get;
    
    private int previousOrderID = -1;
    public int GetCurrentOrderID
    {
        get
        {
            previousOrderID++;
            return previousOrderID;
        }
    }

    private ConfigHelper<GameConfig> gameConfig = new ();

    private OldGameWorld createdWorld;
    
    private DataController data;
    private GuiController gui;
    private TutorialController tutorial;
    private XpController xp;
    private SalesController sales;
    private CameraController cameraController;
    private ManufacturingController manufacturing;

    private void StartGame()
    {       
        sales.StartSimulate();

        // game
        gui.Show<MenuScreen>();
    }

    private void LoadLevel(int currentUniqueLevelIndex, int currentTotalLevelIndex, Action completeLoadAction)
    {
        var loadScreen = gui.FindScreen<LoadScreen>();
        var lookAround = gui.FindUniversalElement<LookAroundContainer>();

        tutorial.Init(cameraController.IsObjectInView);

        loadScreen.Init(() =>
        {
            //createdWorld = Instantiate(worldPrefab);

            xp.Load();
            currentLevelConfigData = levelsConfig.Get.GetLevelData(currentUniqueLevelIndex).Data;

            //sales.Init(this, currentLevelConfigData.MinBuyersCount, currentLevelConfigData.MaxBuyersCount, currentLevelConfigData.OrdersPeriodMin, currentLevelConfigData.OrdersPeriodMax);

            //mining.Init();
            createdWorld.LoadWorld(currentTotalLevelIndex);

            //cameraController.SetMainFocus(createdWorld.CurrentPlayer);
            //world.CurrentPlayer.SetCamera(cameraController);

            var gameScreen = gui.FindScreen<GameScreen>();
            
            var topUi = gui.FindUniversalElement<TopUiContainer>();
            xp.SubscribeOnChangeXp(topUi.OnChangeXp);
            xp.OnLevelUpEvent += (newLevel) =>
            {
                createdWorld.CoffeeShop.CoffeeMachine.UpdateCostPanelsByPlayerLevel();
            };

            var boxOfficeScreen = gui.FindScreen<BoxOfficeScreen>();
            boxOfficeScreen.OnUpdateGivingEvent += createdWorld.OrderMonitor.SetGiving;

            //GameController.Instance.Currency.InitReactables(ctrs);

            createdWorld.Delivery.Load();
        
            createdWorld.CoffeeShop.OrderPlate.OnPlaceEvent += () =>
            {   
                /*
                if (!tutorial.IsCompleted(TutorialController.TutorID_MakeFirstCoffee_Final))
                {
                    tutorial.CompleteTutorial(TutorialController.TutorID_MakeFirstCoffee_GiveCoffeeToBuyer);
                    tutorial.CompleteTutorial(TutorialController.TutorID_MakeFirstCoffee_Final);
                    gui.FindScreen<GameScreen>().TriggerTutorial();
                }
                
            };
        }, completeLoadAction);
        gui.Show(loadScreen);
    }

    #region LevelEvents

    [ContextMenu("TestDoLevelEvent")]
    private void TestDoLevelEvent()
    {
        TryStartNextLevelEvent();
    }

    private void TryStartNextLevelEvent(LevelEventType? exception = null)
    {
        int? currentLevelEventIndex = null;
        if (currentLevelEventIndex == null) return;
    }

    #region Tutorial

    private void StopTutorial()
    {
        //currentTutorialLevelEvent = -1;
        //currentManufacturingChainEventIndex = null;
        //currentManufacturingChain = null;
        //tutorial.ClearTutorailGoal();
    }

    #endregion

    public void OnCompleteOrder(Buyer buyer)
    {
        //bool loop = true; // buyer.OrderData.LoopOrder;
        //int? levelEventIndex = null;
        //if (!loop) levelEventIndex = FindStartedLevelEventWithOrder(buyer.OrderData);

        loopOrdersInProcess--;
        //GameController.Instance.Analytics.LogCompleteLevelEvent(GameController.Instance.Levels.CurrentTotalLevelIndex, $"LoopSold {buyer.OrderData.Items[0].ItemType}");

        data.Save(DataSectionType.Level);
    }

    /// <summary>
    /// Выполнил ли игрок все левел ивенты в уровне
    /// </summary>
    public bool IsAllLevelEventsCompleted()
    {
        return true;
    }

    /// <summary>
    /// Возвращает сколько ивентов Заказа считается запущенным
    /// (может быть такое что ивент начался, но заказ вшитый в этот ивент ещё не доступен, потому что покупатель ещё не успел подойти)
    /// </summary>
    public int GetOrderLevelEventsInProcess()
    {
        int orderEventsCount = loopOrdersInProcess;
        return orderEventsCount;
    }

    /// <summary>
    /// Запрос на ивент Заказа
    /// </summary>
    public void CallBuyerToQueue()
    {
        /*
        if (!createdWorld.CoffeeShop.Open) return;

        var order = manufacturing.GenerateLoopOrder(out var success);
        if (!success) return;

        createdWorld.CallBuyerToQueue(order, false);
        loopOrdersInProcess++;
        
    }

    private int loopOrdersInProcess = 0;

    #endregion

    public Sprite ItemIconSprite(ItemType itemType) => itemsConfig.Get.Items[(int)itemType].IconSprite;

    public void OnCoffeeNameContainerClick(CoffeeNameContainer coffeeNameContainer)
    {
        var enterScreen = gui.FindScreen<EnterCoffeeNameScreen>();
        enterScreen.Init(OnAcceptCoffeeShopName);
        gui.Show(enterScreen);
    }
    
    public void OnChangeOpenCloseStatus(bool status)
    {
        createdWorld.CoffeeShop.SetStatus(status);

        //if (status) analytics.Trigger(RepeatingEventType.OpenStore);

        /*
        if (tutorial.InTutorial && tutorial.CurrentTutorialID == TutorialController.TutorID_Open_Store)
        {
            tutorial.CompleteCurrentTutorial();
            gui.FindScreen<GameScreen>().TriggerTutorial();
        }
        
    }

    public void OnClickDebitCard(DebitCard card)
    {
        if (card.Owner != createdWorld.DefaultBuyersQueue.CurrentBuyer) return;

        CalculateOrderCost(out var cost, out _);

        var terminalScreen = gui.FindScreen<TerminalScreen>();
        terminalScreen.Init(cost, OnBuyerPaymentSuccess);
        gui.Show(terminalScreen);

        FocusCameraForPayment(true);
    }

    public void CalculateOrderCost(out DollarValue totalCost, out int xpReward)
    {
        totalCost = new();
        xpReward = 0;
        
        /*

        foreach (var orderItem in createdWorld.CoffeeShop.OrderPlate.AllDraggables)
        {
            var item = orderItem.GetComponent<Item>();
            var itemData = itemsConfig.Get.Items[(int)item.ItemType];

            totalCost += AllPrices.GetPrice(item.ItemType);
            xpReward += itemData.SaleXpReward;
        }
        
    }

    private void OnBuyerPaymentSuccess()
    {
        gui.ShowMainScreen();
        createdWorld.GiveOrderToBuyerAndRewardToPlayer();

        FocusCameraForPayment(false);
    }

    public void OnClickBuyerCash(BuyerCash buyerCash)
    {
        if (buyerCash.Owner != createdWorld.DefaultBuyersQueue.CurrentBuyer) return;

        CalculateOrderCost(out var cost, out _);
        sales.CalculateHaveCash(cost, out var have);

        DollarValue neededGivingValue = have - cost;

        var boxOfficeScreen = gui.FindScreen<BoxOfficeScreen>();
        boxOfficeScreen.Init(neededGivingValue, OnBuyerPaymentSuccess);
        createdWorld.OrderMonitor.SetReceived(have);
        gui.Show(boxOfficeScreen);

        FocusCameraForPayment(true);
    }

    [ContextMenu("TestFocusForPayment")]
    private void TestFocusForPayment()
    {
        FocusCameraForPayment(true);
    }

    [ContextMenu("EndTestFocusForPayment")]
    private void EndTestFocusForPayment()
    {
        FocusCameraForPayment(false);
    }

    private void FocusCameraForPayment(bool focusForPayment)
    {
        cameraController.TempFocus(focusForPayment ? createdWorld.CoffeeShop.OrderMonitor : null, true);
    }

    private void OnAcceptCoffeeShopName(string newName)
    {
        data.gameData.levelData.storeName = newName;
        data.Save(DataSectionType.Level);

        createdWorld.NameContainer.UseNewName(newName);
    }

    [ContextMenu("TestAddXp")]
    private void TestAddXp()
    {
        xp.AddXp(10);
    }

    public void OnCostPanelClick(CostHandler costHandler)
    {
        var enterPriceScreen = gui.FindScreen<EnterPriceScreen>();
        enterPriceScreen.Init(costHandler.CurrentItemType, costHandler.CurrentPrice, (newPrice) =>
        {
            AllPrices.SetPrice(costHandler.CurrentItemType, newPrice);

            if (!tutorial.IsCompleted(TutorialController.TutorID_SetPrice))
            {
                tutorial.CompleteTutorial(TutorialController.TutorID_SetPrice);
            }
        });

        gui.Show(enterPriceScreen);
    }
    */
}