using UnityEngine;
using System.Collections;

[System.Serializable]
public class zz2DLine
{
    public zz2DLine(zz2DPoint pFrom, zz2DPoint pTo)
    {
        mFrom = pFrom;
        mTo = pTo;
        pFrom.setNextLine(this);
        pTo.setPreviousLine(this);
    }
    public zz2DLine()
    {

    }

    public void    asExtraLine(zz2DPoint pFrom, zz2DPoint pTo)
    {
        mFrom = pFrom;
        mTo = pTo;
    }

    zz2DPoint mFrom;
    public zz2DPoint from
    {
        get { return mFrom; }
        //set { mFrom = value; }
    }

    zz2DPoint mTo;
    public zz2DPoint to
    {
        get { return mTo; }
        //set { mTo = value; }
    }

    public bool isInLine(zz2DPoint pPoint1, zz2DPoint pPoint2)
    {
        if (from == pPoint1
            || from == pPoint2
            || to == pPoint1
            || to == pPoint2)
            return true;
        return false;
    }

    //public bool isCrossLine(zz2DLine otherLine)
    //{
    //    return zz2DPoint.isLineSegmentCross(from.position, to.position,
    //        otherLine.from.position, otherLine.to.position);
    //}

    /// <summary>
    /// 是否与两点组成的直线相交
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns></returns>
    public bool isCrossLine(zz2DPoint point1, zz2DPoint point2)
    {
        return isCrossLine(point1.position, point2.position);
    }

    /// <summary>
    /// 是否与两点组成的直线相交
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns></returns>
    public bool isCrossLine(Vector2 point1, Vector2 point2)
    {
        return zz2DPoint.isLineSegmentCross(from.position, to.position,
            point1, point2);
    }

    //public float LeftOrRight(zz2DPoint point)
    //{
    //    return zz2DPoint.LeftOrRight(from.position, to.position, point.position);
    //}

    public bool isInPositiveSide(zz2DPoint point)
    {
        return zz2DPoint.isConvexPoint(from.position, to.position, point.position);
    }
}