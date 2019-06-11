using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Classic Jarvis March (Gift wrapping method) based on pseudocode from
/// [Cormen, T. H. et al. (2001) Introduction to algorithms. 2nd edn. MIT Press.]
/// </summary>
public static class ConvexHull
{

    // Used for clustering.
    private const float distanceThreshold = 50.0f;

    /// <summary>
    /// Gets the convex hull of the provided list of points, clusters them 
    /// and then returns the points on the hull.
    /// </summary>
    /// <param name="points"> hull contenders. </param>
    /// <returns> List of clustered points in the convex hull. </returns>
    public static List<Vector2> GetConvexHull(List<Vector2> points)
    {
        // Prerequisite to start.
        if (points.Count < 3) return points;

        var convexHull = new List<Vector2>();

        // Get leftmost point in points.
        var currentPoint = points[0];
        points.ForEach(point =>
        {
            if (point.x < currentPoint.x)
                currentPoint = point;
        });


        Vector2 endPoint;
        // do while loop hasn't returned to start.
        do
        {
            // Add current point.
            convexHull.Add(currentPoint);

            // Reset endpoint.
            endPoint = points[0];

            // Loops through all points, trying to find the next hull point.
            points.ForEach(point =>
            {
                if (currentPoint == endPoint || GetOrientation(currentPoint, endPoint, point))
                    endPoint = point;
            });

            currentPoint = endPoint;

        } while (currentPoint != convexHull[0]);


        convexHull = ClusterHullPoints(convexHull);

        return convexHull;
    }

    /// <summary>
    /// Checks the orientation of the three points provided. Returns true if
    /// orientation is counterclockwise, and false if orientation is clockwise
    /// or colinear.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns> true for leftmost orientation false for others.</returns>
    private static bool GetOrientation(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p2.x - p1.x) * (p3.y - p1.y) -
               (p3.x - p1.x) * (p2.y - p1.y) > 0;
    }

    /// <summary>
    /// Clusters points within the distance threshold.
    /// </summary>
    /// <param name="hull"> List of hull points </param>
    /// <returns> List of clustered points </returns>
    private static List<Vector2> ClusterHullPoints(List<Vector2> hull)
    {
        for(var i = 0; i < hull.Count - 1; i++)
        {
            if(Vector2.Distance(hull[i], hull[i+1]) <= distanceThreshold)
            {
                hull.RemoveAt(i+1);
                i--;
            }
        }

        return hull;
    }

}