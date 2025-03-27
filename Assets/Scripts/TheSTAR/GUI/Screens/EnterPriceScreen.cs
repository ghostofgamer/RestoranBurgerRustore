using System;
using Configs;
using TheSTAR.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TheSTAR.GUI
{
    public class EnterPriceScreen : GuiScreen
    {
        [SerializeField] private PointerButton acceptButton;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI profitText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI marketPriceText;
        [SerializeField] private Scrollbar scrollbar;

        [SerializeField] private Color greenColor;
        [SerializeField] private Color redColor;
        [SerializeField] private Color _recomendationColor;
        
        private ItemType itemType;
        private DollarValue currentPrice;
        private ItemData itemData;
        private Action<ItemType, DollarValue> acceptAction;
        private int minSaleCostSimple;
        private int maxSaleCostSimple;
        // private DollarValue _maxRecomendationPrice;

        private GuiController gui;

        private readonly ConfigHelper<ItemsConfig> itemsConfig = new();

        [Inject]
        private void Construct(GuiController gui)
        {
            this.gui = gui;
        }

        public override void Init()
        {
            base.Init();

            acceptButton.Init(OnAcceptClick);

            scrollbar.onValueChanged.AddListener((value) =>
            {
                currentPrice = new((int)MathUtility.ProgressToValue(value, minSaleCostSimple, maxSaleCostSimple));
                DisplayCurrentPrice();
            });
        }

        public void Init(ItemType itemType, DollarValue currentPrice, Action<ItemType, DollarValue> acceptAction)
        {
            this.itemType = itemType;
            this.currentPrice = currentPrice;
            this.acceptAction = acceptAction;

            itemData = itemsConfig.Get.Item(itemType);
            

            // DollarValue recommendedPrice = itemData.CostData.SaleCostRec;
            /*int totalCents = itemData.CostData.SaleCostRec.dollars * 100 + itemData.CostData.SaleCostRec.cents;
            totalCents = (int)(totalCents * 1.15f);
            _maxRecomendationPrice = new DollarValue
            {
                dollars = totalCents / 100,
                cents = totalCents % 100
            };*/
            
            Debug.Log("Recommended Price: " + itemData.CostData.SaleCostRec.dollars + "." + itemData.CostData.SaleCostRec.cents.ToString("D2") + " USD");
            // Debug.Log("Max Recommended Price: " + _maxRecomendationPrice.dollars + "." + _maxRecomendationPrice.cents.ToString("D2") + " USD");
            Debug.Log("MAXMAXMAX " + itemData.CostData.SellCostMaxRecommendation);
            
            icon.sprite = itemData.MainData.IconSprite;
            nameText.text = itemData.MainData.Name;
            costText.text = $"Cost: {TextUtility.FormatPrice(itemData.CostData.BuyCost)}";
            marketPriceText.text = $"Recommended: {TextUtility.FormatPrice(itemData.CostData.SaleCostRec)}";

            minSaleCostSimple = itemData.CostData.SaleCostMin.ToSimpleValue();
            maxSaleCostSimple = itemData.CostData.SaleCostMax.ToSimpleValue();
            int current = currentPrice.ToSimpleValue();
            float progress = MathUtility.GetProgress(current, minSaleCostSimple, maxSaleCostSimple);
            scrollbar.SetValueWithoutNotify(progress);

            DisplayCurrentPrice();
        }

        private void DisplayCurrentPrice()
        {
            priceText.text = $"Price: {TextUtility.FormatPrice(currentPrice)}";

            if (currentPrice > itemData.CostData.SellCostMaxRecommendation)
            {
                priceText.color = redColor;
            }
            else
            {
                priceText.color = _recomendationColor;
            }
            
            if (currentPrice >= itemData.CostData.BuyCost)
            {
                var profit = currentPrice - itemData.CostData.BuyCost;
                profitText.text = $"Profit: {profit}";
                profitText.color = greenColor;
            }
            else
            {
                var antiProfit = itemData.CostData.BuyCost - currentPrice;
                profitText.text = $"Profit: -{antiProfit}";
                profitText.color = redColor;
            }
        }

        private void OnAcceptClick()
        {
            acceptAction?.Invoke(itemType, currentPrice);
            Debug.Log("ТИП " + itemType);
            gui.ShowMainScreen();
        }
    }
}