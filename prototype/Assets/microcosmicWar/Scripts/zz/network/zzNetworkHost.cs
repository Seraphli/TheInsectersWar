using UnityEngine;
using System.Collections;

[System.Serializable]
public class zzHostInfo
{
    public string gameType;
    public string gameName;
    public string comment;

}

public abstract class zzNetworkHost:MonoBehaviour
{
    public abstract void RegisterHost(zzHostInfo pHostInfo);

    public abstract void UnregisterHost();

}