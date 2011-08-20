using UnityEngine;
using System.Collections;

public class zzOverlapSphereDetector : zzDetectorBase
{
    public float radius;

    public override Collider[] detect(int pMaxRequired, LayerMask pLayerMask, detectorFilterFunc pNeedDetectedFunc)
    {
        return Physics.OverlapSphere(transform.position, radius, pLayerMask);
    }

    //public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    //{
    //    return Physics.OverlapSphere(transform.position, radius, pLayerMask);
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}