using System;
using TheSTAR.Utility;
using UnityEngine;
using World;
//using Sirenix.OdinInspector;

namespace Configs
{
    [CreateAssetMenu(menuName = "Data/Items", fileName = "ItemsConfig")]
    public class ItemsConfig : ScriptableObject
    {
        //[Obsolete][SerializeField] private ItemData[] items = new ItemData[0];
        [SerializeField] private ItemConfig[] _items = new ItemConfig[0];

        [Space]
        [SerializeField] private ItemType[] productItemTypes;
        [SerializeField] private ItemType[] decorationItemTypes;

        //public ItemData[] Items => items;
        public ItemData Item(ItemType itemType) => _items[(int)itemType].itemData;
        public ItemType[] ProductItemTypes => productItemTypes;
        public ItemType[] DecorationItemTypes => decorationItemTypes;

        #if UNITY_EDITOR

        [ContextMenu("Parse")]
        private void Parse()
        {
            // logic of parse
            /*
            for (int i = 0; i < items.Length; i++)
            {
                _items[i].itemData = items[i];
                ConfigUtility.Save(_items[i]);
            }
            */

            ConfigUtility.Save(this);
        }

        #endif
    }
}

public enum ItemSectionType
{   
    /// <summary>
    /// Ингредиент для приготовления блюда
    /// </summary>
    Ingredient,

    /// <summary>
    /// Упаковочная бумага
    /// </summary>
    PackingPaper,

    /// <summary>
    /// Стакан
    /// </summary>
    Cup,

    
    FinalBurger,
    
    /// <summary>
    /// Готовый напиток, который можем отдать клиенту, и он будет его пить
    /// </summary>
    FinalDrink,

    /// <summary>
    /// Расходники
    /// </summary>
    Consumables,

    /// <summary>
    /// Большие цистерны
    /// </summary>
    Cistern,

    /// <summary>
    /// Фри, наггетсы
    /// </summary>
    FinalFries,

    /// <summary>
    /// Упакованный финальный бургер
    /// </summary>
    PackingFinalBurger
}

public enum ItemType
{
    Bun,
    BunTop,
    BunBottom,
    CutletRaw,
    CheeseSolid,
    TomatoSolid,
    OnionSolid,
    CabbageSolid,

    FrenchFriesFrozen,
    NuggetsFrozen,

    BurgerPackingPaper,
    FriesPackingPaper,
    NuggetsPackingPaper,

    SodaCup,
    CoffeeCup,

    FinalFrenchFries,
    FinalNuggets,
    FinalBurger_Small,
    FinalBurger_Cheeseburger,
    FinalBurger_Medium,
    FinalBurger_Star,
    FinalBurger_Big,
    FinalBurger_Mega,
    
    ColaCistern,
    LemonCistern,
    OrangeCistern,
    BarberryCistern,
    CoffeeBeans,
    
    FinalCola,
    FinalLemon,
    FinalOrange,
    FinalBarberry,
    FinalCoffee,

    CheesePiece,
    TomatoPiece,
    OnionPiece,
    CabbagePiece,

    FrenchFriesBurnt,
    NuggetsBurnt,

    CutletWell,
    CutletBurnt,

    BurgerPackingPaper_Closed,

    KetchupUsed,
    MustardUsed,
    SmallCompletedBurge,
    Cheeseburger,
    MBurger,
    StarBurger,
    BigBurger,
    MegaBurger
}