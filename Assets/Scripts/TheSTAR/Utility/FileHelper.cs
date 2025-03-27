using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TheSTAR.Utility
{
    public static class FileHelper
    {
        // методы прямого обращения к файлу не работают на андроиде

#if UNITY_EDITOR

        public static void WriteJsonFile(string path, string jsonString)
        {
            File.WriteAllText(path, jsonString);
        }

        /*
        [Obsolete]
        public static string ReadJsonFile(string path)
        {
            return File.ReadAllText(path);
        }
        */
#endif

    }
}