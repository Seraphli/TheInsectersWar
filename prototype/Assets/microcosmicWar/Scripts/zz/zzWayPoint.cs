
using UnityEngine;
using System.Collections;

public class zzWayPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "pathNode.tif");
    }
}