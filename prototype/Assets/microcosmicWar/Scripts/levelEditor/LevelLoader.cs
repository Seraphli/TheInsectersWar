using UnityEngine;

public class LevelLoader:MonoBehaviour
{
    public string settingObject;

    public string rootPath;

    string toPath(string pLevelName)
    {
        string lOut = pLevelName;
        var rootNameLength = rootPath.Length;
        if (rootNameLength > 0)
        {
            if (pLevelName.Length <= rootNameLength + 1
                ||pLevelName.Substring(0,rootNameLength+1)!=(rootPath+"/"))
            {
                lOut = rootPath + "/" + pLevelName;
            }
        }
        return lOut;
    }

    public string levelName
    {
        get
        {
            return _levelName;
        }
        set
        {
            _levelName = toPath(value);
        }
    }

    [SerializeField]
    string _levelName;

    public SceneLoader sceneLoader;

    public void tryLoad(string pUnitySceneName)
    {
        if(System.IO.Directory.Exists(_levelName))
        {
            DontDestroyOnLoad(gameObject);
            Application.LoadLevel(pUnitySceneName);
        }
    }

    public void setPath()
    {
        var lSettingObject = GameObject.Find(settingObject);
        if (lSettingObject&&lSettingObject.GetComponent<LevelLoader>())
        {
            _levelName = lSettingObject.GetComponent<LevelLoader>()._levelName;
            //Destroy(lSettingObject);
        }
        sceneLoader.loadPath = _levelName;
    }

}