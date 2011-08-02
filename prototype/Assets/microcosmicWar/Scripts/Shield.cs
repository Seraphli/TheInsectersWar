using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour 
{
    public int adversaryWeaponLayer = 11;   //阻挡的子弹的层

    void OnTriggerEnter(Collider other)
    {
        //if (!zzCreatorUtility.isHost())
        //    return;

        if (other.gameObject.layer == adversaryWeaponLayer)
        {
            //使用将生命值设为0的方式,消除子弹
            Life lLife = Life.getLifeFromTransform(other.transform);
            if (lLife.networkView && Network.isClient)
                return;
            lLife.makeDead(); 
        }
    }


    public void setOwner(GameObject pOwner)
    {
        _setOwner(pOwner);
        //if (Network.peerType != NetworkPeerType.Disconnected)
        //    gameObject.networkView.RPC("RPCSetOwner", RPCMode.Others, pOwner.networkView.viewID);
    }

    void _setOwner(GameObject pOwner)
    {
        transform.parent = pOwner.transform;
        transform.localPosition = Vector3.zero;
    }

    //[RPC]
    //void RPCSetOwner(NetworkViewID pOwner)
    //{
    //    _setOwner(NetworkView.Find(pOwner).gameObject);
    //}

}