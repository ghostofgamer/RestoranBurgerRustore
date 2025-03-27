using TheSTAR.GUI;
using UnityEngine;

namespace UI.Buttons
{
    public class FreeMoneyAdsButton : AbstractButton
    {
        [SerializeField] private ComputerPayShopScreen _computerPayShopScreen;
    
        public override void OnClick()
        {
            _computerPayShopScreen.AddMoneyFromADS();
        }
    }
}