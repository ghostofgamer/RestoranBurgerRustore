using TheSTAR.Utility;
using UnityEngine;
using UnityEngine.AI;

public class Passer : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private bool debug;

    private TownPathPoint previous;
    private TownPathPoint goal;

    private bool initialized = false;

    private void OnTriggerEnter(Collider other)
    {
        if (debug) Debug.Log(other.name);
        
        if (!initialized) return;
        if (!other.CompareTag("Point")) return;
        if (other.GetComponent<TownPathPoint>() != goal) return;
        
        GoToRandomPointFromCurrent(goal);
    }

    private void SetGoal(TownPathPoint goal)
    {
        this.goal = goal;
        agent.SetDestination(goal.transform.position);
    }

    public void Init(TownPathPoint startPoint)
    {
        anim.SetBool("Walking", true);
        GoToRandomPointFromCurrent(startPoint);
        initialized = true;
    }

    private void GoToRandomPointFromCurrent(TownPathPoint currentPoint)
    {
        TownPathPoint randomNext;
        if (previous != null && currentPoint.To.Length > 1) randomNext = ArrayUtility.GetRandomValue(currentPoint.To, previous);
        else randomNext = ArrayUtility.GetRandomValue(currentPoint.To);

        SetGoal(randomNext);

        previous = currentPoint;
    }

    public void InitAsMoveTo(TownPathPoint to)
    {
        anim.SetBool("Walking", true);
        SetGoal(to);
        initialized = true;
    }
}