using System;
using System.Collections.Generic;
using System.Linq;
using GarbagesContent;
using UnityEngine;
using World;
using Zenject;
using Random = System.Random;

/// <summary>
/// Посадочное место, за которым может расположиться один клиент. Может быть занято или не занято.
/// </summary>
public class BuyerPlace : MonoBehaviour
{
    [SerializeField] private PathPoint point;
    [SerializeField] private Transform sittingPlace;
    [SerializeField] private BuyerTablePlace tablePlace;
    [SerializeField] private BuyerCash cash;
    [SerializeField] private MiniOrderList miniOrderList;
    [SerializeField] private GarbagePackage[] _garbages;
    [SerializeField] private int index;

    private ItemsController items;
    private int _maxPollutaionCategories = 3;
    private GameController _gameController;
    private TutorialController _tutorial;

    public PathPoint Point => point;
    public Transform SittingPlace => sittingPlace;
    public List<Draggable> OrderItems => tablePlace.OrderItems;
    public BuyerTablePlace OrderPlate => tablePlace;
    public TutorInWorldFocus TutorFocus => tablePlace.TutorInWorldFocus;

    public int PollutionLevel => _pollutionLevel;

    private Buyer buyer;

    private bool haveCash;

    private int _pollutionLevel = 0;

    public bool HaveCash => haveCash;
    public BuyerCash Cash => cash;

    public Buyer Buyer => buyer;
    public bool Available => buyer == null && !haveCash;

    public int Index => index;

    [Inject]
    private void Construct(ItemsController items, GameController gameController,
        TutorialController tutorialController)
    {
        this.items = items;
        _gameController = gameController;
        _tutorial = tutorialController;
        //Debug.Log("Construct BuyerPlace");
    }

    public void Init(AllBuyerPlaces allPlaces)
    {
        //orderPlate.OnPlaceEvent += (d) => allPlaces.OnPlaceItemOnBuyerOrderPlate;
        //orderPlate.OnUpdateDraggablesEvent += () => allPlaces.OnChangeItemsInBuyerPlace(this);
        tablePlace.Init(index);
        cash.Init(this);
        DeactivateCash();
    }

    public int GetTrashActiveCount()
    {
        int amount = 0;

        foreach (var garbage in _garbages)
        {
            if (garbage.gameObject.activeSelf)
                amount++;
        }

        return amount;
    }
    
    public void PolluteTable()
    {
        if (_garbages.Length <= 0) return;

        if (_pollutionLevel >= _maxPollutaionCategories) return;

        _pollutionLevel++;

        Random random = new Random();
        List<GarbagePackage> garbagesTable = _garbages.Where(t => !t.IsActive).ToList();

        if (garbagesTable.Count > 0)
        {
            int randomIndex = random.Next(garbagesTable.Count);

            garbagesTable[randomIndex].SetValue(true);
            Debug.Log("Рандомный индекс " + randomIndex);
        }
    }

    public void DecreasePollutionLevel()
    {
        if (_pollutionLevel <= 0) return;

        if (!_tutorial.IsCompleted(TutorialType.ClearTables))
        {
            _tutorial.CompleteTutorial(TutorialType.ClearTables);
            _gameController.TriggerTutorial();
        }
        
        _pollutionLevel--;
        /*_garbages[index].SetActive(false);*/

        Debug.Log("Decreased pollution level: " + _pollutionLevel);
    }

    private void DeactivateGarbages()
    {
        foreach (var garbage in _garbages)
            garbage.gameObject.SetActive(false);
    }

    public void SetBuyer(Buyer buyer)
    {
        this.buyer = buyer;
    }

    public void OnBuyerStartEat()
    {
        //tablePlace.Deactivate();
    }

    public void OnBuyerEndEat()
    {
        //tablePlace.Activate();
    }

    public void ClearBuyer()
    {
        buyer = null;
        //tablePlace.Deactivate();
    }

    public void ShowMiniList(ActiveOrderData orderData)
    {
        miniOrderList.Set(orderData);
        miniOrderList.gameObject.SetActive(true);
    }

    public void HideMiniList()
    {
        miniOrderList.gameObject.SetActive(false);
    }

    public void ClearOrderItems()
    {
        var allDraggables = tablePlace.OrderItems;
        for (int i = allDraggables.Count - 1; i >= 0; i--)
        {
            Draggable draggable = tablePlace.OrderItems[i];
            if (!draggable) continue;

            draggable.CurrentDragger.EndDrag();
            Destroy(draggable.gameObject);
        }

        /*
        List<Item> plateItems = new();
        foreach (var draggable in orderPlate.AllDraggables)
        {
            if (draggable == null) continue;
            var item = draggable.GetComponent<Item>();
            if (item == null) continue;
            plateItems.Add(item);
        }

        foreach (var orderItem in orderData.Items)
        {
            for (int i = 0; i < orderItem.Value; i++)
            {
                var itemOnPlate = plateItems.Find(element => element.ItemType == orderItem.ItemType);
                if (itemOnPlate)
                {
                    itemOnPlate.Draggable.CurrentDragger.EndDrag();
                    Destroy(itemOnPlate.gameObject);
                }
            }
        }
        */
    }

    public void ActivateCash(DollarValue dollarValue, int xpValue)
    {
        cash.SetValue(dollarValue, xpValue);
        cash.gameObject.SetActive(true);
        haveCash = true;
    }

    public void DeactivateCash()
    {
        cash.gameObject.SetActive(false);
        haveCash = false;
    }

    public void CheckItems(out bool haveDrink, out bool haveFood)
    {
        haveDrink = false;
        haveFood = false;

        var allDraggables = tablePlace.OrderItems;

        foreach (var draggalbe in allDraggables)
        {
            if (draggalbe == null) continue;

            var item = draggalbe.GetComponent<Item>();
            if (item == null) continue;

            var section = items.GetItemData(item.ItemType).MainData.SectionType;
            if (section == ItemSectionType.FinalDrink) haveDrink = true;
            else haveFood = true;
        }
    }
}