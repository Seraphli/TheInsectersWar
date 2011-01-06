using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class zzPlaneMesh
//{
//    // The vertices of mesh
//    // 3--2
//    // |  |
//    // 0--1
//    public Vector3[] vertices = new Vector3[]{
//                new Vector3(0,0,0),
//                new Vector3(1,0,0),
//                new Vector3(1,1,0),
//                new Vector3(0,1,0)
//        };

//    // Indices into the vertex array
//    protected int[] triIndices = new int[] { 0, 2, 1, 3, 2, 0 };

//    // UV coordinates
//    public Vector2[] UVs = new Vector2[] { 
//        new Vector2(0, 0), new Vector2(1, 0),
//        new Vector2(1, 1), new Vector2(0, 1) 
//    };

//    public MeshRenderer meshRenderer;
//    public Mesh mesh;

//    public void Init(GameObject pObject)
//    {
//        mesh = new Mesh();

//        MeshFilter lMeshFilter = pObject.GetComponent<MeshFilter>();
//        if (lMeshFilter == null)
//            lMeshFilter = pObject.AddComponent<MeshFilter>();
//        lMeshFilter.mesh = mesh;

//        MeshRenderer lMeshRenderer = pObject.GetComponent<MeshRenderer>();
//        if (lMeshRenderer == null)
//            lMeshRenderer = pObject.AddComponent<MeshRenderer>();
//        //lMeshRenderer.material = verticalRulerMaterial;

//        meshRenderer = lMeshRenderer;

//        UpdateMesh();

//    }

//    public void UpdateMesh()
//    {
//        mesh.vertices = vertices;
//        mesh.uv = UVs;
//        mesh.triangles = triIndices;
//        mesh.normals = new Vector3[]{
//            new Vector3(0,0,-1),new Vector3(0,0,-1),
//            new Vector3(0,0,-1),new Vector3(0,0,-1)
//        };

//    }

//}


class RaceValueShow:MonoBehaviour
{
    zzPlaneMesh planeMesh = new zzPlaneMesh();
    public Material image;

    public float _rate;
    public float rate
    {
        get
        {
            return _rate;
        }

        set
        {
            _rate = value;
            planeMesh.UVs[3].y = value;
            planeMesh.UVs[2].y = value;


            planeMesh.vertices[3].y = value;
            planeMesh.vertices[2].y = value;

            planeMesh.UpdateMesh();
        }
    }

    void Awake()
    {
        planeMesh.Init(gameObject);
        GetComponent<MeshRenderer>().material = image;
        rate = _rate;
    }
        
    //在编辑模式下显示
    void OnDrawGizmosSelected()
    {
        if( !Application.isPlaying )
        {
            Awake();
        }
    }

}