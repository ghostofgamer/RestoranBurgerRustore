using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[CreateAssetMenu(menuName = "Data/Shop", fileName = "ShopConfig")]
public class ShopConfig : ScriptableObject
{
    public ProductData[] products;
}

[Serializable]
public struct ProductData
{
    public ProductCostType costType;
    public float costValue;
    public ShopRewardType rewardType;
    public string SKU;
    public ProductType ProductType;
}

public enum ProductCostType
{
    Real
}

public enum ShopRewardType
{
    AdsFree
}