using DailyTimerContent;
using TheSTAR.GUI;
using UnityEngine;

namespace UI.Buttons
{
    public class SpinADSFortuneButton : AbstractButton
    {
        [SerializeField] private FortuneScreen _fortuneScreen;
        
        public override void OnClick()
        {
            _fortuneScreen.SpinADS();
        }
    }
}