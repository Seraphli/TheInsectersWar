using UnityEngine;
using System.Collections;

public class zzPainterPoint : MonoBehaviour
{
    public zzPainterPoint nextPoint;

    public zz2DPoint pointInfo;

    public Vector2 getVec2Position()
    {
        Vector3 l3DPoint = transform.position;
        Vector2 l2DPoint = new Vector2(l3DPoint.x, l3DPoint.y);
        return l2DPoint;

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
        if (nextPoint)
            Gizmos.DrawLine(transform.position, nextPoint.transform.position);
    }
}