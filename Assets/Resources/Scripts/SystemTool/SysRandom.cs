using System;

public static class SysRandom
{
    private static System.Random _random = new System.Random();

    public static void Seed(int seed)
    {
        _random = new System.Random(seed);
    }

    public static int Range(int min, int max)
    {
        return _random.Next(min, max);
    }

    public static float Value
    {
        get { return (float)_random.NextDouble(); }
    }

    public static int Next(int max)
    {
        return _random.Next(max);
    }
}
