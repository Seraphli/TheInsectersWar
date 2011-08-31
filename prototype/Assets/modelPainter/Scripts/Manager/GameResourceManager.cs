using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class GenericResourceInfo
{
    public ResourceType resourceType = ResourceType.unknown;

    [zzSerialize]
    public string resourceTypeID
    {
        get { return resourceType.ToString(); }
        set
        {
            resourceType = (ResourceType)System.Enum.Parse(typeof(ResourceType), value);
        }
    }

    [SerializeField]
    string _resourceID;

    [zzSerialize]
    public string resourceID
    {
        get { return _resourceID; }
        set { _resourceID = value; }
    }

    public string extension = "";
}

public class GenericResource<T> 
{
    public GenericResource()
    {
        info = new GenericResourceInfo();
    }

    public GenericResource(T t)
    {
        info = new GenericResourceInfo();
        resource = t;
        resourceType = ResourceType.realTime;
        resourceID = generateID();
    }

    public GenericResource(T t, string pResourceID)
    {
        info = new GenericResourceInfo();
        resource = t;
        resourceType = ResourceType.realTime;
        resourceID = pResourceID;
    }

    public GenericResource(T t, GenericResourceInfo pInfo)
    {
        info = pInfo;
        resource = t;
    }

    public T resource;
    public GenericResourceInfo info;
    public ResourceType resourceType
    {
        set { info.resourceType = value; }
        get { return info.resourceType; }
    }
    public string resourceID
    {
        set { info.resourceID=value; }
        get { return info.resourceID; }
    }
    public string extension 
    {
        set { info.extension = value; }
        get { return info.extension; }
    }
    public static string generateID()
    {
        return System.Guid.NewGuid().ToString();
    }

    //public void destroyResource()
    //{
    //    Object.Destroy(resource);
    //}
}

//public class GenericResourceManager<T>
//{
//    Dictionary<string, GenericResource<T>> IdToResource
//        = new Dictionary<string,GenericResource<T>>();

//    public GenericResource<T>  getResource(string pResourceId)
//    {
//        if (IdToResource.ContainsKey(pResourceId))
//            return IdToResource[pResourceId];
//        else
//            return null;
//    }

//    public void setResource(GenericResource<T> pResource,string pResourceId)
//    {
//        IdToResource[pResourceId] = pResource;
//    }

//    public GenericResource<T> addResource(GenericResource<T> pResource,string pResourceId)
//    {
//        pResource.resourceID = pResourceId;
//        IdToResource[pResourceId] = pResource;
//        return pResource;
//    }

//    public void clearResource()
//    {
//        foreach (var lResourceDic in IdToResource)
//        {
//            lResourceDic.Value.destroyResource();
//        }
//    }

//    //public GenericResource<T> addResource(GenericResource<T> pResource)
//    //{
//    //    return addResource(pResource,System.Guid.NewGuid().ToString());
//    //}
//}

public enum ResourceType
{
    unknown = 0,

    //内建
    builtin,

    ////工程
    //project,

    ////外部文件
    //external,

    //实时生成
    realTime,
}

//----------------------------------------------------------

public class SceneModelReader:SceneReader<PaintingModelData>
{
    public SceneModelReader()
    {
        fileExtensionFilter = "*.pmb";
        extensionToReadFunc["pmb"] = readData;
    }

    public PaintingModelData readData(string pFullName)
    {
        PaintingModelData lOut;
        using (var lFile = new FileStream(pFullName, FileMode.Open,
            FileAccess.Read, FileShare.Read))
        {
            using (var lStreamReader = new StreamReader(lFile))
            {
                lOut = PaintingModelData.createDataFromString(lStreamReader.ReadToEnd());
            }
        }
        return lOut;
    }

    public override void clear(GenericResource<PaintingModelData> pData)
    {
        var lResource = pData.resource;
        Object.Destroy(lResource.renderMesh.mesh);
        foreach (var lColliderMeshe in lResource.colliderMeshes)
        {
            Object.Destroy(lColliderMeshe.mesh);
        }
    }
}

public class SceneModelWriter:SceneWriter<PaintingModelData>
{
    public SceneModelWriter()
    {
        defaultFileExtension = "pmb";
        extensionToWriteFunc[defaultFileExtension] = writeModelData;
    }

    public void writeModelData(PaintingModelData pData, string pName)
    {
        using (var lFile = new FileStream(pName + ".pmb", FileMode.Create))
        {
            using(StreamWriter lWriter = new StreamWriter(lFile))
            {
                lWriter.AutoFlush = true;
                lWriter.Write(pData.serializeToString());
            }
        }
    }
}
//---------------------------------------------------------------



public class SceneImageReader : SceneReader<Texture2D>
{
    public bool compress = false;
    public SceneImageReader()
    {
        fileExtensionFilter = "*.gif|*.jpg|*.jpeg|*.png";
        extensionToReadFunc["png"] = readData;
        extensionToReadFunc["jpg"] = readData;
        extensionToReadFunc["jpeg"] = readData;
        extensionToReadFunc["gif"] = readGifData;
    }

    public Texture2D readData(string pFullName)
    {
        Texture2D lOut = new Texture2D(4, 4, TextureFormat.ARGB32, false);
        using (var lFile = new FileStream(pFullName, FileMode.Open,
            FileAccess.Read, FileShare.Read))
        {
            using(BinaryReader lBinaryReader = new BinaryReader(lFile))
            {
                var lMemoryStream = memoryStream;
                int lFileLength = (int)lFile.Length;
                lMemoryStream.SetLength(lFileLength);
                lMemoryStream.Position=0;
                var lBuffer = lMemoryStream.GetBuffer();
                lBinaryReader.Read(lBuffer, 0, lFileLength);
                lOut.LoadImage(lBuffer);
            }
        }
        if (compress)
            lOut.Compress(true);
        return lOut;
    }

    public Texture2D readGifData(string pFullName)
    {
        Texture2D lOut = new Texture2D(4, 4, TextureFormat.ARGB32, false);
        using (var lImage = System.Drawing.Image.FromFile(pFullName))
        {
            var lMemoryStream = memoryStream;
            lMemoryStream.SetLength(0);
            lMemoryStream.Position = 0;
            lImage.Save(lMemoryStream, System.Drawing.Imaging.ImageFormat.Png);
            lOut.LoadImage(lMemoryStream.GetBuffer());
        }
        if (compress)
            lOut.Compress(true);
        return lOut;
    }

    public override void clear(GenericResource<Texture2D> pData)
    {
        Object.Destroy(pData.resource);
    }

    public new void beginReadScene( string rootDirName)
    {
        base.beginReadScene(rootDirName);
        Debug.Log("SceneImageReader.beginReadScene");
    }

    public new void endReadScene()
    {
        if(_memoryStream!=null)
        {
            _memoryStream.Dispose();
            _memoryStream = null;
        }
        Debug.Log("SceneImageReader.endReadScene");
    }

    MemoryStream _memoryStream;

    MemoryStream memoryStream
    {
        get
        {
            if (_memoryStream == null)
                _memoryStream = new MemoryStream(4096 * 2048 * 4 + 1024 * 1024);
            return _memoryStream;
        }

        set
        {
            _memoryStream = value;
        }
    }
}

public class SceneImageWriter : SceneWriter<Texture2D>
{
    public SceneImageWriter()
    {
        defaultFileExtension = "png";
        extensionToWriteFunc[defaultFileExtension] = writePngData;
        extensionToWriteFunc["jpg"] = writePngData;
        extensionToWriteFunc["jpeg"] = writePngData;
    }

    public void writePngData(Texture2D pData, string pName)
    {
        using (var lFile = new FileStream(pName+".png", FileMode.Create))
        {
            using(BinaryWriter lWriter = new BinaryWriter(lFile))
            {
                lWriter.Write(pData.EncodeToPNG());
            }
        }
    }

    //public void writeJpgData(Texture2D pData, string pFullName)
    //{

    //    using (var lBitmap = new System.Drawing.Bitmap(new MemoryStream(pData.EncodeToPNG())))
    //    {
    //        lBitmap.Save(pFullName, System.Drawing.Imaging.ImageFormat.Jpeg);
    //    }
    //}
}


public abstract class SceneReader<T>
{
    protected delegate T ReadDataFunc( string pFullName );

    protected Dictionary<string, ReadDataFunc> extensionToReadFunc
        = new Dictionary<string, ReadDataFunc>();

    string rootDictionary;

    Dictionary<string, string> nameToPath = new Dictionary<string, string>();
    Dictionary<string, GenericResource<T>> nameToData = new Dictionary<string, GenericResource<T>>();

    protected string fileExtensionFilter = "*.pmb";

    void scanDirectory(string pDirectory)
    {
        DirectoryInfo lDirectory = new DirectoryInfo(pDirectory);
        var lExtList = fileExtensionFilter.Split('|');
        foreach (var lExt in lExtList)
        {
            foreach (var lFile in lDirectory.GetFiles(lExt))
            {
                nameToPath[getFileName(lFile)] = lFile.FullName;
            }
        }
    }

    public static string getFileName(FileInfo pFile)
    {
        return Path.GetFileNameWithoutExtension(pFile.FullName);
    }

    public void beginReadScene( string rootDirName)
    {
        rootDictionary = rootDirName;
        scanDirectory(rootDirName);
    }

    public GenericResource<T> createData(T t,string pID)
    {
        GenericResource<T> lOut = new GenericResource<T>(t, pID);
        nameToData[pID] = lOut;
        return lOut;
    }

    public GenericResource<T> getData(string pID)
    {
        GenericResource<T> lOut = null;
        if (!nameToData.TryGetValue(pID, out lOut))
        {
            var lExtension = Path.GetExtension(nameToPath[pID]);
            //去除扩展名中的 点
            lExtension = lExtension.Substring(1, lExtension.Length - 1);
            //nameToData[pID] = lOut;
            lOut = createData(
                extensionToReadFunc[lExtension](nameToPath[pID]),
                pID);
            lOut.extension = lExtension;
        }
        return lOut;
    }


    public void clear()
    {
        foreach (var lDic in nameToData)
        {
            clear(lDic.Value);
        }
    }

    public abstract void clear(GenericResource<T> pData);

    public void endReadScene()
    {
    }

}
public abstract class SceneWriter<T>
{
    protected delegate void WriteDataFunc(T pData, string pFullName);
    string rootDictionary;
    protected string defaultFileExtension = "pmb";
    protected Dictionary<string, WriteDataFunc> extensionToWriteFunc
        = new Dictionary<string,WriteDataFunc>();

    HashSet<string> savedModelID = new HashSet<string>();

    public void beginSaveScene(string rootDirName)
    {
        rootDictionary = rootDirName;
    }

    public void saveData(GenericResource<T> pResourceData)
    {
        string lDataID = pResourceData.resourceID;
        if (!savedModelID.Contains(lDataID))
        {
            string lExtension = pResourceData.extension;
            if (lExtension.Length > 0)
                saveData(pResourceData.resource, lDataID, lExtension);
            else
                saveData(pResourceData.resource, lDataID, defaultFileExtension);
            savedModelID.Add(lDataID);
        }
    }

    public void saveData(T pData, string pDataID, string pFileExtension)
    {
        if (!File.Exists(rootDictionary + "/" + pDataID + "." + pFileExtension))
            writeData(pData, rootDictionary + "/" + pDataID , pFileExtension);
    }

    public void writeData(T pData, string pName, string pFileExtension)
    {
        //Debug.Log("pFileExtension:"+pFileExtension);
        //foreach (var lDic in extensionToWriteFunc)
        //{
        //    Debug.Log(lDic.Key);
        //}
        extensionToWriteFunc[pFileExtension](pData, pName);
    }

    public void endSaveScene()
    {
        savedModelID.Clear();
    }

}

public class GameResourceManager:MonoBehaviour
{

    void OnDestroy()
    {
        singletonInstance = null;
        sceneModelReader.clear();
        sceneImageReader.clear();
    }

    public bool compressImage = false;

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("GameResourceManager");
        singletonInstance = this;
        path = path;
        sceneImageReader.compress = compressImage;
    }

    static protected GameResourceManager singletonInstance;

    public static GameResourceManager Main
    {
        get { return singletonInstance; }
    }

    SceneModelReader sceneModelReader = new SceneModelReader();
    SceneModelWriter sceneModelWriter = new SceneModelWriter();

    SceneImageReader sceneImageReader = new SceneImageReader();
    SceneImageWriter sceneImageWriter = new SceneImageWriter();

    public SerializeScene serializeScene;

    [SerializeField]
    public string _path;

    public string path
    {
        get { return _path; }
        set 
        {
            _path = value;
            fullPath = Application.dataPath + "/../" + _path;
        }
    }

    public GenericResource<PaintingModelData> createModel(PaintingModelData pData)
    {
        return sceneModelReader.createData(pData,GenericResource<PaintingModelData>.generateID());
    }

    public GenericResource<Texture2D> createImage(Texture2D pData)
    {
        return sceneImageReader.createData(pData, GenericResource<Texture2D>.generateID());
    }

    public GenericResource<PaintingModelData> getModel(string pID)
    {
        return sceneModelReader.getData(pID);
    }

    public GenericResource<Texture2D> getImage(string pID)
    {
        return sceneImageReader.getData(pID);
    }

    public string sceneFileName = "main.zzScene";

    public string fullPath;

    public string backupFolder;
    [ContextMenu("backup")]
    public void backupSceneFile()
    {
        if (!Directory.Exists(backupFolder))
            Directory.CreateDirectory(backupFolder);
        var lSceneFileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + "." + sceneFileName;
        var lSceneSave = zzSerializeString.Singleton.pack(serializeScene.serializeTo());
        using (var lSceneFile = new FileStream(backupFolder + "/" + lSceneFileName,
            FileMode.Create))
        {
            //print(lSceneSave);
            StreamWriter lStreamWriter = new StreamWriter(lSceneFile);
            lStreamWriter.AutoFlush = true;
            lStreamWriter.Write(lSceneSave);
        }
    }

    [ContextMenu("save")]
    public void save()
    {
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        sceneModelWriter.beginSaveScene(fullPath);
        sceneImageWriter.beginSaveScene(fullPath);
        var lSceneSave = zzSerializeString.Singleton.pack(serializeScene.serializeTo());
        using (var lSceneFile = new FileStream(fullPath + "/" + sceneFileName,
            FileMode.Create))
        {
            //print(lSceneSave);
            using(StreamWriter lStreamWriter = new StreamWriter(lSceneFile))
            {
                lStreamWriter.AutoFlush = true;
                lStreamWriter.Write(lSceneSave);
            }
        }
        foreach (Transform lObject in serializeScene.enumerateObject)
        {
            var lPaintingMesh = lObject.GetComponent<PaintingMesh>();
            if (lPaintingMesh)
                sceneModelWriter.saveData((GenericResource<PaintingModelData>)lPaintingMesh.modelResource);

            var lRenderMaterial = lObject.GetComponent<RenderMaterialProperty>();
            if (lRenderMaterial && lRenderMaterial.imageResource.resourceType == ResourceType.realTime)
                sceneImageWriter.saveData((GenericResource<Texture2D>)lRenderMaterial.imageResource);

            var lCombineImagePlane = lObject.GetComponent<CombineImagePlane>();
            if(lCombineImagePlane)
            {
                foreach (var lImage in lCombineImagePlane.imageInfo.images)
                {
                    sceneImageWriter.saveData((GenericResource<Texture2D>)lImage);
                }
            }
        }
        sceneModelWriter.endSaveScene();
        sceneImageWriter.endSaveScene();
    }

    [ContextMenu("load")]
    public void load()
    {
        if (File.Exists(fullPath))
            fullPath = (new FileInfo(fullPath)).DirectoryName;

        sceneModelReader.beginReadScene(fullPath);
        sceneImageReader.beginReadScene(fullPath);
        string lSceneSave;
        using (var lSceneFile = new FileStream(fullPath + "/" + sceneFileName,
            FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (StreamReader lStreamReader = new StreamReader(lSceneFile))
            {
                lSceneSave = lStreamReader.ReadToEnd();
            }
        }
        serializeScene.serializeFrom(zzSerializeString.Singleton.unpackToData(lSceneSave));
        sceneModelReader.endReadScene();
        sceneImageReader.endReadScene();
    }
}
