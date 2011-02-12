
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerList:MonoBehaviour
{
    public Dictionary<string, string> serverList = new Dictionary<string, string>();

    public void OnBeginReceive()
    {
        serverList.Clear();
    }

    public void OnReceive(string data, string IP)
    {
        serverList[IP] = data;
    }
}