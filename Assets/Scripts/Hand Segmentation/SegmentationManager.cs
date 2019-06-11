using System.Collections.Generic;
using UnityEngine;

public class SegmentationManager
{
    // Guassian Models for color thresholding.
    private GuassianModel hModel, sModel, vModel;
    private List<GuassianModel> models;

    // Skin blob for hand recognisation.
    private List<SkinBlob> skinObjects;
    private SkinBlob largestSkinObject;

    // Hull points for fingertip detection.
    private List<Vector2> fingertipPoints;

    // Threshold for pixel closeness.
    private const int closeThreshold = 5;

    /// <summary>
    /// Initialises the guassian models for each parameter (H, S, V) with
    /// colors retrieved from the colorspace.
    /// </summary>
    /// <param name="colorPopulation"> List of HSV colors from colorspace.</param>
    public SegmentationManager(List<HSV> colorPopulation)
    {
        models = new List<GuassianModel>();
        hModel = new GuassianModel();
        sModel = new GuassianModel();
        vModel = new GuassianModel();
        models.Add(hModel);
        models.Add(sModel);
        models.Add(vModel);

        // Add new values to each model.
        foreach (var color in colorPopulation)
        {
            hModel.AddMember(color.h);
            sModel.AddMember(color.s);
            vModel.AddMember(color.v);
        }

        // Apply new values to models.
        models.ForEach(model => model.ApplyPopulation());
    }

    /// <summary>
    /// Main segmentation loop. Loops through the entire set of pixels to
    /// detect skin-like pixels. Then Removes noise and enhances the skin
    /// pixels using skinblobs and gets the contourPointss of the skin. Finally
    /// calculating the convex hull. 
    /// Also changes colors of pixels in the texture for debug / visualisation
    /// purposes.
    /// </summary>
    /// <param name="texture"> Contains all the pixels of the image. </param>
    /// <param name="threshold"> 
    /// Contains the color threshold to be considered a skin candidate.
    /// </param>
    /// <returns> Edited texture. </returns>
    public Color[] SegmentColors(WebCamTexture texture, float threshold, DisplayOptions OPTIONS)
    {
        // Get an array of pixels from the texture.
        var pixels = texture.GetPixels();

        // First Loop : Segment and identify skin blobs.
        skinObjects = new List<SkinBlob>();
        largestSkinObject = new SkinBlob(new Vector2());
        int close = 0;
        for (var i = 0; i < pixels.Length; i++)
        {
            // Identify skin coloured pixels and add them to an object.
            // Threshold for skin color lowers if the pixel is close to a definite skin pixel.
            if (PixelThreshold(pixels[i], threshold) || (close > 0 && PixelThreshold(pixels[i], threshold / 2)))
            {
                CheckSkinObjects(Utility.IndexToPoint(texture.width, i));
                close = closeThreshold;
                // Display options - Sets initital segmentation pixels to white
                if (OPTIONS.SHOW_SEGMENTATION_FIRST)
                    pixels[i] = Color.white;
            }
            else
            {
                close--;
                // Display options - Sets other pixels to black
                if (OPTIONS.SHOW_SEGMENTATION_FIRST)
                    pixels[i] = Color.black;
            }

        }

        // Second Loop : focus on largest skin blob, removing noise. Get contour list.

        // Contour list creates candidates for the convex hull.
        var contourPoints = new List<Vector2>();
        // Linewidth dictates the longest uninterrupted line of skin pixels.
        var lineWidth = 0;
        var thisPixel = 0; // 0 black / 1 white.
        var lastPixel = 0; // 0 black / 1 white.

        for (var i = 0; i < pixels.Length; i++)
        {
            var pixelCoords = Utility.IndexToPoint(texture.width, i);
            // If within the skin objects min max boundries. 
            // Also within an even more lenient threshold.
            if (pixelCoords.y < largestSkinObject.GetMaxPoint().y &&
                pixelCoords.y > largestSkinObject.GetMinPoint().y &&
                pixelCoords.x < largestSkinObject.GetMaxPoint().x &&
                pixelCoords.x > largestSkinObject.GetMinPoint().x &&
                PixelThreshold(pixels[i], threshold / 3))
            {
                lineWidth++;
                thisPixel = 1; // Skin pixel
                // Calculates 'center of mass'.
                largestSkinObject.AddToMean(pixelCoords);
                // Display Options - Show pixels after second segmentaiton.
                if (OPTIONS.SHOW_SEGMENTATION_SECOND)
                    pixels[i] = Color.grey;
            }
            else
            {
                // Send linewidth to the skin object and reset linewidth as 
                // black pixel has been reached.
                largestSkinObject.TestWidth(lineWidth);
                lineWidth = 0;
                thisPixel = 0; // Black pixel.
                // Display Options - Show pixels after second segmentaiton.
                if (OPTIONS.SHOW_SEGMENTATION_SECOND)
                    pixels[i] = Color.black;
            }

            // Get a "good enough" contour point list for convex hull calculations.
            // Checks if the previous pixel was classed differently, then if true,
            // adds the current pixel to a list of contour points.
            if (thisPixel != lastPixel)
                contourPoints.Add(pixelCoords);

            lastPixel = thisPixel;
        }


        // Calculate convex hull using contour points.
        var hullPoints = ConvexHull.GetConvexHull(contourPoints);
        fingertipPoints = GetFingertips(hullPoints);




        // Display Options - Set contour pixels to green
        if (OPTIONS.SHOW_CONTOUR)
            contourPoints.ForEach(point =>
            {
                pixels[Utility.PointToIndex(texture.width, point)] = Color.green;
            });

        // Return array of edited pixels for display.
        return pixels;
    }

    /// <summary>
    /// Tests if the point is within the threshold of being in another object.
    /// If not then create a new object. Also prunes skinblobs with tiny areas.
    /// </summary>
    /// <param name="point"> Location of pixel to check. </param>
    private void CheckSkinObjects(Vector2 point)
    {
        bool isObject = false;

        for (var i = 0; i < skinObjects.Count; i++)
        {
            if (skinObjects[i].TestPoint(point))
            {
                isObject = true;
                if (skinObjects[i].GetArea() >= largestSkinObject.GetArea() &&
                    skinObjects[i].GetSize() >= largestSkinObject.GetSize())
                    largestSkinObject = skinObjects[i];
                break;
            }
            else if(skinObjects[i].GetArea() <= 1.0f)
            {
                skinObjects.Remove(skinObjects[i]);
                i--;
            }
        }

        if (!isObject)
        {
            skinObjects.Add(new SkinBlob(point));
        }
    }

    /// <summary>
    /// Converts the pixel to an HSV color and then tests if the pixel is 
    /// within the threshold for each gaussian.
    /// </summary>
    /// <param name="pixel"> Color of the pixel. </param>
    /// <param name="threshold"> Threshold for skin color. </param>
    /// <returns> True if pixel is within threshold. </returns>
    private bool PixelThreshold(Color pixel, float threshold)
    {
        Color.RGBToHSV(pixel, out float h, out float s, out float v);
        return hModel.WithinThreshold(h, threshold) && sModel.WithinThreshold(s, threshold)
               && vModel.WithinThreshold(v, threshold);
    }

    /// <summary>
    /// Removes any hull points that arent above the mean location of the palm.
    /// Also removes them if they are within the radius of the palm
    /// (closed fist).
    /// </summary>
    /// <param name="hull"> List of hull points (Fingertip contenders)</param>
    /// <returns> All detected fingertip points </returns>
    private List<Vector2> GetFingertips(List<Vector2> hull)
    {
        var fingertips = new List<Vector2>();
        var closePoints = 0;
        hull.ForEach(point =>
        {
            // If point is above palm position.
            // If point is outside the palm radius.
            if (point.y > largestSkinObject.GetMeanPoint().y &&
                Vector2.Distance(largestSkinObject.GetMeanPoint(), point) > largestSkinObject.GetWidth() / 2)
            {
                fingertips.Add(point);
                if (Vector2.Distance(largestSkinObject.GetMeanPoint(), point) < largestSkinObject.GetWidth())
                    closePoints++;
            }
        });
        if (closePoints >= hull.Count)
            fingertips.Clear();

        return fingertips;
    }

    /// <summary>
    /// Returns largest skin object (hand).
    /// </summary>
    /// <returns> largest skinblob </returns>
    public SkinBlob GetSkinObject()
    {
        return largestSkinObject;
    }

    /// <summary>
    /// Returns points on hull (fingertips).
    /// </summary>
    /// <returns> list of hull points. </returns>
    public List<Vector2> GetHullPoints()
    {
        return fingertipPoints;
    }
}


