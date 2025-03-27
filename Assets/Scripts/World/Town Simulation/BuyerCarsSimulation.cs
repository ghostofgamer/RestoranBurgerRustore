using UnityEngine;
using TheSTAR.Utility;
using System;

public class BuyerCarsSimulation : MonoBehaviour
{
    [SerializeField] private TownPathPoint[] pointsForSpawn;
    [SerializeField] private TownPathPoint[] pointsForGoAway;
    [SerializeField] private BuyerCar[] carPrefabs;
    [SerializeField] private Transform carsParent;
    
    private BuyerCar[] places; // какие машины стоят в каких парковочных местах

    public bool HaveAvailablePlace(out int placeIndex)
    {
        for (int i = 0; i < places.Length; i++)
        {
            BuyerCar place = places[i];
            if (place == null)
            {
                placeIndex = i;
                return true;
            }
        }

        placeIndex = -1;
        return false;
    }

    public void Init()
    {
        places = new BuyerCar[pointsForSpawn.Length];
    }

    public void GenerateCar(Action<TownCar> onCompleteMoveToDinerAction, int maxBuyersCount)
    {
        var havePlace = HaveAvailablePlace(out var spawnPointIndex);
        if (!havePlace)
        {
            Debug.Log("Нет мест!");
            return;
        }

        var carPrefab = ArrayUtility.GetRandomValue(carPrefabs);
        GenerateCar(carPrefab, spawnPointIndex, onCompleteMoveToDinerAction, maxBuyersCount);
    }

    private void GenerateCar(BuyerCar carPrefab, int spawnPointIndex, Action<TownCar> onCompleteMoveToDinerAction, int maxBuyersCount)
    {
        var car = Instantiate(carPrefab, pointsForSpawn[spawnPointIndex].transform.position, Quaternion.identity, carsParent);
        car.Init(pointsForSpawn[spawnPointIndex], onCompleteMoveToDinerAction);
        car.Init(maxBuyersCount);
        places[spawnPointIndex] = car;
    }

    /// <summary>
    /// Покупатель вернулся в машину
    /// </summary>
    public void BuyerBackToCar(BuyerCar car)
    {
        car.AddBuyer(out var readyToGoAway);
        if (readyToGoAway) GoAway(car);
    }

    /// <summary>
    /// Машина покидает порковачное место
    /// </summary>
    private void GoAway(BuyerCar car)
    {
        int index = 0;
        for (; index < places.Length; index++)
        {
            if (places[index] == car)
            {
                places[index] = null;
                break;
            }
        }

        car.SetGoal(pointsForGoAway[index], (car) =>
        {
            Destroy(car.gameObject);
        });
    }
}