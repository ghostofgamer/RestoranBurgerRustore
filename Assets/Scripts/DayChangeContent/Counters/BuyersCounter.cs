using UnityEngine;
using Zenject;

namespace DayChangeContent.Counters
{
    public class BuyersCounter : MonoBehaviour
    {
        [SerializeField] private DayChange _dayChange;

        private BuyersController _buyersController;
        private int _currentValueBuyers;
        
        public int CurrentSaveBuyers { get;private set; }

        public bool IsAllowedSpawnNewBuyer { get; private set; } = true;

        [Inject]
        private void Construct(BuyersController buyersController)
        {
            _buyersController = buyersController;

            // _buyersController.BuyersCountChanged += ChangeValue;

            _buyersController.BuyersCountIncreased += IncreaseValue;
            _buyersController.BuyersCountDecreased += DecreaseValue;
            _buyersController.InitBuyersCounter(this);
        }

        private void Start()
        {
            // _currentValueBuyers = _buyersController.CreatedBuyers.Count;
            SetValue(_buyersController.CreatedBuyers.Count);

            CurrentSaveBuyers = PlayerPrefs.GetInt("CurrentValueBuyers", 0);
            Debug.Log("_curerentSAVEVALUE " + CurrentSaveBuyers);
        }

        private void ChangeValue(int value)
        {
            SetValue(value);
        }

        public void ClearValue()
        {
            _currentValueBuyers = 0;
            CurrentSaveBuyers = 0;
            PlayerPrefs.SetInt("CurrentValueBuyers", CurrentSaveBuyers);
        }

        private void SetValue(int value)
        {
            _currentValueBuyers = value;

            if (_dayChange.DayOver)
            {
                if (_currentValueBuyers == 0)
                    _dayChange.SetBuyersValue();
            }

            /*_currentSaveBuyers = value;
            PlayerPrefs.SetInt("CurrentValueBuyers", _currentSaveBuyers);

            if (_dayChange.DayOver)
            {
                if (_currentSaveBuyers == 0)
                    _dayChange.SetBuyersValue();
            }*/
        }

        private void IncreaseValue(int allValue)
        {
            CurrentSaveBuyers++;
            PlayerPrefs.SetInt("CurrentValueBuyers", CurrentSaveBuyers);
            SetValue(allValue);
        }

        private void DecreaseValue(int allValue)
        {
            /*CurrentSaveBuyers--;
            PlayerPrefs.SetInt("CurrentValueBuyers", CurrentSaveBuyers);*/
            SetValue(allValue);
        }

        public void SetAllowedSpawnBuyers(bool value)
        {
            IsAllowedSpawnNewBuyer = value;
        }
    }
}