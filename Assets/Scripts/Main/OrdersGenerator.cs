using UnityEngine;
using TheSTAR.Utility;
using System.Collections.Generic;
using Configs;
using Zenject;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// Отвечает за то, чтобы создать новый заказ, исходя из опыта игрока и товаров в наличии
/// </summary>
public class OrdersGenerator
{
    private readonly Dictionary<ItemType, NeededItem> NededItemsToStartOrder = new()
    {
        // { ItemType.FinalFrenchFries, new OrGroup(ItemType.FrenchFriesFrozen, ItemType.FinalFrenchFries)},
        // { ItemType.FinalNuggets, new OrGroup(ItemType.NuggetsFrozen, ItemType.FinalNuggets)},

        // burgers

        {
            ItemType.FinalBurger_Small, new OrGroup(
                new SingleNeededItem(ItemType.FinalBurger_Small),
                new AndGroup(
                    // булка
                    new OrGroup(new SingleNeededItem(ItemType.Bun), new AndGroup(ItemType.BunBottom, ItemType.BunTop)),

                    // катлета
                    new OrGroup(ItemType.CutletRaw, ItemType.CutletWell, ItemType.CutletBurnt)
                ))
        },

        {
            ItemType.FinalBurger_Cheeseburger, new OrGroup(
                new SingleNeededItem(ItemType.FinalBurger_Cheeseburger),
                new AndGroup(
                    // булка
                    new OrGroup(new SingleNeededItem(ItemType.Bun), new AndGroup(ItemType.BunBottom, ItemType.BunTop)),

                    // катлета
                    new OrGroup(ItemType.CutletRaw, ItemType.CutletWell, ItemType.CutletBurnt),

                    // сыр
                    new OrGroup(ItemType.CheeseSolid, ItemType.CheesePiece)
                ))
        },

        {
            ItemType.FinalBurger_Medium, new OrGroup(
                new SingleNeededItem(ItemType.FinalBurger_Medium),
                new AndGroup(
                    // булка
                    new OrGroup(new SingleNeededItem(ItemType.Bun), new AndGroup(ItemType.BunBottom, ItemType.BunTop)),

                    // катлета
                    new OrGroup(ItemType.CutletRaw, ItemType.CutletWell, ItemType.CutletBurnt),

                    // сыр
                    new OrGroup(ItemType.CheeseSolid, ItemType.CheesePiece),

                    // томат
                    new OrGroup(ItemType.TomatoSolid, ItemType.TomatoPiece)
                ))
        },

        {
            ItemType.FinalBurger_Star, new OrGroup(
                new SingleNeededItem(ItemType.FinalBurger_Star),
                new AndGroup(
                    // булка
                    new OrGroup(new SingleNeededItem(ItemType.Bun), new AndGroup(ItemType.BunBottom, ItemType.BunTop)),

                    // катлета
                    new OrGroup(ItemType.CutletRaw, ItemType.CutletWell, ItemType.CutletBurnt),

                    // сыр
                    new OrGroup(ItemType.CheeseSolid, ItemType.CheesePiece),

                    // лук
                    new OrGroup(ItemType.OnionSolid, ItemType.OnionPiece)
                ))
        },

        {
            ItemType.FinalBurger_Big, new OrGroup(
                new SingleNeededItem(ItemType.FinalBurger_Big),
                new AndGroup(
                    // булка
                    new OrGroup(new SingleNeededItem(ItemType.Bun), new AndGroup(ItemType.BunBottom, ItemType.BunTop)),

                    // катлета x2
                    new OrGroup(new CountNeededItem(ItemType.CutletRaw, 2), new CountNeededItem(ItemType.CutletWell, 2),
                        new CountNeededItem(ItemType.CutletBurnt, 2)),

                    // сыр
                    new OrGroup(ItemType.CheeseSolid, ItemType.CheesePiece),

                    // салат
                    new OrGroup(ItemType.CabbageSolid, ItemType.CabbagePiece)
                ))
        },

        {
            ItemType.FinalBurger_Mega, new OrGroup(
                new SingleNeededItem(ItemType.FinalBurger_Mega),
                new AndGroup(
                    // булка
                    new OrGroup(new SingleNeededItem(ItemType.Bun), new AndGroup(ItemType.BunBottom, ItemType.BunTop)),

                    // томат
                    new OrGroup(ItemType.TomatoSolid, ItemType.TomatoPiece),

                    // сыр
                    new OrGroup(ItemType.CheeseSolid, ItemType.CheesePiece),

                    // катлета x2
                    new OrGroup(new CountNeededItem(ItemType.CutletRaw, 2), new CountNeededItem(ItemType.CutletWell, 2),
                        new CountNeededItem(ItemType.CutletBurnt, 2)),

                    // салат
                    new OrGroup(ItemType.CabbageSolid, ItemType.CabbagePiece),

                    // лук
                    new OrGroup(ItemType.OnionSolid, ItemType.OnionPiece)
                ))
        },

        { ItemType.FinalCola, new AndGroup(ItemType.ColaCistern, ItemType.FinalCola) },
        { ItemType.FinalLemon, new AndGroup(ItemType.LemonCistern, ItemType.FinalLemon) },
        { ItemType.FinalOrange, new AndGroup(ItemType.OrangeCistern, ItemType.FinalOrange) },
        { ItemType.FinalBarberry, new AndGroup(ItemType.BarberryCistern, ItemType.FinalBarberry) },
        { ItemType.FinalCoffee, new AndGroup(ItemType.CoffeeBeans, ItemType.FinalCoffee) }
    };

    private ConfigHelper<ItemsConfig> itemsConfig = new();
    private ConfigHelper<BuyersConfig> buyersConfig = new();

    private ItemsController items;
    private XpController xp;
    private TutorialController tutor;
    private Delivery delivery;

    private int currentOrderID = 0;

    [Inject]
    private void Consruct(ItemsController items, XpController xp, TutorialController tutor, Delivery delivery)
    {
        this.items = items;
        this.xp = xp;
        this.tutor = tutor;
        this.delivery = delivery;
    }

    public OrderData GenerateOrder(out bool success, bool tutorValue)
    {
        success = false;
        OrderData loopOrder;
        var availableItemsToOrder = AvailableItemsToOrder(xp.CurrentLevel);
        if (availableItemsToOrder.Length == 0) return default;
        int itemsInOrder;

        itemsInOrder = tutorValue ? 1 : Random.Range(1, 2 + 1);

        //if (!tutor.IsCompleted(TutorialType.CompleteOrders)) itemsInOrder = 1;
        Debug.Log("itemsInOrder " + itemsInOrder);

        if (itemsInOrder == 1)
        {
            var randomItemType = ArrayUtility.GetRandomValue(availableItemsToOrder);
            OrderItemData orderItem = new(randomItemType, 1);
            loopOrder = new OrderData(currentOrderID, orderItem);
        }
        else
        {
            var firstItemType = ArrayUtility.GetRandomValue(availableItemsToOrder);
            var secondItemType = ArrayUtility.GetRandomValue(availableItemsToOrder);

            if (firstItemType == secondItemType)
            {
                if (firstItemType == ItemType.FinalCoffee)
                {
                    OrderItemData orderItem = new(firstItemType, 1);
                    loopOrder = new OrderData(currentOrderID, orderItem);
                }
                else
                {
                    OrderItemData orderItem = new(firstItemType, 2);
                    loopOrder = new OrderData(currentOrderID, orderItem);
                }
            }
            else
            {
                OrderItemData firstOrderItem = new(firstItemType, 1);
                OrderItemData secondOrderItem = new(secondItemType, 1);

                loopOrder = new OrderData(currentOrderID, new OrderItemData[] { firstOrderItem, secondOrderItem });
            }
        }

        success = true;
        return loopOrder;
    }

    // какие товары можно сейчас заказать исходя из уровня игрока 
    private ItemType[] AvailableItemsToOrder(int forLevel)
    {
        List<ItemType> availableItems = new();

        var allOrderItems = buyersConfig.Get.OrderItems;
        var allHaveItems = items.GetAllActiveItemsAsDictionary();

        foreach (var orderItemType in allOrderItems)
        {
            // Пропускаем если ключа нет в словаре NededItemsToStartOrder
            if (!NededItemsToStartOrder.ContainsKey(orderItemType)) continue;

            bool checkLevel = forLevel >= itemsConfig.Get.Item(orderItemType).XpData.NeededLevelForBuy;
            if (!checkLevel) continue;

            bool checkItems = CheckItemsCondition(allHaveItems, NededItemsToStartOrder[orderItemType]);
            if (!checkItems) continue;

            availableItems.Add(orderItemType);
        }

        return availableItems.ToArray();
    }

    private bool CheckItemsCondition(Dictionary<ItemType, int> allHaveItems, NeededItem neededItem)
    {
        if (neededItem is SingleNeededItem singleNeededItem)
        {
            return allHaveItems.ContainsKey(singleNeededItem.ItemType);
        }
        else if (neededItem is CountNeededItem countNeededItem)
        {
            if (allHaveItems.ContainsKey(countNeededItem.ItemType))
                return allHaveItems[countNeededItem.ItemType] >= countNeededItem.Count;
            else return false;
        }
        else if (neededItem is NeededItemGroup group)
        {
            if (neededItem is OrGroup)
            {
                foreach (var element in group.NeededItems)
                {
                    bool check = CheckItemsCondition(allHaveItems, element);
                    if (check) return true;
                }

                return false;
            }
            else if (neededItem is AndGroup)
            {
                foreach (var element in group.NeededItems)
                {
                    bool check = CheckItemsCondition(allHaveItems, element);
                    if (!check) return false;
                }

                return true;
            }
        }

        return false;
    }

    /*
    /// <summary>
    /// Какие предметы должны быть в наличии, чтобы можно быть заказать этот товар
    /// </summary>
    private ItemType[] GetNeededItemsForStartOrder(ItemType itemType)
    {
        ItemType[] result;

        if (NededItemsToStartOrder.ContainsKey(itemType)) result = NededItemsToStartOrder[itemType];
        else result = new ItemType[] {itemType};

        return result;
    }
    */
}

/// <summary>
/// Данные заказа
/// </summary>
[Serializable]
public struct OrderData
{
    private int id;
    [SerializeField] private OrderItemData[] items;
    public OrderItemData[] Items => items;

    public int ID => id;
    public bool LoopOrder => true;

    public bool EqualsOrderValues(OrderData other)
    {
        if (other.items.Length != items.Length) return false;

        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].Equals(other.items[i])) return false;
        }

        return true;
    }

    public OrderData(int id, OrderItemData item)
    {
        this.id = id;
        items = new OrderItemData[] { item };
    }

    public OrderData(int id, OrderItemData[] items)
    {
        this.id = id;
        this.items = items;
    }

    public OrderData(int overrideID, OrderData baseOrder)
    {
        id = overrideID;
        items = new OrderItemData[baseOrder.items.Length];

        for (int i = 0; i < baseOrder.items.Length; i++) items[i] = baseOrder.Items[i];
    }

    public OrderData(OrderData baseOrder)
    {
        id = baseOrder.id;
        items = new OrderItemData[baseOrder.items.Length];

        for (int i = 0; i < baseOrder.items.Length; i++) items[i] = baseOrder.Items[i];
    }

    public bool CanAddItem(ItemType itemType)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].CanAddItem(itemType)) return true;
        }

        return false;
    }

    public bool IsOrderCompleted()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].Completed) return false;
        }

        return true;
    }

    public bool AddItem(ItemType itemType)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].CanAddItem(itemType))
            {
                items[i].AddItem();
                return true;
            }
        }

        return false;
    }

    public void AddAllNeededItems()
    {
        for (int i = 0; i < items.Length; i++) items[i].Fill();
    }

    public override string ToString()
    {
        if (items.Length > 1)
        {
            string itemText = "";
            foreach (var item in items)
            {
                itemText += $"{item.ItemType} {item.CurrentValue}/{item.Value}";
                itemText += "\n";
            }

            return itemText;
        }
        else return $"{items[0].ItemType} {items[0].CurrentValue}/{items[0].Value}";
    }

    public ItemType? NextNeededItem()
    {
        foreach (var orderItem in items)
        {
            if (!orderItem.Completed) return orderItem.ItemType;
        }

        return null;
    }
}

/// <summary>
/// Данные предмета в заказе
/// </summary>
[Serializable]
public struct OrderItemData
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private int value; // балансное значение, сколько предметов необходимо
    private int currentValue; // текущее значение, сколько предметов уже передано

    public ItemType ItemType => itemType;
    public int Value => value;

    public int CurrentValue
    {
        get { return currentValue; }
        set { currentValue = value; }
    }

    public bool Equals(OrderItemData other)
    {
        return itemType == other.itemType && value == other.Value;
    }

    public OrderItemData(ItemType itemType, int value)
    {
        this.itemType = itemType;
        this.value = value;
        this.currentValue = 0;
    }

    public bool CanAddItem() => CanAddItem(itemType);

    public bool CanAddItem(ItemType itemType)
    {
        return this.itemType == itemType && !Completed;
    }

    public void AddItem()
    {
        currentValue++;
    }

    public void Fill()
    {
        currentValue = value;
    }

    public bool Completed => currentValue >= value;
}

public abstract class NeededItem
{
}

/// <summary>
/// Истинно если предмет в наличии
/// </summary>
public class SingleNeededItem : NeededItem
{
    private ItemType itemType;
    public ItemType ItemType => itemType;

    public SingleNeededItem(ItemType itemType)
    {
        this.itemType = itemType;
    }
}

/// <summary>
/// Истинно если нужное количество предмета в наличии
/// </summary>
public class CountNeededItem : NeededItem
{
    private ItemType itemType;
    public ItemType ItemType => itemType;

    private int count;
    public int Count => count;

    public CountNeededItem(ItemType itemType, int count)
    {
        this.itemType = itemType;
        this.count = count;
    }
}

public abstract class NeededItemGroup : NeededItem
{
    private NeededItem[] neededItems;
    public NeededItem[] NeededItems => neededItems;

    public NeededItemGroup(params NeededItem[] neededItems)
    {
        this.neededItems = neededItems;
    }

    public NeededItemGroup(params ItemType[] neededItems)
    {
        this.neededItems = new SingleNeededItem[neededItems.Length];

        for (int i = 0; i < neededItems.Length; i++)
        {
            ItemType element = neededItems[i];
            this.neededItems[i] = new SingleNeededItem(element);
        }
    }
}

/// <summary>
/// Истинно если любой предмет в группе в наличии
/// </summary>
public class OrGroup : NeededItemGroup
{
    public OrGroup(params NeededItem[] neededItems) : base(neededItems)
    {
    }

    public OrGroup(params ItemType[] neededItems) : base(neededItems)
    {
    }
}

/// <summary>
/// Истинно если все предметы в группе в наличии
/// </summary>
public class AndGroup : NeededItemGroup
{
    public AndGroup(params NeededItem[] neededItems) : base(neededItems)
    {
    }

    public AndGroup(params ItemType[] neededItems) : base(neededItems)
    {
    }
}