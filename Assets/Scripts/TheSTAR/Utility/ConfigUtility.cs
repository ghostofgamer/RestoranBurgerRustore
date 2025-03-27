#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace TheSTAR.Utility
{
    public static class ConfigUtility
    {
        public static void Save(ScriptableObject config)
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif