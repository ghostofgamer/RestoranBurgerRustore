using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Мебельная единица в рамках группы посадочных мест. Появляется при покупке. Может содержать одно или несколько посадочных мест. Может быть активно или неактивно
/// </summary>
public class BuyerPlaceFurnitureUnit : MonoBehaviour
{
    [SerializeField] private BuyerPlace[] places;

    private bool active;
    public bool Active => active;
    public BuyerPlace[] Places=>places;

    public void Init(AllBuyerPlaces allBuyerPlaces)
    {
        foreach (var place in places) place.Init(allBuyerPlaces);
    }

    public bool HavePlaces(out List<BuyerPlace> availablePlaces)
    {
        availablePlaces = new();

        if (!active) return false;

        foreach (var place in places)
        {
            if (place.Available) availablePlaces.Add(place);
        }

        return availablePlaces.Count > 0;
    }

    public void Activate()
    {
        active = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public bool HaveCash(out BuyerCash cash)
    {
        cash = null;

        if (!active) return false;

        foreach (var place in places)
        {
            if (place.HaveCash)
            {
                cash = place.Cash;
                return true;
            }
        }

        return false;
    }
}