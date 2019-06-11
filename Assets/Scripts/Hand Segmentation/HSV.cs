using UnityEngine;

/// <summary>
/// An object to store hsv colors. 
/// </summary>
public class HSV 
{
    public float h, s, v;

    public HSV(float h, float s, float v)
    {
        this.h = h;
        this.s = s;
        this.v = v;
    }

    /// <summary>
    /// Returns tuple of the stores hsv color.
    /// </summary>
    /// <returns> vector3 of color. </returns>
    public Vector3 GetVector3()
    {
        return new Vector3(h, s, v);
    }

}
