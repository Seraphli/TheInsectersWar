
using UnityEngine;
using System.Collections;

public class zzWayPointGizmos : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "pathNode.tif");
    }
}