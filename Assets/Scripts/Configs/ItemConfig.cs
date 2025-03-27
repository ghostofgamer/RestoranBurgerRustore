using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "Data/Item")]
public class ItemConfig : ScriptableObject
{
    [SerializeField] public ItemData itemData;
    public ItemData ItemData => itemData;
}

[Serializable]
public class ItemData
{
    [SerializeField] public ItemMainData mainData;
    [SerializeField] private ItemCostData costData;
    [SerializeField] private ItemXpData xpData;
    [SerializeField] private ItemOtherData otherData;
    [SerializeField] private RecipeData recipe;

    public ItemMainData MainData => mainData;
    public ItemCostData CostData => costData;
    public ItemXpData XpData => xpData;
    public ItemOtherData OtherData => otherData;
    public RecipeData Recipe => recipe;
}

[Serializable]
public class ItemMainData
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private ItemSectionType section;
    [SerializeField] private Item itemPrefab;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite iconSprite;

    public ItemType ItemType => itemType;
    public ItemSectionType SectionType => section;
    public Item ItemPrefab => itemPrefab;
    public string Name => itemName;
    public Sprite IconSprite => iconSprite;
}

[Serializable]
public class ItemCostData
{
    [SerializeField] private DollarValue buyCost;
    [SerializeField] private DollarValue saleCostMin;
    [SerializeField] private DollarValue saleCostRec;
    [SerializeField] private DollarValue saleCostMax;
    // [SerializeField]private DollarValue sellCostMaxRecommendation;

    public DollarValue BuyCost => buyCost;
    public DollarValue SaleCostMin => saleCostMin;
    public DollarValue SaleCostRec => saleCostRec;
    public DollarValue SaleCostMax => saleCostMax;
    
    public DollarValue SellCostMaxRecommendation
    {
        get
        {
            // Перевод рекомендованной цены в центы
            int totalCents = saleCostRec.dollars * 100 + saleCostRec.cents;

            // Увеличение на 15%
            totalCents = (int)(totalCents * 1.15f);

            // Перевод обратно в доллары и центы
            return new DollarValue
            {
                dollars = totalCents / 100,
                cents = totalCents % 100
            };
        }
    }
}

[Serializable]
public class ItemXpData
{
    [SerializeField] private int buyXpReward;
    [SerializeField] private int saleXpReward;
    [SerializeField] private int neededLevelForBuy;

    public int BuyXpReward => buyXpReward;
    public int SaleXpReward => saleXpReward;
    public int NeededLevelForBuy => neededLevelForBuy;
}

[Serializable]
public class ItemOtherData
{
    [SerializeField] private int boxValue = 1; // сколько единиц приходит в коробке 1 доставке
    [SerializeField] private int portionsValue = 1; // на сколько порций рассчитана 1 единица товара (для расходников)

    public int BoxValue => boxValue;
    public int PortionsValue => portionsValue;
}

[Serializable]
public class RecipeData
{
    [SerializeField] private ItemType[] pecipeItems;

    public ItemType[] RecipeItems => pecipeItems;
}