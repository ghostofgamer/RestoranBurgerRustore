using UnityEngine;
using Zenject;

namespace DayChangeContent.Counters
{
    public class IncomeCounter : MonoBehaviour
    {
        private CurrencyController _currencyController;

        public DollarValue CurrentTipsSaveAmount { get; private set; }

        public DollarValue CurrentIncomeSaveAmount { get; private set; }
        
        public DollarValue CurrentExpensesSaveAmount { get; private set; }
        
        [Inject]
        private void Construct(CurrencyController currencyController)
        {
            _currencyController = currencyController;
            _currencyController.OnTipsChanged += AddTips;
            _currencyController.OnCurrencyChanged += AddIncome;
            _currencyController.CurrenceReduced += ReduceCurrency;
        }

        private void Start()
        {
            Load();
        }

        private void AddTips(DollarValue dollarValue)
        {
            CurrentTipsSaveAmount += dollarValue;
            SaveValue();
        }

        private void AddIncome(DollarValue dollarValue)
        {
            CurrentIncomeSaveAmount += dollarValue;
            SaveValue();
        }

        private void ReduceCurrency(DollarValue dollarValue)
        {
            CurrentExpensesSaveAmount -= dollarValue;
            SaveValue();
        }

        public void ClearValue()
        {
            CurrentTipsSaveAmount = new DollarValue();
            CurrentIncomeSaveAmount = new DollarValue();
            CurrentExpensesSaveAmount = new DollarValue();
            SaveValue();
        }
        
        private void SaveValue()
        {
            PlayerPrefs.SetInt("CurrentTipsDollarsSaveAmount", CurrentTipsSaveAmount.dollars);
            PlayerPrefs.SetInt("CurrentTipsCentsSaveAmount", CurrentTipsSaveAmount.cents);
            
            PlayerPrefs.SetInt("CurrentIncomeSaveDollarsAmount", CurrentIncomeSaveAmount.dollars);
            PlayerPrefs.SetInt("CurrentIncomeSaveCentsAmount", CurrentIncomeSaveAmount.cents);
            
            PlayerPrefs.SetInt("CurrentExpensesSaveDollarsAmount", CurrentExpensesSaveAmount.dollars);
            PlayerPrefs.SetInt("CurrentExpensesSaveCentsAmount", CurrentExpensesSaveAmount.cents);
        }

        private void Load()
        {
            // Загрузка значений для CurrentTipsSaveAmount
            int tipsDollars = PlayerPrefs.GetInt("CurrentTipsDollarsSaveAmount", 0);
            int tipsCents = PlayerPrefs.GetInt("CurrentTipsCentsSaveAmount", 0);
            CurrentTipsSaveAmount = new DollarValue(tipsDollars, tipsCents);

            // Загрузка значений для CurrentIncomeSaveAmount
            int incomeDollars = PlayerPrefs.GetInt("CurrentIncomeSaveDollarsAmount", 0);
            int incomeCents = PlayerPrefs.GetInt("CurrentIncomeSaveCentsAmount", 0);
            CurrentIncomeSaveAmount = new DollarValue(incomeDollars, incomeCents);

            // Загрузка значений для CurrentExpensesSaveAmount
            int expensesDollars = PlayerPrefs.GetInt("CurrentExpensesSaveDollarsAmount", 0);
            int expensesCents = PlayerPrefs.GetInt("CurrentExpensesSaveCentsAmount", 0);
            CurrentExpensesSaveAmount = new DollarValue(expensesDollars, expensesCents);
        }
    }
}