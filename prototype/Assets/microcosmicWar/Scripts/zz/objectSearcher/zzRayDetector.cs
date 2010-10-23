
using UnityEngine;
using System.Collections;


class zzRayDetector : zzDetectorBase
{
    //Transform _from;
    public Transform _to;

    public override Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc)
    {
        RaycastHit[] lHits;
        lHits = Physics.RaycastAll(getOrigin(), getDirection(), getDistance(), pLayerMask);
        int lOutNum;
        lOutNum = Mathf.Min(pMaxRequired, lHits.Length);
        Collider[] lOut = new Collider[lOutNum];


        //执行探测过滤,未测试
        if (pNeedDetectedFunc!=null)
        {
            int lHitsIndex = 0;
            int lOutIndex = 0;
            for (;lOutIndex < lOutNum && lHitsIndex < lHits.Length;)
            {
                if (pNeedDetectedFunc(lHits[lOutIndex].collider))
                {
                    lOut[lOutIndex] = lHits[lOutIndex].collider;
                    ++lOutIndex;
                }
                ++lHitsIndex;
            }

        }
        else
        {
            for (int lOutIndex = 0; lOutIndex < lOutNum; ++lOutIndex)
            {
                lOut[lOutIndex] = lHits[lOutIndex].collider;
            }

        }
        return lOut;
    }

    public virtual Vector3 getOrigin()
    {
        //return _from.position;
        return transform.position;
    }

    public virtual Vector3 getDirection()
    {
        return (_to.position - transform.position).normalized;
    }

    public virtual float getDistance()
    {
        return (_to.position - transform.position).magnitude;
    }


    //function OnDrawGizmosSelected() 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _to.position);
    }
}
