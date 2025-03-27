using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Xp", fileName = "XpConfig")]
public class XpConfig : ScriptableObject
{
    [SerializeField] private int[] xpToLevelUp; // индекс уровня - сколько нужно опыта для его получения (индекс от 0, отображается как 1)

    public int[] XpToLevelUp => xpToLevelUp;
}