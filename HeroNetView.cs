
using UnityEngine;
using System.Collections;

public class HeroNetView : MonoBehaviour
{

    public zzCharacter character;
    public ActionCommandControl actionCommandControl;
    public GameObject owner;
    public Life life;
    //FIXME_VAR_TYPE transform;

    void Start()
    {
        //GameObject lOwner  = transform.parent.gameObject;
        if (!owner)
            return;
        Hero lHero = owner.GetComponentInChildren<Hero>();
        character = lHero.getCharacter();
        actionCommandControl = owner.GetComponentInChildren<ActionCommandControl>();
        life = owner.GetComponent<Life>();

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
        life = owner.GetComponent<Life>();
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        //life.OnSerializeNetworkView(stream, info);
        if (stream.isReading && Random.value > 0.5f)
            return;
        actionCommandControl.OnSerializeNetworkView(stream, info);
        //character.OnSerializeNetworkView2D(stream, info,
        //    actionCommandControl.getCommand(), life.isAlive());
        //------------------------------------------------------------
        Transform lTransform = character.characterController.transform;
        Vector3 lData = Vector3.zero;
        //---------------------------------------------------
        if (stream.isWriting)
        {
            lData = lTransform.position;
            lData.z = character.yVelocity;
        }
        //---------------------------------------------------
        stream.Serialize(ref lData);
        //---------------------------------------------------
        if (stream.isReading)
        {
            var pUnitActionCommand = actionCommandControl.getCommand();
            character.yVelocity = lData.z;
            lData.z = 0f;
            var lSoldierModelSmoothMove = owner.GetComponent<SoldierModelSmoothMove>();
            lSoldierModelSmoothMove.beginMove();
            lTransform.position = lData;
            lSoldierModelSmoothMove.endMove();

            var lDeltaTime = (float)(Network.time - info.timestamp);
            character.update2D(pUnitActionCommand, UnitFace.getValue(pUnitActionCommand.face),
                life.isAlive(), lDeltaTime * Time.timeScale);
            //lastUpdateTime = Time.time;

        }
        //------------------------------------------------------------
        character.lastUpdateTime = Time.time;
    }

    //void  Update (){
    //}
}