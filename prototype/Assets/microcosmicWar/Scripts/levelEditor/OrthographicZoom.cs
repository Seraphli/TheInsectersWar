using UnityEngine;

public class OrthographicZoom:ScaleBase
{
    public Camera zoomCamera;
    public float maxSize = 40f;
    public float minSize = 1.5f;
    public override float rate 
    { 
        set
        {
            zoomCamera.orthographicSize = Mathf.Lerp(minSize, maxSize, value);
        }
    }
}