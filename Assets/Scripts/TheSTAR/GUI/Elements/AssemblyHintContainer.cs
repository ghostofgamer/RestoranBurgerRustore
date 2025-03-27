using System.Collections.Generic;
using TheSTAR.GUI;
using TMPro;
using UnityEngine;
using Zenject;

public class AssemblyHintContainer : MonoBehaviour
{
    [SerializeField] private AssemblyHintUI tutorialHint;
    [SerializeField] private AssemblyHintUI[] hints;
    [SerializeField] private PointerButton moreBtn;
    [SerializeField] private PointerButton lessBtn;
    [SerializeField] private GameObject numberHandler;
    [SerializeField] private TextMeshProUGUI numberText;

    private bool open = false;
    private int activeBurgerHintsCount = 0;
    private bool inTutorial = false;

    private OrdersManager ordersManager;
    private ItemsController items;
    private TutorialController tutorial;

    private List<ItemType> currentAssembly = new();

    [Inject]
    private void Construct(OrdersManager ordersManager, ItemsController items, GameController gameController, TutorialController tutorial)
    {
        this.ordersManager = ordersManager;
        this.items = items;
        this.tutorial = tutorial;

        ordersManager.OnOrderAcceptedEvent += (a) => OnChangeOrders();
        ordersManager.OnOrderCompletedEvent += (b) => OnChangeOrders();

        gameController.OnChangeAssemblyEvent += OnChagneAssembly;
    }

    public void Init()
    {
        foreach (var hint in hints) 
            hint.Init(OnClick);

        moreBtn.Init(OnClick);
        lessBtn.Init(OnClick);

        UpdateOpenCloseUI();

        tutorial.OnStartTutorialEvent += OnStartTutorial;
        tutorial.OnBreakTutorialEvent += OnBreakMakingFirstBurgerTutorial;
        tutorial.OnCompleteTutorialEvent += OnBreakMakingFirstBurgerTutorial;
    }

    private void OnClick()
    {
        if (activeBurgerHintsCount <= 1) return;

        open = !open;

        UpdateOpenCloseUI();
    }

    private void UpdateOpenCloseUI()
    {
        if (activeBurgerHintsCount <= 1)
        {
            open = false;
            moreBtn.gameObject.SetActive(false);
            lessBtn.gameObject.SetActive(false);
            numberHandler.SetActive(false);
        }

        foreach (var hint in hints) 
            hint.gameObject.SetActive(false);

        if (activeBurgerHintsCount == 0) return;
        else
        {    
            if (open)
            {
                for (int i = 0; i < activeBurgerHintsCount; i++)
                {
                    hints[i].gameObject.SetActive(true);
                }

                numberHandler.SetActive(false);
                moreBtn.gameObject.SetActive(false);
                lessBtn.gameObject.SetActive(true);
            }
            else
            {
                hints[0].gameObject.SetActive(true);
                if (activeBurgerHintsCount > 1)
                {
                    numberHandler.SetActive(true);
                    moreBtn.gameObject.SetActive(true);
                }
                lessBtn.gameObject.SetActive(false);
            }
        }
    }

    private void OnChangeOrders()
    {
        SetBurgerOrdersUI();
        UpdateOpenCloseUI();
    }

    private void SetBurgerOrdersUI()
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

        for (int i = 0; i < burgerOrders.Count && i < hints.Length; i++)
        {
            SetDataToHint(hints[i], burgerOrders[i]);
        }

        activeBurgerHintsCount = burgerOrders.Count;
        numberText.text = activeBurgerHintsCount.ToString();

        if (inTutorial) UpdateDataForTutorialHint();
    }

    private void SetDataToHint(AssemblyHintUI hint, ItemType orderItem)
    {
        var itemData = items.GetItemData(orderItem);
        var recipe = itemData.Recipe.RecipeItems;
        Sprite[] recipeSprites = new Sprite[recipe.Length];
        bool[] checks = new bool[recipe.Length];

        var finalBurger = items.FindItem(orderItem);
        bool totalCompleted = finalBurger != null;
        
        Debug.Log("recipeLength " + recipe.Length);
        
        for (int i = 0; i < recipe.Length; i++)
        {
            Debug.Log("recipe " + i);
            recipeSprites[i] = items.GetItemData(recipe[i]).mainData.IconSprite;

            if (totalCompleted) checks[i] = true;
            else
            {
                if (currentAssembly.Count <= i) checks[i] = false;
                else checks[i] = currentAssembly[i] == recipe[i];
            }
        }

        hint.Set(itemData.mainData.IconSprite, recipeSprites, totalCompleted, checks);
    }

    private void OnChagneAssembly(List<ItemType> assembly)
    {
        this.currentAssembly = assembly;
        SetBurgerOrdersUI();
    }

    private void OnStartTutorial(TutorialType tutorialType, TutorialData tutorData)
    {
        if (tutorialType == TutorialType.AssemblyBurger)
        {
            OnStartMakingFirstBurgerTutorial();
        }
    }
    
    public void OnStartMakingFirstBurgerTutorial()
    {
        //Debug.Log("Assembly hint: show tutor", gameObject);
        tutorialHint.gameObject.SetActive(true);
        inTutorial = true;

        UpdateDataForTutorialHint();
    }

    public void OnBreakMakingFirstBurgerTutorial()
    {
        //Debug.Log("Assembly hint: hide tutor", gameObject);
        tutorialHint.gameObject.SetActive(false);
        inTutorial = false;
    }

    private void UpdateDataForTutorialHint()
    {
        SetDataToHint(tutorialHint, ItemType.FinalBurger_Small);
    }
}