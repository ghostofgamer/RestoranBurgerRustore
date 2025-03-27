using System;
using UnityEngine;

namespace TheSTAR.GUI
{
    public class IndexCallbackButton : MonoBehaviour
    {
        [SerializeField] private PointerButton button;

        public PointerButton Button => button;

        public void Init(int index, Action<int> onClickAction)
        {
            button.Init(() => onClickAction?.Invoke(index));
        }
    }
}