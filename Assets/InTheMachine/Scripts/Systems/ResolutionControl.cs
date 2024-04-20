using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionControl : MonoBehaviour
{
    private int resolutionIndex = 0;
    private Vector2Int[] targetResolutions = { new(960, 540), new(1280, 720), new(1920, 1080), new(2560, 1440), new(3840, 2160) };

    public Vector2Int currentResolution => targetResolutions[resolutionIndex];
    public Vector2Int nextResolution => resolutionIndex+1 >= targetResolutions.Length ? new(9999,9999) : targetResolutions[resolutionIndex +1];

    // Start is called before the first frame update
    void Start()
    {
        foreach (var resolution in Screen.resolutions)
        {
            if ((resolution.width >= nextResolution.x && resolution.height >= nextResolution.y))
            {
                resolutionIndex++;
            } 
        }
        Debug.Log(currentResolution);
        Screen.SetResolution(currentResolution.x,currentResolution.y,FullScreenMode.FullScreenWindow);
    }
}
