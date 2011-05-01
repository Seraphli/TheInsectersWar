using UnityEngine;

public class zzSetRemoteLocalPos:MonoBehaviour
{
    public void setData(Vector3 pPos)
    {
        networkView.RPC("RPCSetRemoteLocalPos", RPCMode.Others, pPos);
    }

    [RPC]
    void RPCSetRemoteLocalPos(Vector3 pPos)
    {
        transform.localPosition = pPos;
    }
}