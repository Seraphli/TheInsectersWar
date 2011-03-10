using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zzPlaneMeshUtility
{

    public static void draw(Mesh pMesh, List<Vector2[]> pSurfaceList,
        List<Vector2[]> pEdgeList,Vector2 pUvScale, Vector2 pPointOffset)
    {

        Dictionary<Vector2, int> lPointToIndex;
        List<Vector2> lPoints;
        zzFlatMeshUtility.getPointAndMap(pEdgeList, out lPointToIndex, out lPoints);

        int lSurfaceTriIndexNum = 0;
        foreach (var lSurface in pSurfaceList)
        {
            lSurfaceTriIndexNum += zzFlatMeshUtility.getTriangleIndexNum(lSurface.Length);
        }


        Vector3[] lVertices;
        Vector3[] lNormals;
        Vector2[] lUVs;

        toMeshPoint(lPoints.ToArray(), pPointOffset,
            out lVertices, out lNormals, out lUVs, pUvScale);

        int[] lTriIndices = new int[lSurfaceTriIndexNum ];

        draw(lTriIndices, 0, pSurfaceList, pEdgeList, lPointToIndex, lPointToIndex.Count);

        pMesh.vertices = lVertices;
        pMesh.triangles = lTriIndices;
        pMesh.uv = lUVs;
        pMesh.normals = lNormals;
    }

    public static int draw(int[] pMeshPointIndex, int pBeginIndex,
        List<Vector2[]> pSurfaceList, List<Vector2[]> pEdgeList,
        Dictionary<Vector2, int> pPointToIndex, int pFlatPointNum)
    {
        //int lFlatTriangleIndexNum = 0;
        int lSurfaceIndex = pBeginIndex;
        foreach (var lSurfacePoints in pSurfaceList)
        {
            lSurfaceIndex = drawSurface(pMeshPointIndex, lSurfaceIndex,
                lSurfacePoints, pPointToIndex, pFlatPointNum);
        }

        return lSurfaceIndex;
    }

    public static int drawSurface(int[] pMeshPointIndex, int pBeginIndex,
        Vector2[] pPoints, Dictionary<Vector2, int> pPointToIndex, int pFlatPointNum)
    {
        return drawSurface(pMeshPointIndex, pBeginIndex,
            zzFlatMeshUtility.toPointIndexList(pPointToIndex, pPoints), pFlatPointNum);
    }

    public static int drawSurface(int[] pMeshPointIndex, int pBeginIndex,
        int[] pPointIndexList, int pFlatPointNum)
    {

        int lSurfaceTriangleNum = pPointIndexList.Length - 2;

        int lIndexInPointList = 1;
        //int pBeginIndex = 0;

        int lfrontSideFirstVerIndex = pPointIndexList[0];
        for (int i = 0; i < lSurfaceTriangleNum; ++i)
        {
            pMeshPointIndex[pBeginIndex++] = lfrontSideFirstVerIndex;

            pMeshPointIndex[pBeginIndex++] = pPointIndexList[lIndexInPointList];

            pMeshPointIndex[pBeginIndex++] = pPointIndexList[lIndexInPointList + 1];


            ++lIndexInPointList;
        }
        return pBeginIndex;


    }

    private static void toMeshPoint(Vector2[] points, Vector2 pPointOffset,
        out Vector3[] lVertices, out Vector3[] lNormals, out Vector2[] lUVs)
    {
        toMeshPoint(points, pPointOffset,
            out  lVertices, out  lNormals, out lUVs, new Vector2(1.0f, 1.0f));
    }

    private static void toMeshPoint(Vector2[] points, Vector2 pPointOffset,
        out Vector3[] lVertices, out Vector3[] lNormals, out Vector2[] lUVs, Vector2 pUVScale)
    {
        lVertices = new Vector3[points.Length ];
        lNormals = new Vector3[points.Length ];
        lUVs = new Vector2[points.Length ];

        for (int i = 0; i < points.Length; ++i)
        {
            Vector2 lPoint = points[i];
            Vector2 lUV = lPoint;
            lUV.Scale(pUVScale);

            lVertices[i].x = lPoint.x + pPointOffset.x;
            lVertices[i].y = lPoint.y + pPointOffset.y;
            lUVs[i] = lUV;
            lNormals[i] = new Vector3(0, 0, -1);

        }
    }

}