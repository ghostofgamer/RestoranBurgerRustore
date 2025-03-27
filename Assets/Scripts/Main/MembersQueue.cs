using ModestTree;
using UnityEngine;

public class MembersQueue : MonoBehaviour
{
    [SerializeField] private PathPoint[] points;

    public PathPoint[] Points => points;

    private IQueueMember[] membersQueue;

    public void Init()
    {
        membersQueue = new IQueueMember[points.Length];
    }

    public IQueueMember TakeOutFirst()
    {
        if (membersQueue.Length == 0 || membersQueue[0] == null) return null;

        var first = membersQueue[0];
        RemoveMember(first);
        return first;
    }

    public void AddMember(IQueueMember member, out int index)
    {
        for (int i = 0; i < membersQueue.Length; i++)
        {
            if (membersQueue[i] == null)
            {
                membersQueue[i] = member;
                member.ChangeQueueIndex(i);
                index = i;
                return;
            }
        }

        index = -1;
    }

    public void RemoveMember(IQueueMember member)
    {
        var buyerIndex = membersQueue.IndexOf(member);

        if (buyerIndex != -1)
        {
            membersQueue[buyerIndex] = null;
            for (int i = buyerIndex + 1; i < membersQueue.Length; i++)
            {
                if (membersQueue[i] == null) break;

                // перемещаем вперед по очереди
                var m = membersQueue[i];
                membersQueue[i] = null;
                membersQueue[i - 1] = m;
                m.ChangeQueueIndex(i - 1);
            }
        }

        member.ChangeQueueIndex(-1);
    }
}

public interface IQueueMember
{
    int QueueIndex {get;}
    void ChangeQueueIndex(int index);
}