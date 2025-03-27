using ReputationContent;
using TheSTAR.GUI;
using UnityEngine;
using Zenject;

namespace DayChangeContent.Counters
{
    public class ReputationCounter : MonoBehaviour
    {
        private Reputation _reputation;
        private GuiController _guiController;

        public int CurrentSaveReputationValue { get; private set; }

        [Inject]
        private void Construct(GuiController guiController)
        {
            _guiController = guiController;
        }

        private void Start()
        {
            _reputation = _guiController.TopUiContainer.Reputation;
            _reputation.NewReputationValueReceived += ChangeValue;
            Load();
        }

        private void ChangeValue(int value)
        {
            Debug.Log("VALUE " + value);
            CurrentSaveReputationValue += value;
            Save();
        }

        public void ClearValue()
        {
            CurrentSaveReputationValue = 0;
            Save();
        }

        private void Load()
        {
            CurrentSaveReputationValue = PlayerPrefs.GetInt("CurrentSaveReputationValue", 0);
        }

        private void Save()
        {
            PlayerPrefs.SetInt("CurrentSaveReputationValue", CurrentSaveReputationValue);
        }
    }
}