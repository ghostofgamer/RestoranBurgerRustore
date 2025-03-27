using System;
using System.Collections;
using System.Collections.Generic;

namespace TheSTAR.Utility
{
    static class EnumUtility
    {
        public static TEnum[] GetValues<TEnum>() where TEnum : System.Enum
        {
            return (TEnum[])System.Enum.GetValues(typeof(TEnum));
        }

        public static int[] GetIndexes<TEnum>() where TEnum : System.Enum
        {
            var values = GetValues<TEnum>();
            int[] allIndexes = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                TEnum value = values[i];
                allIndexes[i] = Convert.ToInt32(value);
            }

            return allIndexes;
        }

        public static bool IsDefined<TEnum>(int i) where TEnum : System.Enum
        {
            return EnumNamedValues<TEnum>().ContainsKey(i);
        }

        public static Dictionary<int, string> EnumNamedValues<T>() where T : System.Enum
        {
            var result = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));

            foreach (int item in values) result.Add(item, Enum.GetName(typeof(T), item)!);
            
            return result;
        }

        public static TEnum GetRandom<TEnum>() where TEnum : System.Enum
        {
            var values = GetValues<TEnum>();
            return ArrayUtility.GetRandomValue(values);
        }
    }
}