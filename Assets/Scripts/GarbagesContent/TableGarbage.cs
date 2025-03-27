using UnityEngine;

namespace GarbagesContent
{
    public class TableGarbage : TouchInteractive
    {
        [SerializeField] private GarbagePackage _garbagePackage;

        public override void OnClick()
        {
            _garbagePackage.Clean();
            _garbagePackage.SetValue(false);
        }
    }
}