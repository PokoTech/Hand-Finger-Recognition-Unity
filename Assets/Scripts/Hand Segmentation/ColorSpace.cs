using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Colorspace is an unity gameobject that takes all colors from
/// the designated space (square starting at a certain point) and
/// returns them as HSV color objects.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ColorSpace : MonoBehaviour
{

    public Vector2 colorSpacePosition;
    public Vector2 colorSpaceSize;

    private float scale;

    private void Start()
    {
        SetColorSpace(colorSpacePosition, colorSpaceSize);
    }

    /// <summary>
    /// Uses the texture from the webcam and gets colors within the defined 
    /// coordinates on screen.
    /// </summary>
    /// <param name="space">
    /// Texture from the webcam.
    /// </param>
    /// <returns>
    /// List of HSV objects converted from the rbg pixels in the space.
    /// </returns>
    public List<HSV> GetColorsInSpace(WebCamTexture space)
    {
        List<HSV> colors = new List<HSV>();
        Color[] blockOfPixels = space.GetPixels((int)colorSpacePosition.x, (int)colorSpacePosition.y,
                                                (int)colorSpaceSize.x, (int)colorSpaceSize.y);

        foreach(var pixel in blockOfPixels)
        {
            Color.RGBToHSV(pixel, out float h, out float s, out float v);
            // Remove harsh colors.
            if(v > 0.1f && s < 0.9f)
            {
                colors.Add(new HSV(h, s, v));
            }
        }

        return colors;
    }

    /// <summary>
    /// Rescales the colourspace so that the visual indicator matches the 
    /// color sample area.
    /// </summary>
    /// <param name="scale"> to scale the space to screensize </param>
    public void SetScale(float scale)
    {
        this.scale = scale;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = colorSpaceSize * scale;
        rectTransform.anchoredPosition = (colorSpacePosition) * scale;
    }

    /// <summary>
    /// Repositions and resizes the space with the given parameters.
    /// </summary>
    /// <param name="position"> point of bottom left corner </param>
    /// <param name="size"> unscaled size of space </param>
    public void SetColorSpace(Vector2 position, Vector2 size)
    {
        // Assign values to variables.
        colorSpacePosition = position;
        colorSpaceSize = size;
        // Move the visual indicator
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = size * scale;
        rectTransform.anchoredPosition = (position) * scale;
    }

}
