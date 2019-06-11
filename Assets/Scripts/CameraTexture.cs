using UnityEngine;

public class CameraTexture
{

    private WebCamTexture deviceCameraTexture;
    private float scale;


    /// <summary>
    /// Initialises a new webcam context with the given resolution.
    /// </summary>
    /// <param name="xResolution">  Width of screen. </param>
    /// <param name="yResolution"> Height of screen.</param>
    public CameraTexture(int xResolution, int yResolution)
    {

        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0)
        {
            Debug.Log("No cameras available");
            return;
        }
        else
        {
            foreach(var device in devices)
            {
                deviceCameraTexture = new WebCamTexture(device.name, xResolution , yResolution);
            }
            scale = ((float)Screen.width / (float)xResolution);
            deviceCameraTexture.Play();
        }
    }
    
    /// <summary>
    /// Getter methods. 
    /// </summary>

    public WebCamTexture GetWebCamTexture()
    {
        return deviceCameraTexture;
    }

    public float GetScale()
    {
        return scale;
    }
}
