using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ImageModelCreator:MonoBehaviour
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
                previewTransform.position = modelTransform.position;
                Vector2 lSize = zzSceneImageGUI.getFitSize(modelMaxSize, _image);
                previewTransform.localScale = new Vector3(lSize.x, lSize.y, 1f);
            }
        }
    }

    public Transform modelTransform;
    public Vector2 modelMaxSize;
    public Transform previewTransform;
    public Renderer previewRenderer;

    void Start()
    {
        if (!previewTransform)
            previewTransform = modelTransform;
    }

    //static Vector2 getFitSize(Vector2 pRealSize, Vector2 pMaxSize)
    //{
    //    float lWidth = pRealSize.x;
    //    float lHeigth = pRealSize.y;

    //    float lWidthHeigthRate = lWidth / lHeigth;

    //    if ((pMaxSize.x / pMaxSize.y) > lWidthHeigthRate)
    //    {
    //        pMaxSize.x = lWidthHeigthRate * pMaxSize.y;
    //    }
    //    else
    //    {
    //        pMaxSize.y = pMaxSize.x / lWidthHeigthRate;
    //    }
    //    return pMaxSize;
    //}

    //static Vector2 getFitScale(Vector2 pRealSize, Vector2 pMaxSize)
    //{
    //    var lFitSize = getFitSize(pRealSize, pMaxSize);
    //    Vector2 lOut = new Vector2(lFitSize.x / pRealSize.x, lFitSize.y / pRealSize.y);
    //    return lOut;
    //}

    static Vector2 getFitScale(Vector2 pRealSize, Vector2 pWantSize)
    {
        return new Vector2(pWantSize.x / pRealSize.x, pWantSize.y / pRealSize.y);
    }

    void OnDrawGizmos()
    {
        if(modelTransform)
        {
            Gizmos.color = Color.white;
            Vector3 lModelCenter = modelTransform.position;
            Vector3 lMaxExtent = modelMaxSize / 2.0f;
            Vector3 lPoint1 = new Vector3(lModelCenter.x - lMaxExtent.x,
                lModelCenter.y + lMaxExtent.y,
                lModelCenter.z);

            Vector3 lPoint2 = new Vector3(lModelCenter.x + lMaxExtent.x,
                lModelCenter.y + lMaxExtent.y,
                lModelCenter.z);

            Vector3 lPoint3 = new Vector3(lModelCenter.x + lMaxExtent.x,
                lModelCenter.y - lMaxExtent.y,
                lModelCenter.z);

            Vector3 lPoint4 = new Vector3(lModelCenter.x - lMaxExtent.x,
                lModelCenter.y - lMaxExtent.y,
                lModelCenter.z);

            Gizmos.DrawLine(lPoint1, lPoint2);
            Gizmos.DrawLine(lPoint2, lPoint3);
            Gizmos.DrawLine(lPoint3, lPoint4);
            Gizmos.DrawLine(lPoint4, lPoint1);
        }
    }

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

    public zzModelPainterProcessor modelPainterProcessor;
    public zzModelPainterControl modelPainterControl;

    [ButtonUI("绘制模型")]
    public void drawModel()
    {
        if (nowPainterOutData!=null && !drawTimer)
        {
            if (nowPainterOutData.haveModelData)
            {
                createObject(nowPainterOutData);
            }
            else
            {
                //modelPainterProcessor = GetComponent<FlatModelCreator>();

                modelPainterProcessor.picture = nowPainterOutData.modelImage.resource;
                modelPainterProcessor.thickness = 1.0f;
                drawTimer = gameObject.AddComponent<zzCoroutineTimer>();
                drawTimer.setImpFunction(stepDrawModel);

            }
        }
    }

    zzCoroutineTimer drawTimer;

    public delegate void AddObjectEvent(GameObject pObject);

    static void nullAddObjectEvent(GameObject pObject)
    {

    }

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    void addModelObjects(GameObject[] pModelObjects)
    {
        foreach (var pObject in pModelObjects)
        {
            addObjectEvent(pObject);
        }
    }

    void stepDrawModel()
    {
        if (!modelPainterControl.doStep())
        {
            Destroy(drawTimer);
            toModelData(modelPainterProcessor.models.transform,nowPainterOutData);

            modelPainterControl.clear();
            //Destroy(drawTimer);
            createObject(nowPainterOutData);
        }
    }

    void toModelData(Transform lModelsTransform, PainterOutData pOut)
    {
        lModelsTransform.position = Vector3.zero;
        lModelsTransform.rotation = Quaternion.identity;
        lModelsTransform.localScale = Vector3.one;
        //lModelsTransform.position = modelTransform.position;

        //nowPainterOutData.paintingModels = new GenericResource<PaintingModelData>[lModelsTransform.childCount];
        //nowPainterOutData.transforms = new zzTransform[lModelsTransform.childCount];
        //nowPainterOutData.modelTexture = new GenericResource<Texture2D>[lModelsTransform.childCount];
        nowPainterOutData.modelCount = lModelsTransform.childCount;
        int i = 0;
        foreach (Transform lSub in lModelsTransform)
        {
            var lTexture = (Texture2D)lSub.Find("Render").GetComponent<Renderer>()
                .material.mainTexture;

            var lGameObject = lSub.gameObject;
            var lModelData = PaintingModelData
                .createData(lGameObject, new Vector2(lTexture.width, lTexture.height));

            //useImageMaterial(lPaintingMesh);

            nowPainterOutData.paintingModels[i]
                = GameResourceManager.Main.createModel(lModelData);
                //= new GenericResource<PaintingModelData>( lModelData );
            nowPainterOutData.transforms[i] = new zzTransform(lSub);

            nowPainterOutData.modelTexture[i]
                = GameResourceManager.Main.createImage(lTexture);
            //new GenericResource<Texture2D>(lTexture);
            ++i;
        }

    }

    class PainterOutData
    {
        public GenericResource<Texture2D> modelImage;
        public GenericResource<PaintingModelData>[] paintingModels;
        public zzTransform[] transforms;
        public GenericResource<Texture2D>[] modelTexture;

        public int modelCount
        {
            set
            {
                paintingModels = new GenericResource<PaintingModelData>[value];
                transforms = new zzTransform[value];
                modelTexture = new GenericResource<Texture2D>[value];
            }
        }

        public bool haveModelData
        {
            get{ return paintingModels!=null ;}
        }
    }

    Dictionary<string, PainterOutData> imgPathToData = new Dictionary<string,PainterOutData>();
    PainterOutData nowPainterOutData;

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
            nowPainterOutData = imgPathToData[lFileInfo.ToString()];
        }
        else
        {
            Texture2D lImage = new Texture2D(4, 4, TextureFormat.ARGB32, false);

            using (var lImageFile = new FileStream(fileBrowserDialog.selectedLocation, FileMode.Open))
            {
                BinaryReader lBinaryReader = new BinaryReader(lImageFile);
                lImage.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));

            }
            nowPainterOutData = new PainterOutData();
            nowPainterOutData.modelImage = GameResourceManager.Main.createImage(lImage);
                //new GenericResource<Texture2D>( lImage);
            imgPathToData[lFileInfo.ToString()] = nowPainterOutData;

        }
        image = nowPainterOutData.modelImage.resource;

    }

    /*
    public string saveName = "temp";

        void saveModel(string pName)
        {
            if (!Directory.Exists(pName))
                Directory.CreateDirectory(pName);

            //确定资源共用关系,定义唯一值
            var lTextures = new Dictionary<Texture2D, string>();
            var lPaintingModelData = new Dictionary<PaintingModelData, string>();
            foreach (Transform lModelObjectTransform in sceneManager)
            {
                GameObject lModelObject = lModelObjectTransform.gameObject;
                var lPaintingMesh =  lModelObject.GetComponent<PaintingMesh>();
                var lTexture2D = lPaintingMesh.material.mainTexture as Texture2D;
                if (!lTextures.ContainsKey(lTexture2D))
                    lTextures[lTexture2D] = System.Guid.NewGuid().ToString();

                if (!lPaintingModelData.ContainsKey(lPaintingMesh.modelData))
                    lPaintingModelData[lPaintingMesh.modelData]
                        = System.Guid.NewGuid().ToString();
            }

            //保存图片
            foreach (var lImgSave in lTextures)
            {
                using (var lImageFile = new FileStream(pName + "/" + lImgSave.Value + ".png",
                    FileMode.Create))
                {
                    BinaryWriter lWriter = new BinaryWriter(lImageFile);
                    lWriter.Write(lImgSave.Key.EncodeToPNG());
                }
            }

            //保存模型
            foreach (var lModelDataSave in lPaintingModelData)
            {
                using (var lImageFile = new FileStream(pName + "/" + lModelDataSave.Value + ".pmb",
                    FileMode.Create))
                {
                    BinaryWriter lWriter = new BinaryWriter(lImageFile);
                    lWriter.Write(lModelDataSave.Key.serializeToString());
                }
            }

            //保存场景
            {
                Hashtable lGroupData = new Hashtable();
                ArrayList lObjectList = new ArrayList(sceneManager.objectCount);
                foreach (Transform lTransform in sceneManager)
                {
                    GameObject lModelObject = lTransform.gameObject;
                    var lPaintingMesh = lModelObject.GetComponent<PaintingMesh>();
                    Hashtable lObjectData = new Hashtable();
                    lObjectData["modelData"] = lPaintingModelData[lPaintingMesh.modelData];
                    lObjectData["customImage"]
                        = lTextures[lPaintingMesh.material.mainTexture as Texture2D];
                    lObjectData["position"] = lTransform.position;
                    lObjectData["rotation"] = lTransform.rotation;
                    lObjectData["scale"] = lTransform.localScale;
                    lObjectList.Add(lObjectData);
                }
                lGroupData["objectList"] = lObjectList;
                using (var lGroupFile = new FileStream(pName + "/" + groupFileName,
                    FileMode.Create))
                {
                    BinaryWriter lWriter = new BinaryWriter(lGroupFile);
                    lWriter.Write(zzSerializeString.Singleton.pack(lGroupData));
                }

            }

        }

        GameResourceManager resourceManager = new GameResourceManager();

        void    readGroup(string pPath)
        {
            resourceManager.scanDirectory(pPath);
            string pStringData;
            using (var lGroupFile = new FileStream(pPath + "/" + groupFileName,
                FileMode.Open))
            {
                BinaryReader lReader = new BinaryReader(lGroupFile);
                pStringData = lReader.ReadString();
            }
            readGroupFromTable((Hashtable)zzSerializeString
                .Singleton.unpackToData(pStringData));
        }

        void    readGroupFromTable(Hashtable pData )
        {
            ArrayList lObjectList = (ArrayList)pData["objectList"];
            List<GameObject> lModelList = new List<GameObject>();

            int i = 0;
            foreach (Hashtable lObjectData in lObjectList)
            {
                GameObject lGameObject = new GameObject("readMoedl"+i);
                Transform lTransform = lGameObject.transform;
                string lModelDataName = (string)lObjectData["modelData"];
                string lCustomImageName = (string)lObjectData["customImage"];
                var lPaintingModelData = resourceManager.getModelFromGuid(lModelDataName);
                PaintingMesh lPaintingMesh = PaintingMesh.create(lGameObject, lPaintingModelData);
                lPaintingMesh.useImageMaterial(resourceManager.getImageFromGuid(lCustomImageName));

                //先设置Transform,在增加凸碰撞模型时,有时会长生错误
                lTransform.position = (Vector3)lObjectData["position"];
                lTransform.rotation = (Quaternion)lObjectData["rotation"];
                lTransform.localScale = (Vector3)lObjectData["scale"];

                lModelList.Add(lGameObject);
                ++i;
                //addModelObjects()
                //lGameObject.GetComponent<Rigidbody>().isKinematic = !inPlaying;

            }

            addModelObjects(lModelList.ToArray());
        }
        */

    void createObject(PainterOutData pPainterOutData)
    {
        var lModelParentObject = new GameObject();
        var lModelsTransform = lModelParentObject.transform;

        List<GameObject> lModelList = new List<GameObject>();

        for (int i=0;i<pPainterOutData.paintingModels.Length;++i)
        {
            GameObject lGameObject = GameSystem.Singleton.createObject("paintingObject");
            lModelList.Add(lGameObject);
            var lPaintingMesh
                = PaintingMesh.create(lGameObject,pPainterOutData.paintingModels[i]);
            //lPaintingMesh.useImageMaterial(pPainterOutData.modelImage);
            lGameObject.GetComponent<RenderMaterialProperty>().imageResource = pPainterOutData.modelTexture[i];
            pPainterOutData.transforms[i].setToTransform(lGameObject.transform);
            lGameObject.transform.parent = lModelsTransform;
        }

        var lPreviewLocalScale = previewTransform.localScale;
        Vector2 lSize = new Vector2(lPreviewLocalScale.x, lPreviewLocalScale.y);
        var lFitScale = getFitScale(modelPainterProcessor.modelsSize, lSize);
        var lScale = new Vector3(lFitScale.x, lFitScale.y, 1.0f);
        lModelsTransform.localScale = lScale;
        lModelsTransform.rotation = previewTransform.rotation;
        lModelsTransform.position = previewTransform.position;

        addModelObjects(lModelList.ToArray());

        Destroy(lModelParentObject);

    }

}