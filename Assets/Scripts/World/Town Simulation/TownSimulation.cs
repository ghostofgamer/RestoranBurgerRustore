using UnityEngine;

public class TownSimulation : MonoBehaviour
{
    [SerializeField] private TownCarsSimulator carsSimulator;

    public void StartSimulate()
    {
        carsSimulator.StartSimulate();
    }
}