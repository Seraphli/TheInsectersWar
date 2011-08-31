using UnityEngine;
using System.Collections;

//读取场景的接口
public class SceneLoader : MonoBehaviour
{

    /// <summary>
    /// 绝对路径
    /// </summary>
    [SerializeField]
    string _loadPath;

    public string loadPath
    {
        get { return _loadPath; }
        set { _loadPath = value; }
    }

    System.Action afterLoadEvent;

    public void addAfterLoadEventReceiver(System.Action pReceiver)
    {
        afterLoadEvent += pReceiver;
    }

    void nullEvent() { }

    void Start()
    {
        if (afterLoadEvent == null)
            afterLoadEvent = nullEvent;
    }

    public void load()
    {
        GameResourceManager.Main.fullPath = _loadPath;
        GameResourceManager.Main.load();
        afterLoadEvent();
    }
}