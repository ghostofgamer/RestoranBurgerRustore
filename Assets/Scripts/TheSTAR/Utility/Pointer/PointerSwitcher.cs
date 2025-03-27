using System;
using UnityEngine;

namespace TheSTAR.GUI
{
    public class PointerSwitcher : MonoBehaviour
    {
        [SerializeField] private PointerSwitchElement[] elements;
        [SerializeField] private int currentIndex;
        [SerializeField] private Color unselectColor;
        [SerializeField] private Color selectColor;

        private Action<int> onSelectElementAction;

        public void Init(Action<int> onSelectElementAction)
        {
            this.onSelectElementAction = onSelectElementAction;

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].Init(i, OnSelect);
            }

            UpdateVisual();
        }

        public void ImitateClick(int index) => OnSelect(index);

        private void OnSelect(int index)
        {
            currentIndex = index;
            UpdateVisual();

            onSelectElementAction?.Invoke(index);
        }

        private void UpdateVisual()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].SetColor(i == currentIndex ? selectColor : unselectColor);
            }
        }
    }
}