using UnityEngine;

namespace GarbagesContent
{
    public class GarbagePackage : MonoBehaviour
    {
        [SerializeField] private int _index;
        [SerializeField] private BuyerPlace _buyerPlace;
        
        public bool IsActive { get; private set; }
        
        public void SetValue(bool isActive)
        {
            IsActive = isActive;
            gameObject.SetActive(isActive);
        }

        public void Clean()
        {
            _buyerPlace.DecreasePollutionLevel();
        }
    }
}