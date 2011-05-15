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
                zzAllocateViewIDManager.setViewID(name, lID);
            }
            else
            {
                zzAllocateViewIDManager.getViewID(name, setNetView);
            }
        }
    }

    void setNetView(NetworkViewID pNetworkViewID)
    {
        networkView.viewID = pNetworkViewID;
    }
}