
public struct zzPoint
{
    public zzPoint(int pX, int pY)
    {
        x = pX;
        y = pY;
    }

    public int x, y;

    public static zzPoint zero = new zzPoint(0, 0);

    public static zzPoint operator +(zzPoint p1, zzPoint p2)
    {
        zzPoint lPoint = new zzPoint(p1.x + p2.x, p1.y + p2.y);
        return lPoint;
    }

    public static zzPoint operator -(zzPoint p1, zzPoint p2)
    {
        zzPoint lPoint = new zzPoint(p1.x - p2.x, p1.y - p2.y);
        return lPoint;
    }

    public static zzPoint operator -(zzPoint p1)
    {
        return new zzPoint(-p1.x, -p1.y);
    }

    public override string ToString()
    {
        return x.ToString() + " " + y;
    }

    public static bool operator ==(zzPoint pLeft, zzPoint pRight)
    {
        return pLeft.x == pRight.x && pLeft.y == pRight.y;
    }

    public static bool operator !=(zzPoint pLeft, zzPoint pRight)
    {
        return !(pLeft == pRight);
    }

}