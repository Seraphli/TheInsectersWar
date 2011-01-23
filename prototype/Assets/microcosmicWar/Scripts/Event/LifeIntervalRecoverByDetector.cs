
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 按设定的间隔补血
/// </summary>
class LifeIntervalRecoverByDetector : MonoBehaviour
{
    public float recoverInterval = 0.2f;

    public int recoverValueEveryTime = 5;

    public int maxRecoverObjectNum = 10;

    public LayerMask recoverLayerMask;

    public zzDetectorBase detector;
    public zzTimer recoverTimer;

    void Awake()
    {
        if (!recoverTimer)
        {
            recoverTimer = gameObject.AddComponent<zzTimer>();
        }
        recoverTimer.setInterval(recoverInterval);
        recoverTimer.setImpFunction(recover);
    }

    void recover()
    {
        var lDetected = detector.detect(maxRecoverObjectNum, recoverLayerMask);
        foreach (var lCollider in lDetected)
        {
            var lLife = Life.getLifeFromTransform(lCollider.transform);
            lLife.injure(-recoverValueEveryTime);
        }
    }

}