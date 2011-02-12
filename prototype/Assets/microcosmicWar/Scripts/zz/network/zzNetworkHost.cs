using UnityEngine;
using System.Collections;

public abstract class zzNetworkHost:MonoBehaviour
{
    public abstract void RegisterHost(zzHostInfo pHostInfo);

    public abstract void UnregisterHost();

}