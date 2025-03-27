using System;
using DayChangeContent;
using UnityEngine;
using TheSTAR.Data;
using TMPro;
using Zenject;
using Random = UnityEngine.Random;
using SoundController = TheSTAR.Sound.SoundController;

namespace TheSTAR.GUI
{
    public class DayOverScreen : GuiScreen
    {
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private TMP_Text _dayValueText;

        [Space] [Header("Customers")] [SerializeField]
        private TMP_Text _totalClientsText;

        [SerializeField] private TMP_Text _totalOrdersText;
        [SerializeField] private TMP_Text _totalCompletedOrdersText;

        [Space] [Header("Restaurant")] [SerializeField]
        private TMP_Text _levelText;

        [SerializeField] private TMP_Text _experienceText;
        [SerializeField] private TMP_Text _incomeText;
        [SerializeField] private TMP_Text _tipsText;
        [SerializeField] private TMP_Text _expensesText;
        [SerializeField] private TMP_Text _reputationText;

        [Space] [Header("ProfitBalance")] [SerializeField]
        private TMP_Text _profitText;

        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private TMP_Text _x2ButtonText;

        private int _currentValueSpin = 1;
        private AdsManager ads;
        private GameController game;
        private GuiController gui;
        private DataController data;
        private CurrencyController currency;
        private XpController xp;
        private SoundController sounds;
        private GuiScreen from;
        private string filePath;
        private DayChange _dayChange;
        private int _currentValueDay;
        private TopUiContainer _topUiContainer;
        private int _currentPrizeIndex = -1;
        private DollarValue _currentTips;
        private int _currentXp;

        public event Action NewDayStarting;

        [Inject]
        private void Construct(GameController game, DataController data, GuiController gui, CurrencyController currency,
            XpController xp, SoundController sounds, AdsManager ads)
        {
            this.game = game;
            this.data = data;
            this.gui = gui;
            this.currency = currency;
            this.xp = xp;
            this.sounds = sounds;
            this.ads = ads;
            _topUiContainer = gui.TopUiContainer;
        }

        public void SetStatistic(int totalClients, int totalOrders, int totalCompletedOrders, DollarValue income,
            DollarValue tips, DollarValue expenses, int level, int experience, int reputation)
        {
            _currentValueDay = PlayerPrefs.GetInt("CurrentValueDay", 1);
            _dayValueText.text = $"Report of the day(Day {_currentValueDay})";
            _totalClientsText.text = $"Total Clients : {totalClients}";
            _totalOrdersText.text = $"Total Orders  : {totalOrders}";
            _totalCompletedOrdersText.text = $"Completed Orders : {totalCompletedOrders}";
            DollarValue currentIncome = income - tips;
            _incomeText.text = $"Income : {currentIncome}";
            _tipsText.text = $"Tips : {tips}";
            _expensesText.text = $"Expenses : <color=red>{expenses}</color>";
            _levelText.text = $"Level : +{level}";
            _experienceText.text = $"Experience : +{experience}";
            _reputationText.text = $"Reputation : {reputation}";
            DollarValue profitDay = income + expenses;
            int totalCents = profitDay.dollars * 100 + profitDay.cents;
            string color = totalCents >= 0 ? "green" : "red";
            // _profitText.text = $"Profit : {profitDay}";
            _profitText.text = $"<color={color}>{profitDay}</color>";
            _balanceText.text = $"Balance : {currency.GetCurrencyValue(CurrencyType.Soft)}";

            _currentPrizeIndex = PlayerPrefs.GetInt("CurrentPrizeIndex", -1);

            if (_currentPrizeIndex < 0)
            _currentPrizeIndex = Random.Range(0, 2);

            _currentTips = tips;
            _currentXp = experience;
            string prizeButtonText = _currentPrizeIndex == 0 ? " x4 Tips " : " x2 XP ";
            _x2ButtonText.text = prizeButtonText;
            PlayerPrefs.SetInt("CurrentPrizeIndex", _currentPrizeIndex);
        }

        public void Init(GuiScreen from, bool useMainMenuFon)
        {
            this.from = from;
            SetUseMainMenuFon(useMainMenuFon);
        }

        public void AdsX2DayEnd()
        {
            ads.ShowRewarded("x2DayEnd", (success) =>
            {
                if (!success) return;
                ActivatePrize();
            });
        }

        private void ActivatePrize()
        {
            if (_currentPrizeIndex == 0)
                currency.AddCurrencyFastMoney(CurrencyType.Soft, _currentTips * 4);
            else
                xp.AddXp(_currentXp *= 2);

            _currentPrizeIndex = -1;
            StartNewDay();
            PlayerPrefs.SetInt("CurrentPrizeIndex", _currentPrizeIndex);
        }

        private void SetButton()
        {
        }

        public override void Init()
        {
            base.Init();
        }

        protected override void OnShow()
        {
            base.OnShow();
        }

        public void StartNewDay()
        {
            NewDayStarting?.Invoke();
            gui.ShowMainScreen();
            _currentValueDay++;
            PlayerPrefs.SetInt("CurrentValueDay", _currentValueDay);
        }

        protected override void OnHide()
        {
        }
    }
}