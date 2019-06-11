///// <author> Elliot Edgington </author>
///// student number: 15806601 username: EE105
///// <title> Gesture Recognition for Human Computer Interaction in Video Games </title>
///// <subject> CI301 - The Individual Project </subject>



using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class acts like the control in the MVC. It handles starting the
/// segmentation process and sending data to the hand model.
/// </summary>
public class OutputImage : MonoBehaviour
{
    public RawImage output;
    public ColorSpace colorSpace;

    public HandModel handModel;

    public DisplayOptions displayOptions;

    // Changeable via the unity submenu.
    public int xResolution, yResolution;


    // Works well with 0.15f.
    [Range(0.0f, 1.0f)]
    public float threshold = 0.15f;

    private CameraTexture cameraTexture;
    private bool onChanged;

    private SegmentationManager segmentationManager;


    /// <summary>
    /// Sort of like a constuctor, works like a 'main'.
    /// </summary>
    private void Start()
    {
        onChanged = false;
        cameraTexture = new CameraTexture(xResolution, yResolution);

        colorSpace.SetScale(cameraTexture.GetScale());
    }

    /// <summary>
    /// The main update loop of the project. Each frames pixels get sent to the
    /// segmentation manager.
    /// </summary>
    private void Update()
    {
        // If the webcam image is unchanged, display image from the WebCamTexture.
        if (!onChanged)
        {
            output.texture = cameraTexture.GetWebCamTexture();
            return;
        }

        var webCamTexture = cameraTexture.GetWebCamTexture();
        // Creating new texture2D for editing privileges.
        var texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);

        // Make Modifications.
        texture2D.SetPixels(segmentationManager.SegmentColors(webCamTexture, threshold, displayOptions));

        // Apply modificaitons.
        texture2D.Apply();

        // Output modified texture to the RawImage.
        output.texture = texture2D;

        // Model the hand to screen.
        handModel.ModelHand(segmentationManager.GetSkinObject(),
                            segmentationManager.GetHullPoints(),
                            cameraTexture.GetScale());
    }

    /// <summary>
    /// Starts the process of segmentation. Initialises a new segmentation manager
    /// with the average colors from colorspace.
    /// </summary>
    public void Calibrate()
    {
        // Reset manager.
        segmentationManager = new SegmentationManager(colorSpace.GetColorsInSpace(cameraTexture.GetWebCamTexture()));
        // Change output.
        onChanged = true;
    }


    public SegmentationManager GetSegmentationManagerForTesting()
    {
        return segmentationManager;
    }
}
