using TheSTAR.GUI;
using UnityEngine;

namespace UI.Buttons
{
    public class FreeSpinButton : AbstractButton
    {
        [SerializeField] private FortuneScreen _fortuneScreen;
        
        public override void OnClick()
        {
            _fortuneScreen.SpinFree();
        }
    }
}
