using System;
using UnityEngine;

namespace DailyTimerContent
{
    public class DailyTimerFortune : DailyTimer
    {
        public event Action TimeOverCompleted;
        
        public event Action TimeNotOverCompleted;

        public void UpdateInfo()
        {
            CheckButtonAvailability(LastPressTime);
        }
        
        public override void CheckButtonAvailability(string lastPressTime)
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
                        TimeOverCompleted?.Invoke();
                    else
                        TimeNotOverCompleted?.Invoke();
                }
            }
            else
            {
                TimeOverCompleted?.Invoke();
                /*foreach (var button in buttonSpin)
                    button.interactable = true;*/
            }
        }
    }
}
