using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameSystem:MonoBehaviour
{
    //public zzGUILibTreeInfo materialTreeUIInfo;

    //public Transform createObjectTransform;

    [System.Serializable]
    public class InfoElement
    {
        public string name;
        public Texture2D image;
        public string showName;
        public Object data;
        public bool isShow = true;
    }

    [System.Serializable]
    public class InfoNode
    {
        public string showName;
        public Texture2D image;
        public Texture2D expandedImage;
        public InfoNode[] nodes = new InfoNode[0];
        public InfoElement[] elements = new InfoElement[0];

        //public zzGUILibTreeNode GUITreeNode
        //{
        //    get
        //    {
        //        zzGUILibTreeNode lOut = new zzGUILibTreeNode();
        //        lOut.name = showName;
        //        lOut.image = image;

        //        var lGUIElements = new zzGUILibTreeElement[elements.Length];
        //        for(int i=0;i<elements.Length;++i)
        //        {
        //            var lInfoElement = elements[i];
        //            var lGUIElement = new zzGUILibTreeElement();
        //            lGUIElement.name = lInfoElement.showName;
        //            lGUIElement.image = lInfoElement.image;
        //            lGUIElement.stringData = lInfoElement.name;
        //            lGUIElements[i] = lGUIElement;
        //        }
        //        lOut.elements = lGUIElements;

        //        var lGUINode = new zzGUILibTreeNode[nodes.Length];
        //        for (int i = 0; i < nodes.Length; ++i)
        //        {
        //            lGUINode[i] = nodes[i].GUITreeNode;
        //        }
        //        lOut.nodes = lGUINode;

        //        return lOut;
        //    }
        //}
    }

    public InfoNode prefabInfoTree;

    public InfoNode settingInfoTree;

    [System.Serializable]
    public class PrefabInfo
    {
        public string name;
        public string showName;
        public GameObject prefab;
    }

    Dictionary<string, GameObject> nameToPrefab = new Dictionary<string,GameObject>();

    [System.Serializable]
    public class RenderMaterialInfo
    {
        public string name;
        public string showName;
        public Material material;
    }

    public PrefabInfo[] PrefabInfoList;

    public GameObject createObject(string pTypeName,Vector3 position,Quaternion rotation)
    {
        var lOut =(GameObject)Instantiate(nameToPrefab[pTypeName], position, rotation);
        lOut.GetComponent<ObjectPropertySetting>().TypeName = pTypeName;
        return lOut;
    }

    public GameObject createObject(string pTypeName)
    {
        GameObject lPrefab;
        if(nameToPrefab.TryGetValue(pTypeName,out lPrefab))
        {
            var lOut = (GameObject)Instantiate(lPrefab);
            lOut.GetComponent<ObjectPropertySetting>().TypeName = pTypeName;
            return lOut;
        }
        Debug.LogError("not supported object type:" + pTypeName);
        return null;
    }

    public RenderMaterialInfo[] renderMaterialInfoList;

    RenderMaterialResource[] renderMaterialResources;

    Dictionary<string, RenderMaterialResource> nameToRenderMaterial
        = new Dictionary<string, RenderMaterialResource>();

    public RenderMaterialResource  getRenderMaterial(string pName)
    {
        return nameToRenderMaterial[pName];
    }

    public RenderMaterialResource getRenderMaterial(int index)
    {
        return renderMaterialResources[index];
    }

    static protected GameSystem singletonInstance;

    public GameObject controlPointLinePrefab;

    public GameObject paintingObjectPrefab;

    public float paintingRenderObjectZ = 0f;

    public Vector3 getRenderObjectPos(Vector3 pPos)
    {
        pPos.z = paintingRenderObjectZ;
        return pPos;
    }

    public static GameSystem Singleton
    {
        get { return singletonInstance; }
    }

    public Material defaultMaterial
    {
        get{return _defaultMaterial;}
    }

    [SerializeField]
    Material _defaultMaterial;

    void creatNameToPrefab(InfoNode pInfoNode)
    {
        var lElements = pInfoNode.elements;
        for (int i = 0; i < lElements.Length; ++i)
        {
            var lElement = lElements[i];
            if (lElement.data)
            {
                nameToPrefab[lElement.name] = (GameObject)lElement.data;
            }
        }

        var lNodes = pInfoNode.nodes;
        for (int i = 0; i < lNodes.Length; ++i)
        {
            creatNameToPrefab(lNodes[i]);
        }
    }

    void OnDestroy()
    {
        singletonInstance = null;
    }

    void Awake()
    {
        if (singletonInstance)
        {
            Debug.LogError("GameSystem.singletonInstance");
        }
        singletonInstance = this;

        renderMaterialResources = new RenderMaterialResource[renderMaterialInfoList.Length];

        for(int i=0;i<renderMaterialInfoList.Length;++i)
        {
            var lResource = new RenderMaterialResource();
            lResource.resourceType = ResourceType.builtin;
            lResource.materialName = renderMaterialInfoList[i].name;
            lResource.material = renderMaterialInfoList[i].material;
            renderMaterialResources[i] = lResource;

            nameToRenderMaterial[lResource.materialName] = lResource;
        }

        //默认材质
        nameToRenderMaterial[""] = renderMaterialResources[0];
        //_defaultMaterial = renderMaterialResources[0].material;
        if (!_defaultMaterial)
            _defaultMaterial = new Material(Shader.Find("Transparent/Diffuse"));

        //foreach (var lPrefabInfo in PrefabInfoList)
        //{
        //    nameToPrefab[lPrefabInfo.name] = lPrefabInfo.prefab;
        //}
        creatNameToPrefab(prefabInfoTree);
        //nameToPrefab["paintingObject"] = paintingObjectPrefab;

    }

}