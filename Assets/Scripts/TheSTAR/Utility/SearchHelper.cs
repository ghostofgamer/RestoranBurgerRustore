using System;
using System.Collections.Generic;
using UnityEngine;

public static class SearchHelper
{
    public delegate bool SearchConditionDelegate<in T>(T item);

    public static T Search<T>(IEnumerable<T> enumerable, SearchConditionDelegate<T> searchCondition) where T : MonoBehaviour
    {
        foreach (var item in enumerable)
        {
            if (searchCondition(item)) return item;
        }

        return null;
    }

    public static T SearchNearest<T>(Transform from, IEnumerable<T> enumerable, SearchConditionDelegate<T> searchCondition) where T : IPositionHandler
    {
        return SearchNearest(from.position, enumerable, searchCondition);
    }

    public static T SearchNearest<T>(Vector2 from, IEnumerable<T> enumerable, SearchConditionDelegate<T> searchCondition) where T : IPositionHandler
    {
        return SearchNearest(from, enumerable, searchCondition, out _);
    }

    public static T SearchNearest<T>(T from, IEnumerable<T> enumerable, SearchConditionDelegate<T> searchCondition) where T : IPositionHandler
    {
        return SearchNearest(from.Position, enumerable, searchCondition, out _);
    }

    public static T SearchNearest<T>(Vector2 from, IEnumerable<T> enumerable, SearchConditionDelegate<T> searchCondition, out bool anyCandidateFound) where T : IPositionHandler
    {
        anyCandidateFound = false;
        T bestCandidate = default;
        float bestDistance = -1;

        foreach (var item in enumerable)
        {
            if (!searchCondition(item)) continue;

            if (!anyCandidateFound)
            {
                bestCandidate = item;
                bestDistance = Vector2.Distance(from, item.Position);
                anyCandidateFound = true;
                continue;
            }
            else
            {
                var distance = Vector2.Distance(from, item.Position);

                if (distance < bestDistance)
                {
                    bestCandidate = item;
                    bestDistance = distance;
                }
            }
        }

        return bestCandidate;
    }
}

public interface IPositionHandler
{
    Vector2 Position { get; }
}