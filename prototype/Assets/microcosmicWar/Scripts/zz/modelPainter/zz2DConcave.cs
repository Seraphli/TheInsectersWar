using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class zz2DConcave
{
    class PointDate
    {
        //0为外边,polygonId-1为孔索引
        public int polygonId = 0;
        public zz2DPoint point;
    }

    /// <summary>
    /// 多边形内,点遍历
    /// </summary>
    class PointIterator
    {
        List<LinkedList<zz2DPoint>> polygons = new List<LinkedList<zz2DPoint>>();

        int polygonIndex = -1;

        //提供下一个数据
        LinkedListNode<zz2DPoint> mNowNode;

        public void addPolygon(LinkedList<zz2DPoint> pPolygon)
        {
            polygons.Add(pPolygon);
            findFistNode(pPolygon);
        }

        //public void addPolygon(List<LinkedList<zz2DPoint>> pPolygon)
        //{
        //    polygons.AddRange(pPolygon);
        //    findFistNode();
        //}

        void findFistNode(LinkedList<zz2DPoint> pPolygon)
        {
            if (mNowNode == null)
            {
                ++polygonIndex;
                mNowNode = pPolygon.First;
            }
        }

        PointDate mNowPointDate;

        public PointDate nowPointDate
        {
            get { return mNowPointDate; }
        }

        /// <summary>
        /// 获取下一个点,无则返回null
        /// </summary>
        /// <returns></returns>
        public  PointDate   moveNext()
        {
            PointDate   lOut = null;
            if(mNowNode!=null)
            {
                lOut = new PointDate();
                lOut.polygonId = polygonIndex;
                lOut.point = mNowNode.Value;
                LinkedListNode<zz2DPoint> lNextNode = mNowNode.Next;

                //考虑到没有 (凹/凸)点列表的情况
                while (lNextNode == null && polygonIndex < polygons.Count-1)
                {
                    lNextNode = polygons[++polygonIndex].First;
                }
                mNowNode = lNextNode;
            }
            mNowPointDate = lOut;
            return lOut;
        }
    }

    zzSimplyPolygon mOutSidePoint;

    List<zzSimplyPolygon> mHolePoint = new List<zzSimplyPolygon>();

    PointIterator   createConcaveIterator()
    {
        PointIterator lOut = new PointIterator();
        lOut.addPolygon(mOutSidePoint.getConcavePoints());
        foreach (var lHolePolygon in mHolePoint)
        {
            lOut.addPolygon(lHolePolygon.getConcavePoints());
        }
        return lOut;
    }

    public  void    printAllConcavePoint()
    {
        PointIterator lConcaveIterator = createConcaveIterator();
        while (lConcaveIterator.moveNext()!= null)
        {
            Debug.Log("polygonId:" 
                + lConcaveIterator.nowPointDate.polygonId + " " 
                + lConcaveIterator.nowPointDate.point.position);
        }
    }

    /// <summary>
    /// 最优点,信息
    /// </summary>
    class candidatePointInfo
    {
		public	const int scoresOfClearSelfConcave = 2;
		public	const int scoresOfClearOtherConcave = 2;
		
        public candidatePointInfo()
        {
            polygonId = 0;
            scores = 0;
        }

        public candidatePointInfo(PointDate pPointDate)
        {
            polygonId = pPointDate.polygonId;
            point = pPointDate.point;
            scores = 0;
        }
        public int polygonId ;
        public zz2DPoint point;
        public int scores ;

        public  PointDate   toPointDate()
        {
            PointDate lOut = new PointDate();
            lOut.polygonId = polygonId;
            lOut.point = point;
            return lOut;
        }

        public void scoresUp()
        {
            ++scores;
        }
		
        public void scoresUp(int up)
        {
			scores+=up;
		}
    }

    /// <summary>
    /// 两点是否可以相连
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns></returns>
    public  bool canLink(zz2DPoint point1,zz2DPoint point2)
    {
        //Vector2 v1 = new Vector2(434, 360);
        //Vector2 v2 = new Vector2(299, 352);
        //if ((point1.position == v1 && point2.position == v2)
        //    ||(point1.position == v2 && point2.position == v1))
        //{
        //    int i = 0;
        //    ++i;
        //}
        bool lCanLink = !point1.isLinked(point2)
                && point1.isEachInPositiveSide(point2)
                //&& !mOutSidePoint.isCrossWithLine(point1.position, point2.position);
                && !mOutSidePoint.isCrossWithLine(point1, point2);
        if(!lCanLink)
            return false;
        foreach (var lHole in mHolePoint)
        {
            lCanLink = !lHole.isCrossWithLine(point1, point2);
            if (!lCanLink)
                return false;
        }
        return lCanLink;
    }

    public bool isConvex
    {
        get { return mHolePoint.Count == 0 && getOutSidePolygon().isConvex(); }
    }

    candidatePointInfo getCandidateConvexPoint(zz2DPoint lPointToSolve)
    {
        candidatePointInfo lCandidatePointInfo=new candidatePointInfo();
        PointIterator lConvexIterator = new PointIterator();
        lConvexIterator.addPolygon(mOutSidePoint.getConvexPoints());
        foreach (var lHole in mHolePoint)
        {
            lConvexIterator.addPolygon(lHole.getConvexPoints());
        }

        while (lConvexIterator.moveNext() != null)
        {
            zz2DPoint lNowPoint = lConvexIterator.nowPointDate.point;
            if (canLink(lPointToSolve, lNowPoint))
            {

                var lNewCandidatePointInfo = new candidatePointInfo(lConvexIterator.nowPointDate);
                lNewCandidatePointInfo.scoresUp();

                if (isConcaveCleared(lPointToSolve, lNowPoint))
                {
                    lNewCandidatePointInfo.scoresUp(candidatePointInfo.scoresOfClearSelfConcave);
                }

                if (lNewCandidatePointInfo.scores > lCandidatePointInfo.scores)
                    lCandidatePointInfo = lNewCandidatePointInfo;

                if (lCandidatePointInfo.scores
                    > candidatePointInfo.scoresOfClearSelfConcave)
                    break;

            }
        }
        return lCandidatePointInfo;

    }

    candidatePointInfo getCandidateConcavePoint(out PointDate lPointDataToSolve)
    {
        var lCandidatePointInfo = new candidatePointInfo();

        PointIterator lConcaveIterator = new PointIterator();
        lConcaveIterator.addPolygon(mOutSidePoint.getConcavePoints());
        foreach (var lHole in mHolePoint)
        {
            lConcaveIterator.addPolygon(lHole.getConcavePoints());
        }

        lPointDataToSolve = lConcaveIterator.moveNext();
        //if(lPointDataToSolve==null)
        //{
        //    //无凹点,返回自身所有的点
        //    return new zzSimplyPolygon[] { mOutSidePoint };
        //}

        zz2DPoint lPointToSolve = lPointDataToSolve.point;

        //遍历凹点
        while (lConcaveIterator.moveNext() != null)
        {
            zz2DPoint lNowPoint = lConcaveIterator.nowPointDate.point;
            if (canLink(lPointToSolve, lNowPoint))
            {

                var lNewCandidatePointInfo = new candidatePointInfo(lConcaveIterator.nowPointDate);
                lNewCandidatePointInfo.scoresUp();

                //对结果评分
                if (isConcaveCleared(lPointToSolve, lNowPoint))
                {
                    lNewCandidatePointInfo.scoresUp(candidatePointInfo.scoresOfClearSelfConcave);
                }

                if (isConcaveCleared(lNowPoint, lPointToSolve))
                {
                    lNewCandidatePointInfo.scoresUp(candidatePointInfo.scoresOfClearOtherConcave);
                }

                //优于前次结果,则替代之前的
                if (lNewCandidatePointInfo.scores > lCandidatePointInfo.scores)
                    lCandidatePointInfo = lNewCandidatePointInfo;

                //连接获得高分时,则不再遍历
                if (lCandidatePointInfo.scores
                    > candidatePointInfo.scoresOfClearSelfConcave + candidatePointInfo.scoresOfClearOtherConcave)
                    break;

            }
        }
        return lCandidatePointInfo;

    }

    public  zz2DConcave[]   stepDecompose()
    {
        if (isConvex)
            return null;

        PointDate lPointDataToSolve;
        var lCandidatePointInfo = getCandidateConcavePoint(out lPointDataToSolve);

        //遍历凸点
        if (lCandidatePointInfo.scores == 0)
        {
            lCandidatePointInfo = getCandidateConvexPoint(lPointDataToSolve.point);

        }

        if (lCandidatePointInfo.scores == 0)
        {
            Debug.LogError("error in cutting");
            return null;
        }

        zz2DConcave[] lResult = cut(this, lPointDataToSolve, lCandidatePointInfo.toPointDate());
        return lResult;

    }

    public  zzSimplyPolygon[]    decompose()
    {
        zz2DConcave[] lResult = stepDecompose();
        if (lResult==null)
            return new zzSimplyPolygon[] { mOutSidePoint };

        var lOut = new List<zzSimplyPolygon>();

        foreach (var lToDecompose in lResult)
        {
            lOut.AddRange(lToDecompose.decompose());
        }

        return lOut.ToArray();
    }

    /// <summary>
    /// 将point链到newLinked后,point上生成的两个角是否有凹角
    /// </summary>
    /// <param name="point"></param>
    /// <param name="newLinked"></param>
    /// <returns></returns>
    public  bool    isConcaveCleared(zz2DPoint point,zz2DPoint newLinked)
    {
        return zz2DPoint.isConvexPoint(point.previousPoint.position, point.position, newLinked.position)
            && zz2DPoint.isConvexPoint(newLinked.position, point.position, point.nextPoint.position);
    }

    public void    setShape(Vector2[] points)
    {
        mOutSidePoint = new zzSimplyPolygon();
        mOutSidePoint.setShape(points);
    }

    public void setShape(zzSimplyPolygon lSimplyPolygon)
    {
        mOutSidePoint = lSimplyPolygon;
    }

    public zzSimplyPolygon getOutSidePolygon()
    {
        return mOutSidePoint;
    }

    public zzSimplyPolygon getHole(int index)
    {
        return mHolePoint[index];
    }
    
    public zzSimplyPolygon[] getHole()
    {
        return mHolePoint.ToArray();
    }

    public int getHoleNum()
    {
        return mHolePoint.Count;
    }

    public void addHole(Vector2[] points)
    {

        zzSimplyPolygon lSimplyPolygon = new zzSimplyPolygon();
        lSimplyPolygon.setShape(points);

        addHole(lSimplyPolygon);
    }

    public void addHole(zzSimplyPolygon pSimplyPolygon)
    {
        mHolePoint.Add(pSimplyPolygon);
    }

    static public int getHoleIndexFromPolygonID(int pPolygonID)
    {
        return pPolygonID - 1;
    }

    static public int getPolygonIDFromHoleIndex(int pHoleIndex)
    {
        return pHoleIndex + 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pConcave"></param>
    /// <param name="Point1Data"></param>
    /// <param name="Point2Data"></param>
    /// <returns></returns>
    static zz2DConcave[] cut(zz2DConcave pConcave, PointDate Point1Data, PointDate Point2Data)
    {
        //if(Point1Data.polygonId!=0)
        //{
        //    Debug.LogError("Point1Data.polygonId!=0");
        //    return null;
        //}

        zz2DConcave lOut0 = new zz2DConcave();

        zzSimplyPolygon[] lCutedPolygon = zzSimplyPolygon.cut(Point1Data.point, Point2Data.point);
        var lHoles = pConcave.getHole();

        if (lCutedPolygon.Length==1)
        {
            if (Point1Data.polygonId == Point2Data.polygonId)
            {
                Debug.LogError("lCutedPolygon.Length==1 && Point1Data.polygonId == Point2Data.polygonId ");
            }
            //两点都在不同的孔上
            if (Point1Data.polygonId != 0 && Point2Data.polygonId != 0)
            {
                lOut0.setShape(pConcave.getOutSidePolygon());
                lOut0.addHole(lCutedPolygon[0]);
                //return new zz2DConcave[] { lOut0 };

            }
            else//一个点在孔上
            {
                lOut0.setShape(lCutedPolygon[0]);
                //for (int i = 0; i < lHoles.Length; ++i)
                //{
                //    if (Point1Data.polygonId != (i + 1) && Point2Data.polygonId != (i + 1))
                //        lOut0.addHole(lHoles[i]);
                //}

            }
            for (int i = 0; i < lHoles.Length; ++i)
            {
                int lHolePolygonID = getPolygonIDFromHoleIndex(i);
                if (Point1Data.polygonId != lHolePolygonID
                    && Point2Data.polygonId != lHolePolygonID)
                    lOut0.addHole(lHoles[i]);
            }

            return new zz2DConcave[] { lOut0 };
        }

        zz2DConcave lOut1 = new zz2DConcave();

        //两个点在相同的孔上
        if (Point1Data.polygonId != 0 && Point2Data.polygonId != 0)
        {
            lOut0.setShape(pConcave.mOutSidePoint);
            if (lCutedPolygon[0].bounds.Contains(lCutedPolygon[1].bounds))
            {
                lOut0.addHole(lCutedPolygon[0]);
                lOut1.setShape(lCutedPolygon[1]);
            }
            else if (lCutedPolygon[1].bounds.Contains(lCutedPolygon[0].bounds))
            {
                lOut0.addHole(lCutedPolygon[1]);
                lOut1.setShape(lCutedPolygon[0]);
            }
            else
                Debug.LogError("两个点在相同的孔上:bounds not contains another");
        }
        else
        {
            lOut0.setShape(lCutedPolygon[0]);
            lOut1.setShape(lCutedPolygon[1]);
        }

        int lHoleIndex = 0;
        foreach (var lHole in lHoles)
        {
            int lPolygonID = getPolygonIDFromHoleIndex(lHoleIndex);
            if (Point1Data.polygonId == lPolygonID || Point2Data.polygonId == lPolygonID)
                continue;

            int lIsInside = lCutedPolygon[0].isInside(lHole.getAllPoints().First.Value.position);
            if(lIsInside>0)
                lOut0.addHole(lHole);
            else if (lIsInside < 0)
                lOut1.addHole(lHole);
            else
                Debug.LogError("isInside == 0");

            ++lHoleIndex;
        }

        return new zz2DConcave[]{lOut0,lOut1};
    }
}