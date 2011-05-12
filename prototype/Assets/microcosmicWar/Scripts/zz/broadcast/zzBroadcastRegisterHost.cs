using UnityEngine;
using System.Collections;

public class zzBroadcastRegisterHost : zzNetworkHost
{
    public zzBroadcast sender;

    public void Awake()
    {
        sender.enabled = false;
    }

    System.Action afterRegister;

    public override void addRegisterSucceedReceiver(System.Action pReceiver)
    {
        afterRegister += pReceiver;
    }

    public override void addRegisterFailReceiver(System.Action pReceiver){}

    public override void RegisterHost(zzHostInfo pHostInfo)
    {
        afterRegister();
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