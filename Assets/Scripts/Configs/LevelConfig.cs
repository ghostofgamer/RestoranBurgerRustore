using System;
using UnityEngine;
using TheSTAR.Utility;

namespace Configs
{
    [Obsolete]
    [CreateAssetMenu(menuName = "Data/Level", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private LevelData data;
        public LevelData Data => data;
    }

    [Serializable]
    public struct LevelData
    {
        [SerializeField] private int minBuyersCount; // сколько минимально покупателей одновременно стоят в очереди
        [SerializeField] private int maxBuyersCount; // сколько максимально покупателей одновременно стоят в очереди

        [Space]
        [SerializeField] private GameTimeSpan ordersPeriodMin; // период появления заказов (если заказов нет совсем, заказ появляется сразу)
        [SerializeField] private GameTimeSpan ordersPeriodMax; // период появления заказов (если заказов нет совсем, заказ появляется сразу)

        public int MinBuyersCount => minBuyersCount;
        public int MaxBuyersCount => maxBuyersCount;
        public GameTimeSpan OrdersPeriodMin => ordersPeriodMin;
        public GameTimeSpan OrdersPeriodMax => ordersPeriodMax;
    }

    public enum LevelEventType
    {
        /// <summary>
        /// Необходимо выполнить заказ
        /// </summary>
        Order,

        /// <summary>
        /// Открывается новый рецепт
        /// </summary>
        NewRecipe,

        /// <summary>
        /// Открывается дверь
        /// </summary>
        OpenDoor,

        /// <summary>
        /// Появляется площадка для покупки рабочего
        /// </summary>
        ActivateBuyWorkerPlace,

        /// <summary>
        /// Появляется площадка для покупки фабрики
        /// </summary>
        ActivateBuyFactoryPlace,

        /// <summary>
        /// Необходимо купить постройку
        /// </summary>
        BuyFactory,

        /// <summary>
        /// Необходимо заработать определённую сумму
        /// </summary>
        GoldCondition,

        /// <summary>
        /// Необходимо купить рабочего
        /// </summary>
        BuyWorker
    }
}