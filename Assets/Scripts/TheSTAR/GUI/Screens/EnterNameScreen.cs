using System;
using UnityEngine;
using TMPro;
using Zenject;

namespace TheSTAR.GUI
{
    public class EnterNameScreen : GuiScreen
    {
        [SerializeField] private PointerButton closeButton;
        [SerializeField] private PointerButton acceptButton;
        [SerializeField] private TMP_InputField inputField;
        
        private Action<string> onAcceptAction;

        private GuiController gui;

        [Inject]
        private void Construct(GuiController gui)
        {
            this.gui = gui;
        }

        public override void Init()
        {
            base.Init();

            closeButton.Init(() =>
            {
                gui.ShowMainScreen();
            });

            acceptButton.Init(() =>
            {
                int maxLength = 26; 
                
                if (inputField.text.Length > maxLength)
                {
                    Debug.LogWarning("Текст превышает максимальную длину!");
                    return;
                }
                
                onAcceptAction?.Invoke(inputField.text);
                gui.ShowMainScreen();
            });
        }

        public void Init(Action<string> onAcceptAction)
        {
            this.onAcceptAction = onAcceptAction;
        }
    }
}