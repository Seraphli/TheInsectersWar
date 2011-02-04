using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class zzFlatMeshUtility
{
    public static GameObject showPicture(Texture2D pTex,string pName)
    {
        GameObject lObject = new GameObject(pName);
        showPicture(pTex, lObject);
        return lObject;
    }

    public static void showPicture(Texture2D pTex, GameObject lObject)
    {

        Mesh lMesh = createPlane((int)pTex.width, (int)pTex.height);
        MeshFilter lMeshFilter = lObject.AddComponent<MeshFilter>();
        lMeshFilter.mesh = lMesh;
        MeshRenderer lMeshRenderer = lObject.AddComponent<MeshRenderer>();
        Material lMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        lMaterial.color = Color.white;
        lMaterial.mainTexture = pTex;
        //lMaterial.SetTexture("_MainTex", pTex);
        lMeshRenderer.sharedMaterial = lMaterial;
    }

    public static Mesh createPlane(float width, float height)
    {
        Mesh lMesh = new Mesh();
        // 3--2
        // |  |
        // 0--1
        
        // Indices into the vertex array
        int[] triIndices = new int[] { 0, 2, 1, 3, 2, 0 };

        // UV coordinates
        Vector2[] UVs = new Vector2[] { 
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(1, 1), new Vector2(0, 1) 
        };

        var vertices = new Vector3[]{
                new Vector3(0.0f,0.0f,0.0f),
                new Vector3(width,0.0f,0.0f),
                new Vector3(width,height,0.0f),
                new Vector3(0.0f,height,0.0f)
        };

        lMesh.vertices = vertices;
        lMesh.uv = UVs;

        lMesh.triangles = triIndices;
        lMesh.normals = new Vector3[]{
            new Vector3(0,0,-1),new Vector3(0,0,-1),
            new Vector3(0,0,-1),new Vector3(0,0,-1)
        };
        return lMesh;

    }

    public static void draw(Mesh pMesh, 
        List<Vector2[]> pSurfaceList, List<Vector2[]> pEdgeList,
         float zThickness,Vector2 pUvScale)
    {
        //考虑到边缘有重复的点的情况,所以点到加进地图中后,在指定索引
        var lPointToIndex = new Dictionary<Vector2, int>();
        int lEdgePointNum = 0;
        foreach (var lEdge in pEdgeList)
        {
            lEdgePointNum += lEdge.Length;
        }
        var lPoints = new List<Vector2>(lEdgePointNum);

        foreach (var lEdge in pEdgeList)
        {
            foreach (var lPoint in lEdge)
            {
                if (!lPointToIndex.ContainsKey(lPoint))
                {
                    lPointToIndex[lPoint] = lPoints.Count;
                    lPoints.Add(lPoint);
                }
            }
        }

        int lSurfaceTriIndexNum = 0;
        foreach ( var lSurface in pSurfaceList)
        {
            lSurfaceTriIndexNum += getTriangleIndexNum(lSurface.Length)*2;
        }

        int lEdgeTriIndexNum = 0;
        foreach (var lEdge in pEdgeList)
        {
            lEdgeTriIndexNum += lEdge.Length*6;;
        }
        //int i = 0;
        //foreach (var lPointMap in lPointToIndex)
        //{
        //    lPoints[i] = lPointMap.Key;
        //    lPointToIndex[lPointMap.Key] = i;
        //    ++i;
        //}

        Vector3[] lVertices;
        Vector3[] lNormals;
        Vector2[] lUVs;

        toMeshPoint(lPoints.ToArray(), zThickness,
            out lVertices, out lNormals, out lUVs, pUvScale);
        int lPointNum = lPoints.Count * 2;
        int lFlatTriangleNum = lPoints.Count - 2;
        //int lFlatIndexNum = lFlatTriangleNum * 3;
        //int lTriangleNum = lFlatTriangleNum * 2 + lPointNum;

        //前面 背面 边
        int[] lTriIndices = new int[lSurfaceTriIndexNum + lEdgeTriIndexNum];

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
            lSurfaceIndex = drawDoubleSurface(pMeshPointIndex, lSurfaceIndex,
                lSurfacePoints, pPointToIndex, pFlatPointNum);
        }

        pBeginIndex += 2 * (lSurfaceIndex-pBeginIndex);

        foreach (var lEdgePoints in pEdgeList)
        {
            pBeginIndex = drawEdge(pMeshPointIndex, pBeginIndex,
                lEdgePoints, pPointToIndex, pFlatPointNum);
        }
        return pBeginIndex;
    }

    public static int drawEdge(int[] pMeshPointIndex, int pBeginIndex, Vector2[] pPoints,
        Dictionary<Vector2, int> pPointToIndex, int pFlatPointNum)
    {
        return drawEdge(pMeshPointIndex, pBeginIndex,
            toPointIndexList(pPointToIndex, pPoints), pFlatPointNum);

    }

    public static int drawEdge(int[] lTriIndices, int pBeginIndex,
        int[] pPointIndexList, int pFlatPointNum)
    {

        //创建边的面

        //int pBeginIndex = lFlatIndexNum * 2;
        int i = 0;
        for (; i < pPointIndexList.Length - 1; ++i)
        {
            lTriIndices[pBeginIndex++] = pPointIndexList[i];
            lTriIndices[pBeginIndex++] = pPointIndexList[i] + pFlatPointNum;
            lTriIndices[pBeginIndex++] = pPointIndexList[i + 1];

            lTriIndices[pBeginIndex++] = pPointIndexList[i + 1];
            lTriIndices[pBeginIndex++] = pPointIndexList[i] + pFlatPointNum;
            lTriIndices[pBeginIndex++] = pPointIndexList[i + 1] + pFlatPointNum;
        }
        lTriIndices[pBeginIndex++] = pPointIndexList[i];
        lTriIndices[pBeginIndex++] = pPointIndexList[i] + pFlatPointNum;
        lTriIndices[pBeginIndex++] = pPointIndexList[0];

        lTriIndices[pBeginIndex++] = pPointIndexList[0];
        lTriIndices[pBeginIndex++] = pPointIndexList[i] + pFlatPointNum;
        lTriIndices[pBeginIndex++] = pPointIndexList[0] + pFlatPointNum;

        return pBeginIndex;

    }


    public static int drawDoubleSurface(int[] pMeshPointIndex, int pBeginIndex,
        Vector2[] pPoints, Dictionary<Vector2,int> pPointToIndex,int pFlatPointNum)
    {
        return drawDoubleSurface(pMeshPointIndex, pBeginIndex,
            toPointIndexList(pPointToIndex,pPoints), pFlatPointNum);
    }

    public static int[] toPointIndexList(Dictionary<Vector2, int> pPointToIndex, Vector2[] pPoints)
    {
        int[] lPointIndexList = new int[pPoints.Length];
        int i = 0;
        foreach (var lPoint in pPoints)
        {
            lPointIndexList[i] = pPointToIndex[lPoint];
            ++i;
        }
        return lPointIndexList;
    }

    public static int drawDoubleSurface(int[] pMeshPointIndex, int pBeginIndex,
        int[] pPointIndexList, int pFlatPointNum)
    {
        //if (pFlatPointNum < 3)
        //    return;

        //创建两侧的面

        int lFlatTriangleNum = pFlatPointNum - 2;
        int lFlatIndexNum = lFlatTriangleNum * 3;

        int lSurfaceTriangleNum = pPointIndexList.Length - 2;

        int lIndexInPointList = 1;
        //int pBeginIndex = 0;

        int lfrontSideFirstVerIndex = pPointIndexList[0];
        int lReverseSideFirstVerIndex = lfrontSideFirstVerIndex + pFlatPointNum;
        for (int i = 0; i < lSurfaceTriangleNum; ++i)
        {

            pMeshPointIndex[pBeginIndex + lFlatIndexNum]
                = lReverseSideFirstVerIndex;
            pMeshPointIndex[pBeginIndex++] = lfrontSideFirstVerIndex;

            pMeshPointIndex[pBeginIndex + lFlatIndexNum]
                = pPointIndexList[lIndexInPointList + 1] + pFlatPointNum;
            pMeshPointIndex[pBeginIndex++] = pPointIndexList[lIndexInPointList];

            pMeshPointIndex[pBeginIndex + lFlatIndexNum]
                = pPointIndexList[lIndexInPointList] + pFlatPointNum;
            pMeshPointIndex[pBeginIndex++] = pPointIndexList[lIndexInPointList + 1];


            ++lIndexInPointList;
        }
        return pBeginIndex;


    }

    public static int getTriangleIndexNum(int pVertexNum)
    {
        return (pVertexNum - 2) * 3;
    }

    public static void draw(Mesh pMesh, Vector2[] points, float zThickness)
    {
        if (points.Length < 3)
            return;
        Vector3[] lVertices;// = new Vector3[lPointNum];
        Vector3[] lNormals;// = new Vector3[lPointNum];
        Vector2[] lUVs;// = new Vector2[lPointNum];
        //int[] lTriIndices = new int[(points.Length-3)*2+3];
        int lFlatTriangleNum = points.Length - 2;
        int lFlatIndexNum = lFlatTriangleNum * 3;

        //lFlatTriangleNum*2+lPointNum
        int lPointNum = points.Length * 2;
        int lTriangleNum = lFlatTriangleNum * 2 + lPointNum;

        //前面 背面 边
        int[] lTriIndices = new int[lTriangleNum * 3];

        toMeshPoint(points, zThickness,out lVertices,out lNormals,out lUVs);

        //创建两侧的面
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

        //创建边的面
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
    private static void toMeshPoint(Vector2[] points, float zThickness,
        out Vector3[] lVertices, out Vector3[] lNormals, out Vector2[] lUVs)
    {
        toMeshPoint(points, zThickness,
            out  lVertices, out  lNormals, out lUVs, new Vector2(1.0f, 1.0f));
    }

    private static void toMeshPoint(Vector2[] points, float zThickness,
        out Vector3[] lVertices,out Vector3[] lNormals,out Vector2[] lUVs,Vector2 pUVScale)
    {
        lVertices = new Vector3[points.Length * 2];
        lNormals = new Vector3[points.Length * 2];
        lUVs = new Vector2[points.Length * 2];
        //3D 点
        float lHalfThickness = zThickness / 2;
        for (int i = 0; i < points.Length; ++i)
        {
            Vector2 lPoint = points[i];
            Vector2 lUV = lPoint;
            lUV.Scale(pUVScale);

            lVertices[i].x = lPoint.x;
            lVertices[i].y = lPoint.y;
            lVertices[i].z = -lHalfThickness;
            lUVs[i] = lUV;
            lNormals[i] = new Vector3(0, 0, -1);


            lVertices[points.Length + i].x = lPoint.x;
            lVertices[points.Length + i].y = lPoint.y;
            lVertices[points.Length + i].z = lHalfThickness;
            lUVs[points.Length + i] = lUV;
            lNormals[points.Length + i] = new Vector3(0, 0, 1);
        }
    }

    public static Vector3[] getNormals(int pVerticesLength)
    {
        Vector3[] lNormals = new Vector3[pVerticesLength];
        int lHalfLength = pVerticesLength / 2;
        for (int i = 0; i < lHalfLength;++i )
        {
            lNormals[i] = new Vector3(0, 0, -1);
        }
        for (int i = lHalfLength; i < pVerticesLength; ++i)
        {
            lNormals[i] = new Vector3(0, 0, 1);
        }
        return lNormals;
    }

    public static Vector2[] verticesCoordToUV(Vector3[] pVertices, Vector2 pScale)
    {
        Vector2[] lOut = new Vector2[pVertices.Length];
        for (int i = 0; i < pVertices.Length;++i )
        {
            Vector3 lCoord = pVertices[i];
            lOut[i] = new Vector2(lCoord.x * pScale.x, lCoord.y * pScale.y);
        }
        return lOut;
    }

    public static Vector2 getUvScaleFromImgSize(Vector2 ImgSize)
    {
        return new Vector2(1.0f/ImgSize.x,1.0f/ ImgSize.y);
    }
}