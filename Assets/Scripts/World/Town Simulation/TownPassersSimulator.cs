using TheSTAR.Utility;
using UnityEngine;

public class TownPassersSimulator : MonoBehaviour
{
    [SerializeField] private TownPathPoint[] allPathPoints;
    [SerializeField] private Passer[] passersPrefabs;
    [SerializeField] private Transform passersParent;
    [SerializeField] private int passersByTypeCount = 2;

    public void StartSimulate()
    {
        foreach (var passerPrefab in passersPrefabs)
        {
            for (int i = 0; i < passersByTypeCount; i++)
            {
                GeneratePasser(passerPrefab);
            }
        }
    }

    private void GeneratePasser(Passer passerPrefab)
    {
        var point = ArrayUtility.GetRandomValue(allPathPoints);
        var passer = Instantiate(passerPrefab, point.transform.position, Quaternion.identity, passersParent);
        passer.Init(point);
    }
}