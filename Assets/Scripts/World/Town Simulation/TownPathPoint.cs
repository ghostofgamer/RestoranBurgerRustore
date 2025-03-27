using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPathPoint : PathPoint
{
    [SerializeField] private TownPathPoint[] to;
    public TownPathPoint[] To => to;
}