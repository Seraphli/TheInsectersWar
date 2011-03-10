using UnityEngine;

public class zzPointBounds
{
    public zzPointBounds(zzPoint point)
    {
        mMin = point;
        mMax = point;
    }

    public zzPointBounds(zzPoint[] points)
    {
        mMin = points[0];
        mMax = points[0];
        for (int i = 1; i < points.Length;++i )
        {
            encapsulate(points[i]);
        }
    }

    public zzPointBounds(zzPointBounds other)
    {
        mMin = other.mMin;
        mMax = other.mMax;
    }

    zzPoint mMin;

    public zzPoint min
    {
        get { return mMin; }
        set { mMin = value; }
    }
    zzPoint mMax;

    public zzPoint max
    {
        get { return mMax; }
        set { mMax = value; }
    }

    public void encapsulate(zzPoint point)
    {
        mMax.x = Mathf.Max(mMax.x, point.x);
        mMax.y = Mathf.Max(mMax.y, point.y);

        mMin.x = Mathf.Min(mMin.x, point.x);
        mMin.y = Mathf.Min(mMin.y, point.y);
    }
}
