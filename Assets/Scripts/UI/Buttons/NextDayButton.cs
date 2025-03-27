using TheSTAR.GUI;
using UnityEngine;

namespace UI.Buttons
{
    public class NextDayButton : AbstractButton
    {
        [SerializeField] private DayOverScreen _dayOverScreen;
        
        public override void OnClick()
        {
            _dayOverScreen.StartNewDay();
        }
    }
}