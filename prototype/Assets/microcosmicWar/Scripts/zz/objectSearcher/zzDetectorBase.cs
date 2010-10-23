
using UnityEngine;
using System.Collections;

public abstract class zzDetectorBase : MonoBehaviour
{
    //过滤不需要被探测的物体
    public delegate bool detectorFilterFunc(Collider pCollider);

    //bool allNeedDetected(Collider pCollider)
    //{
    //    return true;
    //}

    //越高的优先级 越被先处理
    public int priority = 0;

    public Collider[] detect(int pMaxRequired, LayerMask pLayerMask)
    {
        //detect(pMaxRequired, pLayerMask, allNeedDetected);

        return detect(pMaxRequired, pLayerMask, null);
    }

    public abstract Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc);

    public virtual int getPriority()
    {
        return priority;
    }


    //void Reset()
    //{
    //    zzObjectSearcher lzzObjectSearcher = (zzObjectSearcher)zzUtilities.needComponent(gameObject, typeof(zzObjectSearcher));
    //    lzzObjectSearcher.setDetector(this);
    //}
}