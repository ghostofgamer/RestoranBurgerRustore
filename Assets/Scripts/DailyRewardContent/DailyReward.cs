using DecorationContent;
using TheSTAR.GUI;
using UnityEngine;
using Zenject;

namespace DailyRewardContent
{
    public class DailyReward : MonoBehaviour
    {
        private GuiController _gui;
        private XpController _xp;
        private CurrencyController _currencyController;
        private GameWorld _gameWorld;
        private FastFood _fastFood;
        private DecorationSystem _decorationSystem;
        
        [Inject]
        private void Consruct(GuiController gui, XpController xp, CurrencyController currency)
        {
            _gui = gui;
            _xp = xp;
            _currencyController = currency;
            // _gameWorld = world;
        }

        private void Start()
        {
            _gameWorld = _gui.GameWorld;
            _fastFood = _gui.GameWorld.FastFood;
            _decorationSystem = _gui.GameWorld.DecorationSystem;
        }

        public void Claim(int index)
        {
            switch (index)
            {
                case 0:
                    _currencyController.AddCurrencyFastMoney(CurrencyType.Soft, new(50, 0), true);
                    break;
                
                case 1:
                    _xp.AddXp(50);
                    break;
                
                case 2:
                    _currencyController.AddCurrencyFastMoney(CurrencyType.Soft, new(250, 0), true);
                    break;
                
                case 3:
                    _xp.AddXp(300);
                    break;
                
                case 4:
                    _currencyController.AddCurrencyFastMoney(CurrencyType.Soft, new(400, 0), true);
                    break;
                
                case 5:
                    _xp.AddXp(600);
                    break;
                
                case 6:
                    TakeSuperPrize();
                    break;
            }
        }

        private void TakeSuperPrize()
        {
            if (_decorationSystem.GetActivationValueDecoration(_decorationSystem.CurrentDailyRewardDecoration))
            {
                _decorationSystem.ActivateDecoration(_decorationSystem.CurrentDailyRewardDecoration);
                Debug.Log("актвируем ДЕКОР ");
            }
            else
            {
                _currencyController.AddCurrencyFastMoney(CurrencyType.Soft, new(500, 0), true);
                Debug.Log("актвируем БАБКИ ");
            }
        }
    }
}