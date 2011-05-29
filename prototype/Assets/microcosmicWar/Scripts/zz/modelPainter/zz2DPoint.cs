using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

//class zz2DPointBehaviour:MonoBehaviour
//{
//    public zz2DPoint point;
//}

[System.Serializable]
public class zz2DPoint
{
    public zz2DPoint(zz2DPoint other)
    {
        mPosition = other.mPosition;
        //convexAngle = other.convexAngle;
        autoCheckConvex = true;
    }

    public zz2DPoint(Vector2 pPosition)
    {
        mPosition = pPosition;
        used = false;
        mDiagonals = new List<zz2DLine>();
        autoCheckConvex = true;
    }

    public zz2DPoint(zzPainterPoint painterPoint)
    {
        autoCheckConvex = true;
        Vector3 lPos = painterPoint.transform.position;
        mPosition.x = lPos.x;
        mPosition.y = lPos.y;
        used = false;
        mDiagonals = new List<zz2DLine>();

        painterPoint.pointInfo = this;
    }

    Vector2 mPosition;
    public Vector2 position
    {
        get { return mPosition; }
        //set { mPosition = value; }
    }

    //zz2DNeighbourPointInfo mNextPoint;
    //public zz2DNeighbourPointInfo nextPoint
    //{
    //    get { return mNextPoint; }
    //    //set { mNeighbourPointInfo = value; }
    //}

    //void setNextPoint(zz2DNeighbourPointInfo pNextPoint)
    //{
    //    mNextPoint = pNextPoint;
    //}



    //zz2DNeighbourPointInfo mPreviousPoint;
    //public zz2DNeighbourPointInfo previousPoint
    //{
    //    get { return mPreviousPoint; }
    //    //set { mNeighbourPointInfo = value; }
    //}

    //void setNeighbourPointInfo(zz2DNeighbourPointInfo pPreviousPoint)
    //{
    //    mPreviousPoint = pPreviousPoint;
    //}
    [SerializeField]
    zz2DLine mNextLine;
    public zz2DLine nextLine
    {
        get { return mNextLine; }
    }

    internal void setNextLine(zz2DLine value)
    {
        mNextLine = value;
        if ( autoCheckConvex && mPreviousLine != null)
            convexAngle = isConvexPoint(previousPoint.position, position, nextPoint.position);
    }

    public bool autoCheckConvex = true;

    [SerializeField]
    zz2DLine mPreviousLine;
    public zz2DLine previousLine
    {
        get { return mPreviousLine; }
    }

    internal void setPreviousLine(zz2DLine value)
    {
        mPreviousLine = value;
        if (mNextLine != null)
            convexAngle = isConvexPoint(previousPoint.position,position,nextPoint.position);
    }

    public static bool isConvexPoint(Vector3 prePoint,Vector3 point,Vector3 nextPoint)
    {
        return LeftOrRight(
                prePoint,
                point,
                nextPoint)<0;
    }

    public zz2DPoint nextPoint
    {
        get
        {
            if (mNextLine == null)
                return null;
            return mNextLine.to;
        }
    }

    public zz2DPoint previousPoint
    {
        get
        {
            if (mPreviousLine == null)
                return null;
            return mPreviousLine.from;
        }
    }

    List<zz2DLine> mDiagonals;

    internal void addDiagonal(zz2DLine pDiagonal)
    {
        mDiagonals.Add(pDiagonal);
    }

    ReadOnlyCollection<zz2DLine> getDiagonals()
    {
        return mDiagonals.AsReadOnly();
    }

    public zz2DPoint getDiagonalPoint(int index)
    {
        zz2DLine lDiagonal = mDiagonals[index];
        return lDiagonal.from == this ? lDiagonal.to : lDiagonal.from;
    }
	
	public bool isDiagonalPoint(zz2DPoint point)
	{
		for(int i=0;i<mDiagonals.Count;++i)
		{
			if(getDiagonalPoint(i)==point)
				return true;
		}
		return false;
	}

    public int diagonalNum
    {
        get { return mDiagonals.Count; }
    }

    /// <summary>
    /// 参数点是否位于此点两边的正面
    /// </summary>
    /// <param name="pOtherPoint"></param>
    /// <returns></returns>
    public bool isInPositiveSide(zz2DPoint pOtherPoint)
    {
        if(convexAngle)
        {
            return previousLine.isInPositiveSide(pOtherPoint) 
                && nextLine.isInPositiveSide(pOtherPoint) ;
        }

        return previousLine.isInPositiveSide(pOtherPoint) 
            || nextLine.isInPositiveSide(pOtherPoint) ;
    }

    /// <summary>
    /// 两点是否都在对方的正面
    /// </summary>
    /// <param name="pOtherPoint"></param>
    /// <returns></returns>
    public bool isEachInPositiveSide(zz2DPoint pOtherPoint)
    {
        return this.isInPositiveSide(pOtherPoint) && pOtherPoint.isInPositiveSide(this);
    }

    public bool isLinked(zz2DPoint pOtherPoint)
    {
        return nextPoint == pOtherPoint || previousPoint == pOtherPoint;
    }

    /// <summary>
    /// 得到点的左右边,分割靠Y轴
    /// </summary>
    /// <param name="side1"></param>
    /// <param name="side2"></param>
    /// <returns>小于0 则边都在点左侧,大于0 则都在点右侧,等于0，则两边分部 或者有个在垂直方向上</returns>
    public int getLeftRightSideInX( out zz2DLine side1, out zz2DLine side2)
    {
        side1 = previousLine;
        side2 = nextLine;
        float x = position.x;
        if (previousPoint.position.x < x)
        {
            if (nextPoint.position.x < x)
                return -1;
            else
            {
                //side1 = PreviousLine;
                //side2 = nextLine;
                return 0;
            }
        }
        else
        {
            if (nextPoint.position.x > x)
                return 1;
            else
            {
                side2 = previousLine;
                side1 = nextLine;
                return 0;
            }
        }
    }

    //public static bool isLineSegmentCross(Vector2 pFirst1, Vector2 pFirst2, Vector2 pSecond1, Vector2 pSecond2)
    //{
    //    //每个线段的两点都在另一个线段的左右不同侧，则能断定线段相交 
    //    //公式对于向量(x1,y1)->(x2,y2),判断点(x3,y3)在向量的左边,右边,还是线上. 
    //    //p=x1(y3-y2)+x2(y1-y3)+x3(y2-y1).p<0 左侧, p=0 线上, p>0 右侧 
    //    double Linep1, Linep2;
    //    //判断pSecond1和pSecond2是否在pFirst1->pFirst2两侧 
    //    Linep1 = pFirst1.x * (pSecond1.y - pFirst2.y) +
    //     pFirst2.x * (pFirst1.y - pSecond1.y) +
    //     pSecond1.x * (pFirst2.y - pFirst1.y);
    //    Linep2 = pFirst1.x * (pSecond2.y - pFirst2.y) +
    //     pFirst2.x * (pFirst1.y - pSecond2.y) +
    //     pSecond2.x * (pFirst2.y - pFirst1.y);
    //    if (((Linep1 ^ Linep2) >= 0) && !(Linep1 == 0 && Linep2 == 0))//符号位异或为0:pSecond1和pSecond2在pFirst1->pFirst2同侧 
    //    {
    //        return false;
    //    }
    //    //判断pFirst1和pFirst2是否在pSecond1->pSecond2两侧 
    //    Linep1 = pSecond1.x * (pFirst1.y - pSecond2.y) +
    //     pSecond2.x * (pSecond1.y - pFirst1.y) +
    //     pFirst1.x * (pSecond2.y - pSecond1.y);
    //    Linep2 = pSecond1.x * (pFirst2.y - pSecond2.y) +
    //     pSecond2.x * (pSecond1.y - pFirst2.y) +
    //     pFirst2.x * (pSecond2.y - pSecond1.y);
    //    if (((Linep1 ^ Linep2) >= 0) && !(Linep1 == 0 && Linep2 == 0))//符号位异或为0:pFirst1和pFirst2在pSecond1->pSecond2同侧 
    //    {
    //        return false;
    //    }
    //    //否则判为相交 
    //    return true;
    //} 
    static float mult(Vector2 p1, Vector2 p2, Vector2 p0)
    {
        return ((p1.x - p0.x) * (p2.y - p0.y) - (p2.x - p0.x) * (p1.y - p0.y));
    }

    //确定两条线段是否相交
    public static bool isLineSegmentCross(Vector2 lLine1Start, Vector2 lLine1End, Vector2 lLine2Start, Vector2 lLine2End)
    {
        return Mathf.Max(lLine1Start.x, lLine1End.x) >= Mathf.Min(lLine2Start.x, lLine2End.x)
            && Mathf.Max(lLine1Start.y, lLine1End.y) >= Mathf.Min(lLine2Start.y, lLine2End.y)
            && Mathf.Max(lLine2Start.x, lLine2End.x) >= Mathf.Min(lLine1Start.x, lLine1End.x)
            && Mathf.Max(lLine2Start.y, lLine2End.y) >= Mathf.Min(lLine1Start.y, lLine1End.y)
            && mult(lLine2Start, lLine1End, lLine1Start) * mult(lLine1End, lLine2End, lLine1Start) >= 0
            && mult(lLine1Start, lLine2End, lLine2Start) * mult(lLine2End, lLine1End, lLine2Start) >= 0;
    } 
    /// <summary>
    /// 判断点c在线段ab的左边还是右边，如果返回值大于0在左边，如果小于0在右边，否则共线 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    static public float LeftOrRight(Vector3 a, Vector3 b, Vector3 c)
    {
        a.x -= c.x; a.y -= c.y;
        b.x -= c.x; b.y -= c.y;
        return a.x * b.y - a.y * b.x;
    }


    //是否<180
    public bool convexAngle;

    public bool used = false;

    public bool haveDiagonal
    {
        get { return mDiagonals.Count > 0; }
    }

    //点在多边形中的节点
    internal LinkedListNode<zz2DPoint> listNode;

    internal int pointType=0;
    
}

