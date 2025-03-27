using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DailyTimerContent
{
    public class DailyTimer : MonoBehaviour
    {
        public Button[] buttonSpin;
        protected DateTime LastTimesSpin;
        [SerializeField] private string _lastTimeSave;
        [SerializeField] protected string LastPressTime;
        [SerializeField] private string _descriptionSave;

        private void Start()
        {
            LoadTime();
        }

        private void LoadTime()
        {
            if (PlayerPrefs.HasKey(_lastTimeSave))
            {
                string key = LastPressTime;
                string lastPressTimeString = PlayerPrefs.GetString(key);
                LastTimesSpin = DateTime.Parse(lastPressTimeString);
                CheckButtonAvailability(LastPressTime);
            }
            else
            {
                LastTimesSpin = DateTime.MinValue;
            }
        }

        public virtual void CheckButtonAvailability(string lastPressTime)
        {
            string key = lastPressTime;

            if (PlayerPrefs.HasKey(key))
            {
                string timing = PlayerPrefs.GetString(lastPressTime);
                DateTime tim;

                if (DateTime.TryParse(timing, out tim))
                {
                    // if (DateTime.Now - tim >= TimeSpan.FromHours(24))
                    if (DateTime.Now - tim >= TimeSpan.FromSeconds(10))
                    {
                        foreach (var button in buttonSpin)
                            button.interactable = true;
                    }
                    else
                    {
                        foreach (var button in buttonSpin)
                            button.interactable = false;
                    }
                }
            }
            else
            {
                foreach (var button in buttonSpin)
                    button.interactable = true;
            }
        }

        private void Update()
        {
            UpdateTime(LastPressTime);
        }

        private void UpdateTime(string lastPressTime)
        {
            string timing = PlayerPrefs.GetString(lastPressTime);

            if (!string.IsNullOrEmpty(timing))
            {
                DateTime tim;
                if (DateTime.TryParse(timing, out tim))
                {
                    // if (DateTime.Now - tim >= TimeSpan.FromHours(24))
                    if (DateTime.Now - tim >= TimeSpan.FromSeconds(10))
                        CheckButtonAvailability(lastPressTime);
                }
                else
                {
                    Debug.LogError("Не удалось преобразовать строку в DateTime: " + timing);
                }
            }
        }

        public void StartButtonClick()
        {
            OnButtonClick(LastPressTime,_lastTimeSave,_descriptionSave);
        }
        
        private void OnButtonClick(string lastPressTime,string lastTimeSave, string descriptionSave)
        {
            LastTimesSpin = DateTime.Now;
            PlayerPrefs.SetString(lastPressTime, DateTime.Now.ToString());
            PlayerPrefs.SetString(lastTimeSave, descriptionSave);
            PlayerPrefs.Save();

            /*foreach (var button in buttonSpin)
                button.interactable = false;*/
        }
    }
}