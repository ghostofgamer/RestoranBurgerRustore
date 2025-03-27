using UnityEngine;
using UnityEngine.AI;
using TheSTAR.Utility;
using System;

public class TownCar : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private TownPathPoint previous;
    private TownPathPoint goal;

    private bool initialized = false;

    private Action<TownCar> onStuckAction;

    private void OnTriggerEnter(Collider other)
    {
        if (!initialized) return;
        if (!other.CompareTag("Point")) return;
        if (other.GetComponent<TownPathPoint>() != goal) return;
        
        GoToRandomPointFromCurrent(goal);
    }

    public void SetGoal(TownPathPoint goal)
    {
        this.goal = goal;
        agent.SetDestination(goal.transform.position);
    }

    public void SetGoal(TownPathPoint goal, Action<TownCar> onStuckAction)
    {
        this.goal = goal;
        this.onStuckAction = onStuckAction;
        agent.SetDestination(goal.transform.position);
    }

    public void Init(TownPathPoint startPoint, Action<TownCar> onStuckAction)
    {
        this.onStuckAction = onStuckAction;
        GoToRandomPointFromCurrent(startPoint);
        initialized = true;
    }

    private void GoToRandomPointFromCurrent(TownPathPoint currentPoint)
    {
        TownPathPoint randomNext;
        
        if (previous != null && currentPoint.To.Length > 1) randomNext = ArrayUtility.GetRandomValue(currentPoint.To, previous);
        else
        {
            if (currentPoint.To.Length == 0)
            {
                OnStuck();
                return;
            }
            else randomNext = ArrayUtility.GetRandomValue(currentPoint.To);
        }

        SetGoal(randomNext);

        previous = currentPoint;
    }

    private void OnStuck()
    {
        onStuckAction?.Invoke(this);
    }
}