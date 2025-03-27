using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TheSTAR.Utility
{
    public static class MathUtility
    {
        public static int Round(float value)
        {
            var result = -1;
            var residue = value % 1;
            result = (int)value;
            if (residue >= 0.5f) result++;

            return result;
        }

        /// <summary>
        /// Ограничивает значение между минимальным и максимальным
        /// </summary>
        public static int Limit(int value, int min, int max) => (int)Limit((float)value, min, max);

        /// <summary>
        /// Ограничивает значение между минимальным и максимальным
        /// </summary>
        public static float Limit(float value, float min, float max) => MathF.Max(MathF.Min(value, max), min);

        /// <summary>
        /// Если значение выходит за максимум, оно принимает минимальное значение. Если выходит за минимум - принимает максимальное
        /// </summary>
        public static int LimitRound(int value, int min, int max) => (int)LimitRound((float)value, min, max);

        /// <summary>
        /// Если значение выходит за максимум, оно принимает минимальное значение. Если выходит за минимум - принимает максимальное
        /// </summary>
        public static float LimitRound(float value, float min, float max)
        {
            if (value > max) value = min;
            else if (value < min) value = max;

            return value;
        }

        public static Vector2 LimitForCircle(Vector2 value, float maxDistance)
        {
            var currentDistance = (float)Math.Sqrt(value.x * value.x + value.y * value.y);
            if (currentDistance > maxDistance) value *= maxDistance / currentDistance;
            return value;
        }

        public static bool InBounds(int value, int min, int max) => InBounds((float)value, min, max);
        public static bool InBounds(float value, float min, float max) => (min <= value && value <= max);

        public static bool InBounds(IntVector2 value, IntVector2 min, IntVector2 max) =>
            InBounds(value.X, min.X, max.X) && InBounds(value.Y, min.Y, max.Y);

        public static bool InBounds(Vector2 value, Vector2 min, Vector2 max) =>
            InBounds(value.x, min.x, max.x) && InBounds(value.y, min.y, max.y);

        public static bool IsIntValue(string s, out int value)
        {
            s = s.Replace(" ", "");
            value = -1;

            var isMinus = false;
            if (s[0] == '-')
            {
                isMinus = true;
                s = s.Remove(0, 1);
            }

            foreach (char symbol in s)
            {
                if (!char.IsNumber(symbol)) return false;
            }

            if (string.IsNullOrEmpty(s))
            {
                value = 0;
                return true;
            }

            value = Convert.ToInt32(s) * ((isMinus ? -1 : 1));

            return true;
        }

        public static float Difference(float a, float b) => Math.Abs(a - b);

        /// <summary>
        /// Возвращает прогресс от 0 до 1 для value между min и max
        /// </summary>
        public static float GetProgress(float value, float min, float max) => (value - min) / (max - min);

        /// <summary>
        /// Конвертирует прогресс от 0 до 1 в значение от min до max
        /// </summary>
        public static float ProgressToValue(float progress, float min, float max) => (max - min) * progress + min;

        /// <summary>
        /// Конвертирует прогресс от 0 до 1 в значение между a и b
        /// </summary>
        public static Vector3 ProgressToValue(float progress, Vector3 a, Vector3 b)
        {
            float x = ProgressToValue(progress, a.x, b.x);
            float y = ProgressToValue(progress, a.y, b.y);
            float z = ProgressToValue(progress, a.z, b.z);
            return new(x, y, z);
        }

        /// <summary>
        /// Конвертирует прогресс от 0 до 1 в значение между a и b
        /// </summary>
        public static Color ProgressToValue(float progress, Color color0, Color color1)
        {
            float r = ProgressToValue(progress, color0.r, color1.r);
            float g = ProgressToValue(progress, color0.g, color1.g);
            float b = ProgressToValue(progress, color0.b, color1.b);
            float a = ProgressToValue(progress, color0.a, color1.a);

            return new(r, g, b, a);
        }

        /// <summary>
        /// Возвращает положение среднее между точками A и B
        /// </summary>
        public static Vector2 MiddlePosition(Vector2 a, Vector2 b)
        {
            float x = ProgressToValue(0.5f, Math.Min(a.x, b.x), Math.Max(a.x, b.x));
            float y = ProgressToValue(0.5f, Math.Min(a.y, b.y), Math.Max(a.y, b.y));
            return new(x, y);
        }

        public static Vector3 MergeVector3(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.x + b.x,
                a.y + b.y,
                a.z + b.z);
        }

        public static WorldDirection ReverseWorldDirection(WorldDirection value)
        {
            return value switch
            {
                WorldDirection.North => WorldDirection.South,
                WorldDirection.South => WorldDirection.North,
                WorldDirection.West => WorldDirection.East,
                WorldDirection.East => WorldDirection.West,
                _ => WorldDirection.North,
            };
        }

        public static void DoWithProbability(float probability, Action action, Action elseAction = null)
        {
            if (Random.Range(0f, 1f) <= probability) action();
            else elseAction?.Invoke();
        }
    }

    public struct IntVector2
    {
        private int x, y;

        public int X => x;
        public int Y => y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public IntVector2(Vector2 value)
        {
            x = (int)value.x;
            y = (int)value.y;
        }

        public IntVector2(WorldDirection direction)
        {
            switch (direction)
            {
                case WorldDirection.North:
                    y = -1;
                    x = 0;
                    break;
                case WorldDirection.South:
                    y = 1;
                    x = 0;
                    break;

                case WorldDirection.West:
                    y = 0;
                    x = -1;
                    break;
                case WorldDirection.East:
                    y = 0;
                    x = 1;
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }
        }

        public static explicit operator IntVector2(Vector2 value)
        {
            return new IntVector2(value);
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }
    }

    [Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public int RandomValue => Random.Range(min, max + 1);
    }

    [Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public float RandomValue => Random.Range(min, max);
    }

    [Serializable]
    public struct IntPos2D : IPositionHandler
    {
        public int x;
        public int y;

        public IntPos2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public IntPos2D(string str)
        {
            int prefixLength = str.IndexOf(':');
            if (prefixLength != -1) str = str.Substring(prefixLength + 1);

            var values = str.Split(' ');
            x = Convert.ToInt32(values[0]);
            y = Convert.ToInt32(values[1]);
        }

        public override string ToString()
        {
            return $"{x} {y}";
        }

        public static IntPos2D operator +(IntPos2D a, IntPos2D b)
        {
            return new IntPos2D(a.x + b.x, a.y + b.y);
        }
    
        public static implicit operator Vector2(IntPos2D intPos2D) => new Vector2(intPos2D.x, intPos2D.y);
    
        public static int Distance(IntPos2D a, IntPos2D b)
        {
            return Math.Max(Math.Abs(a.x - b.x), Math.Abs(a.y - b.y));
        }
    
        public Vector2 Position => new (x, -y);
    }

    [Serializable]
    public struct IntPos3D
    {
        public int x;
        public int y;
        public int z;

        public IntPos3D(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public IntPos3D(string str)
        {
            int prefixLength = str.IndexOf(':');
            if (prefixLength != -1) str = str.Substring(prefixLength + 1);

            var values = str.Split(' ');
            x = Convert.ToInt32(values[0]);
            y = Convert.ToInt32(values[1]);
            z = Convert.ToInt32(values[2]);
        }

        public override string ToString()
        {
            return $"{x} {y} {z}";
        }

        public static IntPos3D operator +(IntPos3D a, IntPos3D b)
        {
            return new IntPos3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }
    }
}

public enum WorldDirection
{
    North,
    South,
    West,
    East
}