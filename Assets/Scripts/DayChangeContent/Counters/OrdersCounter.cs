using UnityEngine;
using Zenject;

namespace DayChangeContent.Counters
{
    public class OrdersCounter : MonoBehaviour
    {
        private GameWorldInteraction _gameWorldInteraction;
        private BuyersController _buyersController;

        public int CurrentSaveOrdersAmount { get; private set; }
        public int CurrentSaveCompletedOrdersAmount { get; private set; }

        [Inject]
        private void Construct(GameWorldInteraction gameWorldInteraction, BuyersController buyersController)
        {
            _gameWorldInteraction = gameWorldInteraction;
            _buyersController = buyersController;
            _gameWorldInteraction.OrderAdded += AddOrder;
            _buyersController.OrderCompleted += AddCompletedOrder;
        }

        private void Start()
        {
            CurrentSaveOrdersAmount = PlayerPrefs.GetInt("CurrentSaveOrdersAmount", 0);
            CurrentSaveCompletedOrdersAmount = PlayerPrefs.GetInt("CurrentSaveCompletedOrdersAmount", 0);
        }

        private void AddOrder()
        {
            CurrentSaveOrdersAmount++;
            SaveValue();
        }

        private void AddCompletedOrder()
        {
            CurrentSaveCompletedOrdersAmount++;
            SaveValue();
        }

        public void ClearValue()
        {
            CurrentSaveOrdersAmount = 0;
            CurrentSaveCompletedOrdersAmount = 0;
            SaveValue();
        }

        private void SaveValue()
        {
            PlayerPrefs.SetInt("CurrentSaveOrdersAmount", CurrentSaveOrdersAmount);
            PlayerPrefs.SetInt("CurrentSaveCompletedOrdersAmount", CurrentSaveCompletedOrdersAmount);
        }
    }
}