using UnityEngine;

namespace IAPContent
{
    public class UIInfo : MonoBehaviour
    {
        [SerializeField] private GameObject _removeAdsButton;

        private void Start()
        {
            UpdateRemoveAdsButton();
        }

        public void UpdateRemoveAdsButton()
        {
            bool removeAds = PlayerPrefs.GetInt("removeADS") == 1;
            _removeAdsButton.SetActive(!removeAds);
        }
    }
}