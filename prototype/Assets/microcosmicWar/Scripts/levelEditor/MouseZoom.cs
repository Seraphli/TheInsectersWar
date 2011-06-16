using UnityEngine;

public abstract class ScaleBase : MonoBehaviour
{
    public abstract float range { set; }
}

public class MouseZoom : MonoBehaviour
{
    public ScaleBase[] scaleList;

    public float wantRange;
    public float nowRange;
    public float perInEveryChange = 0.1f;
    public float minPerInEveryChange = 0.05f;
    public float damping = 0.5f;

    public float lastTime = 0f;

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
        lastTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
        {
            //wantSize = Mathf.Max(wantSize - sizeInEveryChange, minSize);
            wantRange = Mathf.Max(Mathf.Max(wantRange * (1 - perInEveryChange),minPerInEveryChange), 0f);

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
        {
            //wantSize = Mathf.Min(wantSize + sizeInEveryChange, maxSize);
            wantRange = Mathf.Min(Mathf.Max(wantRange * (1 + perInEveryChange), minPerInEveryChange), 1f);
        }

        //if (!Mathf.Approximately(wantSize, zoomCamera.orthographicSize))
        //    zoomCamera.orthographicSize = Mathf.Lerp(zoomCamera.orthographicSize,
        //        wantSize, Time.deltaTime/damping);
        if (!Mathf.Approximately(wantRange, nowRange))
        {
            nowRange = Mathf.Lerp(nowRange, wantRange,
                (Time.realtimeSinceStartup - lastTime) / damping);
            setRange(nowRange);
            lastTime = Time.realtimeSinceStartup;
        }
    }


}