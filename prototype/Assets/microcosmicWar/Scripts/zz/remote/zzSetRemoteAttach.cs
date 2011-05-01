using UnityEngine;

public class zzSetRemoteAttach:MonoBehaviour
{
    public void setData(Transform pParent)
    {
        networkView.RPC("RPCSetRemoteParent", RPCMode.Others, pParent.networkView.viewID);
    }

    [RPC]
    void RPCSetRemoteAttach(NetworkViewID pID)
    {
        transform.parent = NetworkView.Find(pID).transform;
        transform.position = Vector3.zero;
    }
}