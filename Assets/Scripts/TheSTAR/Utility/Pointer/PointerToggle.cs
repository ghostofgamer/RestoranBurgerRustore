using System;
using UnityEngine;

namespace TheSTAR.GUI
{
    public class PointerToggle : MonoBehaviour
    {
        [SerializeField] private PointerButton toggleButton;
        [SerializeField] private RectTransform toggleHandle;
        [SerializeField] private float toggleDistance = 25f;

        private bool currentValue;
        private Action<bool> toggleAction;

        public void Init(bool currentValue, Action<bool> toggleAction)
        {
            this.toggleAction = toggleAction;
            toggleButton.Init(OnToggleButtonClick);
            SetValueWithoutNotify(currentValue);
        }

        private void SetValueWithoutNotify(bool value)
        {
            currentValue = value;
            UpdateToggleVisual();
        }

        private void UpdateToggleVisual()
        {
            toggleHandle.anchoredPosition = new Vector2(toggleDistance * (currentValue ? 1 : -1), 0);
        }

        private void OnToggleButtonClick()
        {
            currentValue = !currentValue;
            UpdateToggleVisual();
            toggleAction(currentValue);
        }

        #region Test

        [ContextMenu("TestSetOn")]
        private void TestSetOn()
        {
            SetValueWithoutNotify(true);
        }

        [ContextMenu("TestSetOff")]
        private void TestSetOff()
        {
            SetValueWithoutNotify(false);
        }

        #endregion
    }
}