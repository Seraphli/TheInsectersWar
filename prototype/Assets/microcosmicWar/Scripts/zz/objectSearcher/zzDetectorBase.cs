
using UnityEngine;
using System.Collections;

public abstract class zzDetectorBase : MonoBehaviour
{


    //越高的优先级 越被先处理
    public int priority = 0;

    public abstract Collider[] detector(int pMaxRequired, LayerMask pLayerMask);

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