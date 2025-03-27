using UnityEngine;
using UnityEngine.UI;

namespace UI.Buttons
{
    public abstract class AbstractButton : MonoBehaviour
    {
        protected Button Button;

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            Button.onClick.AddListener(OnClick); 
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnClick); 
        }

        public abstract void OnClick();
    }
}