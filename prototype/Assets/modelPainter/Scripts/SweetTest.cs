using UnityEngine;
using System.Collections;

public class SweetTest : MonoBehaviour
{
    public Rigidbody sweepRigidbody;
    public Transform beginPositon;
    public Vector3 direction;
    public float distance;


    [ContextMenu("test")]
    void test()
    {
        sweepRigidbody.transform.position = beginPositon.position;
        RaycastHit[] lHits = sweepRigidbody.SweepTestAll(direction, distance);
        foreach (var lHit in lHits)
        {
            print(lHit.transform.name);
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawLine(beginPositon.position, beginPositon.position + direction.normalized * distance);
    }
}