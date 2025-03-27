using UnityEngine;
using System;
using TheSTAR.Utility;

namespace TheSTAR.GUI
{
    [CreateAssetMenu(fileName = "GuiConfig", menuName = "Data/Gui")]
    public class GuiConfig : ScriptableObject
    {
        [SerializeField] private GuiController guiControllerPrefab;
        [SerializeField] private GuiScreen[] screenPrefabs;
        [SerializeField] private GuiUniversalElement[] universalElementPrefabs;

        public GuiController GuiControllerPrefab => guiControllerPrefab;
        public GuiScreen[] ScreenPrefabs => screenPrefabs;
        public GuiUniversalElement[] UniversalElementPrefabs => universalElementPrefabs;

        #if UNITY_EDITOR

        [ContextMenu("Sort")]
        private void Sort()
        {
            Array.Sort(screenPrefabs);
            Array.Sort(universalElementPrefabs);
            ConfigUtility.Save(this);
        }

        #endif
    }
}