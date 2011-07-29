using UnityEngine;

public class WMGameConfig:MonoBehaviour
{
    [System.Serializable]
    public class WMGameConfigData
    {
        public WMGameConfigData(){}
        public WMGameConfigData(WMGameConfigData pOther)
        {
            version = int.Parse(pOther.versionString);
            mapFolderName = pOther.mapFolderName;
        }
        public string versionString;
        public int version;
        public string mapFolderName;
    }

    public static int version
    {
        get { return _data.version; }
    }

    public static string mapFolderName
    {
        get { return _data.mapFolderName; }
    }

    public static bool checkMapAvailable(string pMap)
    {
        if (System.IO.Directory.Exists(_data.mapFolderName +
            System.IO.Path.DirectorySeparatorChar + pMap))
        {
            return true;
        }
        return false;
    }

    static WMGameConfigData _data;

    [SerializeField]
    WMGameConfigData data = new WMGameConfigData();

    public static WMGameConfig Singleton
    {
        get
        {
            return singletonInstance;
        }
    }

    static protected WMGameConfig singletonInstance = null;

    void OnDestroy()
    {
        singletonInstance = null;
    }

    [ContextMenu("Print Data")]
    void printData()
    {
        print("version:" + _data.version+"\n"
            + "Map Folder Name:" + _data.mapFolderName);
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("have singletonInstance");
        singletonInstance = this;
        if (_data == null)
        {
            _data = new WMGameConfigData(data);
        }
    }
}