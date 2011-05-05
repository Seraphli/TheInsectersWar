using UnityEngine;
using System.Collections;

public class zzBroadcastRegisterHost : zzNetworkHost
{
    public zzBroadcast sender;

    public void Awake()
    {
        sender.enabled = false;
    }

    public override void RegisterHost(zzHostInfo pHostInfo)
    {
        Hashtable lSentedData = new Hashtable();
        lSentedData["gameName"] = pHostInfo.gameName;
        lSentedData["gameType"] = pHostInfo.gameType;
        lSentedData["comment"] = pHostInfo.comment;
        lSentedData["GUID"] = pHostInfo.guid;
        lSentedData["port"] = pHostInfo.port;
        sender.sentedData = zzSerializeString.Singleton.pack(lSentedData);
        sender.enabled = true;
    }

    public override void UnregisterHost()
    {
        sender.enabled = false;
    }
}