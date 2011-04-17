using UnityEngine;

public class MouseZoom : MonoBehaviour
{
    public Camera zoomCamera;
    public float maxSize = 40f;
    public float minSize = 1.5f;
    public float damping = 0.5f;

    public float wantSize = 9.5f;
    public float sizeInEveryChange = 1f;

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
        {
            wantSize = Mathf.Max(wantSize - sizeInEveryChange, minSize);

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
        {
            wantSize = Mathf.Min(wantSize + sizeInEveryChange, maxSize);
        }
        if (!Mathf.Approximately(wantSize, zoomCamera.orthographicSize))
            zoomCamera.orthographicSize = Mathf.Lerp(zoomCamera.orthographicSize,
                wantSize, Time.deltaTime/damping);
    }


}