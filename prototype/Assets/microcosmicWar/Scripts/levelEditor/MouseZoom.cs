using UnityEngine;

public abstract class ScaleBase : MonoBehaviour
{
    public abstract float range { set; }
}

public class MouseZoom : MonoBehaviour
{
    public Camera zoomCamera;
    public float maxSize = 40f;
    public float minSize = 1.5f;

    public float wantSize = 9.5f;
    public float sizeInEveryChange = 1f;

    public ScaleBase[] scaleList;

    public float wantRange;
    public float nowRange;
    public float rangeInEveryChange = 0.08f;
    public float damping = 0.5f;

    void setRange(float lRange)
    {
        foreach (var lScale in scaleList)
        {
            lScale.range = lRange;
        }
    }

    void Start()
    {
        setRange(nowRange);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
        {
            //wantSize = Mathf.Max(wantSize - sizeInEveryChange, minSize);
            wantRange = Mathf.Max(wantRange - rangeInEveryChange, 0f);

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
        {
            //wantSize = Mathf.Min(wantSize + sizeInEveryChange, maxSize);
            wantRange = Mathf.Min(wantRange + rangeInEveryChange, 1f);
        }

        //if (!Mathf.Approximately(wantSize, zoomCamera.orthographicSize))
        //    zoomCamera.orthographicSize = Mathf.Lerp(zoomCamera.orthographicSize,
        //        wantSize, Time.deltaTime/damping);
        if (!Mathf.Approximately(wantRange, nowRange))
        {
            nowRange = Mathf.Lerp(nowRange, wantRange, Time.deltaTime / damping);
            setRange(nowRange);
        }
    }


}