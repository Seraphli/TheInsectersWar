
using UnityEngine;
using System.Collections;

class zzSphereDetector : zzDetectorBase
{
    public float radius;
    public Transform direction;

    public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        return Physics.SphereCastAll(getOrigin(), radius, getDirection(), getDistance(), pLayerMask);

    }

    public virtual float getDistance()
    {
        return (direction.position - transform.position).magnitude;
    }

    public virtual Vector3 getOrigin()
    {
        return transform.position;
    }

    public virtual Vector3 getDirection()
    {
        return (direction.position - transform.position).normalized;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        zzUtilities.GizmosArrow(transform.position, direction.position);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(direction.position, radius);
    }
}