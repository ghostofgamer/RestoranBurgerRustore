using System;
using UnityEngine;

namespace World
{
    // возможно ci бесполезен
    public abstract class CiObject : MonoBehaviour
    {
        public abstract bool CanInteract { get; }
    }
}