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

    public static void draw(Mesh pMesh, Vector2[] points, float zThickness)
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
        int lTriangleNum = lFlatTriangleNum * 2 + lPointNum;

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
}