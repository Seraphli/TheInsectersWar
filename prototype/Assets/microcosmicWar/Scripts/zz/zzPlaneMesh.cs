using UnityEngine;
using System.Collections;

public class zzPlaneMesh
{

    // The vertices of mesh
    // 3--2
    // |  |
    // 0--1
    public Vector3[] vertices = new Vector3[]{
                new Vector3(0,0,0),
                new Vector3(1,0,0),
                new Vector3(1,1,0),
                new Vector3(0,1,0)
        };

    // Indices into the vertex array
    public int[] triIndices = new int[] { 0, 2, 1, 3, 2, 0 };

    // UV coordinates
    public Vector2[] UVs = new Vector2[] { 
        new Vector2(0, 0), new Vector2(1, 0),
        new Vector2(1, 1), new Vector2(0, 1) 
    };

    Mesh mesh;

    public void initMesh(Mesh pMesh)
    {
        mesh = pMesh;
        UpdateMesh();
    }

    public void Init(GameObject pObject)
    {
        mesh = new Mesh();

        MeshFilter lMeshFilter = pObject.GetComponent<MeshFilter>();
        if (lMeshFilter == null)
            lMeshFilter = pObject.AddComponent<MeshFilter>();
        lMeshFilter.mesh = mesh;

        MeshRenderer lMeshRenderer = pObject.GetComponent<MeshRenderer>();
        if (lMeshRenderer == null)
            lMeshRenderer = pObject.AddComponent<MeshRenderer>();
        //lMeshRenderer.material = verticalRulerMaterial;

        //meshRenderer = lMeshRenderer;

        UpdateMesh();

    }

    public void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.triangles = triIndices;
        mesh.normals = new Vector3[]{
            new Vector3(0,0,-1),new Vector3(0,0,-1),
            new Vector3(0,0,-1),new Vector3(0,0,-1)
        };

    }

    public enum PivotType
    {
        center,
        leftBottom,
    }

    public Vector3[] resize(Vector2 pSize, PivotType pPivotType)
    {
        return resize(pSize.x, pSize.y, pPivotType);
    }

    public Vector3[] resize(float pWidth, float pHeigth, PivotType pPivotType)
    {
        switch (pPivotType)
        {
            case PivotType.center:
                vertices = new Vector3[]{
                    new Vector3(-pWidth/2,-pHeigth/2,0),
                    new Vector3(pWidth/2,-pHeigth/2,0),
                    new Vector3(pWidth/2,pHeigth/2,0),
                    new Vector3(-pWidth/2,pHeigth/2,0)
                };
                break;
            case PivotType.leftBottom:
                vertices = new Vector3[]{
                    new Vector3(0.0f,0.0f,0.0f),
                    new Vector3(pHeigth,0.0f,0.0f),
                    new Vector3(pHeigth,pWidth,0.0f),
                    new Vector3(0.0f,pWidth,0.0f)
                };
                break;
        }

        return vertices;
    }
}