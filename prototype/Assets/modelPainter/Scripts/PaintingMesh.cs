using UnityEngine;
using System.Collections;

public class PaintingModelResource
{
    public string modelName;
    public PaintingModelData modelData;
    public ResourceType resourceType;
}

public class PaintingModelData
{
    //public PaintingModelData()
    //{
    //    Debug.Log("new PaintingModelData");
    //}

    public struct ModelData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

        public void setTransform(Transform pTransform)
        {
            localPosition = pTransform.localPosition;
            localRotation = pTransform.localRotation;
            localScale = pTransform.localScale;
        }

        public void setToTransform(Transform pTransform)
        {
            pTransform.localPosition = localPosition;
            pTransform.localRotation = localRotation;
            pTransform.localScale = localScale;
        }

        public Mesh mesh;
    }

    public ModelData renderMesh;
    public ModelData[] colliderMeshes;

    //用来计算UV
    public Vector2 pictureSize;

    static public Hashtable serializeToTable(ModelData pModelData)
    {
        var lData = new Hashtable();
        lData["localPosition"] = pModelData.localPosition;
        lData["localRotation"] = pModelData.localRotation;
        lData["localScale"] = pModelData.localScale;
        lData["mesh"] = serializeToTable(pModelData.mesh);
        return lData;
    }

    static public Hashtable serializeToTable(Mesh pMesh)
    {
        var lData = new Hashtable();
        lData["vertices"] = pMesh.vertices;
        //lData["uv"] = pMesh.uv;
        lData["triangles"] = pMesh.triangles;

        return lData;
    }

    static public ModelData createModelDataFromTable(Hashtable pData, Vector2 pPictureSize)
    {
        ModelData lOut;
        lOut.localPosition = (Vector3)pData["localPosition"];
        lOut.localRotation = (Quaternion)pData["localRotation"];
        lOut.localScale = (Vector3)pData["localScale"];
        lOut.mesh = createMeshFromTable((Hashtable)pData["mesh"], pPictureSize);
        return lOut;
    }

    static public Mesh  createMeshFromTable(Hashtable pData,Vector2 pPictureSize)
    {
        Mesh lOut = new Mesh();
        var lVertices = (Vector3[])pData["vertices"];
        lOut.vertices = lVertices;
        lOut.triangles = (int[])pData["triangles"];
        lOut.uv = zzFlatMeshUtility.verticesCoordToUV(lOut.vertices,
            zzFlatMeshUtility.getUvScaleFromImgSize(pPictureSize));
        if(lVertices.Length>0
            && lVertices[0].z == lVertices[lVertices.Length - 1].z)
            lOut.normals = zzFlatMeshUtility.getSingleFaceNormals(lOut.vertices.Length);
        else
            lOut.normals = zzFlatMeshUtility.getNormals(lOut.vertices.Length);
        return lOut;
    }

    public string serializeToString()
    {
        var lData = new Hashtable();
        lData["pictureSize"] = pictureSize;
        lData["renderMesh"] = serializeToTable(renderMesh);
        var lColliderMeshesData = new ArrayList(colliderMeshes.Length);
        foreach (var lColliderMeshe in colliderMeshes)
        {
            lColliderMeshesData.Add(serializeToTable(lColliderMeshe));
        }
        lData["colliderMeshes"] = lColliderMeshesData;
        return zzSerializeString.Singleton.pack(lData);
    }

    static public PaintingModelData createDataFromString(string pStringData)
    {
        PaintingModelData lOut = new PaintingModelData();
        Hashtable lData = (Hashtable)zzSerializeString.Singleton.unpackToData(pStringData);
        lOut.pictureSize = (Vector2)lData["pictureSize"];
        lOut.renderMesh =
            createModelDataFromTable((Hashtable)lData["renderMesh"], lOut.pictureSize);

        ArrayList lColliderMeshesData = (ArrayList)lData["colliderMeshes"];
        lOut.colliderMeshes = new ModelData[lColliderMeshesData.Count];
        for (int i = 0; i < lColliderMeshesData.Count;++i )
        {
            lOut.colliderMeshes[i] =
            createModelDataFromTable((Hashtable)lColliderMeshesData[i], lOut.pictureSize);
        }
        return lOut;
    }

    static public PaintingModelData createData(GameObject pModelObject, Vector2 pPictureSize)
    {
        var lPaintingMesh = new PaintingModelData();
        var lMeshFilter = pModelObject.transform.Find("Render").GetComponent<MeshFilter>();
        lPaintingMesh.renderMesh.mesh = lMeshFilter.mesh;
        lPaintingMesh.renderMesh.setTransform(lMeshFilter.transform);
        var lMeshCollider = pModelObject.GetComponentsInChildren<MeshCollider>();
        //var lColliderMeshes = new Mesh[lMeshCollider.Length]; 
        var lColliderMeshes = new ModelData[lMeshCollider.Length];
        for (int i = 0; i < lMeshCollider.Length; ++i)
        {
            lColliderMeshes[i].mesh = lMeshCollider[i].sharedMesh;
            lColliderMeshes[i].setTransform(lMeshCollider[i].transform);
        }
        lPaintingMesh.colliderMeshes = lColliderMeshes;

        lPaintingMesh.pictureSize = pPictureSize;

        return lPaintingMesh;

    }

}

public class PaintingMesh : zzEditableObject
{
    public ModelResourceInfo _modelResource
        = new ModelResourceInfo();

    [zzSerialize]
    public ModelResourceInfo modelResource
    {
        get { return _modelResource; }
        set 
        {
            _modelResource = value;
            updateModel();
            //modelData = value.resource;
            //modelName = value.resourceID;
            //_resourceType = value.resourceType;
        }
    }

    new void Awake()
    {
        var lRenderMaterialProperty = gameObject.GetComponent<RenderMaterialProperty>();
        if(lRenderMaterialProperty)
            lRenderMaterialProperty.paintRenderer
                = transform.Find("Render").GetComponent<MeshRenderer>();
        base.Awake();
    }

    public static PaintingMesh create(GameObject lObject, GenericResource<PaintingModelData> pResource)
    {
        PaintingMesh lOut = lObject.GetComponent<PaintingMesh>();
        lOut.modelResource = pResource;
        return lOut;
    }

    public static PaintingMesh  create(GameObject lObject,PaintingModelData pData)
    {
        //print("GameObject" + lObject.name);
        Transform lTransform = lObject.transform;
        var lOut = lObject.GetComponent<PaintingMesh>();

        //lOut.modelData = pData;
        {
            //图形模型
            //var lRenderObject = new GameObject("Render");
            var lRenderObjectTransform = lObject.transform.Find("Render");
            var lRenderObject = lRenderObjectTransform.gameObject;
            lOut.paintRenderer = lRenderObject.GetComponent<MeshRenderer>();
            lRenderObject.GetComponent<MeshFilter>().mesh = pData.renderMesh.mesh;
            lRenderObject.transform.parent = lTransform;
            pData.renderMesh.setToTransform(lRenderObject.transform);

        }

        int i = 0;
        foreach (var lColliderMeshes in pData.colliderMeshes)
        {
            //物理模型
            var lColliderObject = new GameObject("Collider" + i);
            lColliderObject.transform.parent = lTransform;
            var lMeshCollider = lColliderObject.AddComponent<MeshCollider>();
            lMeshCollider.convex = true;
            lMeshCollider.sharedMesh = lColliderMeshes.mesh;
            lColliderMeshes.setToTransform(lColliderObject.transform);
            ++i;
        }
        return lOut;
    }

    //ResourceType _resourceType;

    public PaintingModelData modelData;
    public string modelName;

    [zzSerializeIn]
    public string resourceID
    {
        //get { return modelName; }
        set
        {
            modelResource.resourceID = value;
            if (modelResource.resourceType != ResourceType.unknown)
                updateModel();
        }
    }

    void updateModel()
    {
        create(gameObject, modelResource.resource.resource);
    }

    [zzSerializeIn]
    public string resourceTypeID
    {
        //get { return _resourceType.ToString(); }
        set
        {
            modelResource.resourceType = (ResourceType)System.Enum.Parse(typeof(ResourceType), value);
            if (modelResource.resourceID.Length > 0)
                updateModel();
        }
    }
    //----------------------------------------------

    [SerializeField]
    Vector2 _extraTextureOffset = Vector2.zero;

    public Vector2 extraTextureOffset
    {
        get { return _extraTextureOffset; }
        set
        {
            _extraTextureOffset = value;
            material.mainTextureOffset
                = material.mainTextureOffset + _extraTextureOffset;
        }
    }

    [SerializeField]
    Vector2 _extraTextureScale = new Vector2(1.0f, 1.0f);

    public Vector2 extraTextureScale
    {
        get { return _extraTextureScale; }
        set
        {
            _extraTextureScale = value;
            var lMainTextureScale = material.mainTextureScale;
            lMainTextureScale.Scale(_extraTextureScale);
            material.mainTextureScale = lMainTextureScale;
        }
    }

    public string materialName;
    public bool useCustomImage = false;
    public string imageName;

    public Material sharedMaterial;

    [SerializeField]
    private Material _material;

    public Material material
    {
        get { return _material; }
        set 
        {
            _material = value;
            _meshRenderer.material = value;
        }
    }

    [SerializeField]
    private MeshRenderer _meshRenderer;

    public MeshRenderer paintRenderer
    {
        get { return _meshRenderer; }
        set 
        {
            _meshRenderer = value;
            //_material = value.material;
        }
    }

    public void useImageMaterial(Texture2D pTexture)
    {
        var lMaterial = new Material(Shader.Find("Diffuse"));
        lMaterial.mainTexture = pTexture;
        sharedMaterial = null;
        material = lMaterial;
        useCustomImage = true;
    }
}