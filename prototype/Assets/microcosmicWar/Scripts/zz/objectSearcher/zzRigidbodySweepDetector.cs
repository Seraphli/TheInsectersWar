using UnityEngine;
using System.Collections;

public class zzRigidbodySweepDetector : zzDetectorBase
{
    public Rigidbody sweepRigidbody;
    public Vector3 beginPositon
    {
        get { return sweepRigidbody.position; }

        set { sweepRigidbody.transform.position = value; }
    }
    public Vector3 direction;
    public float distance;

    public Vector3 worldDirection
    {
        get { return transform.TransformDirection(direction); }
    }

    public void Awake()
    {
        sweepRigidbody.gameObject.active = false;
    }

    public override RaycastHit[] _impDetect(LayerMask pLayerMask)
    {
        return SweetTest();
    }

    public RaycastHit[] SweetTest()
    {
        sweepRigidbody.gameObject.active = true;
        var lOut = sweepRigidbody.SweepTestAll(worldDirection, distance);
        sweepRigidbody.gameObject.active = false;
        return lOut;

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(beginPositon, beginPositon + worldDirection.normalized * distance);
    }
}