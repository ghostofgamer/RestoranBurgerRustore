using System;
using UnityEngine;
using TheSTAR.Utility;

namespace TheSTAR.GUI
{
    public class GuiScreen : GuiObject, IComparable<GuiScreen>, IComparableType<GuiScreen>
    {
        [SerializeField] private bool pause;
        [SerializeField] private bool usePurchaseMessage;
        [SerializeField] private bool useTopUiContainer;
        [SerializeField] private bool useLookAround;
        [SerializeField] private bool useMainMenuFon;

        public bool Pause => pause;
        public bool UsePurchaseMessage => usePurchaseMessage;
        public bool UseTopUiContainer => useTopUiContainer;
        public bool UseLookAround => useLookAround;
        public bool UseMainMenuFon => useMainMenuFon;

        public int CompareTo(GuiScreen other) => ToString().CompareTo(other.ToString());
        public int CompareToType<T>() where T : GuiScreen => ToString().CompareTo(typeof(T).ToString());

        protected void SetUseMainMenuFon(bool useMainMenuFon)
        {
            this.useMainMenuFon = useMainMenuFon;
        }

        protected void SetUseTopUI(bool use)
        {
            this.useTopUiContainer = use;
        }
    }
}