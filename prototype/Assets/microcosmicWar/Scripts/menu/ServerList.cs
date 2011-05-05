
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerList:MonoBehaviour
{
    public Dictionary<string, zzHostData> serverList =new Dictionary<string, zzHostData>();

    public void OnBeginReceive()
    {
        serverList.Clear();
    }

    public void OnReceive(zzHostData data)
    {
        serverList[data.guid] = data;
    }
}