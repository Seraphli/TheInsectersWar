using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class zzPolygon
{
    List<zz2DPoint> mPoints = new List<zz2DPoint>();

    public List<zz2DPoint> clonePoint()
    {
        return new List<zz2DPoint>(mPoints);
    }

    List<zz2DLine> mLines = new List<zz2DLine>();

    public List<zz2DLine> cloneLine()
    {
        return new List<zz2DLine>(mLines);
    }
    public void addShape(zzPainterPoint[] points)
    {
        if (points.Length < 3)
            return;
        zz2DPoint lFirstPoint = new zz2DPoint(points[0]);
        mPoints.Add(lFirstPoint);
        zz2DPoint lPrePoint = addPoint(points[1], lFirstPoint);
        for (int i = 2; i < points.Length; ++i)
        {
            zz2DPoint lNewPoint = addPoint(points[i], lPrePoint);

            //若新的点在之前的线右边,则之前的角为凸角;
            //zz2DPoint.LeftOrRight(
            //lPrePoint.previousPoint.position,
            //lPrePoint.position,
            //lNewPoint.position) < 0;//lPrePoint's next point is lNewPoint

            lPrePoint = lNewPoint;
        }
        zz2DPoint lLastPoint = lPrePoint;

        zz2DLine lLastLine = new zz2DLine(lPrePoint, lFirstPoint);
        mLines.Add(lLastLine);

        //lLastPoint.convexAngle = zz2DPoint.LeftOrRight(
        //    lLastPoint.previousPoint.position,
        //    lLastPoint.position,
        //    lLastPoint.nextPoint.position) < 0;

        //lFirstPoint.convexAngle = zz2DPoint.LeftOrRight(
        //    lFirstPoint.previousPoint.position,
        //    lFirstPoint.position,
        //    lFirstPoint.nextPoint.position) < 0;

        //sortPoint(mPoints);
    }

    /// <summary>
    /// 顺时针围起来的是正
    /// </summary>
    /// <param name="points"></param>
    public void    addShape(Vector2[] points)
    {
        if (points.Length < 3)
            return;
        zz2DPoint lFirstPoint = new zz2DPoint(points[0]);
        mPoints.Add(lFirstPoint);
        zz2DPoint lPrePoint = addPoint(points[1],lFirstPoint);
        for(int i=2;i<points.Length;++i)
        {
            zz2DPoint lNewPoint =  addPoint(points[i], lPrePoint);

            //若新的点在之前的线右边,则之前的角为凸角;
                //zz2DPoint.LeftOrRight(
                //lPrePoint.previousPoint.position,
                //lPrePoint.position,
                //lNewPoint.position) < 0;//lPrePoint's next point is lNewPoint

            lPrePoint = lNewPoint;
        }
        zz2DPoint lLastPoint = lPrePoint;

        zz2DLine lLastLine = new zz2DLine(lPrePoint, lFirstPoint);
        mLines.Add(lLastLine);

        //lLastPoint.convexAngle = zz2DPoint.LeftOrRight(
        //    lLastPoint.previousPoint.position,
        //    lLastPoint.position,
        //    lLastPoint.nextPoint.position) < 0;

        //lFirstPoint.convexAngle = zz2DPoint.LeftOrRight(
        //    lFirstPoint.previousPoint.position,
        //    lFirstPoint.position,
        //    lFirstPoint.nextPoint.position) < 0;

        //sortPoint(mPoints);
    }

    void triangulating()
    {
        var lPointStorage = new List<zz2DPoint>();
        //var lLineStorage = new Dictionary<zz2DLine, bool>();
        var lSideStorage = new HashSet<zz2DLine>();

        var lDiagonals = new List<zz2DLine>();

        lPointStorage.Add(mPoints[0]);
        lSideStorage.Add(mPoints[0].previousLine);
        lSideStorage.Add(mPoints[0].nextLine);
        mPoints[0].used = true;
        lPointStorage.Add(mPoints[1]);
        mPoints[1].used = true;

        for (int i = 2; i < mPoints.Count; ++i)
        {
            zz2DLine lPointLeftSide, lPointRightSide;
            mPoints[i].used = true;
            foreach (var lStoragedPoint in lPointStorage)
            {
            }
        }
    }

    ///// <summary>
    ///// Y扫描线 扫过 点时
    ///// </summary>
    ///// <param name="lPoint"></param>
    ///// <param name="pSideStorage"></param>
    //static void YSweepEnter(zz2DPoint lPoint, HashSet<zz2DLine> pSideStorage)
    //{
    //    pSideStorage
    //}

    /// <summary>
    /// 创建新点并连接到原来的点,返回创建的新点
    /// </summary>
    /// <param name="pointPos"></param>
    /// <param name="lPrePoint"></param>
    /// <returns></returns>
    private zz2DPoint addPoint(Vector2 pointPos, zz2DPoint lPrePoint)
    {
        zz2DPoint lPoint = new zz2DPoint(pointPos);
        mPoints.Add(lPoint);
        zz2DLine lLine = new zz2DLine(lPrePoint, lPoint);
        mLines.Add(lLine);
        lPrePoint = lPoint;
        return lPoint;
    }

    private zz2DPoint addPoint(zzPainterPoint pointPos, zz2DPoint lPrePoint)
    {
        zz2DPoint lPoint = new zz2DPoint(pointPos);
        mPoints.Add(lPoint);
        zz2DLine lLine = new zz2DLine(lPrePoint, lPoint);
        mLines.Add(lLine);
        lPrePoint = lPoint;
        return lPoint;
    }


    //private zz2DPoint addPoint(zzPainterPoint pointPos, zz2DPoint lPrePoint)
    //{
    //    zz2DPoint lPoint = new zz2DPoint(pointPos);
    //    mPoints.Add(lPoint);
    //    zz2DLine lLine = new zz2DLine(lPrePoint, lPoint);
    //    mLines.Add(lLine);
    //    lPrePoint = lPoint;
    //    return lPoint;
    //}


    static void toConvex(List<zz2DPoint> points)
    {
        int lShapeBlockIndex = 0;
        var lPointToBlock = new Dictionary<zz2DPoint, int>();
        //lPointToBlock[points[0]] = lShapeBlockIndex++;
        foreach (var lPoint in points)
        {
            if (!lPointToBlock.ContainsKey(lPoint.nextPoint)
                && !lPointToBlock.ContainsKey(lPoint.previousPoint))
                lPointToBlock[lPoint] = lShapeBlockIndex++;
        }
    }

    public void testOneLineShape()
    {
        Debug.Log("point list");
        foreach (zz2DPoint lPoint in mPoints)
        {
            Debug.Log("point:" + lPoint.position
                + " pre:" + lPoint.previousPoint.position
                + " next:" + lPoint.nextPoint.position);
        }

        Debug.Log("line list");
        foreach (zz2DLine lLine in mLines)
        {
            Debug.Log("from:" + lLine.from.position + " to" + lLine.to.position);
        }
    }
}
