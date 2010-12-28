using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class zzOutlineSweeper
//{
//    UnityEngine.Texture2D picture;
//    //int mWidth;
//    //int mHeight;
//    PiexlData[,] mPiexlDatas;

//    class  PiexlData
//    {
//        int polygonID;
//    }

//    void    setPicture(UnityEngine.Texture2D pPicture)
//    {
//        picture = pPicture;
//        mPiexlDatas = new PiexlData[lHeight, lWidth];
//    }

//    public Vector2[] Sweeper()
//    {
//        int lWidth = picture.width;
//        int lHeight = picture.height;

//        for (int lHeightIndex = 0; lHeightIndex < lHeight; ++lHeightIndex)
//        {
//            bool lPreActive = false;
//            for (int lWidthIndex = 0; lWidthIndex < lWidth; ++lWidthIndex)
//            {
//                bool lActive = isActivePiexl(pPicture.GetPixel(lWidthIndex,lHeightIndex));
//                if (!lPreActive && lActive)
//                {
//                    if(mPiexlDatas[lHeightIndex,lWidthIndex]!=null)
//                    {

//                    }
//                }
//                lPreActive = lActive;
//            }
//        }

//    }

//    public Vector2[]   sweeperOutline(int x,int y,int ID)
//    {

//    }

//    public  Vector2 UVToPostion(int x,int y)
//    {
//        return new Vector2(x,-y);
//    }

//    public Vector2[] Sweeper(UnityEngine.Texture2D pPicture)
//    {
//        int lWidth = pPicture.width;
//        int lHeight = pPicture.height;

//        return null;
//    }

//    public  bool    isActivePiexl(UnityEngine.Color pColor)
//    {
//        return pColor.a != 0;
//    }
//}
public struct zzPoint
{
    public zzPoint(int pX,int pY)
    {
        x = pX;
        y = pY;
    }

    public int x, y;

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

    public override string ToString()
    {
        return x.ToString()+" "+y;
    }
	
	public static bool operator==(zzPoint pLeft,zzPoint pRight)
	{
		return pLeft.x == pRight.x && pLeft.y == pRight.y;
	}
	
	public static bool operator!=(zzPoint pLeft,zzPoint pRight)
	{
		return !(pLeft==pRight);
	}

}

class zzOutlineSweeper
{
    public class ActiveChart
    {
        //public ActiveChart(Texture2D pTex)
        //{

        //}

        public ActiveChart(int pWidth, int pHeight)
        {
            _width = pWidth;
            _height = pHeight;
            mImage = new bool[pWidth, pHeight];
        }

        bool[,]    mImage;

        int _width;
        public int width
        {
            get { return _width; }
        }

        int _height;
        public int height
        {
            get { return _height; }
        }

        public void setActive(zzPoint pPoint, bool pActive)
        {
            setActive(pPoint.x, pPoint.y, pActive);
        }

        public void setActive(int x,int y,bool pActive)
        {
            mImage[x, y] = pActive;
        }

        public bool isActive(zzPoint pPoint)
        {
            return isActive(pPoint.x, pPoint.y);
        }

        public bool isActive(int x,int y)
        {
            return mImage[x, y];
        }

        public bool isInside(zzPoint pPoint)
        {
            return isInside(pPoint.x, pPoint.y);
        }

        public bool isInside(int x, int y)
        {
            return x >= 0
                    && x < width
                    && y >= 0
                    && y < height;
        }

        public Texture2D asTexture()
        {
            Texture2D lOut = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
            for (int x = 0; x < _width;++x )
            {
                for(int y=0;y<_height;++y)
                {
                    if (isActive(x,y))
                    {
                        lOut.SetPixel(x,y,Color.black);
                    }
                    else
                    {
                        lOut.SetPixel(x, y, Color.clear);
                    }
                }
            }
            lOut.Apply();
            return lOut;
        }
    }

    class DetectDirection
    {
        public DetectDirection()
        {
            mDirectionIndex = 0;
        }

        DetectDirection(int pDirectionIndex)
        {
            mDirectionIndex = pDirectionIndex;
        }

        public int x
        {
            get { return Direction[mDirectionIndex,0]; }
            //set { mX = value; }
        }

        public int y
        {
            get { return Direction[mDirectionIndex,1]; }
            //set { mY = value; }
        }

        int mDirectionIndex = 0;

        static readonly int[,] Direction = new int[8, 2] { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };

        public DetectDirection    nextDirection()
        {
            int lDirectionIndex = mDirectionIndex + 1;
            if (lDirectionIndex == 8)
                lDirectionIndex = 0;
            return new DetectDirection(lDirectionIndex);
        }

        public void    toNextDirection()
        {
            ++mDirectionIndex;
            if (mDirectionIndex == 8)
                mDirectionIndex = 0;
        }

        public void    toPreDirection()
        {
            --mDirectionIndex;
            if (mDirectionIndex == -1)
                mDirectionIndex = 7;
        }
		
		public zzPoint	getOfsetPoint(zzPoint pPoint)
		{
			return new zzPoint(pPoint.x+x,pPoint.y+y);
		}
    }

    public static void removeIsolatedPoint(ActiveChart tex)
    {
        for (int y = 0; y < tex.height; ++y)
        {
            for (int x = 0; x < tex.width; ++x)
            {
                if (tex.isActive(x,y)&&isIsolatedPoint(new zzPoint(x, y), tex))
                {
                    tex.setActive(x, y, false);
                }
            }
        }
    }

    public class SweeperResult
    {
        public Vector2[] edge;
        public List<Vector2[]> holes = new List<Vector2[]>();
    }

    //public static List<Vector2[]> getEdge(ActiveChart tex, float ignoreDistance)
    //{
    //    List<Vector2[]> lOut = new List<Vector2[]>();
    //    bool[,] lProgressdMark = new bool[tex.width, tex.height];
    //    //removeIsolatedPoint(tex);
    //    for (int y = 0; y < tex.height;++y )
    //    {
    //        bool lPreIsActive = false;
    //        for(int x=0;x<tex.width;++x)
    //        {
    //            //bool lNowIsActive = isActivePiexl(tex.GetPixel(x, y));
    //            bool lNowIsActive = tex.isActive(x, y);
    //            if (!lPreIsActive && lNowIsActive && !lProgressdMark[x,y]
    //                //&& !isIsolatedPoint(new zzPoint(x, y), tex)
    //                )
    //            {
    //                lOut.Add(
    //                    getFloatSimplifyLine(beginEdge(tex, x, y, lProgressdMark),ignoreDistance)
    //                    );
    //            }

    //            lPreIsActive = lNowIsActive;
    //        }
    //    }
    //    return lOut;
    //}

    public static List<SweeperResult> sweeper(ActiveChart tex, float ignoreDistance)
    {
        List<SweeperResult> lOut = new List<SweeperResult>();
        int[,] lProgressdMark = new int[tex.width, tex.height];
        //removeIsolatedPoint(tex);
        int lPolygonMark = 1;
        for (int y = 0; y < tex.height;++y )
        {
            bool lPreIsActive = false;
            int lPreMark = 0;
            for(int x=0;x<tex.width;++x)
            {
                //bool lNowIsActive = isActivePiexl(tex.GetPixel(x, y));
                bool lNowIsActive = tex.isActive(x, y);
                if (!lPreIsActive && lNowIsActive && lProgressdMark[x,y]==0)
                {
                    //为外边
                    SweeperResult lSweeperResult = new SweeperResult();
                    lSweeperResult.edge
                        = getFloatSimplifyLine(
                            beginEdge(tex, x, y, lProgressdMark, lPolygonMark++), ignoreDistance
                            );
                    lOut.Add(lSweeperResult);
                        
                }
                else if( lPreIsActive && !lNowIsActive && lProgressdMark[x-1, y] == 0)
                {
                    //为孔
                    lOut[lPreMark - 1].holes.Add(
                        getFloatSimplifyLine(
                            beginEdge(tex, x-1, y, lProgressdMark, lPreMark), ignoreDistance
                            )
                        );
                }

                if (lProgressdMark[x, y] != 0)
                    lPreMark = lProgressdMark[x, y];

                lPreIsActive = lNowIsActive;
            }
        }
        return lOut;
    }

    public static bool isIsolatedPoint(zzPoint pPoint,ActiveChart tex)
    {
        var lDetectDirection = new DetectDirection();
		
		var lNextPoint = lDetectDirection.getOfsetPoint(pPoint);
        //lDetectDirection.toNextDirection();
        //var isNextActivePiexl = isActivePiexl(tex.GetPixel(lNextPoint.x, lNextPoint.y));
        var isNextActivePiexl = tex.isActive(pPoint);
		
		var lNextNextPoint = new zzPoint();// = lDetectDirection.getOfsetPoint(pPoint);
		bool isNextNextActivePiexl;// = isActivePiexl(tex.GetPixel(lNextNextPoint.x, lNextNextPoint.y));
		
        for (int i = 0; i < 8;++i )
        {
            lDetectDirection.toNextDirection();
			lNextNextPoint = lDetectDirection.getOfsetPoint(pPoint);
            //isNextNextActivePiexl = isActivePiexl(tex.GetPixel(lNextNextPoint.x, lNextNextPoint.y));
            isNextNextActivePiexl = tex.isInside(lNextNextPoint)&&tex.isActive(lNextNextPoint);
			
			if(isNextActivePiexl && isNextNextActivePiexl)
				return false;
			isNextActivePiexl = isNextNextActivePiexl;
			
        }
        return true;
    }

    static Vector2[] getFloatSimplifyLine(zzPoint[] pLine, float ignoreDistance)
    {
        pLine = simplifyCircleLine(pLine, ignoreDistance);
        Vector2[]   lOut = new Vector2[pLine.Length];
        for (int i = 0; i < pLine.Length;++i )
        {
            lOut[pLine.Length-i-1] = new Vector2(pLine[i].x, pLine[i].y);
        }
        return lOut;
    }

    static zzPoint[] simplifyCircleLine(zzPoint[] pLine, float ignoreDistance)
    {
        List<zzPoint> lOut = new List<zzPoint>();
        lOut.Capacity = pLine.Length/2;
        List<zzPoint> lStraightLine = new List<zzPoint>();
        lStraightLine.Capacity = pLine.Length/2;

        lOut.Add(pLine[0]);
        lStraightLine.Add(pLine[0]);
        lStraightLine.Add(pLine[1]);
        for (int i = 2; i < pLine.Length;++i )
        {
            //zzPoint lPoint = pLine[i];
            //if (lStraightLine.Count == 1)
            //{
            //    lStraightLine.Add(lPoint);
            //}
            //else
            //{
            zzPoint lPoint = pLine[i];
            lStraightLine.Add(lPoint);
            if (!isStraight(lStraightLine, ignoreDistance))
            {
                //Debug.Log(lStraightLine.Count);
                zzPoint lEndPoint = lStraightLine[lStraightLine.Count - 2];
                lOut.Add(lEndPoint);
                lStraightLine.Clear();
                lStraightLine.Add(lEndPoint);
                lStraightLine.Add(lPoint);
            }
        }

        lStraightLine.Add(pLine[pLine.Length - 1]);
        if (!isStraight(lStraightLine, ignoreDistance))
            lOut.Add(pLine[pLine.Length - 1]);

        return lOut.ToArray();
    }

    static bool isStraight(List<zzPoint> pLine, float maxDistance)
    {
        zzPoint firstPoint = pLine[0];
        zzPoint lastPoint = pLine[pLine.Count-1];

        for (int i = 1; i < pLine.Count - 1;++i )
        {
            //if (sqrDistanceLine(firstPoint, lastPoint, pLine[i]) > 3.0f)
            if (distance(firstPoint, lastPoint, pLine[i]) > maxDistance)
            {
                //Debug.Log(i);
                return false;
            }
        }
        return true;
    }

    static zzPoint[] beginEdge(ActiveChart tex, int beginX, int beginY, int[,] pProgressdMark, int pMark)
	{
        return beginEdge(tex, new zzPoint(beginX, beginY), pProgressdMark, pMark);
	}
	
    //static bool isInsideImg(zzPoint pPoint,Texture2D tex)
    //{
    //    return pPoint.x >= 0
    //            && pPoint.x < tex.width
    //            && pPoint.y >= 0
    //            && pPoint.y < tex.height;
    //}

    static zzPoint[] beginEdge(ActiveChart tex, zzPoint beginPosition, int[,] pProgressdMark, int pMark)
    {
        List<zzPoint> lOut = new List<zzPoint>();
        lOut.Add(beginPosition);
        pProgressdMark[beginPosition.x, beginPosition.y] = pMark;
        DetectDirection lDetectDirection = new DetectDirection();
        //int nowX=beginX,nowY=beginY;
		zzPoint lNowPoint = beginPosition;
        //int lNextX,lNextY;
		zzPoint lNextPoint;
		int lTotalNum = tex.width*tex.height;
        while(true)
        {
			
			lNextPoint = lDetectDirection.getOfsetPoint(lNowPoint);
            var lNextDirection = lDetectDirection.nextDirection();
			var lNextNextPoint = lNextDirection.getOfsetPoint(lNowPoint);

            if (tex.isInside(lNextPoint) && tex.isActive(lNextPoint))
            {
				
				lNowPoint = lNextPoint;

                if (lNowPoint == beginPosition)
                    break;
                pProgressdMark[lNowPoint.x, lNowPoint.y] = pMark;
                lOut.Add(lNowPoint);
				lDetectDirection.toPreDirection();
				lDetectDirection.toPreDirection();
            }
            else
            {
                //不可用,继续顺时针寻找
                lDetectDirection.toNextDirection();
            }
			
			if(lOut.Count>lTotalNum)
			{
				Debug.LogError("error ");
				break;
			}
        }

        return lOut.ToArray();
    }

    static bool isActivePiexl(UnityEngine.Color pColor)
    {
        return pColor.a != 0;
    }


    static float sqrDistance(zzPoint p1, zzPoint p2)     //   返回两点之间的距离平方
    {
        return (float)((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
    }
    static float distance(zzPoint p1, zzPoint p2) 
    {
        return Mathf.Sqrt(sqrDistance(p1, p2));
    }

    //   a和b是线段的两个端点，   c是检测点 
    //static float sqrDistanceLine(zzPoint a, zzPoint b, zzPoint c)     //   a和b是线段的两个端点，   c是检测点
    //{

    //    zzPoint ab = b - a;

    //    zzPoint ac = c - a;

    //    int f = ab.x * ac.x + ab.y * ac.y;

    //    if (f < 0)
    //    {
    //        //Debug.LogError("f < 0 ");
    //        //Debug.LogError(""+a+"; "+b+"; "+c);
    //        return sqrDistance(a, c);
    //    }

    //    int d = ab.x * ab.x + ab.y + ab.y;

    //    if (f > d)
    //    {
    //        //Debug.LogError(" f > 0 ");
    //        //Debug.LogError("" + a + "; " + b + "; " + c);
    //        return sqrDistance(b, c);
    //    }

    //    f /= d;

    //    zzPoint D;
    //    D.x = a.x + f * ab.x;       //   c在ab线段上的投影点
    //    D.y = a.y + f * ab.y;
    //    return sqrDistance(a, D);

    //} 

    static float distance(zzPoint PA, zzPoint PB, zzPoint P3)
    {

        float a, b, c;
        a = distance(PB, P3);
        if (a <= 0.00001)
            return 0.0f;
        b = distance(PA, P3);
        if (b <= 0.00001)
            return 0.0f;
        c = distance(PA, PB);
        if (c <= 0.00001)
            return a;//如果PA和PB坐标相同，则退出函数，并返回距离  
        //------------------------------  

        if (a * a >= b * b + c * c)//--------图3--------  
            return b;      //如果是钝角返回b  
        if (b * b >= a * a + c * c)//--------图4-------  
            return a;      //如果是钝角返回a  

        //图1  
        float l = (a + b + c) / 2;     //周长的一半  
        float s = Mathf.Sqrt(l * (l - a) * (l - b) * (l - c));  //海伦公式求面积，也可以用矢量求  
        return 2 * s / c;
    }  

}