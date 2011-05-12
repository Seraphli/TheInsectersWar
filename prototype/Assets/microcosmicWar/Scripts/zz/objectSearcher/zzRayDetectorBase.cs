
using UnityEngine;
using System.Collections;


public abstract class zzRayDetectorBase : zzDetectorBase
{
    public Transform _to;

    public virtual Vector3 getOrigin()
    {
        //return _from.position;
        return transform.position;
    }

    public virtual Vector3 getDirection()
    {
        return (_to.position - transform.position);
    }

    public virtual float getDistance()
    {
        return (_to.position - transform.position).magnitude;
    }

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
