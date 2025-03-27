using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TheSTAR.Sound;

namespace TheSTAR.GUI
{
    public class ValueSwitcher : MonoBehaviour
    {
        [SerializeField] private PointerButton minusButton;
        [SerializeField] private PointerButton plusButton;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private int minVale = 1;
        [SerializeField] private int maxVale = 99;

        private int currentValue;

        private Action<int> changeValueAction;

        public void Init(SoundController sounds)
        {
            minusButton.Init(sounds);
            plusButton.Init(sounds);
        }

        public void Init(Action<int> changeValueAction, ScrollRect scrollRect)
        {
            this.changeValueAction = changeValueAction;
            minusButton.Init(OnMinusClick);
            plusButton.Init(OnPlusClick);

            if (scrollRect != null)
            {
                minusButton.SetScrollable(scrollRect);
                plusButton.SetScrollable(scrollRect);
            }
        }

        public void SetValueWithoutNotify(int value)
        {
            currentValue = value;
            valueText.text = value.ToString();
        }

        private void OnMinusClick()
        {
            currentValue--;
            if (currentValue < minVale) currentValue = minVale;
            valueText.text = currentValue.ToString();
            changeValueAction?.Invoke(currentValue);
        }

        private void OnPlusClick()
        {
            currentValue++;
            if (currentValue > maxVale) currentValue = maxVale;
            valueText.text = currentValue.ToString();
            changeValueAction?.Invoke(currentValue);
        }
    }
}