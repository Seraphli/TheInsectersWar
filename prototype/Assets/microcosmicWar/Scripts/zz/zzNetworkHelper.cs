using UnityEngine;
using System.Collections;

public class zzNetworkHelper:MonoBehaviour
{
    System.Action callWhenClient;
    System.Action callWhenServer;

    public void addAsClientCall(System.Action pCall)
    {
        callWhenClient += pCall;
    }

    public void addAsServerCall(System.Action pCall)
    {
        callWhenServer += pCall;
    }

    IEnumerator Start()
    {
        while (Network.peerType == NetworkPeerType.Disconnected)
        {
            yield return null;
        }
        if (Network.isServer && callWhenServer != null)
            callWhenServer();
        if (Network.isClient && callWhenClient != null)
            callWhenClient();
    }
}