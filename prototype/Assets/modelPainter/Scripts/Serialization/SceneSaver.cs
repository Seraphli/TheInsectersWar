using UnityEngine;
using System.Collections;

//保存场景的接口
public class SceneSaver:MonoBehaviour
{
    [SerializeField]
    string saveFolder = "Scene";

    /// <summary>
    /// 相对路径
    /// </summary>
    [SerializeField]
    string _savePath;

    public string savePath
    {
        get { return _savePath; }
        set { _savePath = value; }
    }

    public void save()
    {
        GameResourceManager.Main.path = saveFolder+"/"+_savePath;
        GameResourceManager.Main.save();
    }
}