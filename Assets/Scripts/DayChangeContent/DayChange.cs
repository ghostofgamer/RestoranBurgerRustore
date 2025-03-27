using DayChangeContent.Counters;
using TheSTAR.GUI;
using UnityEngine;
using Zenject;

namespace DayChangeContent
{
    public class DayChange : MonoBehaviour
    {
        [SerializeField] private DayNightCycle _dayNightCycle;
        [SerializeField] private BuyersCounter _buyersCounter;
        [SerializeField] private OrdersCounter _ordersCounter;
        [SerializeField] private IncomeCounter _incomeCounter;
        [SerializeField] private LevelsCounter _levelsCounter;
        [SerializeField] private ReputationCounter _reputationCounter;

        private TopUiContainer _topUiContainer;
        private GuiController _gui;
        private TimeView _timeView;
        private BuyersController _buyersController;
        private DayOverScreen _dayOverScreen;
        private bool _buyersOver = false;
        private FastFood _fastFood;

        public bool DayOver { get; private set; } = false;

        [Inject]
        private void Construct(GuiController guiController, TopUiContainer topUiContainer,
            BuyersController buyersController, DayOverScreen dayOverScreen, GameWorld world)
        {
            _topUiContainer = topUiContainer;
            _timeView = _topUiContainer.TimeView;
            _gui = guiController;
            _dayNightCycle.InitTimeText(_timeView.TimeValueText);
            _buyersController = buyersController;
            _dayOverScreen = dayOverScreen;
            _dayOverScreen.NewDayStarting += _dayNightCycle.StartNewDay;
            _fastFood = world.FastFood;
            _fastFood.RestaurantOpened += StartCycle;
            // _dayOverScreen = _gui.DayOverScreen;
        }

        private void OnEnable()
        {
            // _dayNightCycle.DayOverCompleted += OpenDayStatistic;
            _dayNightCycle.DayOverCompleted += SetDayOverStage;
            _dayNightCycle.NewDayStarted += SetNewDay;
            _dayNightCycle.SetNightLighting += SetActiveNightLight;
        }

        private void OnDisable()
        {
            // _dayNightCycle.DayOverCompleted -= OpenDayStatistic;
            _dayNightCycle.DayOverCompleted -= SetDayOverStage;
            _dayNightCycle.NewDayStarted -= SetNewDay;
            _dayNightCycle.SetNightLighting -= SetActiveNightLight;
        }

        private void SetDayOverStage()
        {
            _buyersCounter.SetAllowedSpawnBuyers(false);
            DayOver = true;
            OpenDayStatistic();
        }

        public void SetBuyersValue()
        {
            _buyersOver = true;
            OpenDayStatistic();
        }

        public void SetActiveNightLight(bool value)
        {
            _fastFood.SetValueNightLight(value);
        }

        private void OpenDayStatistic()
        {
            /*if (!DayOver || !_buyersOver)
                return;*/
            if (!DayOver || _buyersController.CreatedBuyers.Count > 0)
                return;

            SetValue(true);

            _dayOverScreen.SetStatistic(
                _buyersCounter.CurrentSaveBuyers, _ordersCounter.CurrentSaveOrdersAmount,
                _ordersCounter.CurrentSaveCompletedOrdersAmount, _incomeCounter.CurrentIncomeSaveAmount,
                _incomeCounter.CurrentTipsSaveAmount, _incomeCounter.CurrentExpensesSaveAmount,
                _levelsCounter.CurrentSaveAddedLevels, _levelsCounter.CurrentSaveAddedXP,
                _reputationCounter.CurrentSaveReputationValue);

            _topUiContainer.DayOverButton.gameObject.SetActive(true);

            // _gui.ShowForeachScreen<DayOverScreen>();
            // _gui.GetNeedScreen<DayOverScreen>().SetActive(true);
        }

        private void StartCycle()
        {
            // _dayNightCycle.enabled = true;
            _dayNightCycle.SetOpenValue(true);
        }

        private void SetNewDay()
        {
            SetValue(false);
            _buyersCounter.SetAllowedSpawnBuyers(true);
            _buyersCounter.ClearValue();
            _ordersCounter.ClearValue();
            _incomeCounter.ClearValue();
            _levelsCounter.ClearValue();
            _reputationCounter.ClearValue();
            _fastFood.OpenClosedBoard.Reset();
            _dayNightCycle.SetOpenValue(false);
            _dayNightCycle.SetDayTime();
            _dayNightCycle.ResetDay();
        }

        private void SetValue(bool value)
        {
            DayOver = value;
            _buyersOver = value;
        }
    }
}