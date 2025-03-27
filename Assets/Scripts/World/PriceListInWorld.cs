using Configs;
using TheSTAR.Utility;
using UnityEngine;
using Zenject;

public class PriceListInWorld : MonoBehaviour
{
    [SerializeField] private UnityDictionary<ItemType, CostPanel[]> costPanels = new();

    private readonly ConfigHelper<ItemsConfig> itemsConfig = new();

    public CostPanel FindPanel(ItemType itemType) => costPanels.Get(itemType)[0];

    private XpController xp;
    private TutorialController tutorial;

    [Inject]
    private void Construct(XpController xp, AllPrices allPrices, TutorialController tutorial)
    {
        this.xp = xp;
        this.tutorial = tutorial;
        
        xp.SubscribeOnLevelUp(OnChangeLevel);
        allPrices.SubscribeToSetPrice(OnSetPrice);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var pair in costPanels.KeyValues)
        {
            var itemData = itemsConfig.Get.Item(pair.Key);
            
            foreach (var panel in pair.Value)
            {
                panel.Init(pair.Key);
                panel.gameObject.SetActive(xp.CurrentLevel >= itemData.XpData.NeededLevelForBuy);
            }
        }
    }

    private void OnChangeLevel(int level)
    {
        foreach (var costPair in costPanels.KeyValues)
        {
            var itemData = itemsConfig.Get.Item(costPair.Key);
            
            foreach (var panel in costPair.Value) 
                panel.gameObject.SetActive(level >= itemData.XpData.NeededLevelForBuy);
        }
    }

    private void OnSetPrice(ItemType itemType, DollarValue dollarValue)
    {
        if (costPanels.ContainsKey(itemType))
        {
            foreach (var panel in costPanels.Get(itemType)) panel.DisplayCost(dollarValue);
        }

        switch (itemType)
        {
            case ItemType.FinalBurger_Cheeseburger:
                if (!tutorial.IsCompleted(TutorialType.UpdateMenu_Cheeseburger)) tutorial.CompleteTutorial(TutorialType.UpdateMenu_Cheeseburger);
                break;

            case ItemType.FinalBurger_Medium:
                if (!tutorial.IsCompleted(TutorialType.UpdateMenu_burgerM)) tutorial.CompleteTutorial(TutorialType.UpdateMenu_burgerM);
                break;

            /*case ItemType.FinalFrenchFries:
                if (!tutorial.IsCompleted(TutorialType.UpdateMenu_frenchFries)) tutorial.CompleteTutorial(TutorialType.UpdateMenu_frenchFries);
                break;*/

            case ItemType.FinalBurger_Star:
                if (!tutorial.IsCompleted(TutorialType.UpdateMenu_starburger)) tutorial.CompleteTutorial(TutorialType.UpdateMenu_starburger);
                break;

            case ItemType.FinalBurger_Mega:
                if (!tutorial.IsCompleted(TutorialType.UpdateMenu_mega)) tutorial.CompleteTutorial(TutorialType.UpdateMenu_mega);
                break;
        }
    }
}