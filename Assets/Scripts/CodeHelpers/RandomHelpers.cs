//using UnityEngine;
using System.Collections;
using System;

public static class RandomHelper
{
    static RandomHelper()
    {
        anyRandom = new Random();
    }

    static Random anyRandom; //This random does not affect by seeds and stuff

    public static double AnyValue { get { return anyRandom.NextDouble(); } }

    public static float AnyRange(float min, float max)
    {
        return min + (float)AnyValue * (max - min);
    }

    public static int AnyRange(int min, int max)
    {
        return anyRandom.Next(min, max);
    }
}
