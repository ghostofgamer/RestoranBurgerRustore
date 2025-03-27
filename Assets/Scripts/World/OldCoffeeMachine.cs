using System;
using Configs;
using TheSTAR.Utility;
using UnityEngine;
using Zenject;

[Obsolete]
public class OldCoffeeMachine : MonoBehaviour
{
    /*
    [SerializeField] private TouchInteractive touchInteractive;
    [SerializeField] private GameObject portafilterVisual;
    [SerializeField] private Dragger coffeeDragger;
    [SerializeField] private Filler milkFiller;

    [Header("Cost")]
    [SerializeField] private CostHandler espressoCostHander;
    [SerializeField] private CostHandler americanoCostHander;
    [SerializeField] private CostHandler capuccinoCostHander;

    [Space]
    [SerializeField] private TutorInWorldFocus coffeeMachineTutorFocus;
    [SerializeField] private TutorInWorldFocus setEspressoCostTutorFocus;

    private bool havePortafilter = false;
    public bool HavePortafilter => havePortafilter;
    public bool CanMakeEspresso => havePortafilter && !coffeeDragger.CurrentDraggable;

    public TutorInWorldFocus CoffeeMachineTutorFocus => coffeeMachineTutorFocus;
    public TutorInWorldFocus SetEspressoCostTutorFocus => setEspressoCostTutorFocus;

    private GameController game;
    private GameWorldInteraction gameWorldInteraction;
    private XpController xp;
    private ItemsController items;

    [Inject]
    private void Consruct(GameController game, GameWorldInteraction gameWorldInteraction, XpController xp, ItemsController items)
    {
        this.game = game;
        this.gameWorldInteraction = gameWorldInteraction;
        this.xp = xp;
        this.items = items;
    }

    public bool HaveCup(out Cup cup)
    {
        if (!coffeeDragger.CurrentDraggable)
        {
            cup = null;
            return false;
        }
        else
        {
            cup = coffeeDragger.CurrentDraggable.GetComponent<Cup>();
            return true;
        }
    }

    public bool CanFill(ItemType itemType) => milkFiller.CanFill(itemType);

    public bool CanUseMilk => milkFiller.CanUse;

    void Start()
    {
        Init();
    }

    private void Init()
    {                
        touchInteractive.OnClickEvent += () =>
        {
            gameWorldInteraction.OnClickCoffeeMachine(this);
        };

        espressoCostHander.Init();
        americanoCostHander.Init();
        capuccinoCostHander.Init();

        espressoCostHander.SetItem(ItemType.EspressoFinal);
        americanoCostHander.SetItem(ItemType.AmericanoFinal);
        capuccinoCostHander.SetItem(ItemType.CappuccinoFinal);

        // todo цену кофе доставать из сохранений
        var espressoData = items.GetItemData(ItemType.EspressoFinal);
        espressoCostHander.SetPrice(espressoData.SaleCostRec);

        var americanoData = items.GetItemData(ItemType.AmericanoFinal);
        americanoCostHander.SetPrice(americanoData.SaleCostRec);

        var capuccinoData = items.GetItemData(ItemType.CappuccinoFinal);
        capuccinoCostHander.SetPrice(capuccinoData.SaleCostRec);

        UpdateCostPanelsByPlayerLevel();
    }

    public void SetPortafilter(Portafilter portafilter)
    {
        if (portafilter.Draggable.CurrentDragger) portafilter.Draggable.CurrentDragger.EndDrag();
        Destroy(portafilter.gameObject);
        havePortafilter = true;
        UpdateVisual();

        //analytics.Trigger(RepeatingEventType.SetPortafilterInCoffeeMachine);
    }

    public void UsePortafilter()
    {
        havePortafilter = false;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        portafilterVisual.SetActive(havePortafilter);
    }

    public void FillCup(ItemType coffeeType)
    {
        var cupDrabbagle = coffeeDragger.CurrentDraggable;
        if (!cupDrabbagle) return;

        var cup = cupDrabbagle.GetComponent<Cup>();
        if (!cup) return;

        var currentCupContent = cup.Dragger.CurrentDraggable;
        if (currentCupContent != null)
        {
            cup.Dragger.EndDrag();
            Destroy(currentCupContent.gameObject);
        }

        //var drink = game.World.Items.CreateItem(coffeeType);
        //cup.Dragger.StartDrag(drink.Draggable);
    }

    private DollarValue GetPrice(ItemType itemType)
    {
        if (itemType == ItemType.EspressoFinal || itemType == ItemType.EspressoDrink) return espressoCostHander.CurrentPrice;
        else if (itemType == ItemType.AmericanoFinal || itemType == ItemType.AmericanoDrink ) return americanoCostHander.CurrentPrice;
        else if (itemType == ItemType.CappuccinoFinal || itemType == ItemType.CappuccinoDrink) return capuccinoCostHander.CurrentPrice;

        return default;
    }

    public void Fill(Item item) => milkFiller.Fill(item);

    public void SetCup(Item cup)
    {
        if (!coffeeDragger.HavePlace(cup.ItemType)) return;
        if (cup.Draggable.CurrentDragger) cup.Draggable.CurrentDragger.EndDrag();
        coffeeDragger.StartDrag(cup.Draggable);

        //analytics.Trigger(RepeatingEventType.SetCupToCoffeeMachine);
    }

    public void UseMilk()
    {
        milkFiller.Use();
    }

    public void UpdateCostPanelsByPlayerLevel()
    {
        int currentLevel = xp.CurrentLevel;

        bool availableEspresso = true;
        bool availableAmericano = currentLevel >= items.GetItemData(ItemType.AmericanoFinal).NeededLevelForBuy;
        bool availableCapuccino = currentLevel >= items.GetItemData(ItemType.CappuccinoFinal).NeededLevelForBuy;

        espressoCostHander.gameObject.SetActive(availableEspresso);
        americanoCostHander.gameObject.SetActive(availableAmericano);
        capuccinoCostHander.gameObject.SetActive(availableCapuccino);
    }
    */
}