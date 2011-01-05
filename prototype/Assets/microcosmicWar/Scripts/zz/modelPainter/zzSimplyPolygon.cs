using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class zzSimplyPolygon
{
    LinkedList<zz2DPoint> mConcavePoints = new LinkedList<zz2DPoint>();
    LinkedList<zz2DPoint> mConvexPoints = new LinkedList<zz2DPoint>();
    LinkedList<zz2DPoint> mAllPoints = new LinkedList<zz2DPoint>();

    public int pointNum
    {
        get { return mAllPoints.Count; }
    }

    public LinkedList<zz2DPoint> getConvexPoints()
    {
        return mConvexPoints;
    }

    public bool    isConvex()
    {
        return mConcavePoints.Count == 0;
    }

    //class cutHelper
    //{
    //    public  zzSimplyPolygon cutedPolygon;
    //    public  zz2DPoint   newBeginPoint;
    //    public  zz2DPoint   newEndPoint;
    //    public  void    cut(zzSimplyPolygon otherPolygon,zz2DPoint pBeginPoint,zz2DPoint pEndPoint)
    //    {
    //        cutedPolygon = new zzSimplyPolygon();
            
    //        cutedPolygon.bounds = new zz2DBounds(otherPolygon.bounds);

    //        LinkedListNode<zz2DPoint>   lNode = otherPolygon.mAllPoints.First

    //        //第一个点
    //        zz2DPoint lFirstPoint = copyOtherPolygonPoint(lNode);

    //        //第二个点
    //        zz2DPoint lPrePoint = copyOtherPolygonPoint(lNode);

    //        linkPoint(lFirstPoint,lPrePoint);

    //        while(lNode!=null)
    //        {
    //            zz2DPoint lNewPoint = copyOtherPolygonPoint(lNode);
    //            linkPoint(lPrePoint,lNewPoint);
    //            lPrePoint = lNewPoint;
    //        }
    //        linkPoint(lPrePoint,lFirstPoint);

    //    }

    //    zz2DPoint copyOtherPolygonPoint
    //}

    //public zzSimplyPolygon(zzSimplyPolygon otherPolygon, zz2DPoint[] mapPoints, List<zz2DPoint>   mapResult )
    //{
    //    bounds = new zz2DBounds(otherPolygon.bounds);

    //    LinkedListNode<zz2DPoint> lNode = otherPolygon.mAllPoints.First;

    //    var lMapPoints = new List<zz2DPoint>(mapPoints);

    //    //第一个点
    //    zz2DPoint lFirstPoint = copyOtherPolygonPoint(ref lNode, lMapPoints, mapResult);

    //    //第二个点
    //    zz2DPoint lPrePoint = copyOtherPolygonPoint(ref lNode, lMapPoints, mapResult);

    //    linkPoint(lFirstPoint,lPrePoint);

    //    while(lNode!=null)
    //    {
    //        zz2DPoint lNewPoint = copyOtherPolygonPoint(ref lNode, lMapPoints, mapResult);
    //        linkPoint(lPrePoint,lNewPoint);
    //        lPrePoint = lNewPoint;
    //    }
    //    linkPoint(lPrePoint,lFirstPoint);
    //}

    //public  zzSimplyPolygon()
    //{

    //}

    //public zz2DPoint copyOtherPolygonPoint(ref LinkedListNode<zz2DPoint> pNode, List<zz2DPoint> mapPoints, List<zz2DPoint> mapResult)
    //{
    //    zz2DPoint lPoint = new zz2DPoint(pNode.Value);
    //    lPoint.listNode = mAllPoints.AddLast(lPoint);
    //    pNode = pNode.Next;

    //    if (lPoint.convexAngle)
    //        mConcavePoints.AddLast(lPoint);

    //    for (int i=0;i<mapPoints.Count; ++i )
    //    {
    //        if (mapPoints[i] == pNode.Value)
    //        {
    //            mapResult.Add(mapPoints[i]);
    //            mapPoints.RemoveAt(i);
    //            break;
    //        }
    //    }
    //    return lPoint;
    //}

    /// <summary>
    /// 从其他多边形 复制点
    /// </summary>
    /// <param name="pNode"></param>
    /// <param name="copyConcave">复制凹凸的判断,并将自动检测关闭</param>
    /// <returns></returns>
    zz2DPoint addOtherPolygonPoint(ref zz2DPoint pNode, bool copyConcave)
    {
        zz2DPoint lPoint = new zz2DPoint(pNode);
        lPoint.listNode = mAllPoints.AddLast(lPoint);

        if (copyConcave)
        {
            lPoint.convexAngle = pNode.convexAngle;
            lPoint.autoCheckConvex = false;
            checkConcave(lPoint);
        }

        bounds.encapsulate(lPoint.position);

        pNode = pNode.nextPoint;

        return lPoint;
    }

    zz2DPoint addOtherPolygonPoint(ref  zz2DPoint pNode)
    {
        return addOtherPolygonPoint(ref pNode, true);
    }

    public zz2DBounds bounds;

    internal LinkedList<zz2DPoint> getAllPoints()
    {
        return mAllPoints;
    }

    internal LinkedList<zz2DPoint> getConcavePoints()
    {
        return mConcavePoints;
    }

    public Vector2[]   getShape()
    {
        if(shapePoint!=null)
            return shapePoint;

        Vector2[]   lOut = new Vector2[mAllPoints.Count];
        int i = 0;
        foreach (var lPoint in mAllPoints)
        {
            lOut[i] = lPoint.position;
            ++i;
        }
        shapePoint = lOut;
        return lOut;
    }

    Vector2[] shapePoint;

    public void setShape(Vector2[] points )
    {
        //Debug.Log(points.Length);
        if (points.Length < 3)
        {
            Debug.LogError("points.Length < 3");
            return;
        }
        shapePoint = points;

        mConvexPoints = new LinkedList<zz2DPoint>();
        mConcavePoints = new LinkedList<zz2DPoint>();
        mAllPoints = new LinkedList<zz2DPoint>();

        //第一个点
        zz2DPoint lFirstPoint = new zz2DPoint(points[0]);
        bounds = new zz2DBounds(points[0]);
        lFirstPoint.listNode = mAllPoints.AddLast(lFirstPoint);


        //第二个点
        zz2DPoint lPrePoint = addPoint(points[1], lFirstPoint);

        for (int i = 2; i < points.Length; ++i)
        {
            zz2DPoint lNewPoint = addPoint(points[i], lPrePoint);
            checkConcave(lPrePoint);
            lPrePoint = lNewPoint;
        }
        zz2DPoint lLastPoint = lPrePoint;

        linkPoint(lLastPoint, lFirstPoint);
        checkConcave(lLastPoint);
        checkConcave(lFirstPoint);
        //Debug.Log(mAllPoints.Count);
    }

    void    checkConcave(zz2DPoint point)
    {
        if (point.convexAngle)
            mConvexPoints.AddLast(point);
        else
            mConcavePoints.AddLast(point);
    }

    /// <summary>
    /// 创建新点并连接到原来的点,返回创建的新点
    /// </summary>
    /// <param name="pointPos"></param>
    /// <param name="lPrePoint"></param>
    /// <returns></returns>
    private zz2DPoint addPoint(Vector2 pointPos, zz2DPoint lPrePoint)
    {
        bounds.encapsulate(pointPos);
        zz2DPoint lPoint = new zz2DPoint(pointPos);
        lPoint.listNode = mAllPoints.AddLast(lPoint);
        //zz2DLine lLine = new zz2DLine(lPrePoint, lPoint);
        linkPoint(lPrePoint,lPoint);
        return lPoint;
    }

    void linkPoint(zz2DPoint from, zz2DPoint to)
    {
        zz2DLine lLine = new zz2DLine(from, to);
    }

    //private zz2DPoint addPointWithoutBounds(Vector2 pointPos, zz2DPoint lPrePoint)
    //{
    //    zz2DPoint lPoint = new zz2DPoint(pointPos);
    //    lPoint.listNode = mAllPoints.AddLast(lPoint);
    //    zz2DLine lLine = new zz2DLine(lPrePoint, lPoint);
    //    return lPoint;
    //}


    public bool isCrossWithLine(Vector2 lineBegin, Vector2 lineEnd)
    {
        //先判断有无和边界盒相交
        if (!bounds.intersect(lineBegin, lineEnd))
            return false;

        foreach (var lPoint in mAllPoints)
        {
            if (lPoint.nextLine.isCrossLine(lineBegin, lineEnd))
                return true;
        }
        return false;
    }


    public bool isCrossWithLine(zz2DPoint lineBegin, zz2DPoint lineEnd)
    {
        //先判断有无和边界盒相交
        if (!bounds.intersect(lineBegin.position, lineEnd.position))
            return false;

        //Debug.Log("bounds.intersect(lineBegin, lineEnd)");

        foreach (var lPoint in mAllPoints)
        {
            if (
                //不和lineBegin相连
                lPoint != lineBegin
                && lPoint.nextPoint != lineBegin

                //不和lineEnd相连
                && lPoint != lineEnd
                && lPoint.nextPoint != lineEnd

                && lPoint.nextLine.isCrossLine(lineBegin.position, lineEnd.position)
                )
                return true;
        }
        return false;
    }


    /// <summary>
    /// 判断点是否在多边形内部
    /// 参考:http://haibarali.blog.163.com/blog/static/23474013200722813356858/
    /// </summary>
    /// <param name="t"></param>
    /// <returns>1 为内,0为在边上,-1为在外部</returns>
    public int isInside(Vector2 t)
    {
        //先判断是否在边界盒内
        if (bounds.Contains(t))
        {
            int  sum, preQuadrant, nowQuadrant;
            var lNode = mAllPoints.First;
            Vector2 lPointPos = lNode.Value.position;
            lPointPos.x -= t.x;
            lPointPos.y -= t.y;
            // 计算象限
            preQuadrant = getQuadrant(lPointPos);
            //preQuadrant = lPointPos.x >= 0 ? (lPointPos.y >= 0 ? 0 : 3) : (lPointPos.y >= 0 ? 1 : 2);
            lNode = lNode.Next;
            sum = 0;
            Vector2 lPrePointPos = lPointPos;
            //for ( , i = 1 ; i <= n ; i ++ )
            while (lNode != null)
            {
                lPointPos = lNode.Value.position;
                lPointPos.x -= t.x;
                lPointPos.y -= t.y;
                if (
                    (lPointPos.x == 0 && lPointPos.y == 0)
                    || !isInsideAddNode(lPrePointPos,lPointPos, ref preQuadrant, ref sum))
                    return 0;  // 被测点为多边形顶点

                lNode = lNode.Next;
                lPrePointPos = lPointPos;
            }

            //最后一个点,到第一个点
            lPointPos = mAllPoints.First.Value.position;
            lPointPos.x -= t.x;
            lPointPos.y -= t.y;
            if (
                (lPointPos.x == 0 && lPointPos.y == 0)
                || !isInsideAddNode(lPrePointPos, lPointPos, ref preQuadrant, ref sum))
                return 0;  // 被测点为多边形顶点

            if (sum != 0)
                return 1;//printf( "Within\n" ) ;
            else
                return -1;//printf( "Outside\n" ) ;

        }
        return -1;

    }

    int     getQuadrant(Vector2 pPoint)
    {
        return pPoint.x >= 0 ? (pPoint.y >= 0 ? 0 : 3) : (pPoint.y >= 0 ? 1 : 2);
    }

    bool isInsideAddNode(Vector2 pPrePoint, Vector2 pNowPoint, ref int oQuadrant,ref int oSum)
    {
        
        // 计算叉积
        float f = pNowPoint.y * pPrePoint.x - pNowPoint.x * pPrePoint.y;
        if (f == 0 && pPrePoint.x * pNowPoint.x <= 0 && pPrePoint.y * pNowPoint.y <= 0)
            return false; //点在边上

        // 计算象限
        int nowQuadrant = getQuadrant(pNowPoint);
        if (nowQuadrant == (oQuadrant + 1) % 4)
            oSum += 1;                // 情况1:P[i+1]在P[i]的下一象限。此时弧长和加π/2；
        else if (nowQuadrant == (oQuadrant + 3) % 4)
            oSum -= 1;                // 情况2:P[i+1]在P[i]的上一象限。此时弧长和减π/2；
        else if (nowQuadrant == (oQuadrant + 2) % 4) // 情况3:P[i+1]在Pi的相对象限
        {
            if (f > 0)
                oSum += 2;
            else
                oSum -= 2;
        }
        oQuadrant = nowQuadrant;
        return true;
    }


    public bool isCrossWithLine(Vector2 lineBegin, Vector2 lineEnd,zz2DPoint exceptPoint)
    {
        //先判断有无和边界盒相交
        if (!bounds.intersect(lineBegin, lineEnd))
            return false;

        //Debug.Log("bounds.intersect(lineBegin, lineEnd)");

        foreach (var lPoint in mAllPoints)
        {
            if (
                lPoint!=exceptPoint
                && lPoint.nextPoint!=exceptPoint
                && lPoint.nextLine.isCrossLine(lineBegin, lineEnd)
                )
                return true;
        }
        return false;
    }

    //public zzSimplyPolygon[]    cut(zz2DPoint pPoint1,zz2DPoint pPoint2)
    //{
    //    if (
    //        pPoint1.listNode.List != mAllPoints
    //        || pPoint2.listNode.List != mAllPoints
    //        )
    //        Debug.LogError("error cut point");

    //    var lNewPolygonPoints = new List<zz2DPoint>(2);
    //    var lCuted = new zzSimplyPolygon(this, new zz2DPoint[] { pPoint1, pPoint2 }, lNewPolygonPoints);
    //    zz2DPoint lPoint1, lPoint2;
    //    lPoint1 = lNewPolygonPoints[0];
    //    lPoint2 = lNewPolygonPoints[1];
    //    return null;
    //}

    void addOtherPolygonPoint(zz2DPoint pBeginPoint, zz2DPoint pEndPoint,
        out zz2DPoint pFirstPoint, out zz2DPoint pLastPoint)
    {
        zz2DPoint lNode = pBeginPoint;
        pFirstPoint = addOtherPolygonPoint(ref lNode, false);
        zz2DPoint lPrePoint = pFirstPoint;

        //zz2DPoint lEndNode = pEndPoint;
        while (lNode != pEndPoint)
        {
            zz2DPoint lNewPoint = addOtherPolygonPoint(ref lNode);
            linkPoint(lPrePoint, lNewPoint);
            lPrePoint = lNewPoint;
        }

        pLastPoint = addOtherPolygonPoint(ref lNode, false);

        linkPoint(lPrePoint, pLastPoint);
    }

    public static zzSimplyPolygon  getSubPolygon(zz2DPoint pBeginPoint,zz2DPoint pEndPoint)
    {
        if (
            pBeginPoint.listNode.List != pEndPoint.listNode.List 
            )
        {
            Debug.LogError("point is not in the polygon");
            return null;
        }

        zzSimplyPolygon lOut = new zzSimplyPolygon();

        //zz2DPoint lNode = pBeginPoint;
        lOut.bounds = new zz2DBounds(pBeginPoint.position);
        zz2DPoint lFirstPoint;
        zz2DPoint lLastPoint;
        lOut.addOtherPolygonPoint(pBeginPoint, pEndPoint, out lFirstPoint, out  lLastPoint);


        lOut.linkPoint(lLastPoint, lFirstPoint);

        lOut.checkConcave(lLastPoint);
        lOut.checkConcave(lFirstPoint);

        return lOut;
    }

    public static zzSimplyPolygon combinePolygon(zz2DPoint pPolygon1Point, zz2DPoint pPolygon2Point)
    {

        zzSimplyPolygon lOut = new zzSimplyPolygon();

        lOut.bounds = new zz2DBounds(pPolygon1Point.position);
        zz2DPoint lPolygon1FirstPoint;
        zz2DPoint lPolygon1LastPoint;
        lOut.addOtherPolygonPoint(pPolygon1Point, pPolygon1Point,
            out lPolygon1FirstPoint, out  lPolygon1LastPoint);
        //zz2DPoint l1EndNode = lOut.addOtherPolygonPoint(ref lNode, false);
        //lOut.linkPoint(lPrePoint, l1EndNode);

        zz2DPoint lPolygon2FirstPoint;
        zz2DPoint lPolygon2LastPoint;
        lOut.addOtherPolygonPoint(pPolygon2Point, pPolygon2Point,
            out lPolygon2FirstPoint, out  lPolygon2LastPoint);

        lOut.linkPoint(lPolygon1LastPoint, lPolygon2FirstPoint);
        lOut.linkPoint(lPolygon2LastPoint, lPolygon1FirstPoint);

        lOut.checkConcave(lPolygon1FirstPoint);
        lOut.checkConcave(lPolygon1LastPoint);
        lOut.checkConcave(lPolygon2FirstPoint);
        lOut.checkConcave(lPolygon2LastPoint);

        return lOut;
    }

    public static zzSimplyPolygon[] cut(zz2DPoint pPoint1, zz2DPoint pPoint2)
    {
        if (pPoint1.listNode.List == pPoint2.listNode.List)
            return new zzSimplyPolygon[] { getSubPolygon(pPoint1, pPoint2), getSubPolygon(pPoint2, pPoint1) };
        else
            return new zzSimplyPolygon[] { combinePolygon(pPoint1, pPoint2) };
        return null;

    }
}
