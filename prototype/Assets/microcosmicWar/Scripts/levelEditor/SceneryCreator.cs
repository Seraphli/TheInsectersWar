using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SceneryCreator:MonoBehaviour
{

    [SerializeField]
    Texture2D _image;

    [ImageUI(verticalDepth = 0)]
    public Texture2D image
    {
        get { return _image; }
        set
        {
            _image = value;
            if (previewRenderer)
                previewRenderer.material.mainTexture = _image;
            if (_image)
            {
                var lPosition = previewTransform.position;
                var lDrawPosition = drawTransform.position;
                lPosition.x = lDrawPosition.x;
                lPosition.y = lDrawPosition.y;
                previewTransform.position = lPosition;
                Vector2 lSize = zzSceneImageGUI.getFitSize(drawMaxSize, _image);
                previewTransform.localScale = new Vector3(lSize.x, lSize.y, 1f);
            }
        }
    }

    public Transform drawTransform;
    public Vector2 drawMaxSize;
    public Transform previewTransform;
    public Renderer previewRenderer;



    GameObject fileBrowserDialogObject;
    zzFileBrowserDialog fileBrowserDialog;
    public string lastLocation = "";

    [ButtonUI("打开png图片文件")]
    public void fileOpenDialog()
    {
        if (!fileBrowserDialog)
        {
            fileBrowserDialog = zzFileBrowserDialog.createDialog();
            fileBrowserDialog.addFileFilter("", new string[] { "*.png", "*.jpg", "*.jpeg" });
            fileBrowserDialog.relativePosition = new Rect(0.0f, 0.0f, 6.0f / 7.0f, 4.0f / 5.0f);
            fileBrowserDialog.useRelativePosition = new zzGUIRelativeUsedInfo(false, false, true, true);
            fileBrowserDialog.horizontalDockPosition = zzGUIDockPos.center;
            fileBrowserDialog.verticalDockPosition = zzGUIDockPos.center;
            fileBrowserDialog.selectedLocation = lastLocation;
            fileBrowserDialog.fileSelectedCallFunc = OnReadImage;

        }

    }

    class OutData
    {
        public OutData(Texture2D lImage, string pExtension)
        {
            width = lImage.width;
            height = lImage.height;

            if (!Mathf.IsPowerOfTwo(lImage.width)
                || !Mathf.IsPowerOfTwo(lImage.height))
            {
                TextureScale.Point(lImage,
                    Mathf.NextPowerOfTwo(lImage.width), Mathf.NextPowerOfTwo(lImage.height));
            }
            resource =GameResourceManager.Main.createImage(lImage);
            resource.extension = pExtension;
        }
        public int width;
        public int height;
        public GenericResource<Texture2D> resource;
    }

    Dictionary<string, OutData> imgPathToData = new Dictionary<string, OutData>();
    OutData nowOutData;


    void OnReadImage(zzInterfaceGUI pGUI)
    {
        lastLocation = fileBrowserDialog.selectedLocation;
        readImage(lastLocation);
    }

    void readImage(string pPath)
    {
        FileInfo lFileInfo = new FileInfo(pPath);
        if (imgPathToData.ContainsKey(lFileInfo.ToString()))
        {
            nowOutData = imgPathToData[lFileInfo.ToString()];
        }
        else
        {
            Texture2D lImage = new Texture2D(4, 4, TextureFormat.ARGB32, false);

            using (var lImageFile = new FileStream(fileBrowserDialog.selectedLocation, FileMode.Open))
            {
                BinaryReader lBinaryReader = new BinaryReader(lImageFile);
                lImage.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));
            }
            var  lExtension = System.IO.Path.GetExtension(fileBrowserDialog.selectedLocation);
            lExtension = lExtension.Substring(1, lExtension.Length - 1);
            nowOutData = new OutData(lImage, lExtension);
            imgPathToData[lFileInfo.ToString()] = nowOutData;

        }
        image = nowOutData.resource.resource;
    }

    
    [ButtonUI("创建布景")]
    public void createScenery()
    {
        if(nowOutData!=null)
        {
            createObject(nowOutData);
        }
    }

    public delegate void AddObjectEvent(GameObject pObject);

    static void nullAddObjectEvent(GameObject pObject)
    {

    }

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    void createObject(OutData pPainterOutData)
    {
        GameObject lGameObject = GameSystem.Singleton.createObject("sceneryObject");
        lGameObject.GetComponent<RenderMaterialProperty>().imageResource = pPainterOutData.resource;
        var lTransform = lGameObject.transform;
        lTransform.localScale = previewTransform.localScale;
        lTransform.rotation = previewTransform.rotation;
        lTransform.position = previewTransform.position;
        addObjectEvent(lGameObject);
    }
}