using UnityEngine;
using System.Collections.Generic;

public class zzWayPoint:MonoBehaviour
{
    [SerializeField]
    Transform _lineCenter;

    //public zzWayPoint[] nextPoints = new zzWayPoint[0] { };
    //public zzWayPoint[] backPoints = new zzWayPoint[0] { };

    public Vector3 lineCenter
    {
        get
        {
            return _lineCenter.position;
        }
    }

    //public void OnRemove()
    //{
    //    foreach (var lNextPoint in nextPoints)
    //    {
    //        lNextPoint.removeBackPoint(this);
    //    }
    //    foreach (var lBackPoint in backPoints)
    //    {
    //        var lNextPoints = new List<zzWayPoint>(lBackPoint.nextPoints);

    //        //移除成功则替换原数组
    //        if (lNextPoints.Remove(this))
    //        {
    //            lBackPoint.nextPoints = lNextPoints.ToArray();
    //        }
    //    }
    //}

    //void addBackPoint(zzWayPoint pPoint)
    //{
    //    var lBackPoints = new List<zzWayPoint>(backPoints);
    //    lBackPoints.Add(pPoint);
    //    backPoints = lBackPoints.ToArray();
    //}

    //void removeBackPoint(zzWayPoint pPoint)
    //{
    //    var lBackPoints = new List<zzWayPoint>(backPoints);

    //    //移除成功则替换原数组
    //    if (lBackPoints.Remove(pPoint))
    //        backPoints = lBackPoints.ToArray();
    //}

    //public bool addNextPoint(zzWayPoint pPoint)
    //{
    //    var lNextPoints = new List<zzWayPoint>(nextPoints);
    //    if (!lNextPoints.Contains(pPoint))
    //    {
    //        lNextPoints.Add(pPoint);
    //        nextPoints = lNextPoints.ToArray();
    //        pPoint.addBackPoint(this);
    //        return true;
    //    }
    //    return false;
    //}

    //public void removeNextPoint(zzWayPoint pPoint)
    //{
    //    var lNextPoints = new List<zzWayPoint>(nextPoints);

    //    //移除成功则替换原数组
    //    if (lNextPoints.Remove(pPoint))
    //    {
    //        nextPoints = lNextPoints.ToArray();
    //        pPoint.removeBackPoint(this);
    //    }
    //}

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