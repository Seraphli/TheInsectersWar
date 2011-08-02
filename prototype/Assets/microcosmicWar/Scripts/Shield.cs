using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour 
{
    public int adversaryWeaponLayer = 11;   //�赲���ӵ��Ĳ�

    void OnTriggerEnter(Collider other)
    {
        //if (!zzCreatorUtility.isHost())
        //    return;

        if (other.gameObject.layer == adversaryWeaponLayer)
        {
            //ʹ�ý�����ֵ��Ϊ0�ķ�ʽ,�����ӵ�
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