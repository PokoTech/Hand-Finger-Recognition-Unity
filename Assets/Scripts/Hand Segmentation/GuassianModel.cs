using System;
using System.Collections.Generic;


/// <summary>
/// Guassian model of values (color values) for probability checking.
/// </summary>
public class GuassianModel 
{
    private float mean, deviation, variance;
    private List<float> population;

    public GuassianModel()
    {
        population = new List<float>();
    }

    /// <summary>
    /// Calculates probability of value being in the population
    /// (Area underneath the probability density function).
    /// </summary>
    /// <param name="x"> Value to check. </param>
    /// <returns> Probability of being in population. </returns>
    private double CalculateProbability(float x)
    {
        return 0.5 * (1 + Utility.Erf((x - mean) / Math.Sqrt(2 * variance)));
    }

    /// <summary>
    /// Calculates the mean of all members in the population.
    /// </summary>
    /// <returns> Mean of population. </returns>
    private float CalculateMean()
    {
        float sum = 0;
        foreach (var member in population)
        {
            sum += member;
        }
        return sum / population.Count;
    }

    /// <summary>
    /// Calculates the variance of the population.
    /// </summary>
    /// <param name="mean"> Mean of population. </param>
    /// <returns> Variance in population. </returns>
    private float CalculateVariance(float mean)
    {
        float sum = 0;
        foreach (var member in population)
        {
            sum += (float)Math.Pow((member - mean), 2);
        }

        return sum / (population.Count - 1);
    }

    /// <summary>
    /// Square roots variance to get standard deviation of population.
    /// </summary>
    /// <param name="variance"> Variance of popualtion. </param>
    /// <returns> Standard deviation of population. </returns>
    private float CalculateStandardDeviation(float variance)
    {
        return (float)Math.Sqrt(variance);
    }

    /// <summary>
    /// Returns if value is over the threshold. 
    /// </summary>
    /// <param name="x"> Value to check. </param>
    /// <param name="threshold"> Threshold to compare. </param>
    /// <returns> True if value is above threshold. </returns>
    public bool WithinThreshold(float x, float threshold)
    {
        return CalculateProbability(x) >= threshold;
    }


    /// <summary>
    /// Adds multiple members to the population.
    /// </summary>
    /// <param name="members"> Array of values. </param>
    public void AddMembers(float[] members)
    {
        population.AddRange(members);
    }

    /// <summary>
    /// Adds a single member to the population.
    /// </summary>
    /// <param name="member"> Single value. </param>
    public void AddMember(float member)
    {
        population.Add(member);
    }

    /// <summary>
    /// Recalculates new mean, variance and standard deviation with
    /// new population.
    /// </summary>
    public void ApplyPopulation()
    {
        mean = CalculateMean();
        variance = CalculateVariance(mean);
        deviation = CalculateStandardDeviation(variance);
    }

}
