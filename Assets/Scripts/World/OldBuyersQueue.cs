using System;
using System.Collections.Generic;
using UnityEngine;
using Configs;
using Random = UnityEngine.Random;
using TheSTAR.Utility;

namespace World
{
    // todo очередь не должна менеджерить покупателями, она должна только содержать положения
    [Obsolete]
    public class OldBuyersQueue : MonoBehaviour
    {
        [SerializeField] private Transform queueCorePlace;
        [SerializeField] private Buyer[] buyerPrefabs;
        [SerializeField] private Transform buyerStartPos;
        [SerializeField] private Transform buyerEndPos;
        [SerializeField] private Transform lookAt;
        [SerializeField] private Transform interactionZoneTran;
        [SerializeField] private Transform buyersParent;
        
        [Space]
        [SerializeField] private Vector3 buyerOffset = new Vector3(1.2f, 0, 0);

        public Transform InteractionZoneTran => interactionZoneTran;

        public bool CanInteract => buyersInQueue.Count > 0;

        private int lastCreatedBuyerVisual = -1;

        private List<Buyer> buyersInQueue = new();
        public List<Buyer> BuyersInQueue => buyersInQueue;
        private Dictionary<int, List<Buyer>> createdBuyers;
        private Dictionary<ItemType, int> itemsInStorage; // айтемы находящиеся в хранилище, их можно продать
        private Dictionary<ItemType, int> itemsOnTheWay; // айтемы находящиеся в пути, их нельзя продать

        public Buyer CurrentBuyer
        {
            get
            {
                if (buyersInQueue.Count == 0) return null;
                else return buyersInQueue[0];
            }
        }

        private LevelController levelController;

        public event Action<Buyer> OnBuyerEnterQueue;
        public event Action<Buyer> OnBuyerExitQueue;
        public event Action<OrderData> OnUpdateCurrentOrderEvent;
        public event Action OnClearCurrentOrderEvent;

        public bool Contains(Buyer b) => buyersInQueue.Contains(b);

        public ItemType[] AvailableItemsTypesInCurrentOrders(out List<Buyer> buyersForUniqueItemTypes)
        {
            buyersForUniqueItemTypes = new();

            Dictionary<ItemType, int> itemsToSkip = ArrayUtility.Merge(itemsInStorage, itemsOnTheWay);
            List<ItemType> uniqueItemTypes = new();
            OrderData order;
            OrderItemData orderItem;
            Buyer buyer;

            for (int i = 0; i < buyersInQueue.Count; i++)
            {
                buyer = buyersInQueue[i];
                order = buyer.OrderData;

                for (int itemIndex = 0; itemIndex < order.Items.Length; itemIndex++)
                {
                    orderItem = order.Items[itemIndex];

                    if (uniqueItemTypes.Contains(orderItem.ItemType)) continue;

                    if (itemsToSkip.ContainsKey(orderItem.ItemType) && itemsToSkip[orderItem.ItemType] > 0)
                    {
                        itemsToSkip[orderItem.ItemType]--;
                        continue;
                    }

                    uniqueItemTypes.Add(orderItem.ItemType);
                    buyersForUniqueItemTypes.Add(buyer);
                }
            }

            return uniqueItemTypes.ToArray();
        }

        /// <summary>
        /// Можно ли предоставить айтем этого типа какому либо покупателю (если да - вернуть этого покупателя)
        /// </summary>
        public bool CanGiveItemToBuyer(ItemType itemType, out Buyer resultBuyer)
        {
            resultBuyer = null;
            Buyer buyer;
            for (int i = 0; i < buyersInQueue.Count; i++)
            {
                buyer = buyersInQueue[i];
                if (buyer.OrderData.CanAddItem(itemType))
                {
                    resultBuyer = buyer;
                    return true;
                }
            }

            return false;
        }

        public void Init(LevelController levelController)
        {
            this.levelController = levelController;

            createdBuyers = new();
            for (int i = 0; i < buyerPrefabs.Length; i++) createdBuyers.Add(i, new());

            itemsOnTheWay = new();
            itemsInStorage = new();
        }

        private Vector3 GetQueuePlace(int placeIndex) => queueCorePlace.position + buyerOffset * placeIndex;

        /// <summary>
        /// Появляется новый покупатель, идёт на своё место
        /// </summary>
        public void CallBuyerToQueue(OrderData orderData, bool skipMoving)
        {
            var buyer = GetBuyerFrotPool(orderData, buyerStartPos.position);
            int indexInQueue = buyersInQueue.Count;
            bool newFirst = indexInQueue == 0;
            buyersInQueue.Add(buyer);

            Vector3 toPos = GetQueuePlace(indexInQueue);

            if (skipMoving)
            {
                buyer.transform.position = toPos;
                //buyer.LookAtDefault();
                BuyerReachedQueuePlace(buyer, newFirst);
            }
            //else buyer.SetDestination(toPos, (b) => BuyerReachedQueuePlace(b, newFirst));

            if (indexInQueue < buyersInQueue.Count - 1) UpdateBuyersDestinations();
        }

        private Buyer GetBuyerFrotPool(OrderData data, Vector3 createPos)
        {
            int buyerVisualIndex;
            if (lastCreatedBuyerVisual == -1)
            {
                buyerVisualIndex = Random.Range(0, buyerPrefabs.Length);
            }
            else
            {
                buyerVisualIndex = Random.Range(0, buyerPrefabs.Length - 1);
                if (buyerVisualIndex >= lastCreatedBuyerVisual) buyerVisualIndex++;
            }

            // try get from pool
            if (createdBuyers.ContainsKey(buyerVisualIndex))
            {
                Buyer buyer;
                for (int i = 0; i < createdBuyers[buyerVisualIndex].Count; i++)
                {
                    buyer = createdBuyers[buyerVisualIndex][i];
                    if (!buyer.Active)
                    {
                        //buyer.Activate(data, createPos);
                        return buyer;
                    }
                }
            }

            return CreateBuyer(data, createPos, buyerVisualIndex);
        }

        [Obsolete]
        private Buyer CreateBuyer(OrderData data, Vector3 createPos, int visualIndex)
        {
            var buyer = Instantiate(buyerPrefabs[visualIndex], createPos, Quaternion.identity, buyersParent);
            //buyer.SetQueue(this);
            //buyer.Init(data, lookAt);
            //buyer.OnGiveItemToOrderEvent += (b) => DelayCheckBuyersToCompleteOrders();
            lastCreatedBuyerVisual = visualIndex;
            createdBuyers[visualIndex].Add(buyer);
            return buyer;
        }

        // это раньше вызывалось когда покупатель впервые вставал в какое-то место в очереди (не вызывалось когда в очередной раз встаёт в очередь)
        private void BuyerReachedQueuePlace(Buyer buyer, bool newFirst)
        {
            //levelController.AddAvailableOrder(buyer);
            OnBuyerEnterQueue?.Invoke(buyer);

            //DelayCheckBuyersToCompleteOrders();

            if (newFirst) OnUpdateCurrentOrderEvent?.Invoke(buyer.OrderData);
        }

        public void RemoveBuyerFromQueue(Buyer buyerWithOrder)
        {
            //buyerWithOrder.SetDestination(buyerEndPos.position, (buyer) => buyer.Deactivate());
            if (buyersInQueue[0] == buyerWithOrder) OnClearCurrentOrderEvent?.Invoke();
            buyersInQueue.Remove(buyerWithOrder);

            OnBuyerExitQueue?.Invoke(buyerWithOrder);
            
            UpdateBuyersDestinations();
        }

        private void UpdateBuyersDestinations()
        {
            Buyer buyer;
            for (int i = 0; i < buyersInQueue.Count; i++)
            {
                buyer = buyersInQueue[i];
                /*
                buyer.SetDestination(GetQueuePlace(i), (buyer) =>
                {
                    if (buyersInQueue[0] == buyer) OnUpdateCurrentOrderEvent?.Invoke(buyer.OrderData);
                });
                */
            }
        }
    }
}