using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheSTAR.Utility
{
    public static class TextUtility
    {
        public static string NumericValueToText(int value, NumericTextFormatType format) => NumericValueToText((float)value, format);
        public static string NumericValueToText(float value, NumericTextFormatType format)
        {
            int bigPart;
            int smallPart;

            switch (format)
            {
                case NumericTextFormatType.None: return value.ToString();

                case NumericTextFormatType.SimpleFloat:
                    int intPart = (int)value;
                    int floatPart = (int)((value - intPart) * 10);

                    if (floatPart == 0) return $"{intPart}";
                    else return $"{intPart}.{floatPart}";

                case NumericTextFormatType.RoundToInt: return ((int)value).ToString();

                case NumericTextFormatType.CompactFromK:

                    if (value < 10) return value.ToString();

                    value = (int)value;

                    // до тысяч
                    if (value < 1000) return value.ToString();

                    // тысячи
                    else if (value < 1000000)
                    {
                        bigPart = (int)(value / 1000);

                        // Используем точку (5497 -> 5.4K)
                        if (bigPart < 10)
                        {
                            smallPart = (int)((value - (bigPart * 1000))/100);
                            if (smallPart == 0) return $"{bigPart}K";
                            else return $"{bigPart}.{smallPart}K";
                        }

                        // Не используем точку (54978 -> 54K)
                        else return $"{bigPart}K";
                    }

                    // миллионы
                    else if (value < 1000000000)
                    {
                        bigPart = (int)(value / 1000000);

                        // Используем точку (5497097 -> 5.4M)
                        if (bigPart < 10)
                        {
                            smallPart = (int)((value - (bigPart * 1000000)) / 100000);
                            return $"{bigPart}.{smallPart}M";
                        }

                        // Не используем точку (54970971 -> 54M)
                        else return $"{bigPart}M";
                    }

                    // миллиарды
                    else
                    {
                        bigPart = (int)(value / 1000000000);

                        // Используем точку (5497097000 -> 5.4B)
                        if (bigPart < 10)
                        {
                            smallPart = (int)((value - (bigPart * 1000000000)) / 100000000);
                            return $"{bigPart}.{smallPart}B";
                        }

                        // Не используем точку (54970971000 -> 54B)
                        else return $"{bigPart}B";

                    }

                case NumericTextFormatType.CompactFromM:

                    value = (int)value;

                    // до миллиона
                    if (value < 1000000) return value.ToString();

                    // миллионы
                    else if (value < 1000000000)
                    {
                        bigPart = (int)(value / 1000000);

                        // Используем точку (5497097 -> 5.4M)
                        if (bigPart < 10)
                        {
                            smallPart = (int)((value - (bigPart * 1000000)) / 100000);
                            return $"{bigPart}.{smallPart}M";
                        }

                        // Не используем точку (54970971 -> 54M)
                        else return $"{bigPart}M";
                    }

                    // миллиарды
                    else
                    {
                        bigPart = (int)(value / 1000000000);

                        // Используем точку (5497097000 -> 5.4B)
                        if (bigPart < 10)
                        {
                            smallPart = (int)((value - (bigPart * 1000000000)) / 100000000);
                            return $"{bigPart}.{smallPart}B";
                        }

                        // Не используем точку (54970971000 -> 54B)
                        else return $"{bigPart}B";

                    }

                case NumericTextFormatType.DollarPriceCompactInt:
                    var compactValue = NumericValueToText((int)value, NumericTextFormatType.CompactFromK);
                    return $"${compactValue}";
                
                case NumericTextFormatType.DollarPriceFullInt:
                    return $"${(int)value}";
            }

            Debug.LogError("Не удалось выполнить преобразование");
            return "Error";
        }

        public static string NumericValuesToPrice(int mainValue, int additionalValue, bool useDollar)
        {
            string costText = $"{mainValue}";

            if (additionalValue < 10) costText += $",0{additionalValue}";
            else costText += $",{additionalValue}";
            
            if (useDollar) costText = $"${costText}";

            return costText;
        }

        public static string FormatPrice(DollarValue dollarValue, bool useDollar = true)
        {
            return NumericValuesToPrice(dollarValue.dollars, dollarValue.cents, useDollar);
        }

        public static string TimeToText(TimeSpan value, bool useSeconds = true)
        {
            string result = "";

            if (value.Hours > 0) result += $"{value.Hours}h ";
            if (value.Minutes > 0) result += $"{value.Minutes}m ";
            if (useSeconds && value.Seconds > 0) result += $"{value.Seconds}s "; // todo написать типы форматов по аналогии с NumericTextFormatType

            return result;
        }

        public static string TimeToText(GameTimeSpan value, bool useSeconds = true)
        {
            string result = "";

            bool anyFound = false;

            if (value.Hours > 0)
            {
                result += $"{value.Hours}h ";
                anyFound = true;
            }
            if (value.Minutes > 0)
            {
                result += $"{value.Minutes}m ";
                anyFound = true;
            }
            if (useSeconds && value.Seconds > 0)
            {
                result += $"{value.Seconds}s ";
                anyFound = true;
            }

            if (!anyFound) result = $"{value.Seconds}s";

            return result;
        }
    }

    public enum NumericTextFormatType
    {
        /// <summary> Текст никак не форматируется и возвращается в исходном виде </summary>
        None,

        /// <summary> Значение округляется до одного символа после точки (Например 5.497 -> 5.4) </summary>
        SimpleFloat,

        /// <summary> Значение округляется до целых  (Например 5.497 -> 5) </summary>
        RoundToInt,

        /// <summary> Значение представляется компактно от тысяч (Например 5497 -> 5.4K) </summary>
        CompactFromK,

        /// <summary> Значение представляется компактно от миллионов (Например 5497497 -> 5.4M) </summary>
        CompactFromM,

        /// <summary>
        /// Значение представляется в виде ценника в долларах без дробной части (Например 1125.67 -> $1K)
        /// </summary>
        DollarPriceCompactInt,

        /// <summary>
        /// Значение представляется в виде ценника в долларах без дробной части (Например 1125.67 -> $1125)
        /// </summary>
        DollarPriceFullInt
    }
}
