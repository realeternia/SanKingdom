using System;
using UnityEngine;

public static class BattleRandom
{
    private static System.Random _random = new System.Random();

    public static void Seed(int seed)
    {
        _random = new System.Random(seed);
    }

    public static void Reset()
    {
        _random = new System.Random();
    }

    public static int Range(int min, int max)
    {
        return _random.Next(min, max);
    }

    public static float Value
    {
        get { return (float)_random.NextDouble(); }
    }

    public static Vector2 InsideUnitCircle
    {
        get
        {
            float angle = (float)_random.NextDouble() * 2f * (float)Math.PI;
            float radius = (float)Math.Sqrt(_random.NextDouble());
            return new Vector2((float)Math.Cos(angle) * radius, (float)Math.Sin(angle) * radius);
        }
    }
}
