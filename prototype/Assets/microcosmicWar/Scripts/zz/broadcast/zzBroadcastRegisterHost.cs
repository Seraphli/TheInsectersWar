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
        //gameType = pHostInfo.gameType;
        //gameName = pHostInfo.gameName;
        sender.sentedData = pHostInfo.comment;
        sender.enabled = true;
    }

    public override void UnregisterHost()
    {
        sender.enabled = false;
    }
}