using UnityEngine;

public class zzSetRemoteParent:MonoBehaviour
{
    public void setData(Transform pParent)
    {
        networkView.RPC("RPCSetRemoteParent", RPCMode.Others, pParent.networkView.viewID);
    }

    [RPC]
    void RPCSetRemoteParent(NetworkViewID pID)
    {
        transform.parent = NetworkView.Find(pID).transform;
    }
}