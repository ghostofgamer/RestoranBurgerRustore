using UnityEngine;
using TheSTAR.Utility;
using DG.Tweening;

public class TownCarsSimulator : MonoBehaviour
{
    [SerializeField] private TownPathPoint[] pointsForSpawn;
    [SerializeField] private TownCar[] carPrefabs;
    [SerializeField] private Transform carsParent;

    [Space]
    [SerializeField] private float minDelay = 1f;
    [SerializeField] private float maxDelay = 10f;

    private Tweener waitTweener;

    public void StartSimulate()
    {
        GenerateRandomCar();
        WaitForCar();
    }

    private void WaitForCar()
    {
        waitTweener?.Kill();

        waitTweener =
        DOVirtual.Float(0f, 1, Random.Range(minDelay, maxDelay), (temp) => {}).SetEase(Ease.Linear).OnComplete(() =>
        {
            GenerateRandomCar();
            WaitForCar();
        });
    }

    [ContextMenu("GenerateRandomCar")]
    private void GenerateRandomCar()
    {
        var carPrefab = ArrayUtility.GetRandomValue(carPrefabs);
        GenerateCar(carPrefab);
    }

    private void GenerateCar(TownCar carPrefab)
    {
        var point = ArrayUtility.GetRandomValue(pointsForSpawn);
        var car = Instantiate(carPrefab, point.transform.position, Quaternion.identity, carsParent);
        car.Init(point, (c) =>
        {
            Destroy(c.gameObject);
        });
    }
}