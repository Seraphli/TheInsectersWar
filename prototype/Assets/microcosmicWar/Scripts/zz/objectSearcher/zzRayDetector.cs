
using UnityEngine;
using System.Collections;


class zzRayDetector : zzDetectorBase
{
    //Transform _from;
    public Transform _to;

    public override Collider[] detector(int pMaxRequired, LayerMask pLayerMask)
    {
        RaycastHit[] lHits;
        lHits = Physics.RaycastAll(getOrigin(), getDirection(), getDistance(), pLayerMask);
        int lOutNum;
        lOutNum = Mathf.Min(pMaxRequired, lHits.Length);
        Collider[] lOut = new Collider[lOutNum];
        for (int i = 0; i < lOutNum; ++i)
        {
            lOut[i] = lHits[i].collider;
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
