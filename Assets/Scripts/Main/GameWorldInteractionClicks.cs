using System;
using TheSTAR.GUI;
using World;
using TheSTAR.Data;
using System.Collections.Generic;
using UnityEngine;
using TheSTAR.Sound;

public partial class GameWorldInteraction
{
    public event Action NotTable;
    
    public event Action OrderAdded;
    
    public void OnClickComputer(Computer computer)
    {
        sounds.Play(SoundType.ButtonClickWet);
        gui.Show<ComputerStoreScreen>();
    }

    public void OnClickCoffeeMachine(CoffeeMachine coffeeMachine)
    {
        var currentDraggable = player.CurrentDraggable;
        if (currentDraggable)
        {
            // пробуем заполнить
            var currentPlayerItem = currentDraggable.GetComponent<Item>();
            if (currentPlayerItem == null) return;
            if (coffeeMachine.Filler.CanFill(currentPlayerItem.ItemType))
            {
                sounds.Play(SoundType.coffee_bean_bag_1);
                coffeeMachine.Filler.Fill(currentPlayerItem);

                /*
                if (!tutorial.IsCompleted(TutorialType.PrepareCoffee))
                {
                    tutorial.CompleteTutorial(TutorialType.PrepareCoffee);
                    game.TriggerTutorial();
                }
                
                if (tutorial.InTutorial && tutorial.CurrentTutorial == TutorialType.CompleteOrders)
                {
                    game.TriggerTutorial();
                }
                */
            }
            else if (currentPlayerItem.ItemType == ItemType.CoffeeCup)
            {
                // пробуем получить кофе
                if (!coffeeMachine.Filler.CanUse) return;

                coffeeMachine.Filler.Use(() =>
                {
                    sounds.Play(SoundType.water_pour_liquid_into_styrofoam_cup_coffee_zy_fi2nu);
                    items.Replace(currentPlayerItem, ItemType.FinalCoffee);
                });
            }
        }
    }

    public void OnClickSodaMachine(SodaMachine sodaMachine)
    {
        var currentDraggable = player.CurrentDraggable;
        if (currentDraggable == null) return;

        // пробуем заполнить
        var currentPlayerItem = currentDraggable.GetComponent<Item>();
        if (currentPlayerItem == null) return;

        var itemSection = items.GetItemData(currentPlayerItem.ItemType).mainData.SectionType;
        if (itemSection != ItemSectionType.Cistern) return;

        var availableFiller = sodaMachine.GetFiller(currentPlayerItem.ItemType);
        if (availableFiller == null) return;

        availableFiller.Fill(currentPlayerItem);
    }

    public void OnItemsHandlerClick(ItemsHandler itemsHandler)
    {
        var ordersTray = itemsHandler.GetComponent<OrderTray>();
        if (ordersTray)
        {
            var draggable = player.CurrentDraggable;
            if (draggable)
            {
                if (!tutorial.IsCompleted(TutorialType.PutBurgerOnTray))
                {
                    tutorial.CompleteTutorial(TutorialType.PutBurgerOnTray);
                    _gameController.TriggerTutorial();
                }
                
                TryPlaceItemToItemsHandler(itemsHandler);
                return;
            }

            if (ordersTray.HaveAllNeededItems)
            {
                if (player.HavePlace(ordersTray.Draggable, out var place))
                {
                    if (!tutorial.IsCompleted(TutorialType.PickUpTray))
                    {
                        tutorial.CompleteTutorial(TutorialType.PickUpTray);
                        _gameController.TriggerTutorial();
                    }
                    
                    place.StartDrag(ordersTray.Draggable, true);
                }
            }
            else
            {
                game.OnWrongOrderOnTray();
            }
        }
        else TryPlaceItemToItemsHandler(itemsHandler);
    }


    public void OnClickToDraggable(Draggable draggable)
    {
        var playerDraggable = player.CurrentDraggable;
        
        if (player.CurrentDraggable != null)
        {
            var itemComponent = draggable.GetComponent<Item>();

            if (itemComponent != null && itemComponent.ItemType == ItemType.CoffeeCup||
                itemComponent != null&&itemComponent.ItemType == ItemType.SodaCup)
            {
                Debug.Log("Занято в руке");
                return;
            }
        }

        if (player.CurrentDraggable == null)
        {
            var itemComponent = draggable.GetComponent<Item>();

            if (itemComponent != null && itemComponent.ItemType == ItemType.CoffeeCup)
            {
                if (draggable.GetComponent<EmbeddableItem>().Dragger.CurrentDraggable != null)
                {
                    Debug.Log("COFFEEEEEEEEEEEEEEEE");
                    return;
                }
            }
            
            if (itemComponent != null&&itemComponent.ItemType == ItemType.SodaCup)
            {
                if (draggable.GetComponent<EmbeddableItem>().Dragger.CurrentDraggable != null)
                {
                    Debug.Log("SADAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                    return;
                }
            }
        }
        
        if (playerDraggable && !player.HavePlace(draggable, out _))
        {
            if (TryPlaceCupToCup()) return;
            if (TryPackBurger()) return;

            var dragger = draggable.CurrentDragger;
            if (!dragger) return;

            var group = dragger.Group;
            if (!group) return;

            var itemsHandler = group.GetComponent<ItemsHandler>();
            if (!itemsHandler) return;

            OnItemsHandlerClick(itemsHandler);
            
            return;
        }

        var assemblyItem = draggable.GetComponent<AssemblyItem>();
        if (assemblyItem && assemblyItem.InAssembly)
        {
            TryAssemblyFocus(out var success);
            if (success) return;

            // пробуем переместить в лоток. Если не получается - берем в руки
            if (TryReturnAssemblyItemToHandler(assemblyItem)) return;
        }
        
        if (player.HavePlace(draggable, out var place))
        {
            var item = draggable.GetComponent<Item>();
            if (item)
            {
                var itemSection = items.GetItemData(item.ItemType).mainData.SectionType;
                if (itemSection == ItemSectionType.FinalBurger)
                {
                    TryAssemblyFocus(out _);
                    return;
                }
            }
            place.StartDrag(draggable);
        }
    
        bool TryPlaceCupToCup()
        {
            return false;
        }

        bool TryPackBurger()
        {
            bool packCompleted = false;
            var playerDraggable = player.CurrentDraggable;
            if (playerDraggable == null) return false;

            var playerItem = playerDraggable.GetComponent<Item>();
            if (playerItem == null) return false;

            var playerItemSection = items.GetItemData(playerItem.ItemType).mainData.SectionType;

            if (playerItemSection == ItemSectionType.FinalBurger)
            {
                var burger = playerItem;

                var clickedItem = draggable.GetComponent<Item>();
                if (clickedItem == null) return false;

                if (clickedItem.ItemType == ItemType.BurgerPackingPaper)
                {
                    var paper = clickedItem;
                    //Debug.Log("в руках бургер - кликнули на упаковку - в руках должна оказаться упаковка в коротой лежит этот бургер");

                    if (paper.Draggable.CurrentDragger) paper.Draggable.CurrentDragger.EndDrag();
                    var newPaper = items.Replace(paper, ItemType.BurgerPackingPaper_Closed);
                    if (player.HavePlace(newPaper.Draggable, out var place))
                    {
                        place.StartDrag(newPaper.Draggable);
                        newPaper.GetComponent<PackingPaperItem>().Dragger.StartDrag(burger.Draggable);
                    }
                }
            }

            return packCompleted;
        }

        // пробуем вернуть сборочный предмет в лоток
        bool TryReturnAssemblyItemToHandler(AssemblyItem assemblyItem)
        {
            var allContainers = game.World.FastFood.AllAssemblyContainers;

            List<AssemblyItemsContainer> emptySlicedContainers = new();
            for (int i = allContainers.Length - 1; i >= 0; i--)
            {
                AssemblyItemsContainer container = allContainers[i];
                if (container.IsEmpty) emptySlicedContainers.Add(container);
                else if (container.CanUse(assemblyItem.Item.ItemType, out _, out _))
                {
                    container.TryPlace(assemblyItem.Item, out _);
                    return true;
                }
            }

            if (emptySlicedContainers.Count > 0)
            {
                emptySlicedContainers[0].TryPlace(assemblyItem.Item, out _);
                sounds.Play(SoundType.Gotovka);
                return true;
            }

            return false;
        }
    }

    public void OnTrashClick(Trash trash)
    {
        TryThrowCurrentDraggable();
    }

    public void OnClickBuyer(Buyer buyer)
    {
        /*
        var order = ordersManager.FindOrder(buyer);
        if (order == null)
        {
            ordersManager.TryAcceptOrder(buyer, buyer.Place, buyer.OrderData);

            if (!tutorial.IsCompleted(TutorialType.CompleteOrders)) game.TriggerTutorial();
        }
        else
        {
            var playerDraggable = player.Dragger.CurrentDraggable;
            if (playerDraggable != null) TryPlaceItemToItemsHandler(buyer.Place.OrderPlate);
        }
        */
    }

    public void OnCostPanelClick(CostPanel costPanel)
    {
        sounds.Play(SoundType.ButtonClickWet);

        // открыть окошко настройки цены
        var enterPriceScreen = gui.FindScreen<EnterPriceScreen>();
        enterPriceScreen.Init(costPanel.ItemType, allPrices.GetPrice(costPanel.ItemType), (itemType, newPrice) =>
        {
            allPrices.SetPrice(itemType, newPrice);

            if (!tutorial.IsCompleted(TutorialType.SetPrice) && itemType == ItemType.FinalBurger_Small)
            {
                tutorial.CompleteTutorial(TutorialType.SetPrice);
                game.TriggerTutorial();
            }
        });
        gui.Show(enterPriceScreen);
    }

    public void OnCashClick(BuyerCash cash)
    {
        currency.AddCurrency(cash.DollarValue);
        xp.AddXp(cash.XpValue);
        cash.Place.DeactivateCash();

        /*
        if (!tutorial.IsCompleted(TutorialType.CompleteOrders))
        {
            data.gameData.tutorialData.completedOrdersForTutorial++;
            if (data.gameData.tutorialData.completedOrdersForTutorial >= 2) tutorial.CompleteTutorial(TutorialType.CompleteOrders);

            game.TriggerTutorial();
        }
        */
    }

    public void OnDinerNameContainerClick(FastFoodNameContainer dinerNameContainer)
    {
        var enterScreen = gui.FindScreen<EnterNameScreen>();
        enterScreen.Init((newName) =>
        {
            data.gameData.levelData.fastFoodName = newName;
            data.Save(DataSectionType.Level);
            dinerNameContainer.DisplayName(newName);

            if (!tutorial.IsCompleted(TutorialType.SetFastFoodName))
            {
                tutorial.CompleteTutorial(TutorialType.SetFastFoodName);
                game.TriggerTutorial();
            }
        });
        gui.Show(enterScreen);
    }

    public void OnGarbageClick(Garbage garbage)
    {
        gui.FindScreen<GameScreen>().SetPlayerFocus(null);
        garbage.Clean();
    }

    public void OnSlicedContainerClick(AssemblyItemsContainer slicedContainer)
    {
        // todo если не в фокусе сборки, то можем только положить айтемы в контейнер. Чтобы начать сборку нужно перейти в фокус
        var playerDraggable = player.CurrentDraggable;
        if (!playerDraggable)
        {
            TryAssemblyFocus(out var success);
            if (success) return;

            if (!slicedContainer.IsEmpty && 
                slicedContainer.CurrentCutType == CutType.PackingPaper &&
                game.World.FastFood.FinalBurgersGroup.HavePlace(ItemType.BurgerPackingPaper_Closed, out var placeForFinalBurger))
            {
                //Debug.Log("Пытаемся упаковать бургер");
                var finalBurger = game.World.FastFood.AssemblingBoard.TryGetFinalItem();
                if (finalBurger)
                {
                    //Debug.Log("Пакуем");
                    
                    var burger = slicedContainer.InterBurger(finalBurger);
                    burger.transform.position = finalBurger.transform.position;
                    burger.transform.rotation = finalBurger.transform.rotation;
                    placeForFinalBurger.StartDrag(burger.GetComponent<Item>().Draggable);
                    
                    
                    var paper = slicedContainer.AutoGetItem(true);
                    paper.Draggable.CurrentDragger.EndDrag();
                    var newPaper = items.Replace(paper, ItemType.BurgerPackingPaper_Closed);
                    newPaper.transform.position = finalBurger.transform.position;
                    newPaper.transform.rotation = finalBurger.transform.rotation;
                    newPaper.GetComponent<PackingPaperItem>().Dragger.StartDrag(finalBurger.Draggable);
                    // placeForFinalBurger.StartDrag(newPaper.Draggable);
                    newPaper.Draggable.gameObject.SetActive(false);

                    if (!tutorial.IsCompleted(TutorialType.AssemblyBurger))
                    {
                        tutorial.CompleteTutorial(TutorialType.AssemblyBurger);
                        game.TriggerTutorial();
                    }
                }
                //else Debug.Log("Не удаётся упаковать");
                return;
            }

            if (!game.World.FastFood.AssemblingBoard.HavePlace()) return;

            var item = slicedContainer.AutoGetItem(game.World.FastFood.AssemblingBoard.Deep > 0);
            if (item == null) return;

            item.Draggable.CurrentDragger.EndDrag();
            game.World.FastFood.AssemblingBoard.Place(item.GetComponent<AssemblyItem>());
            sounds.Play(SoundType.Gotovka);
            
            return;
        }
        else
        {
            var item = playerDraggable.GetComponent<Item>();
            
            if (item)
            {
                if (item.ItemType == ItemType.CutletWell)
                {
                    if (!tutorial.IsCompleted(TutorialType.TakeToAssemblyTable) &&
                        tutorial.IsCompleted(TutorialType.TakeCutlet))
                    {
                       
                        tutorial.CompleteTutorial(TutorialType.TakeToAssemblyTable);
                        game.TriggerTutorial();
                    }
                }
                
                TrySetItemToSlicedContainer(item, slicedContainer);
                return;
            }

            var box = playerDraggable.GetComponent<Box>();
            if (box)
            {
                if (box.IsEmpty) return;

                var draggable = box.FindFirstItem(out _);
                if (!draggable) return;

                var boxItem = draggable.GetComponent<Item>();
                if (!boxItem) return;

                TrySetItemToSlicedContainer(boxItem, slicedContainer);
                
                if (!tutorial.IsCompleted(TutorialType.CutBun) && box.IsEmpty)
                {
                    tutorial.CompleteTutorial(TutorialType.CutBun);
                    game.TriggerTutorial();
                }
                return;
            }
        }
    }

    public void OnAssemblingBoardClick(AssemblingBoard assemblingBoard)
    {
        var draggable = player.CurrentDraggable;
        if (!draggable)
        {
            TryAssemblyFocus(out _);
            return;
        }

        var burgerIngredient = draggable.GetComponent<AssemblyItem>();
        if (burgerIngredient)
        {
            assemblingBoard.Place(burgerIngredient);
            return;
        }

        var box = draggable.GetComponent<Box>();
        if (box)
        {
            var boxDraggable = box.CurrentDraggable;
            if (boxDraggable)
            {
                var burgerIngredientInBox = boxDraggable.GetComponent<AssemblyItem>();
                if (burgerIngredientInBox)
                {
                    assemblingBoard.Place(burgerIngredientInBox);
                    return;
                }
            }
        }
    }

    public void OnMonitorClick(OrderMonitor orderMonitor)
    {
        if (orderMonitor.currentBuyer == null) return;

        CalculateOrderCost((OrderData)orderMonitor.currentOrderData, out var costDollar, out var xpReward);

        var paymentType = PaymentType.Cash; // orderMonitor.currentBuyer.PaymentType;

        if (paymentType == PaymentType.DebitCard)
        {
            var terminal = gui.FindScreen<TerminalScreen>();
            terminal.Init(costDollar, () =>
            {   
                OnPaymentCompleted(orderMonitor, costDollar);
            });
            orderMonitor.SetOrderCost(costDollar);
            orderMonitor.SetStatus(MonitorStatus.CardPayment);
            gui.Show(terminal);
            camera.TempFocus(orderMonitor, true);
        }
        else if (paymentType == PaymentType.Cash)
        {
            var boxOfficeScreen = gui.FindScreen<BoxOfficeScreen>();
            CalculateHaveCash(costDollar, out var have);
            DollarValue change = have - costDollar;
            boxOfficeScreen.Init(change, () =>
            {
                OnPaymentCompleted(orderMonitor, costDollar);
            });
            orderMonitor.SetOrderCost(costDollar);
            orderMonitor.SetReceived(have);
            orderMonitor.SetStatus(MonitorStatus.CashPayment);
            gui.Show(boxOfficeScreen);
            camera.TempFocus(orderMonitor, true);
        }
    }

    private void OnPaymentCompleted(OrderMonitor orderMonitor, DollarValue costDollar)
    {
        sounds.Play(SoundType.Cash_Register_2);

        // todo сделать защиту, если заказ сделан то должно быть необходимо обслужить клиента (запомнить заказ и клиента даже если перезаходим в игру)
        ordersManager.TryAcceptOrder(orderMonitor.currentBuyer, orderMonitor.currentBuyer.Place, (OrderData)orderMonitor.currentOrderData);
        currency.AddCurrency(costDollar);
        camera.TempFocus(null, false);
        gui.ShowMainScreen();
        OrderAdded?.Invoke();
    }

    public void OnGriddleClick(Griddle griddle)
    {
        var playerDraggable = player.CurrentDraggable;
        if (!playerDraggable) return;

        var item = playerDraggable.GetComponent<Item>();
        if (item)
        {
            TrySetItemToGriddle(item, griddle);
            return;
        }

        var box = playerDraggable.GetComponent<Box>();
        if (box)
        {
            if (box.IsEmpty) return;

            var draggable = box.FindFirstItem(out _);
            if (!draggable) return;

            var boxItem = draggable.GetComponent<Item>();
            if (!boxItem) return;

            TrySetItemToGriddle(boxItem, griddle);
            
            return;
        }
    }

    private void TrySetItemToGriddle(Item item, Griddle griddle)
    {
        var griddleItem = item.GetComponent<GriddleItem>();
        if (griddleItem == null) return;

        griddle.TryPlace(griddleItem, out var success);

        if (success)
        {
            if (!tutorial.IsCompleted(TutorialType.PlaceCutletToGrill))
            {
                tutorial.CompleteTutorial(TutorialType.PlaceCutletToGrill);
                game.TriggerTutorial();
                return;
            }
        }
    }

    public void OnDeepFryerClick(DeepFryer deepFryer)
    {
        var playerDraggable = player.CurrentDraggable;
        if (!playerDraggable) return;

        var item = playerDraggable.GetComponent<Item>();
        if (item)
        {
            TrySetItemToDeepTryer(item, deepFryer);
            return;
        }

        var box = playerDraggable.GetComponent<Box>();
        if (box)
        {
            if (box.IsEmpty) return;

            var draggable = box.FindFirstItem(out _);
            if (!draggable) return;

            var boxItem = draggable.GetComponent<Item>();
            if (!boxItem) return;

            TrySetItemToDeepTryer(boxItem, deepFryer);
            
            return;
        }
    }

    public void OnSodaFillerClick(SodaFiller sodaFiller)
    {
        var currentDraggable = player.CurrentDraggable;
        if (!currentDraggable) return;

        // пробуем заполнить
        var currentPlayerItem = currentDraggable.GetComponent<Item>();
        if (currentPlayerItem == null) return;

        if (currentPlayerItem.ItemType == ItemType.SodaCup)
        {
            // пробуем получить кофе
            if (!sodaFiller.Filler.CanUse) return;

            sodaFiller.Filler.Use(() =>
            {
                sounds.Play(SoundType.water_pour_liquid_into_styrofoam_cup_coffee_zy_fi2nu);
                items.Replace(currentPlayerItem, sodaFiller.To);
            });
        }
    }

    public void OnBuyerTablePlaceClick(BuyerTablePlace buyerTablePlace)
    {
        Debug.Log("Положил на место");
        var playerDraggable = player.CurrentDraggable;
        
        if (playerDraggable == null) return;

        if (!playerDraggable.TryGetComponent<OrderTray>(out var tray)) return;

        Debug.Log("buyerTablePlace.Index " + buyerTablePlace.Index);
        Debug.Log("tray.CurrentOrderIndex.Index " + tray.CurrentOrderIndex);
        if (buyerTablePlace.Index != tray.CurrentOrderIndex)
        {
            NotTable?.Invoke();
            buyerTablePlace.DecreasePer();
            return;
        }

        foreach (var draggable in tray.AllDraggables)
        {
            draggable.CurrentDragger.EndDrag();
            var itemType = draggable.GetComponent<Item>().ItemType;

            if (itemType == ItemType.FinalCoffee)
            {
                analytics.Trigger(RepeatingEventType.Give_Coffee);
            }
            else if (itemType == ItemType.FinalBarberry)
            {
                analytics.Trigger(RepeatingEventType.Give_BarberrySoda);
            }
            else if (itemType == ItemType.FinalOrange)
            {
                analytics.Trigger(RepeatingEventType.Give_OrangeSoda);
            }
            else if (itemType == ItemType.FinalLemon)
            {
                analytics.Trigger(RepeatingEventType.Give_LemonSoda);
            }
            else if (itemType == ItemType.FinalCola)
            {
                analytics.Trigger(RepeatingEventType.Give_PlumColdSoda);
            }
            else if (itemType == ItemType.BurgerPackingPaper_Closed)
            {
                if (draggable.TryGetComponent<PackingPaperItem>(out var ppi))
                {
                    var packingItem = ppi.Dragger.CurrentDraggable.GetComponent<Item>();
                    if (packingItem)
                    {
                        if (packingItem.ItemType == ItemType.FinalBurger_Small)
                        {
                            analytics.Trigger(RepeatingEventType.Give_Burger_S);
                        }
                        else if (packingItem.ItemType == ItemType.FinalBurger_Cheeseburger)
                        {
                            analytics.Trigger(RepeatingEventType.Give_Cheeseburger);
                        }
                        else if (packingItem.ItemType == ItemType.FinalBurger_Medium)
                        {
                            analytics.Trigger(RepeatingEventType.Give_Burger_M);
                        }
                        else if (packingItem.ItemType == ItemType.FinalBurger_Star)
                        {
                            analytics.Trigger(RepeatingEventType.Give_StarBurger);
                        }
                        else if (packingItem.ItemType == ItemType.FinalBurger_Big)
                        {
                            analytics.Trigger(RepeatingEventType.Give_BigBurger);
                        }
                        else if (packingItem.ItemType == ItemType.FinalBurger_Mega)
                        {
                            analytics.Trigger(RepeatingEventType.Give_MegaBurger);
                        }
                    }
                }
            }

            buyerTablePlace.Group.HavePlace(itemType, out var availablePlace);
            availablePlace.StartDrag(draggable);
        }

        game.ResetOrderTray(tray.IndexOrderTray);
        ordersManager.CompleteOrder(buyerTablePlace.Index);
    }

    public void OnSousContainerClick(SousContainer sousContainer)
    {
        TryAssemblyFocus(out var success);
        if (success) return;
        
        if (!game.World.FastFood.AssemblingBoard.HavePlace()) return;
        var item = items.CreateItem(sousContainer.SousType, sousContainer.CreateItemPos.position);
        game.World.FastFood.AssemblingBoard.Place(item.GetComponent<AssemblyItem>());
    }

    public void OnAssemblyTableClick(AssemlbyTable table)
    {
        var playerDraggable = player.CurrentDraggable;
        if (playerDraggable && playerDraggable.TryGetComponent<Item>(out var item) && item.ItemType == ItemType.BurgerPackingPaper_Closed)
        {
            if (game.World.FastFood.FinalBurgersGroup.HavePlace(item.ItemType, out var place))
            {
                place.StartDrag(item.Draggable);
                return;
            }
        }

        TryAssemblyFocus(out _);
    }
}