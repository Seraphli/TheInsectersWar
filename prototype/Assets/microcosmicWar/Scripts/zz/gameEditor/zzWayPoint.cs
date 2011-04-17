using UnityEngine;
using System.Collections.Generic;

public class zzWayPoint:MonoBehaviour
{
    public zzWayPoint[] nextPoints = new zzWayPoint[0]{};

    public void addNextPoint(zzWayPoint pPoint)
    {
        var lNextPoints = new List<zzWayPoint>(nextPoints);
        if (!lNextPoints.Contains(pPoint))
        {
            lNextPoints.Add(pPoint);
            nextPoints = lNextPoints.ToArray();
        }
    }

    public void removeNextPoint(zzWayPoint pPoint)
    {
        var lNextPoints = new List<zzWayPoint>(nextPoints);

        //移除成功则替换原数组
        if (lNextPoints.Remove(pPoint))
            nextPoints = lNextPoints.ToArray();
    }

    public zzWayPoint[] getNeighbor(float pRange,float pYLimit, LayerMask pPointMask, LayerMask pPreventMask)
    {
        //print("" + pRange + ":" + pPointMask + ":" + pPreventMask);
        var lColliders = Physics.OverlapSphere(transform.position, pRange, pPointMask);
        //print(lColliders.Length);
        List<zzWayPoint> lOut = new List<zzWayPoint>(lColliders.Length);
        var lPosition = transform.position;
        foreach (var lCollider in lColliders)
        {
            var lTransform = lCollider.transform;
            var lWayPoint = lTransform.parent.GetComponent<zzWayPoint>();
            if (lWayPoint && lWayPoint!=this)
            {
                var lVector = lTransform.position - lPosition;
                var lDistance = lVector.magnitude;
                if (Mathf.Abs(lVector.y) <= pYLimit && !Physics.Raycast(lPosition,
                        lVector,lDistance, pPreventMask))
                    lOut.Add(lWayPoint);
            }
        }
        //print(lOut.Count);
        return lOut.ToArray();
    }
}