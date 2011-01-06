
using UnityEngine;
using System.Collections;

public class zzSceneImageGUI:MonoBehaviour
{
    //[SerializeField]
    //Texture2D _image;

    public Vector2 _size;

    public Texture2D image
    {
        get { return GetComponent<MeshRenderer>().material.mainTexture as Texture2D; }

        set
        {
            GetComponent<MeshRenderer>().material.mainTexture = value;
            resizeMesh();
            //image = value;
        }
    }

    void resizeMesh()
    {
        GetComponent<MeshFilter>().mesh.vertices
            = planeMesh.resize(getFitSize(), zzPlaneMesh.PivotType.center);
    }

    void Awake()
    {
        getMeshFilter().mesh = getMesh();
    }

    Mesh getMesh()
    {
        initSharedMaterial();
        Mesh lMesh = new Mesh();
        initMesh(lMesh);
        return lMesh;

    }

    Vector2 getFitSize()
    {
        Texture2D lImage = gameObject.GetComponent<MeshRenderer>()
            .material.mainTexture as Texture2D;
        if (lImage)
            return getFitSize(_size, lImage);
        return _size;
    }

    static Vector2 getFitSize(Vector2 pSize, Texture2D pImage)
    {
        float lWidth = (float)pImage.width;
        float lHeigth = (float)pImage.height;

        float lWidthHeigthRate = lWidth / lHeigth;

        if ((pSize.x / pSize.y) > lWidthHeigthRate)
        {
            pSize.x = lWidthHeigthRate * pSize.y;
        }
        else
        {
            pSize.y = pSize.x / lWidthHeigthRate;
        }
        return pSize;
    }

    void initMesh(Mesh pMesh)
    {
        planeMesh.resize(getFitSize(), zzPlaneMesh.PivotType.center);
        planeMesh.initMesh(pMesh);
    }

    MeshFilter      getMeshFilter()
    {
        MeshFilter lMeshFilter = GetComponent<MeshFilter>();
        if (lMeshFilter == null)
            lMeshFilter = gameObject.AddComponent<MeshFilter>();
        return lMeshFilter;

    }

    MeshRenderer    getRenderer()
    {
        MeshRenderer lMeshRenderer = GetComponent<MeshRenderer>();
        if (lMeshRenderer == null)
            lMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        return lMeshRenderer;
    }

    Material    initSharedMaterial()
    {
        var lMeshRenderer = getRenderer();
        Material lMaterial = lMeshRenderer.sharedMaterial;
        if (lMaterial==null)
        {
            lMaterial = new Material(Shader.Find("Transparent/Diffuse"));
            lMeshRenderer.sharedMaterial = lMaterial;
        }

        return lMaterial;
    }

    zzPlaneMesh planeMesh = new zzPlaneMesh();


    //在编辑模式下显示
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            getMeshFilter().sharedMesh = getMesh();

        }
    }

    //// The vertices of mesh
    //// 3--2
    //// |  |
    //// 0--1
    //protected Vector3[] vertices = new Vector3[]{
    //            new Vector3(0,0,0),
    //            new Vector3(1,0,0),
    //            new Vector3(1,1,0),
    //            new Vector3(0,1,0)
    //    };

    //// Indices into the vertex array
    //protected int[] triIndices = new int[] { 0, 2, 1, 3, 2, 0 };

    //// UV coordinates
    //protected Vector2[] UVs = new Vector2[] { 
    //    new Vector2(0, 0), new Vector2(1, 0),
    //    new Vector2(1, 1), new Vector2(0, 1) 
    //};

    //Vector3[] createStandardMesh(float pWidth, float pHeigth)
    //{
    //    vertices = new Vector3[]{
    //            new Vector3(-pWidth/2,-pHeigth/2,0),
    //            new Vector3(pWidth/2,-pHeigth/2,0),
    //            new Vector3(pWidth/2,pHeigth/2,0),
    //            new Vector3(-pWidth/2,pHeigth/2,0)
    //    };
    //    return vertices;
    //}
}