using System;
using UnityEngine;
using TheSTAR.GUI;

namespace TheSTAR.GUI
{
    public class GDPRScreen : GuiScreen
    {
        [SerializeField] private PointerButton acceptButton;
        [SerializeField] private PointerButton termsButton;
        [SerializeField] private PointerButton privacyButton;

        public const string PrivatyURL = "https://madpixel.dev/privacy.html";
        private const string TermsURL = "https://madpixel.dev/privacy.html";

        private Action acceptAction;

        public override void Init()
        {
            base.Init();

            acceptButton.Init(OnAcceptClick);
            termsButton.Init(OnTermsClick);
            privacyButton.Init(OnPrivacyClick);
        }

        public void Init(Action acceptAction)
        {
            this.acceptAction = acceptAction;
        }

        private void OnTermsClick()
        {
            Application.OpenURL(TermsURL);
        }

        private void OnPrivacyClick()
        {
            Application.OpenURL(PrivatyURL);
        }

        private void OnAcceptClick()
        {
            acceptAction?.Invoke();
        }
    }
}