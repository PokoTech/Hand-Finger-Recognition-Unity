using UnityEngine;


/// <summary>
/// This object exists to parse to the segmentation manager, it activates 
/// and deactivates coloursing of certain stages of the project to help get
/// a better view.
/// </summary>
public class DisplayOptions : MonoBehaviour
{
    [Header("Activate and deactive certain stages within the project.")]

    [Tooltip("Initial colour segmentation using hand gaussian colours.")]
    public bool SHOW_SEGMENTATION_FIRST = false;

    [Tooltip("Segmentation after skin blob detection and lower threshold.")]
    public bool SHOW_SEGMENTATION_SECOND = false;

    [Tooltip("Contour of detected skin")]
    public bool SHOW_CONTOUR = false;

}
