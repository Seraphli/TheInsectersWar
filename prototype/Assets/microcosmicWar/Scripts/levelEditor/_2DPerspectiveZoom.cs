using UnityEngine;

public class _2DPerspectiveZoom : ScaleBase
{
    public Transform zoomCameraTransform;
    public float to = 5f;
    public float from = 1.5f;
    public override float rate
    {
        set
        {
            var lPosition = zoomCameraTransform.position;
            lPosition.z = Mathf.Lerp(from, to, value);
            zoomCameraTransform.position = lPosition;
        }
    }
}