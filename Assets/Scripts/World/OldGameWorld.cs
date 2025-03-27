using Configs;
using UnityEngine;
using Unity.AI.Navigation;
using TheSTAR.Data;
using TheSTAR.GUI;
using System.Collections.Generic;
using TheSTAR.Utility;
using Zenject;
using System;

namespace World
{
    [Obsolete]
    public class OldGameWorld : MonoBehaviour
    {
        /*
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Player playerPrefab;
        
        [Space]
        [SerializeField] private NavMeshSurface[] surfaces;
        [SerializeField] private Light worldLight;
        //[SerializeField] private CoffeeShop coffeeShop;
        [SerializeField] private Trash trash;

        [Space]
        [SerializeField] private ItemsController items;
        [SerializeField] private TutorCharacter tutorCharacter;
        [SerializeField] private TownPathPoint endPointForTutorCharacter;

        [Space]
        [SerializeField] private TownPassersSimulator passers;
        [SerializeField] private TownCarsSimulator cars;

        [Space]
        [SerializeField] private Delivery delivery;
        [SerializeField] private CoffeeNameContainer nameContainer;
        [SerializeField] private AllPrices allPrices;

        public Player CurrentPlayer { get; private set; }

        private int levelIndex;
        private ConfigHelper<ItemsConfig> itemsConfig = new();

        private const float ActivateLightDelay = 0.1f;

        //public BuyersQueue DefaultBuyersQueue => coffeeShop.BuyersQueue;
        //public OrderMonitor OrderMonitor => coffeeShop.OrderMonitor;
        //public CoffeeShop CoffeeShop => coffeeShop;
        public ItemsController Items => items;
        public Trash Trash => trash;
        public TutorCharacter TutorCharacter => tutorCharacter;
        public TownPathPoint EndPointForTutorCharacter => endPointForTutorCharacter;
        public Delivery Delivery => delivery;
        public CoffeeNameContainer NameContainer => nameContainer;
        public AllPrices AllPrices => allPrices;

        private DataController data;
        private LevelController level;
        private GameController game;
        private XpController xp;
        private CurrencyController currency;
        private GuiController gui;
        private TutorialController tutorial;
        private ManufacturingController manufacturing;
        private AnalyticsManager analytics;
        private SalesController sales;

        [Inject]
        private void Construct(
            DataController data, 
            GameController game,
            XpController xp, 
            CurrencyController currency,
            GuiController gui,
            TutorialController tutorial,
            ManufacturingController manufacturing,
            AnalyticsManager analytics,
            SalesController sales)
        {
            Debug.Log("Construct GameWorld");
            this.data = data;
            this.game = game;
            this.xp = xp;
            this.currency = currency;
            this.gui = gui;
            this.tutorial = tutorial;
            this.manufacturing = manufacturing;
            this.analytics = analytics;
            this.sales = sales;
        }

        public void LoadWorld(int levelIndex)
        {
            this.levelIndex = levelIndex;
            game.AllPrices.Init();

            //coffeeShop.Init();

            /*
            coffeeShop.BuyersQueue.OnUpdateCurrentOrderEvent += (OrderData orderData) => 
            {
                level.CalculateOrderCost(out var dollarCost, out _);
                coffeeShop.OrderMonitor.SetOrder(orderData);
                currentOrderData = orderData;

                //analytics.Trigger(RepeatingEventType.TakeOrder);
                
                if (!tutorial.IsCompleted(TutorialController.TutorID_MakeFirstCoffee_Final))
                {
                    gui.FindScreen<GameScreen>().TriggerTutorial();
                }
            };

            coffeeShop.BuyersQueue.OnClearCurrentOrderEvent += () => 
            {
                coffeeShop.OrderMonitor.ClearOrder();
                currentOrderData = null;
            };
            
            if (CurrentPlayer != null) Destroy(CurrentPlayer.gameObject);
            SpawnPlayer();

            // init buyer queue
            //if (coffeeShop.BuyersQueue != null) coffeeShop.BuyersQueue.Init(level);

            //coffeeShop.OrderPlate.OnUpdateDraggablesEvent += OnUpdateItemsOnOrderPlate;

            BakeNavigationSurface();

            //tutorCharacter.gameObject.SetActive(!tutorial.IsCompleted(TutorialController.TutorID_TalkWithTutorPerson));

            passers.StartSimulate();
            cars.StartSimulate();

            nameContainer.Init();

            Invoke(nameof(ActivateLight), ActivateLightDelay);
        }

        private OrderData? currentOrderData;

        private void OnUpdateItemsOnOrderPlate()
        {
            //Debug.Log("OnUpdateItemsOnOrderPlate");
            bool canStartPayment = CanStartPayment();

            if (canStartPayment)
            {
                level.CalculateOrderCost(out var cost, out _);
                coffeeShop.OrderMonitor.SetOrderCost(cost);
                var buyer = coffeeShop.BuyersQueue.CurrentBuyer;
                if (buyer.PaymentType == PaymentType.DebitCard)
                {
                    buyer.SetPaymentStatus(PaymentType.DebitCard);
                    coffeeShop.OrderMonitor.SetStatus(MonitorStatus.CardPayment);
                }
                else
                {
                    buyer.SetPaymentStatus(PaymentType.Cash);
                    coffeeShop.OrderMonitor.SetStatus(MonitorStatus.CashPayment);
                }
            }
            else
            {
                var buyer = coffeeShop.BuyersQueue.CurrentBuyer;
                if (buyer) buyer.SetPaymentStatus(null);
                coffeeShop.OrderMonitor.SetStatus(MonitorStatus.Order);
            }

            bool CanStartPayment()
            {
                if (currentOrderData == null)
                {
                    //Debug.Log("Заказ не найден");
                    return false;
                }

                var allDraggablesOnOrderPlate = coffeeShop.OrderPlate.AllDraggables;
                List<Item> itemsToGiveToBuyer = new();
                if (allDraggablesOnOrderPlate.Count != currentOrderData.Value.Items.Length)
                {
                    //Debug.Log("Выложенные предметы не соответствуют заказу");
                    return false;
                }

                foreach (var elementInOrder in currentOrderData.Value.Items)
                {
                    var temp = allDraggablesOnOrderPlate.Find(e => e.GetComponent<Item>().ItemType == elementInOrder.ItemType);
                    if (temp == null)
                    {
                        //Debug.Log("Выложенные предметы не соответствуют заказу");
                        return false;
                    }

                    allDraggablesOnOrderPlate.Remove(temp);
                    itemsToGiveToBuyer.Add(temp.GetComponent<Item>());
                }
                
                //Debug.Log("Предметы соответствуют!");
                return true;
            }
        }

        // мини-игра по оплате пройдена, покупатель забирает заказ, игрок получает вознаграждение
        public void GiveOrderToBuyerAndRewardToPlayer()
        {
            // Выдача
            level.CalculateOrderCost(out var cost, out int xpReward);
            currency.AddCurrency(cost);
            xp.AddXp(xpReward);

            var buyer = coffeeShop.BuyersQueue.CurrentBuyer;
            var allDraggablesOnOrderPlate = coffeeShop.OrderPlate.AllDraggables;

            foreach (var orderItem in currentOrderData.Value.Items)
            {
                var itemType = orderItem.ItemType;
                if (buyer.OrderData.CanAddItem(itemType))
                {
                    var itemOnOrderPlate = allDraggablesOnOrderPlate.Find(item => item.GetComponent<Item>().ItemType == itemType);
                    if (itemOnOrderPlate)
                    {
                        itemOnOrderPlate.CurrentDragger.EndDrag();
                        Destroy(itemOnOrderPlate.gameObject);
                    }
                    
                    buyer.GiveItem(itemType);
                }
            }

            sales.CompleteOrder(buyer);
            data.gameData.levelData.completedOrdersCount++;
        }
    
        private void SpawnPlayer()
        {
            CurrentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity, transform);
        }

        [ContextMenu("BakeNavigationSurface")]
        public void BakeNavigationSurface()
        {
            Debug.Log("BakeNavigationSurface");
            foreach (var surface in surfaces) surface.BuildNavMesh();
        }

        [ContextMenu("ActivateLight")]
        private void ActivateLight()
        {
            worldLight.gameObject.SetActive(true);
        }

        /// <summary>
        /// Вызов покупателя с указанным заказом
        /// </summary>
        public void CallBuyerToQueue(OrderData order, bool skipMoving)
        {
            //coffeeShop.BuyersQueue.CallBuyerToQueue(order, skipMoving);
        }

        public bool CanGiveToBuyers(ItemType item, out BuyersQueue queue)
        {
            
            if (coffeeShop.BuyersQueue != null && coffeeShop.BuyersQueue.CanGiveItemToBuyer(item, out _))
            {
                queue = coffeeShop.BuyersQueue;
                return true;
            }

            queue = null;
            return false;
            
        }
        */
    }
}