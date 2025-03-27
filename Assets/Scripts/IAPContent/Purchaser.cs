using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace IAPContent
{
    public class Purchaser : MonoBehaviour
    {
        [SerializeField] private UIInfo _uiInfo;
        private CurrencyController currency;

        [Inject]
        private void Construct(CurrencyController currency)
        {
            this.currency = currency;
        }    
        
        public void OnPurchaseCompleted(Product product)
        {
            switch (product.definition.id)
            {
                case "com.serbull.iaptutorial.money100":
                    AddMoney(100);
                    break;

                case "com.serbull.iaptutorial.removeads":
                    RemoveAds();
                    break;
                
                case "com.serbull.iaptutorial.money500":
                    AddMoney(500);
                    break;
                
                case "com.serbull.iaptutorial.money1100":
                    AddMoney(1100);
                    break;
            }
        }

        private void RemoveAds()
        {
            PlayerPrefs.SetInt("removeADS", 1);
            Debug.Log("On Purchase RemoveAds Completed");
            _uiInfo.UpdateRemoveAdsButton();
        }

        private void AddMoney(int value)
        {
           var vale =  currency.GetCurrencyValue(CurrencyType.Soft);
            Debug.Log("Dollar Value After" + vale);
            currency.AddCurrencyFastMoney(CurrencyType.Soft, new(value, 0), true);
            Debug.Log("Dollar Value PPP" + vale);
            Debug.Log("On Purchase AddMoney Completed");
        }
    }
}