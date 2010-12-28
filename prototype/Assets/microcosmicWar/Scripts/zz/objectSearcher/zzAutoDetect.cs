
using UnityEngine;
using System.Collections;

/// <summary>
/// 自动执行探测
/// </summary>
public class zzAutoDetect:MonoBehaviour
{
    public zzDetectorBase detector;

    public LayerMask detectLayerMask;

    //被返回物体的最大数量
    public int maxNumInDetected = 1;

    public float detectInterval = 0.1f;

    public Collider[] result = new Collider[0];

    void Start()
    {
        var lTimer = gameObject.AddComponent<zzCoroutineTimer>();
        lTimer.setInterval(detectInterval);
        lTimer.setImpFunction(detect);
    }

    void detect()
    {
        result = detector.detect(maxNumInDetected, detectLayerMask);
    }
}