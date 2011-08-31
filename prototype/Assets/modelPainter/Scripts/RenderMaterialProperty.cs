using UnityEngine;
using System.Collections;


public class RenderMaterialResource
{
    public string materialName;
    public Material material;
    public Texture2D image;
    public ResourceType resourceType;
}

public class RenderMaterialProperty:MonoBehaviour
{
    public RenderMaterialResourceInfo _imageResource
        = new RenderMaterialResourceInfo();

    //public void useImageMaterial(Texture2D pTexture)
    //{
    //    var lMaterial = new Material(GameSystem.Singleton.defaultMaterial);
    //    lMaterial.mainTexture = pTexture;
    //    _sharedMaterial = null;
    //    material = lMaterial;
    //}

    [zzSerialize]
    public RenderMaterialResourceInfo imageResource
    {
        get { return _imageResource; }
        set 
        { 
            _imageResource = value;
            //_resourceType = value.resourceType;
            //materialName = value.resourceID;
            //useImageMaterial(value.resource.resource);
            updateMaterial();
        }
    }


    [SerializeField]
    private Material _sharedMaterial;

    public Material sharedMaterial
    {
        get { return _sharedMaterial; }
        set 
        {
            //共享材质 必须先被赋值
            _sharedMaterial = value;
            material = new Material(value);
        }
    }

    [SerializeField]
    private Material _material;

    //[SerializeField]
    //ResourceType _resourceType = ResourceType.unknown;

    //public ResourceType resourceType
    //{
    //    get { return _resourceType; }
    //}

    //void Awake()
    //{
    //    if (materialName.Length == 0)
    //        _resourceType = ResourceType.unknown;
    //    if (_material)
    //        _material = new Material(_material);
    //}

    void Start()
    {
        //if (_resourceType== ResourceType.builtin)
        //{
        //    sharedMaterial = _meshRenderer.material;
        //}
        if (_imageResource.resourceType == ResourceType.unknown)
        {
            _imageResource.resourceType = ResourceType.builtin;
            updateMaterial();
        }
    }

    void releaseMaterial()
    {
        if (_material)
        {
            Destroy(_material);

        }
    }

    //--------------------------------------------------------------

    [SerializeField]
    Vector2 _extraTextureOffset = Vector2.zero;

    [zzSerialize]
    public Vector2 extraTextureOffset
    {
        get { return _extraTextureOffset; }
        set
        {
            _extraTextureOffset = value;
            //material.mainTextureOffset
            //    = material.mainTextureOffset + _extraTextureOffset;
        }
    }

    [SerializeField]
    Vector2 _extraTextureScale = new Vector2(1.0f, 1.0f);

    [zzSerialize]
    public Vector2 extraTextureScale
    {
        get { return _extraTextureScale; }
        set
        {
            _extraTextureScale = value;
            //var lMainTextureScale = material.mainTextureScale;
            //lMainTextureScale.Scale(_extraTextureScale);
            //material.mainTextureScale = lMainTextureScale;
        }
    }

    void updateTextureTransform()
    {
        if (sharedMaterial)
        {
            material.mainTextureOffset
                = sharedMaterial.mainTextureOffset + _extraTextureOffset;
            var lMainTextureScale = sharedMaterial.mainTextureScale;
            lMainTextureScale.Scale(_extraTextureScale);
            material.mainTextureScale = lMainTextureScale;
        }
        else
        {
            material.mainTextureOffset = _extraTextureOffset;
            material.mainTextureScale = _extraTextureScale;
        }
    }


    //public string materialName;

    [zzSerializeIn]
    public string resourceID
    {
        //get { return materialName; }
        set
        {
            _imageResource.resourceID = value;
            if (_imageResource.resourceType != ResourceType.unknown)
                updateMaterial();
        }
    }

    [zzSerializeIn]
    public string resourceTypeID
    {
        //get { return _resourceType.ToString(); }
        set
        {
            _imageResource.resourceType = (ResourceType)System.Enum.Parse(typeof(ResourceType), value);
            if (_imageResource.resourceID.Length > 0)
                updateMaterial();
        }
    }

    public void updateMaterial()
    {
        switch (_imageResource.resourceType)
        {
            case ResourceType.builtin:
                //setMaterial(GameSystem.Singleton.getRenderMaterial(_imageResource.resourceID));
                sharedMaterial = (Material)_imageResource;
                visibleChangedCheck(_imageResource.resourceID);
                break;
            case ResourceType.realTime:
                {
                    try
                    {
                        sharedMaterial = GameSystem.Singleton.defaultMaterial;
                        material.mainTexture = _imageResource.resource.resource;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(e);
                        _imageResource.resourceType = ResourceType.builtin;
                        _imageResource.resourceID = string.Empty;
                        sharedMaterial = RenderMaterialResourceInfo.exceptionMaterial;
                    }
                    break;
                }
        }
    }

    //public string imageName;

    public Material material
    {
        get { return _meshRenderer.material; }
        set
        {
            releaseMaterial();
            _material = value;
            _meshRenderer.material = value;
            updateTextureTransform();
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
        }
    }

    public string invisibleMaterialName = "undefined";

    public string invisibleLayerName = "undefined";

    void visibleChangedCheck(string lNewMaterialName)
    {
        var lInvisibleLayer = LayerMask.NameToLayer(invisibleLayerName);
        if (_meshRenderer.gameObject.layer == lInvisibleLayer
            || lNewMaterialName == invisibleMaterialName)
        {
            if (lNewMaterialName == invisibleMaterialName)
                _meshRenderer.gameObject.layer = lInvisibleLayer;
            else
                _meshRenderer.gameObject.layer = 0;
        }
    }

    public void setMaterial(RenderMaterialResource pRenderMaterialInfo)
    {
        visibleChangedCheck( pRenderMaterialInfo.materialName);

        sharedMaterial = pRenderMaterialInfo.material;
        _imageResource.resourceID = pRenderMaterialInfo.materialName;
        _imageResource.resourceType = pRenderMaterialInfo.resourceType;


    }

    void OnDestroy()
    {
        releaseMaterial();
    }

    static RenderMaterialGUI renderMaterialGUI= new RenderMaterialGUI();

    public static IPropertyGUI PropertyGUI
    {
        get 
        {
            return renderMaterialGUI;
        }
    }

}

public class RenderMaterialGUI:IPropertyGUI
{
    Vector2 scrollPosition;

    public override void OnPropertyGUI(MonoBehaviour pObject)
    {
        var lRenderMaterialProperty = (RenderMaterialProperty)pObject;
        GUILayout.Label("材质列表");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(100));
        var lSelected = drawMaterialSelectList();
        GUILayout.EndScrollView();

        if (lSelected!=null
            &&lSelected.material != lRenderMaterialProperty.sharedMaterial)
        {
            lRenderMaterialProperty.setMaterial(lSelected);
        }
    }

    RenderMaterialResource drawMaterialSelectList()
    {
        RenderMaterialResource lOut = null;
        var lRenderMaterialInfoList = GameSystem.Singleton.renderMaterialInfoList;
        for (int i = 0; i < lRenderMaterialInfoList.Length;++i )
        {
            if (GUILayout.Button(lRenderMaterialInfoList[i].showName))
                lOut = GameSystem.Singleton.getRenderMaterial(i);
        }
        return lOut;
    }
}