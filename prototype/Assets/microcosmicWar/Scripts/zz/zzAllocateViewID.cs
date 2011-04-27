using UnityEngine;

public class zzAllocateViewID:MonoBehaviour
{
    void Awake()
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            if (Network.isServer)
            {
                var lID = Network.AllocateViewID();
                networkView.viewID = lID;
                zzAllocateViewIDManager.Singleton
                    .setViewID(name, lID);
            }
            else
            {
                zzAllocateViewIDManager.Singleton.getViewID(name, setNetView);
            }
        }
    }

    void setNetView(NetworkViewID pNetworkViewID)
    {
        networkView.viewID = pNetworkViewID;
    }
}