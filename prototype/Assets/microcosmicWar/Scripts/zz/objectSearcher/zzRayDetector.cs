
using UnityEngine;
using System.Collections;


class zzRayDetector : zzDetectorBase
{
    //Transform _from;
    public Transform _to;

    public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        return Physics.RaycastAll(getOrigin(), getDirection(), getDistance(), pLayerMask);
   
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _to.position);
    }

}
