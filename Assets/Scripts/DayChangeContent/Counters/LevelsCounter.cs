using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace DayChangeContent.Counters
{
    public class LevelsCounter : MonoBehaviour
    {
        private XpController _xpController;
        
        public int CurrentSaveAddedXP {get; private set; }
        public int CurrentSaveAddedLevels{get; private set; }

        [Inject]
        private void Construct(XpController xpController)
        {
            _xpController = xpController;
            _xpController.OnProfitXpValue += AddXP;
            _xpController.OnLevelUpEvent += AddLevel;
        }

        private void Start()
        {
            LoadValue();
        }

        public void ClearValue()
        {
            CurrentSaveAddedXP = 0;
            CurrentSaveAddedLevels = 0;
            SaveValue();
        }

        private void AddXP(int value)
        {
            CurrentSaveAddedXP += value;
            SaveValue();
        }

        private void AddLevel(int value)
        {
            CurrentSaveAddedLevels += value;
            SaveValue();
        }

        private void LoadValue()
        {
            CurrentSaveAddedXP = PlayerPrefs.GetInt("СurrentSaveAddedXP", 0);
            CurrentSaveAddedLevels = PlayerPrefs.GetInt("CurrentSaveAddedLevels", 0);
        }

        private void SaveValue()
        {
            PlayerPrefs.SetInt("СurrentSaveAddedXP", CurrentSaveAddedXP);
            PlayerPrefs.SetInt("CurrentSaveAddedLevels", CurrentSaveAddedLevels);
        }
    }
}