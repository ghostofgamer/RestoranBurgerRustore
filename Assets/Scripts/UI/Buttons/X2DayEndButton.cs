using TheSTAR.GUI;
using UnityEngine;

namespace UI.Buttons
{
    public class X2DayEndButton : AbstractButton
    {
        [SerializeField] private DayOverScreen _dayOverScreen;
        
        public override void OnClick()
        {
            _dayOverScreen.AdsX2DayEnd();
        }
    }
}