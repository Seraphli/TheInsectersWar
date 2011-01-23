
using UnityEngine;
using System.Collections;

public class HeroNetView : MonoBehaviour
{

    public zzCharacter character;
    public ActionCommandControl actionCommandControl;
    public GameObject owner;
    //FIXME_VAR_TYPE transform;

    void Start()
    {
        //GameObject lOwner  = transform.parent.gameObject;
        if (!owner)
            return;
        Hero lHero = owner.GetComponentInChildren<Hero>();
        character = lHero.getCharacter();
        actionCommandControl = owner.GetComponentInChildren<ActionCommandControl>();

        if (Network.peerType != NetworkPeerType.Disconnected && networkView.isMine)
        {
            networkView.RPC("RPCSetOwner", RPCMode.Others, owner.networkView.viewID);
        }

    }

    public void setOwner(GameObject pOwner)
    {
        gameObject.name = "NS";
        transform.parent = pOwner.transform;
        owner = pOwner;

    }

    [RPC]
    public void RPCSetOwner(NetworkViewID pOwnerID)
    {
        GameObject lOwnerHeroObject = NetworkView.Find(pOwnerID).gameObject;

        gameObject.name = "NS";
        transform.parent = lOwnerHeroObject.transform;
        owner = lOwnerHeroObject;

        //-------------------------------------------
        Hero lHero = owner.GetComponentInChildren<Hero>();
        character = lHero.getCharacter();
        actionCommandControl = owner.GetComponentInChildren<ActionCommandControl>();
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        character.OnSerializeNetworkView(stream, info);
        actionCommandControl.OnSerializeNetworkView(stream, info);
    }

    //void  Update (){
    //}
}