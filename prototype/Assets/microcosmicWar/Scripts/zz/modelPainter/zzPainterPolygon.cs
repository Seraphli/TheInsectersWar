using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class zzPainterPolygon : MonoBehaviour
{
    public zz2DBounds bounds;
    public zzPainterPoint rootPainterPoints;

    public bool needDraw = true;

    public List<zzPainterPoint> getAllPoint()
    {
        var lPoint = rootPainterPoints;
        var lPainterPoints = new List<zzPainterPoint>();
        Hashtable lHaveDrawn = new Hashtable();
        while (lPoint && (!lHaveDrawn.ContainsKey(lPoint)))
        {
            Vector3 l3DPoint = lPoint.transform.position;
            Vector2 l2DPoint = new Vector2(l3DPoint.x, l3DPoint.y);
            lHaveDrawn[lPoint] = true;
            //l2DPoints.AddLast(l2DPoint);
            lPainterPoints.Add(lPoint);
            lPoint = lPoint.nextPoint;
        }
        return lPainterPoints;
    }

    public void setPointInfo<T>(T pointsInfo) where T : IEnumerable<zz2DPoint>, ICollection<zz2DPoint>
    {
        if (pointsInfo.Count != transform.childCount)
        {
            Debug.LogError("pointsInfo.Count != transform.childCount");
            Debug.LogError("pointsInfo.Count :" + pointsInfo.Count);
            Debug.LogError("transform.childCount :" + transform.childCount);
        }

        var lPoint = rootPainterPoints;
        foreach (var lInfo in pointsInfo)
        {
            //boundsEncapsulate(lInfo.position);
            lPoint.pointInfo = lInfo;
            lPoint = lPoint.nextPoint;
        }

    }

    //void  boundsEncapsulate(Vector2 point)
    //{
    //    if (bounds==null)
    //        bounds = new zz2DBounds(point);
    //    else
    //        bounds.encapsulate(point);
    //}

    void OnDrawGizmos()
    {
        if (bounds!=null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube((bounds.max + bounds.min)/2.0f, (bounds.max - bounds.min) );

        }
    }

}