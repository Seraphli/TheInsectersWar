
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    //public abstract Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc);

    public virtual RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        Debug.LogError("public virtual RaycastHit[] _impDetect()");
        return null;
    }

    public virtual Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc)
    {
        RaycastHit[] lHits;
        //lHits = Physics.SphereCastAll(getOrigin(), radius, getDirection(), Mathf.Infinity, pLayerMask);
        lHits = _impDetect(pLayerMask);
        int lOutNum;
        lOutNum = Mathf.Min(pMaxRequired, lHits.Length);
        Collider[] lOut;


        //执行探测过滤,未测试
        if (pNeedDetectedFunc != null)
        {
            int lHitsIndex = 0;
            int lOutIndex = 0;
            var lColliderList = new List<Collider>(lOutNum);
            while ( lOutIndex < lOutNum && lHitsIndex < lHits.Length )
            {
                if (pNeedDetectedFunc(lHits[lOutIndex].collider))
                {
                    //lOut[lOutIndex] = lHits[lOutIndex].collider;
                    lColliderList.Add(lHits[lOutIndex].collider);
                    ++lOutIndex;
                }
                ++lHitsIndex;
            }
            lOut = lColliderList.ToArray();

        }
        else
        {
            lOut = new Collider[lOutNum];

            for (int lOutIndex = 0; lOutIndex < lOutNum; ++lOutIndex)
            {
                lOut[lOutIndex] = lHits[lOutIndex].collider;
            }

        }
        return lOut;
    }

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