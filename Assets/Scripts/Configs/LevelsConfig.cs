using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Configs
{
    [Obsolete]
    [CreateAssetMenu(menuName = "Data/Levels", fileName = "LevelsConfig")]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private int firstUnlockLevelCost = 50;
        [SerializeField] private int defaultUnlockLevelCost = 500;
        
        public LevelConfig GetLevelData(int levelIndex) => levels[levelIndex];
        public int FirstUnlockLevelCost => firstUnlockLevelCost;
        public int DefaultUnlockLevelCost => defaultUnlockLevelCost;
    }
}