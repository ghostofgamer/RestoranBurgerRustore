using System;
using TheSTAR.Utility;
using TMPro;
using UnityEngine;
using Zenject;

namespace TheSTAR.GUI
{
    public class TerminalScreen : GuiScreen
    {
        [SerializeField] private PointerButton closeBtn;
        [SerializeField] private IndexCallbackButton[] numberButtons;
        [SerializeField] private PointerButton deleteLastButton;
        [SerializeField] private PointerButton clearAllButton;
        [SerializeField] private PointerButton dotButton;
        [SerializeField] private PointerButton acceptBtn;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Animator anim;

        private DollarValue neededCost;
        private int currentValue = 0;
        private bool useDot = false; // используется ли дробная часть
        private int? firstNumberAfterDot = null;
        private int? secondNumberAfterDot = null;

        private Action successAction;

        private GuiController gui;

        [Inject]
        private void Construct(GuiController gui)
        {
            this.gui = gui;
        }

        public override void Init()
        {
            base.Init();

            closeBtn.Init(gui.ShowMainScreen);

            for (int i = 0; i < numberButtons.Length; i++)
            {
                numberButtons[i].Init(i, OnNumberButtonClick);
            }

            deleteLastButton.Init(OnDeleteClick);
            clearAllButton.Init(OnClearAllClick);
            dotButton.Init(OnDotClick);
            acceptBtn.Init(OnAcceptClick);
        }

        public void Init(DollarValue neededCost, Action successAction)
        {
            this.neededCost = neededCost;
            this.successAction = successAction;
        }

        protected override void OnShow()
        {
            base.OnShow();
            ClearAllValue();
        }

        private void DisplayValue()
        {
            string displayText = "";
            displayText += TextUtility.NumericValueToText(currentValue, NumericTextFormatType.DollarPriceFullInt);
            if (useDot)
            {
                displayText += ".";
                if (firstNumberAfterDot != null)
                {
                    displayText += firstNumberAfterDot.ToString();

                    if (secondNumberAfterDot != null)
                    {
                        displayText += secondNumberAfterDot.ToString();
                    }
                }
            }

            valueText.text = displayText;
        }

        private void OnNumberButtonClick(int value)
        {
            if (useDot)
            {
                if (firstNumberAfterDot == null) firstNumberAfterDot = value;
                else if (secondNumberAfterDot == null) secondNumberAfterDot = value;
                else return;
            }
            else if (currentValue < 100)
            {
                currentValue *= 10;
                currentValue += value;
            }
            else
            {
                useDot = true;
                firstNumberAfterDot = value;
            }
            
            DisplayValue();
        }
        
        private void OnDeleteClick()
        {
            if (useDot)
            {
                if (secondNumberAfterDot != null) secondNumberAfterDot = null;
                else if (firstNumberAfterDot != null) firstNumberAfterDot = null;
                else useDot = false;
            }
            else
            {
                currentValue /= 10;
            }
            
            DisplayValue();
        }

        private void OnDotClick()
        {
            if (useDot) return;
            useDot = true;
            DisplayValue();
        }

        private void OnAcceptClick()
        {
            int dollars = currentValue;
            int cents;

            if (firstNumberAfterDot == null) cents = 0;
            else
            {
                cents = firstNumberAfterDot.Value * 10;
                if (secondNumberAfterDot != null) cents += secondNumberAfterDot.Value;
            }

            if (dollars == neededCost.dollars && cents == neededCost.cents) successAction?.Invoke();
            else anim.SetTrigger("Error");
        }
    
        private void OnClearAllClick()
        {
            ClearAllValue();
        }

        private void ClearAllValue()
        {
            currentValue = 0;
            useDot = false;
            firstNumberAfterDot = null;
            secondNumberAfterDot = null;
            DisplayValue();
        }
    }
}