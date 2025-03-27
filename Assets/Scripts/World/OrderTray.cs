using System;
using System.Collections.Generic;
using UnityEngine;
using World;

public class OrderTray : MonoBehaviour
{
    [SerializeField] private ItemsHandler itemsHandler;
    [SerializeField] private MiniOrderList miniOrderList;
    [SerializeField] private Draggable draggable;
    [SerializeField] private Rigidbody body;
    [SerializeField] private int _indexOrderTray;

    private Buyer _buyer;

    public Buyer Buyer => _buyer;

    private int _numberTable = -1;
    
    public int NumberTable => _numberTable;

    public int IndexOrderTray => _indexOrderTray;

    public Draggable Draggable => draggable;

    private ActiveOrderData currentOrderData;

    public int CurrentOrderIndex
    {
        get
        {
            if (currentOrderData == null) return -1;
            else return currentOrderData.OrderIndex;
        }
    }

    public List<Draggable> AllDraggables => itemsHandler.AllDraggables;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        itemsHandler.OnPlaceEvent += (item) => OnPlaceItem();

        draggable.OnStartDragEvent += OnStartDrag;
        draggable.OnEndDragEvent += OnEndDrag;
        draggable.OnThrowEvent += OnThrow;
    }

    public void SetValueCurrentIndexTable(int index)
    {
        _numberTable = index;
    }
    
    public void AddBuyer(Buyer buyer)
    {
        _buyer = buyer;
    }

    public void ClearBuyer()
    {
        _buyer = null;
    }

    public void SetOrder(ActiveOrderData orderData)
    {
        currentOrderData = orderData;
        miniOrderList.Set(orderData);
        miniOrderList.gameObject.SetActive(true);
    }

    private void OnPlaceItem()
    {
        if (CheckHaveAllNeededItems())
        {
            OnAllNeededItemsPlaced();
        }
    }

    private bool CheckHaveAllNeededItems()
    {
        List<ItemType> neededItems = new();

        foreach (var orderItem in currentOrderData.OrderData.Items)
        {
            for (int i = 0; i < orderItem.Value; i++)
            {
                neededItems.Add(orderItem.ItemType);
            }
        }

        List<ItemType> haveItems = new();

        foreach (var draggable in itemsHandler.AllDraggables)
        {
            var item = draggable.GetComponent<Item>();
            if (item == null) continue;

            if (item.ItemType == ItemType.BurgerPackingPaper_Closed)
            {
                haveItems.Add(item.GetComponent<PackingPaperItem>().Dragger.CurrentDraggable.GetComponent<Item>()
                    .ItemType);
            }
            else if (item.ItemType == ItemType.SmallCompletedBurge || item.ItemType == ItemType.Cheeseburger ||
                     item.ItemType == ItemType.MBurger || item.ItemType == ItemType.StarBurger ||
                     item.ItemType == ItemType.BigBurger || item.ItemType == ItemType.MegaBurger)
            {
                haveItems.Add(item.GetComponent<PackingPaperItem>().Dragger.CurrentDraggable.GetComponent<Item>()
                    .ItemType);
            }
            else haveItems.Add(item.ItemType);
        }

        if (neededItems.Count != haveItems.Count) return false;

        foreach (var neededItem in neededItems)
        {
            if (!haveItems.Contains(neededItem)) return false;

            haveItems.Remove(neededItem);
        }

        return true;
    }

    private void OnAllNeededItemsPlaced()
    {
        Debug.Log("Есть все необходимые предметы");

        haveAllNeededItems = true;

        foreach (var draggable in itemsHandler.AllDraggables)
        {
            draggable.ForceDeactivateCol();
        }
    }

    private bool haveAllNeededItems = false;
    public bool HaveAllNeededItems => haveAllNeededItems;

    private void OnStartDrag()
    {
        body.isKinematic = true;
    }

    private void OnEndDrag()
    {
        body.isKinematic = false;
    }

    private void OnThrow(Vector3 force)
    {
        body.AddForce(force, ForceMode.Impulse);
    }

    public void Clear()
    {
        currentOrderData = null;
        OnEndDrag();
        haveAllNeededItems = false;
        miniOrderList.gameObject.SetActive(false);
        body.isKinematic = true;
    }
}