using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prodvides an object to define clustered locations of skin pixels.
/// </summary>
public class SkinBlob
{
    private Vector2 minPoint, maxPoint;
    private Vector2 medianPoint, meanPoint;

    private float distanceThreshold;
    private int size;
    private int sampleSize;

    // Largest width of uninterrupted skin pixels.
    private int width;

    // How close the pixels to be considered part of this object.
    private const float searchRange = 2.0f;

    public SkinBlob(Vector2 point)
    {
        minPoint = point;
        maxPoint = point;
        medianPoint = point;
        meanPoint = point;

        distanceThreshold = searchRange;
        size = 1;
        sampleSize = 0;

        width = 0;
    }

    /// <summary>
    /// Compares new value with stored values replacing them with either the
    /// minimum or maximum value.
    /// </summary>
    /// <param name="point"> New min/max point contender. </param>
    private void Comparepoint(Vector2 point)
    {
        // x point.
        if (point.x < minPoint.x)
            minPoint.x = point.x;

        if (point.x > maxPoint.x)
            maxPoint.x = point.x;

        // y point.
        if (point.y < minPoint.y)
            minPoint.y = point.y;

        if (point.y > maxPoint.y)
            maxPoint.y = point.y;

        // Update stored values.
        size++;
        medianPoint = (maxPoint + minPoint) / 2;
        distanceThreshold = Vector2.Distance(minPoint, medianPoint) + searchRange;
    }

    /// <summary>
    /// Adds point to the rolling average. 
    /// </summary>
    /// <param name="point"> New point value. </param>
    public void AddToMean(Vector2 point)
    {
        sampleSize++;
        meanPoint = Utility.GetVector2RollingAverage(meanPoint, point, sampleSize);
    }

    /// <summary>
    /// Tests if point is within the distancethreshold. 
    /// </summary>
    /// <param name="point"> Point to test. </param>
    /// <returns> Whether the test passed or failed. </returns>
    public bool TestPoint(Vector2 point)
    {
        if(Vector2.Distance(point, medianPoint) <= distanceThreshold)
        {
            Comparepoint(point);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether or not the new width is higher than the old one. 
    /// </summary>
    /// <param name="newWidth"> Width to test. </param>
    public void TestWidth(int newWidth)
    {
        if (newWidth > width)
            width = newWidth;

    }

    /// <summary>
    /// Getter methods.
    /// </summary>

    public int GetSize()
    {
        return size;
    }

    public float GetArea()
    {
        var area = maxPoint - minPoint;
        return area.x * area.y;
    }

    public Vector2 GetMeanPoint()
    {
        return meanPoint;
    }

    public Vector2 GetMinPoint()
    {
        return minPoint;
    }

    public Vector2 GetMaxPoint()
    {
        return maxPoint;
    }

    public int GetWidth()
    {
        return width;
    }

}
