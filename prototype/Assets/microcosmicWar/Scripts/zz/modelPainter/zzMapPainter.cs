using UnityEngine;
using System.Collections;
using System.Collections.Generic;


using PointGroup = System.Collections.Generic.List<UnityEngine.Vector2>;
public class zzMapPainter : MonoBehaviour
{
    //public PainterPoint[] painterPoints;
    // Use this for initializations
    //public zzPainterPoint rootPainterPoints;
    List<zz2DLine> mLine = new List<zz2DLine>();

    public zzPainterPolygon mOutSidePolygon;
    public zzPainterPolygon mHole;

    public zzPainterPoint polygonPoint;
    public zzPainterPoint point;

    [ContextMenu("Test")]
    void test()
    {
        var lPolygon = zzSimplyPolygonDebuger.toPolygon(polygonPoint);
        Vector3 lPos = point.transform.position;
        Vector2 lPoint = new Vector2(lPos.x, lPos.y);
        print(lPolygon.isInside(lPoint));
    }

    public zzPainterPoint lineBegin;
    public zzPainterPoint lineEnd;


    public zzPainterPoint cutBegin;
    public zzPainterPoint cutEnd;

    Vector2[] toArray(List<zzPainterPoint> pPoints)
    {
        var l2DPoints = new PointGroup();
        foreach (var lPoint in pPoints)
        {
            Vector3 l3DPoint = lPoint.transform.position;
            Vector2 l2DPoint = new Vector2(l3DPoint.x, l3DPoint.y);
            l2DPoints.Add(l2DPoint);
        }
        return l2DPoints.ToArray();
    }

    public Texture2D testPicture;

    void Start()
    {

        //}
        //var lShapeList = new List<Vector2[]>();
        //var lPainterPoints = new List<zzPainterPoint>();    
        //foreach(Transform lSub in transform)
        //{
        //    zzPainterPolygon lPainterPolygon = lSub.gameObject.GetComponent<zzPainterPolygon>();
        //    if (lPainterPolygon && lPainterPolygon.needDraw)
        //    {
        //        List<zzPainterPoint> lAllPolygonPoint = lPainterPolygon.getAllPoint();
        //        lPainterPoints.AddRange(lAllPolygonPoint) ;
        //        lShapeList.Add(toArray(lAllPolygonPoint));
        //    }
                
        //}


        //}

        //lineTo3DTest();

        //createLine(testPicture);


        //lEdges = createMesh(lEdges);

    }

    //private void createLine(Texture2D tex)
    //{
    //    List<Vector2[]> lEdges = zzOutlineSweeper.getEdge(tex,1.7f);
    //    foreach (var lEdge in lEdges)
    //    {
    //        zzSimplyPolygon lPolygon = new zzSimplyPolygon();
    //        lPolygon.setShape(lEdge);
    //        zzSimplyPolygonDebuger.createDebuger(lPolygon, "Picture");
    //    }
    //}
    //private List<Vector2[]> createMesh(Texture2D tex)
    //{
    //    List<Vector2[]> lEdges = zzOutlineSweeper.getEdge(tex, 1.7f);
    //    foreach (var lEdge in lEdges)
    //    {
    //        //zzSimplyPolygon lPolygon = new zzSimplyPolygon();
    //        //lPolygon.setShape(lEdge);
    //        //zzSimplyPolygonDebuger.createDebuger(lPolygon, "Picture");
    //        zz2DConcave lConcave = new zz2DConcave();
    //        lConcave.setShape(lEdge);
    //        zzSimplyPolygon[] lConvexs = lConcave.decompose();
    //        createFlatMesh(lConvexs, "myMesh", null);
    //    }
    //    return lEdges;
    //}

    private void lineTo3DTest()
    {
        zz2DConcave lConcave = new zz2DConcave();
        lConcave.setShape(toArray(mOutSidePolygon.getAllPoint()));
        lConcave.addHole(toArray(mHole.getAllPoint()));
        //zzPolygon polygon;
        //zz2DTriangle[]  lTriangles;
        //mLine = 
        //    zzDecomposePolygon.triangulatingDecompose(lShapeList,out polygon,out lTriangles);

        //var polygonPoint = polygon.clonePoint();

        mOutSidePolygon.setPointInfo(lConcave.getOutSidePolygon().getAllPoints());
        mOutSidePolygon.bounds = lConcave.getOutSidePolygon().bounds;

        mHole.setPointInfo(lConcave.getHole(0).getAllPoints());
        mHole.bounds = lConcave.getHole(0).bounds;
        //print(lConcave)
        lConcave.printAllConcavePoint();

        print(mOutSidePolygon.bounds.Contains(point.getVec2Position()));
        print("Cross:" + lConcave.getOutSidePolygon().isCrossWithLine(lineBegin.getVec2Position(), lineEnd.getVec2Position()));

        //zzSimplyPolygonDebuger.createDebuger(zzSimplyPolygon.getSubPolygon(cutBegin.pointInfo, cutEnd.pointInfo), "PolygonDebuger");
        zzSimplyPolygon[] lConvexs = lConcave.decompose();
        zzSimplyPolygonDebuger.createDebuger(lConvexs, "decompose");
        createFlatMesh(lConvexs, "myMesh", null);
    }

    void Update()
    {
        foreach (var lLine in mLine)
        {
            Debug.DrawLine(lLine.from.position,lLine.to.position);
        }
    }

    static void  toConvex(PointGroup pPointLine,List<Vector2[]> pConvexShapeList)
    {
        List<Vector2> lConvexShape = new List<Vector2>();
        //int lIndexNum = pPointLine.Count - 2;
		int lIndexNum = pPointLine.Count;
        float lLefRight = zz2DPoint.LeftOrRight(pPointLine[0], pPointLine[1], pPointLine[2]);
        lConvexShape.Add(pPointLine[0]);
        lConvexShape.Add(pPointLine[1]);
        lConvexShape.Add(pPointLine[2]);
        if(pPointLine.Count == 3)
        {
            pConvexShapeList.Add(lConvexShape.ToArray());
            return;
        }
        int lPoint1Index = 1;
        int lPoint2Index = 2;
        for (int lPoint3Index = 3; lPoint3Index < lIndexNum; ++lPoint3Index)
        {
            //float lNewLefRight = LeftOrRight(pPointLine[lPoint1Index], pPointLine[lPoint2Index], pPointLine[lPoint3Index]);
            //float lToRootLefRight = LeftOrRight(pPointLine[lPoint2Index], pPointLine[lPoint3Index], pPointLine[0]);
            //if (lLefRight * lNewLefRight < 0 || lLefRight * lToRootLefRight<0 )//凹
            if (isInConvex(pPointLine, lPoint1Index, lPoint2Index, lPoint3Index, lLefRight))
            {
                lConvexShape.Add(pPointLine[lPoint3Index]);
                lPoint1Index = lPoint2Index;
                lPoint2Index = lPoint3Index;
            }
            else
            {
                PointGroup lCutedGroup = new PointGroup();
                lCutedGroup.Add(pPointLine[lPoint3Index - 1]);
                lCutedGroup.Add(pPointLine[lPoint3Index]);
                ++lPoint3Index;
                for (; 
                    lPoint3Index < lIndexNum 
                    && !isInConvex(pPointLine, lPoint1Index, lPoint2Index, lPoint3Index, lLefRight); 
                    ++lPoint3Index)
                {
                    lCutedGroup.Add(pPointLine[lPoint3Index]);
                }
                lCutedGroup.Add(pPointLine[lPoint3Index]);
                toConvex(lCutedGroup, pConvexShapeList);

                //因为后面还会加
                --lPoint3Index;

            }
        }
        pConvexShapeList.Add(lConvexShape.ToArray());
    }

    static bool isInConvex(PointGroup pPointLine, int pointIndex1, int pointIndex2, int pointIndex3, float firstLefRight)
    {
        float lNewLefRight = zz2DPoint.LeftOrRight(pPointLine[pointIndex1], pPointLine[pointIndex2], pPointLine[pointIndex3]);
        float lToRootLefRight = zz2DPoint.LeftOrRight(pPointLine[pointIndex2], pPointLine[pointIndex3], pPointLine[0]);
        if (firstLefRight * lNewLefRight < 0 || firstLefRight * lToRootLefRight<0 )//凹
        {
            return false;
        }
        return true;
    }

    static GameObject createFlatMesh(zzSimplyPolygon[] pPolygons, string pName, Transform pParent)
    {
        GameObject lOut;
        Transform   lParent;
        if(pParent)
        {
            lOut = pParent.gameObject;
            lParent = pParent;
        }
        else
        {
            lOut = new GameObject(pName);
            lParent = lOut.transform;
        }

        int i = 0;
        foreach (var lPolygon in pPolygons)
        {
            createFlatMesh(lPolygon.getShape(),pName+i,lParent);
            ++i;
        }
        return lOut;
    }

    static GameObject createFlatMesh(Vector2[] points,string pName,Transform parent)
    {
        GameObject lOut = new GameObject(pName);
        lOut.transform.parent = parent;
        MeshFilter lMeshFilter = lOut.AddComponent<MeshFilter>();
        MeshRenderer lMeshRenderer = lOut.AddComponent<MeshRenderer>();
        MeshCollider lMeshCollider = lOut.AddComponent<MeshCollider>();
        draw(lMeshFilter.mesh, points,10.0f);
        lMeshCollider.sharedMesh = lMeshFilter.mesh;
        lMeshCollider.convex = true;
        //MeshFilter lMeshFilter = lOut
        return lOut;
    }

    // Update is called once per frame
    //void Update () {

    //}

    static void draw(Mesh pMesh, Vector2[] points, float zThickness)
    {
        float lHalfThickness = zThickness;
        if (points.Length < 3)
            return;
        int lPointNum = points.Length * 2;
        Vector3[] lVertices = new Vector3[lPointNum];
        Vector3[] lNormals = new Vector3[lPointNum];
        Vector2[] lUVs = new Vector2[lPointNum];
        //int[] lTriIndices = new int[(points.Length-3)*2+3];
        int lFlatTriangleNum = points.Length - 2;
        int lFlatIndexNum = lFlatTriangleNum * 3;

        //lFlatTriangleNum*2+lPointNum
        int lTriangleNum = lFlatTriangleNum*2+lPointNum;

        //前面 背面 边
        int[] lTriIndices = new int[lTriangleNum * 3];

        for (int i = 0; i < points.Length; ++i)
        {
            Vector2 lPoint = points[i];
            lVertices[i].x = lPoint.x;
            lVertices[i].y = lPoint.y;
            lVertices[i].z = -lHalfThickness;
            lUVs[i] = lPoint;
            lNormals[i] = new Vector3(0, 0, -1);


            lVertices[points.Length + i].x = lPoint.x;
            lVertices[points.Length + i].y = lPoint.y;
            lVertices[points.Length + i].z = lHalfThickness;
            lUVs[points.Length + i] = lPoint;
            lNormals[points.Length + i] = new Vector3(0, 0, 1);
        }

        {
            int lPointIndex = 1;
            int lTriIndiceIndex = 0;
            for (int i = 0; i < lFlatTriangleNum; ++i)
            {
                int lOffsetToReverseSide = lPointIndex + points.Length;
                lTriIndices[lTriIndiceIndex + lFlatIndexNum] = points.Length;
                lTriIndices[lTriIndiceIndex++] = 0;

                lTriIndices[lTriIndiceIndex + lFlatIndexNum] = lOffsetToReverseSide + 1;
                lTriIndices[lTriIndiceIndex++] = lPointIndex;
    ;
                lTriIndices[lTriIndiceIndex + lFlatIndexNum] = lOffsetToReverseSide;
                lTriIndices[lTriIndiceIndex++] = lPointIndex + 1;
                //lTriIndices[llTriIndiceIndex++] = lPointIndex + 2;
                //++llTriIndiceIndex;
                ++lPointIndex;
            }

        }

        {
            int lTriIndiceIndex = lFlatIndexNum * 2;
            int i = 0;
            for (; i < points.Length - 1; ++i)
            {
                lTriIndices[lTriIndiceIndex++] = i;
                lTriIndices[lTriIndiceIndex++] = i + points.Length;
                lTriIndices[lTriIndiceIndex++] = i + 1;

                lTriIndices[lTriIndiceIndex++] = i + 1;
                lTriIndices[lTriIndiceIndex++] = i + points.Length;
                lTriIndices[lTriIndiceIndex++] = i + 1 + points.Length;
            }
                lTriIndices[lTriIndiceIndex++] = i;
                lTriIndices[lTriIndiceIndex++] = i + points.Length;
                lTriIndices[lTriIndiceIndex++] = 0;

                lTriIndices[lTriIndiceIndex++] = 0;
                lTriIndices[lTriIndiceIndex++] = i + points.Length;
                lTriIndices[lTriIndiceIndex++] = 0 + points.Length;
        }

        pMesh.vertices = lVertices;
        pMesh.triangles = lTriIndices;
        pMesh.uv = lUVs;
        pMesh.normals = lNormals;
    }

    /// <summary>
    /// 判断点c在线段ab的左边还是右边，如果返回值大于0在左边，如果小于0在右边，否则共线 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    static float LeftOrRight(Vector3 a, Vector3 b, Vector3 c)
    {
        a.x -= c.x; a.y -= c.y;
        b.x -= c.x; b.y -= c.y;
        return a.x * b.y - a.y * b.x;
    }

    //static bool is_tu(Vector3[] p)
    //{
    //    int n = p.Length;
    //    if (n < 4) return false;
    //    float a, b;
    //    a = LeftOrRight(p[0], p[1], p[2]);
    //    for (; --n > 2; ++p, a = b)
    //    {
    //        b = LeftOrRight(p[0], p[1], p[2]);
    //        if (a * b < 0)
    //            return false;
    //    }
    //    return true;
    //}

}
