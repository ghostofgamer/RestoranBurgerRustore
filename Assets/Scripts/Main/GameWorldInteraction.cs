using TheSTAR.GUI;
using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using System;
using TheSTAR.Utility;
using Configs;
using System.Collections.Generic;
using TheSTAR.Data;
using TheSTAR.Sound;


public partial class GameWorldInteraction
{
    private GameController game;
    private DataController data;
    private GuiController gui;
    private Player player;
    private CameraController camera;
    private OrdersManager ordersManager;
    private ItemsController items;
    private AllPrices allPrices;
    private CurrencyController currency;
    private XpController xp;
    private TutorialController tutorial;
    private AnalyticsManager analytics;
    private SoundController sounds;
    private GameController _gameController;

    private event Action OnAnyChangePlayerDraggerEvent;

    private readonly ConfigHelper<ItemsConfig> itemsConfig = new();

    private int currentLayerMask;
    public int CurrentInteractionLayerMask => currentLayerMask;

    // с какими физическими слоями игрок может взаимодействовать в определённых условиях в игре
    private readonly Dictionary<PlayerInteractionScenario, string[]> layersForInteractionScenarios = new()
    {
        {
            PlayerInteractionScenario.EmptyHands,
            new string[] { "Default", "Item", "Box", "Machine", "Buyer", "AssemblyTable" }
        },
        { PlayerInteractionScenario.ClosedBoxInHands, new string[] { "Default", "Bin" } },
        { PlayerInteractionScenario.OpenBoxInHands, new string[] { "Default", "ItemsContainer", "Bin" } },
        { PlayerInteractionScenario.EmptyBoxInHands, new string[] { "Default", "Bin" } },
        {
            PlayerInteractionScenario.DefaultItemInHands,
            new string[] { "Default", "ItemsContainer", "Machine", "Bin", "Buyer" }
        },
        {
            PlayerInteractionScenario.EmptySodaCupDefaultItemInHands,
            new string[] { "SodaFiller", "Default", "ItemsContainer", "Bin", "Buyer" }
        },
        { PlayerInteractionScenario.OrderTrayInHands, new string[] { "Default", "BuyerTablePlace" } },
        {
            PlayerInteractionScenario.ContainerWithAvailablePlacesInHands,
            new string[] { "Default", "ItemsContainer", "Machine", "Bin", "Buyer", "Item" }
        },
    };

    [Inject]
    private void Construct(
        GameController game,
        DataController data,
        GuiController gui,
        Player player,
        CameraController camera,
        OrdersManager ordersManager,
        ItemsController items,
        AllPrices allPrices,
        CurrencyController currency,
        XpController xp,
        TutorialController tutorial,
        AnalyticsManager analytics,
        SoundController sounds,
        GameController gameController)
    {
        this.game = game;
        this.data = data;
        this.gui = gui;
        this.player = player;
        this.camera = camera;
        this.ordersManager = ordersManager;
        this.items = items;
        this.allPrices = allPrices;
        this.currency = currency;
        this.xp = xp;
        this.tutorial = tutorial;
        this.analytics = analytics;
        this.sounds = sounds;
        _gameController = gameController;
        Init();
    }

    private void Init()
    {
        this.gui.InitGameWorldInteraction(this);

        Debug.Log("Interaction init...");

        var gameScreen = gui.FindScreen<GameScreen>();

        gameScreen.JoystickContainer.JoystickInputEvent += (Vector2 joystikInput) =>
        {
            // recalculate joystikInput using current camera rotation
            var cameraAngle = camera.CurrentCameraAngleY;
            Quaternion rotation = Quaternion.Euler(0, cameraAngle, 0);
            Vector3 rotatedVector = rotation * new Vector3(joystikInput.x, 0, joystikInput.y);
            joystikInput = new Vector2(rotatedVector.x, rotatedVector.z);
            player.JoystickInput(joystikInput);
        };

        gameScreen.MainInteractionClickEvent += () =>
        {
            var currentFocus = camera.RayVision.CurrentFocus;
            if (!currentFocus) return;

            currentFocus.OnClick();
        };

        gameScreen.OnOpenClickEvent += () =>
        {
            var draggable = player.CurrentDraggable;
            if (!draggable) return;

            var box = draggable.GetComponent<BoxOpenClose>();
            if (!box) return;

            box.Open();
            gui.FindScreen<GameScreen>().UpdateInteractionButtons();

            SetInteractionScenarioByBox(box);

            if (tutorial.InTutorial && tutorial.CurrentTutorial == TutorialType.PlacePackingBoxToShelf)
            {
                game.TriggerTutorial();
            }
        };

        gameScreen.OnCloseClickEvent += () =>
        {
            var draggable = player.CurrentDraggable;
            if (!draggable) return;

            var box = draggable.GetComponent<BoxOpenClose>();
            if (!box) return;

            box.Close();
            gui.FindScreen<GameScreen>().UpdateInteractionButtons();

            SetInteractionScenarioByBox(box);

            if (tutorial.InTutorial && tutorial.CurrentTutorial == TutorialType.PlacePackingBoxToShelf)
            {
                game.TriggerTutorial();
            }
        };

        gameScreen.OnUseClickEvent += () =>
        {
            var draggable = player.CurrentDraggable;

            var currentFocus = camera.RayVision.CurrentFocus;
            if (!currentFocus) return;
        };

        gameScreen.OnPlaceClickEvent += () =>
        {
            var currentFocus = camera.RayVision.CurrentFocus;
            if (!currentFocus) return;

            var shelf = currentFocus.GetComponent<ItemsHandler>();
            if (!shelf) return;
            TryPlaceItemToItemsHandler(shelf);
        };

        gameScreen.OnThrowClickEvent += () => { TryThrowCurrentDraggable(); };

        var lookAround = gui.FindUniversalElement<LookAroundContainer>();

        lookAround.StartLookAroundEvent += gameScreen.OnStartLookAround;

        lookAround.ClickEvent += (PointerEventData pointer) =>
        {
            bool forceClick = false;
            if (gui.CurrentScreen is not GameScreen)
            {
                if (gui.CurrentScreen is AssemblyScreen) forceClick = true;
                else return;
            }

            // Create a Ray from the camera through the mouse click position
            Ray ray = Camera.main.ScreenPointToRay(pointer.position);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, currentLayerMask))
            {
                // Check if the hit object is the one you want
                if (hit.collider == null) return;

                var touchInteractive = hit.collider.gameObject.GetComponent<TouchInteractive>();
                if (touchInteractive == null) return;

                if (forceClick) touchInteractive.OnClickForce();
                else touchInteractive.OnClick();
            }
        };

        lookAround.LookAroundEvent += player.RotateCam;

        OnAnyChangePlayerDraggerEvent += () => { gameScreen.UpdateInteractionButtons(); };

        camera.RayVision.OnChangeCurrentFocusEvent += gameScreen.SetPlayerFocus;
        player.OnPlayerStartDragEvent += OnStartPlayerDrag;
        player.OnPlayerEndDragEvent += OnEndPlayerDrag;

        SetInteractionScenario(PlayerInteractionScenario.EmptyHands);

        game.OnChangeAssemblyEvent += OnChagneAssembly;
        game.OnStartAssemblingFocusEvent += UpdateWorldOutlineHintForAssembly;
        game.OnEndAssemblingFocusEvent += HideAssemblyWorldHint;
        //Debug.Log("Interaction init completed");
    }

    private void TrySetItemToDeepTryer(Item item, DeepFryer deepFryer)
    {
        var deepFryerItem = item.GetComponent<DeepFryerItem>();
        if (deepFryerItem == null) return;

        deepFryer.TryPlace(deepFryerItem);
    }

    private void TryPlaceItemToItemsHandler(ItemsHandler itemsHandler)
    {
        //if (!itemsHandler.HavePlace()) return;

        var draggable = player.CurrentDraggable;
        if (!draggable) return;

        // пытаемся положить продукт из рук игрока, если игрок держит продукт
        var item = draggable.GetComponent<Item>();

        if (item)
        {
            Dragger availablePlace;

            if (item.ItemType == ItemType.CutletWell)
            {
                if (!itemsHandler.IsAssemblyTable)
                    return;
                else
                    if (!itemsHandler.HavePlace(item.ItemType, out availablePlace)) return;
            }
            else
            {
                if (!itemsHandler.HavePlace(item.ItemType, out availablePlace)) return;
            }

            var productSection = itemsConfig.Get.Item(item.ItemType).MainData.SectionType;

            if (itemsHandler.CanPlaceBySectionType(productSection))
            {
                availablePlace.StartDrag(item.Draggable);
                OnAnyChangePlayerDraggerEvent();
                return;
            }
        }

        // пытаемся выложить продукт из коробки, если в руках игрока коробка с продуктом
        var box = draggable.GetComponent<Box>();

        if (box)
        {
            if (box is BoxOpenClose boxOpenClose && !boxOpenClose.IsOpen) return;

            var productInBox = box.CurrentDraggable;
            if (!productInBox) return;

            var productFromBox = box.FindFirstItem(out var boxProductDragger).GetComponent<Item>();
            var itemType = productFromBox.ItemType;
            Dragger availableplace;

            if (productFromBox.ItemType == ItemType.CoffeeCup || productFromBox.ItemType == ItemType.SodaCup)
            {
                if (!itemsHandler.HavePlaceCoffeeCup(productFromBox.ItemType, out availableplace))
                    return;
            }
            else if (productFromBox.ItemType == ItemType.BurgerPackingPaper ||
                     productFromBox.ItemType == ItemType.Bun ||
                     productFromBox.ItemType == ItemType.CabbageSolid ||
                     productFromBox.ItemType == ItemType.OnionSolid ||
                     productFromBox.ItemType == ItemType.TomatoSolid || productFromBox.ItemType == ItemType.CheeseSolid)
            {
                if (!itemsHandler.IsAssemblyTable)
                    return;
                else
                {
                    if (!itemsHandler.HavePlace(productFromBox.ItemType, out availableplace))
                        return;
                }
            }
            else if (productFromBox.ItemType == ItemType.CutletRaw)
            {
                if (!itemsHandler.IsAssemblyTable && !itemsHandler.IsCutletRawContainer)
                    return;
                else
                {
                    if (!itemsHandler.HavePlace(productFromBox.ItemType, out availableplace))
                        return;
                }
            }
            else
            {
                Debug.Log("ТУТААА");
                if (!itemsHandler.HavePlace(productFromBox.ItemType, out availableplace))
                    return;
            }

            // if (!itemsHandler.HavePlace(productFromBox.ItemType, out var availableplace)) return;

            var productSection = itemsConfig.Get.Item(productFromBox.ItemType).MainData.SectionType;

            if (itemsHandler.CanPlaceBySectionType(productSection))
            {
                boxProductDragger.EndDrag();
                availableplace.StartDrag(productFromBox.Draggable);
                OnAnyChangePlayerDraggerEvent();

                SetInteractionScenarioByBox(box);

                // todo иметь в виду что предмет ставим не в ItemsHandler а в сборочный контейнер
                //if (box.IsEmpty && tutorial.InTutorial && tutorial.CurrentTutorial == TutorialType.PlacePackingBoxToShelf && itemType == ItemType.BurgerPackingPaper)
                //{
                //    tutorial.CompleteCurrentTutorial();
                //    game.TriggerTutorial();
                //}
                if (box.IsEmpty && itemType == ItemType.CutletRaw &&
                    !tutorial.IsCompleted(TutorialType.PlaceCutletToTray))
                {
                    tutorial.CompleteTutorial(TutorialType.PlaceCutletToTray);
                    game.TriggerTutorial();
                    return;
                }

                return;
            }
        }
    }

    private void TryThrowCurrentDraggable()
    {
        var currentDraggable = player.CurrentDraggable;
        if (!currentDraggable) return;

        sounds.Play(SoundType.snd_pickup_item_v4);
        currentDraggable.CurrentDragger.EndDrag(camera.ForwardDirection);

        /*
        if (tutorial.InTutorial && tutorial.CurrentTutorialID == TutorialController.TutorID_ThrowEmptyBoxToTrash)
        {
            tutorial.CompleteCurrentTutorial();
            gui.FindScreen<GameScreen>().TriggerTutorial();
        }
        */
    }

    private void OnStartPlayerDrag(Dragger dragger, Draggable d)
    {
        sounds.Play(SoundType.snd_pickup_item_v1);

        //Debug.Log("Interaction: OnStartdrag");
        var gameScreen = gui.FindScreen<GameScreen>();
        gameScreen.OnStartDrag(d);
        game.OnPlayerStartDrag(d);

        // analytics

        var box = d.GetComponent<Box>();
        if (box != null)
        {
            SetInteractionScenarioByBox(box);
            return;
        }

        var item = d.GetComponent<Item>();
        if (item != null)
        {
            if (item.ItemType == ItemType.SodaCup)
                SetInteractionScenario(PlayerInteractionScenario.EmptySodaCupDefaultItemInHands);
            else //if (item.ItemType == ItemType.CutletRaw || item.ItemType == ItemType.CutletWell || item.ItemType == ItemType.CutletBurnt) 
            {
                if (player.HavePlace(d, out _))
                    SetInteractionScenario(PlayerInteractionScenario.ContainerWithAvailablePlacesInHands);
                else SetInteractionScenario(PlayerInteractionScenario.DefaultItemInHands);
            }

            return;
        }

        var tray = d.GetComponent<OrderTray>();
        if (tray != null)
        {
            SetInteractionScenario(PlayerInteractionScenario.OrderTrayInHands);
            return;
        }
    }

    private void SetInteractionScenarioByBox(Box box)
    {
        if (box is BoxOpenClose openCloseBox)
        {
            if (!openCloseBox.IsOpen) SetInteractionScenario(PlayerInteractionScenario.ClosedBoxInHands);
            else
            {
                if (box.IsEmpty) SetInteractionScenario(PlayerInteractionScenario.EmptyBoxInHands);
                else SetInteractionScenario(PlayerInteractionScenario.OpenBoxInHands);
            }
        }
        else
        {
            SetInteractionScenario(PlayerInteractionScenario.OpenBoxInHands);
        }
    }

    private void OnEndPlayerDrag(Draggable d)
    {
        game.OnPlayerEndDrag();

        if (player.CurrentDraggable)
        {
            SetInteractionScenario(PlayerInteractionScenario.ContainerWithAvailablePlacesInHands);
            return;
        }

        gui.FindScreen<GameScreen>().OnTotalEndDrag();
        SetInteractionScenario(PlayerInteractionScenario.EmptyHands);
    }

    private void SetInteractionScenario(PlayerInteractionScenario scenario)
    {
        currentLayerMask = LayerMask.GetMask(layersForInteractionScenarios[scenario]);
        camera.RayVision.SetCurrentLayerMask(currentLayerMask);
    }

    private void TrySetItemToSlicedContainer(Item item, AssemblyItemsContainer slicedContainer)
    {
        slicedContainer.TryPlace(item, out var success);

        if (success) sounds.Play(SoundType.Gotovka);

        if (success && item.ItemType == ItemType.BurgerPackingPaper &&
            !tutorial.IsCompleted(TutorialType.PlacePackingBoxToShelf))
        {
            tutorial.CompleteTutorial(TutorialType.PlacePackingBoxToShelf);
            game.TriggerTutorial();
        }
    }

    public void CalculateOrderCost(OrderData orderData, out DollarValue totalCost, out int xpReward)
    {
        totalCost = new();
        xpReward = 0;

        foreach (var orderItem in orderData.Items)
        {
            totalCost += allPrices.GetPrice(orderItem.ItemType) * orderItem.Value;
            xpReward += itemsConfig.Get.Item(orderItem.ItemType).XpData.SaleXpReward;
        }
    }

    private int[] haveDollarsVariants = new int[] { 1, 5, 10, 20, 50 };

    public void CalculateHaveCash(DollarValue needHaveValue, out DollarValue have)
    {
        int haveDollars = 0;
        int haveCents = 0;

        while (haveDollars < needHaveValue.dollars) haveDollars += ArrayUtility.GetRandomValue(haveDollarsVariants);

        if (needHaveValue.cents > 0 && haveDollars == needHaveValue.dollars) haveDollars++;

        have = new(haveDollars, haveCents);
    }

    private TouchInteractive lastWorldAssemblyHint;

    /// <summary>
    /// Подсветить текущий лоток
    /// </summary>
    public void UpdateWorldOutlineHintForAssembly()
    {
        if (lastWorldAssemblyHint != null) lastWorldAssemblyHint.SetForceFocus(false);

        if (!game.InAssemblyFocus) return;

        if (tutorial.IsCompleted(TutorialType.TakeCutlet) && !tutorial.IsCompleted(TutorialType.AssemblyBurger))
        {
            UpdateWorldOutlineHintForAssembly(ItemType.FinalBurger_Small);
            return;
        }

        if (ordersManager.ActiveOrders.Count == 0)
        {
            return;
        }
        else
        {
            var allActiveOrders = ordersManager.ActiveOrders;

            List<ItemType> burgerOrders = new();

            foreach (var activeOrder in allActiveOrders)
            {
                var itemsInOrder = activeOrder.OrderData.Items;

                foreach (var item in itemsInOrder)
                {
                    var section = items.GetItemData(item.ItemType).mainData.SectionType;
                    if (section != ItemSectionType.FinalBurger) continue;

                    burgerOrders.Add(item.ItemType);
                }
            }

            if (burgerOrders.Count > 0) UpdateWorldOutlineHintForAssembly(burgerOrders[0]);
            else
            {
                Debug.Log("No burger orders");
            }
        }
    }

    public void UpdateWorldOutlineHintForAssembly(ItemType orderItem)
    {
        var itemData = items.GetItemData(orderItem);
        var recipe = itemData.Recipe.RecipeItems;

        var finalBurger = items.FindItem(orderItem);
        bool totalCompleted = finalBurger != null;

        if (totalCompleted)
        {
            bool packed = finalBurger.Draggable.CurrentDragger &&
                          finalBurger.Draggable.CurrentDragger.transform.parent.GetComponent<PackingPaperItem>() !=
                          null;

            if (packed) return;
            else ShowAssemblyWorldHintForNextItem(ItemType.BurgerPackingPaper);
            return;
        }
        else
        {
            for (int i = 0; i < recipe.Length; i++)
            {
                if (currentAssembly.Count <= i)
                {
                    //Debug.Log("Следующий предмет в рецепте: " + recipe[i]);
                    ShowAssemblyWorldHintForNextItem(recipe[i]);
                    return;
                }
                else
                {
                    if (currentAssembly[i] != recipe[i])
                    {
                        //Debug.Log("Несоответствие, убрать верхний предмет");
                        var lastItem = game.World.FastFood.AssemblingBoard.GetLastItem();
                        if (lastItem != null) ShowAssemblyWorldHint(lastItem.TouchInteractive);
                        return;
                    }
                }
            }
        }

        //hint.Set(itemData.mainData.IconSprite, recipeSprites, totalCompleted, checks);
    }

    public void HideAssemblyWorldHint()
    {
        if (lastWorldAssemblyHint != null) lastWorldAssemblyHint.SetForceFocus(false);
    }

    public void ShowAssemblyWorldHintForNextItem(ItemType neededItem)
    {
        // найти лоток, в котором есть этот айтем и показать на него пальцем
        if (neededItem == ItemType.KetchupUsed)
        {
            ShowAssemblyWorldHint(game.World.FastFood.KetchupContainer.Touch);
        }
        else if (neededItem == ItemType.MustardUsed)
        {
            ShowAssemblyWorldHint(game.World.FastFood.GorchizaContainer.Touch);
        }
        else if (neededItem == ItemType.BurgerPackingPaper)
        {
            var container = game.World.FastFood.FindAssemblyItemsContainerByType(neededItem);
            if (!container) return;

            var item = container.AutoGetItem(true);
            if (!item) return;

            ShowAssemblyWorldHint(item.TouchInteractive);
        }
        else
        {
            var container = game.World.FastFood.FindAssemblyItemsContainerByType(neededItem);
            if (!container) return;

            ShowAssemblyWorldHint(container.TouchInteractive);
        }
    }

    public void ShowAssemblyWorldHint(TouchInteractive newHintObject)
    {
        if (lastWorldAssemblyHint != null) lastWorldAssemblyHint.SetForceFocus(false);

        newHintObject.SetForceFocus(true);
        lastWorldAssemblyHint = newHintObject;
    }

    private List<ItemType> currentAssembly = new();

    private void OnChagneAssembly(List<ItemType> assembly)
    {
        this.currentAssembly = assembly;
        UpdateWorldOutlineHintForAssembly();
    }

    private void TryAssemblyFocus(out bool success)
    {
        success = false;
        if (player.CurrentDraggable) return;
        if (game.InAssemblyFocus) return;

        gui.Show<AssemblyScreen>();
        camera.TempFocus(game.World.FastFood.AssemblingBoard, true);

        sounds.Play(SoundType.ButtonClickWet);

        success = true;
    }
}