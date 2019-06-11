using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    /// <summary>
    /// Converts index of a 2D array to point on screen.
    /// </summary>
    /// <param name="width"> Width of texture. </param>
    /// <param name="index"> Index of array. </param>
    /// <returns> Point on screen. </returns>
    public static Vector2 IndexToPoint(int width, int index)
    {
        int x, y;
        y = (int)Mathf.Floor(index / width);
        x = index % width;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Converts point on screen to index of 2D array.
    /// </summary>
    /// <param name="width"> Width of texture. </param>
    /// <param name="point"> Point on screen. </param>
    /// <returns> Index of array. </returns>
    public static int PointToIndex(int width, Vector2 point)
    {
        return (int)Mathf.Floor(point.y * width + point.x);
    }

    /// <summary>
    /// Returns the rolling average of two numbers.
    /// </summary>
    /// <param name="oldAverage"> Old rolling average. </param>
    /// <param name="newValue"> New value to add to rolling average.</param>
    /// <param name="size"> Size of entire average. </param>
    /// <returns> New rolling average. </returns>
    public static Vector2 GetVector2RollingAverage(Vector2 oldAverage, Vector2 newValue, int size)
    {
        Vector2 newAverage = oldAverage * (size - 1) / size + newValue / size;
        return newAverage;
    }


    /// <summary>
    ///  Returns the angle between three points.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    public static float AngleBetween3Points(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return Vector2.Angle(p1 - p2, p3 - p2);
    }

    /// <summary>
    /// Erf code from https://www.johndcook.com/blog/csharp_erf/ by John D. Cook.
    /// The Erf function doesn't exist within c# math libraries. 
    /// </summary>
    public static double Erf(double x)
    {
        // constants
        double a1 = 0.254829592;
        double a2 = -0.284496736;
        double a3 = 1.421413741;
        double a4 = -1.453152027;
        double a5 = 1.061405429;
        double p = 0.3275911;

        // Save the sign of x
        int sign = 1;
        if (x < 0)
            sign = -1;
        x = Math.Abs(x);

        // A&S formula 7.1.26
        double t = 1.0 / (1.0 + p * x);
        double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

        return sign * y;
    }
    /// Erf code from https://www.johndcook.com/blog/csharp_erf/ by John D. Cook.
}
