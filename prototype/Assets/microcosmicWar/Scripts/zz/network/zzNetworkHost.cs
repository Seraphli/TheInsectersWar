using UnityEngine;
using System.Collections;

[System.Serializable]
public class zzHostInfo
{
    public zzHostInfo()
    {
        _guid = System.Guid.NewGuid().ToString();
    }

    public string gameType;
    public string gameName;
    public string comment;

    //使用的端口,客户端获取的值不一定一样
    public int port;

    [SerializeField]
    string _guid;

    public string guid
    {
        get
        {
            return _guid;

        }
    }
}

public struct zzHostData
{
    public string gameType;
    public string gameName;
    public string IP;
    public int port;
    public string guid;
    public string comment;
}

public abstract class zzNetworkHost:MonoBehaviour
{
    public abstract void RegisterHost(zzHostInfo pHostInfo);

    public abstract void UnregisterHost();

    public abstract void addRegisterSucceedReceiver(System.Action pReceiver);

    public abstract void addRegisterFailReceiver(System.Action pReceiver);

}