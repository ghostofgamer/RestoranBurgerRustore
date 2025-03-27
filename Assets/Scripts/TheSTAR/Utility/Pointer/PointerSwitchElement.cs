using System;
using UnityEngine;

namespace TheSTAR.GUI
{
    public class PointerSwitchElement : MonoBehaviour
    {
        [SerializeField] private PointerButton button;

        private int index;

        public void Init(int index, Action<int> onSelectAction)
        {
            this.index = index;
            button.Init(() => onSelectAction(this.index));
        }

        public void SetColor(Color color) => button.Img.color = color;
    }
}