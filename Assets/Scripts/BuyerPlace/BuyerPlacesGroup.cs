using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Группа посадочных мест (места за барной стойкой, места за столами)
/// </summary>
public class BuyerPlacesGroup : MonoBehaviour
{
    [SerializeField] private BuyerPlaceFurnitureUnit[] furnitureUnits;

    public void Init(AllBuyerPlaces allBuyerPlaces)
    {
        foreach (var unit in furnitureUnits) unit.Init(allBuyerPlaces);
    }

    public bool HaveEmptyPlaces(out List<BuyerPlace> availablePlaces)
    {        
        availablePlaces = new();
        
        // Debug.Log(furnitureUnits.Length);
        
        foreach (var furnitureUnit in furnitureUnits)
        {
            if (!furnitureUnit.Active) continue;
         
            if (furnitureUnit.HavePlaces(out var placesInFurnitureUnit))
            {
                foreach (var place in placesInFurnitureUnit) availablePlaces.Add(place);
            }
        }

        return availablePlaces.Count > 0;
    }

    public void SetPlaces(bool[] activePlaces)
    {
        for (int i = 0; i < furnitureUnits.Length; i++)
        {
            if (activePlaces[i]) furnitureUnits[i].Activate();
            else furnitureUnits[i].Deactivate();
        }
    }

    public bool HaveCash(out BuyerCash cash)
    {
        cash = null;

        foreach (var unit in furnitureUnits)
        {
            if (unit.HaveCash(out cash)) return true;
        }

        return false;
    }
}