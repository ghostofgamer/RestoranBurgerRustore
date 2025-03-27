using System;
using TMPro;
using UnityEngine;

namespace TheSTAR.GUI
{
    public class DialogScreen : GuiScreen
    {
        [SerializeField] private PointerButton acceptButton;
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private TextMeshProUGUI buttonText;

        private Action acceptAction;

        public override void Init()
        {
            base.Init();
            acceptButton.Init(OnActionButtonClick);
        }

        public void Init(Action acceptAction)
        {
            this.acceptAction = acceptAction;
        }

        public void Init(string mainText, string buttonText, Action acceptAction)
        {
            this.mainText.text = mainText;
            this.buttonText.text = buttonText;
            this.acceptAction = acceptAction;
        }

        private void OnActionButtonClick()
        {
            acceptAction?.Invoke();
        }
    }
}