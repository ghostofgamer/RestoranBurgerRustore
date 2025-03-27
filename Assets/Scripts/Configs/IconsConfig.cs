using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Icons", fileName = "IconsConfig")]
public class IconsConfig : ScriptableObject
{
    [SerializeField] private UnityDictionary<CurrencyType, Sprite> currencyIcons = new();

    public Sprite GetCurrencyIcon(CurrencyType currencyType) => currencyIcons.Get(currencyType);
}