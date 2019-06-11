using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Outputs model of hand to screen showing the palm of the hand and the 
/// fingertip points.
/// </summary>
public class HandModel : MonoBehaviour
{
    public Image palmImage;
    public Image fingertipPrefab;

    public Text fingerCountText;

    private RectTransform palmImageRect;
    private CircleCollider2D palmImageCollider;

    private List<Image> fingertips;

    private void Start()
    {
        palmImageRect = palmImage.GetComponent<RectTransform>();
        palmImageCollider = palmImage.GetComponent<CircleCollider2D>();
        fingertips = new List<Image>();
    }

    /// <summary>
    /// Uses the parameters to reposition the images on the screen.
    /// </summary>
    /// <param name="skinObject"> Blob of skin </param>
    /// <param name="fingertipPositions"> fingertip coordinates </param>
    /// <param name="scale"> screen scale </param>
    public void ModelHand(SkinBlob skinObject, List<Vector2> fingertipPositions, float scale)
    {
        // Set palm position.
        palmImageRect.anchoredPosition = skinObject.GetMeanPoint() * scale;
        palmImageRect.sizeDelta = new Vector2((skinObject.GetWidth() / 2) * scale , (skinObject.GetWidth() / 2) * scale);

        palmImageCollider.radius = skinObject.GetWidth() / 2;

        // Add new fingertips if neccesary.
        if(fingertips.Count < fingertipPositions.Count)
        {
            var numberOfNewFingetips = fingertipPositions.Count - fingertips.Count;
            for (var i = 0; i < numberOfNewFingetips; i++)
            {
                fingertips.Add(Instantiate(fingertipPrefab, transform));
            }
        }

        // Set location of fingertips.
        for(var i = 0; i < fingertips.Count; i++)
        {
            if(i < fingertipPositions.Count)
            {
                fingertips[i].gameObject.SetActive(true);
                fingertips[i].GetComponent<RectTransform>().anchoredPosition =
                    fingertipPositions[i] * scale;
            }
            else
            {
                fingertips[i].gameObject.SetActive(false);
            }
        }

        CountFingers(fingertipPositions.Count);
    }

    /// <summary>
    /// Outputs number of fingers detected to the text on screen.
    /// </summary>
    /// <param name="numberOfFingers"> Number of fingertip points. </param>
    public void CountFingers(int numberOfFingers)
    {
        if (numberOfFingers > 5)
            numberOfFingers = 5;

        fingerCountText.text = numberOfFingers.ToString();
    }
}
