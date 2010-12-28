using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zz2DBounds
{
    public  zz2DBounds(Vector2  point)
    {
        mMin = point;
        mMax = point;
    }

    public zz2DBounds(zz2DBounds other)
    {
        mMin = other.mMin;
        mMax = other.mMax;
    }

    //public zz2DBounds(Vector2 point1, Vector2 point2)
    //{
    //    mMin = point1;
    //    mMax = point1;
    //    encapsulate(point2);
    //}

    Vector2 mMin;

    public Vector2 min
    {
        get { return mMin; }
        set { mMin = value; }
    }
    Vector2 mMax;

    public Vector2 max
    {
        get { return mMax; }
        set { mMax = value; }
    }

    public  void encapsulate(Vector2  point)
    {
        mMax.x = Mathf.Max(mMax.x, point.x);
        mMax.y = Mathf.Max(mMax.y, point.y);

        mMin.x = Mathf.Min(mMin.x, point.x);
        mMin.y = Mathf.Min(mMin.y, point.y);
    }

    
    public bool XContains(float x)
    {
        return mMin.x <= x && x <= mMax.x;

    }

    public bool YContains(float y)
    {
        return mMin.y <= y && y <= mMax.y;
    }

    public bool Contains(Vector2 point)
    {
        //return mMin.x <= point.x
        //        && point.x <= mMax.x
        //        && mMin.y <= point.y
        //        && point.y <= mMax.y;
        return XContains(point.x) && YContains(point.y);
    }

    public bool Contains(zz2DBounds pBound)
    {
        return Contains(pBound.mMax) && Contains(pBound.mMin);
    }

    //public bool intersect( zz2DBounds otherBounds)
    //{

    //}

    public bool intersect(Vector2 pointBegin, Vector2 pointEnd)
    {
        //return Contains(pointBegin) && Contains(pointEnd);
        if (
                (//X轴投影上的判断
                XContains(pointBegin.x)
                || XContains(pointEnd.x)
                || (mMax.x - pointBegin.x) * (mMin.x - pointEnd.x) < 0//点在异测
                )
            &&
                (//Y轴投影上的判断
                YContains(pointBegin.y)
                || YContains(pointEnd.y)
                || (mMax.y - pointBegin.y) * (mMin.y - pointEnd.y) < 0//点在异测
                )
            )
            return true;
        return false;
    }

    //bool _intersectLine(Vector2 pointBegin, Vector2 pointEnd)
    //{
    //    float t1, t2, t3, t4;

    //    t1 = (pointEnd.y - pointBegin.y) * (rec.p1.x - pointBegin.x) - (pointEnd.x - pointBegin.x) * (rec.p1.y - pointBegin.y);
    //    t2 = (pointEnd.y - pointBegin.y) * (rec.p1.x - pointBegin.x) - (pointEnd.x - pointBegin.x) * (rec.p2.y - pointBegin.y);
    //    t3 = (pointEnd.y - pointBegin.y) * (rec.p2.x - pointBegin.x) - (pointEnd.x - pointBegin.x) * (rec.p1.y - pointBegin.y);
    //    t4 = (pointEnd.y - pointBegin.y) * (rec.p2.x - pointBegin.x) - (pointEnd.x - pointBegin.x) * (rec.p2.y - pointBegin.y);

    //    if (t1 > 0 && t2 > 0 && t3 > 0 && t4 > 0)
    //        return 0;
    //    if (t1 < 0 && t2 < 0 && t3 < 0 && t4 < 0)
    //        return 0;
    //    return 1;
    //}
}