﻿
using UnityEngine;
using System.Collections;

public class HeroNetView : MonoBehaviour
{

    public zzCharacter character;
    public ActionCommandControl actionCommandControl;
    public GameObject owner;
    public Life life;
    public SoldierModelSmoothMove soldierModelSmoothMove;
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
        soldierModelSmoothMove = owner.GetComponent<SoldierModelSmoothMove>();

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
        soldierModelSmoothMove = owner.GetComponent<SoldierModelSmoothMove>();
    }

    //class NetData
    //{
    //    public NetData(Vector3 p1, char p2, double pTimestamp)
    //    {
    //        vector = p1;
    //        command = p2;
    //        timestamp = pTimestamp;
    //    }
    //    public Vector3 vector;
    //    public char command;
    //    public double timestamp;
    //}
    //System.Collections.Generic.Queue<NetData> netDataQueue
    //    = new System.Collections.Generic.Queue<NetData>();

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        //life.OnSerializeNetworkView(stream, info);
        //if (stream.isReading && Random.value > 0.5f)
        //    return;
        //actionCommandControl.OnSerializeNetworkView(stream, info);
        //character.OnSerializeNetworkView2D(stream, info,
        //    actionCommandControl.getCommand(), life.isAlive());
        //------------------------------------------------------------
        Transform lTransform = character.characterController.transform;
        Vector3 lVectorData = Vector3.zero;
        //---------------------------------------------------
        char lCommand = (char)System.Convert.ToByte(actionCommandControl.commandValue & 0xff);

        //---------------------------------------------------
        stream.Serialize(ref lCommand);
        //---------------------------------------------------
        if (stream.isWriting)
        {
            lVectorData = lTransform.position;
            lVectorData.z = character.yVelocity;
        }
        //---------------------------------------------------
        stream.Serialize(ref lCommand);
        stream.Serialize(ref lVectorData);
        //---------------------------------------------------
        if (stream.isReading)
        {
            double lTimestamp = info.timestamp;
            //{
            //    double lDelay = 0.2;
            //    float lPacketLossRatio = 0.1f;
            //    NetData lData = null;
            //    if (Random.value > lPacketLossRatio)
            //        netDataQueue.Enqueue(new NetData(lVectorData, lCommand, info.timestamp));
            //    while (netDataQueue.Count > 0
            //        && Network.time - netDataQueue.Peek().timestamp > lDelay)
            //    {
            //        lData = netDataQueue.Dequeue();
            //    }
            //    if (lData != null)
            //    {
            //        lCommand = lData.command;
            //        lVectorData = lData.vector;
            //        lTimestamp = lData.timestamp;
            //    }
            //    else
            //        return;
            //}
            soldierModelSmoothMove.beginMove();
            actionCommandControl.commandValue = ((byte)lCommand) & 0xff;
            var pUnitActionCommand = actionCommandControl.getCommand();
            character.yVelocity = lVectorData.z;
            lVectorData.z = 0f;
            lTransform.position = lVectorData;

            var lDeltaTime = (float)(Network.time - lTimestamp);
            character.update2D(pUnitActionCommand, UnitFace.getValue(pUnitActionCommand.face),
                life.isAlive(), lDeltaTime * Time.timeScale);
            //lastUpdateTime = Time.time;
            character.lastUpdateTime = Time.time;
            soldierModelSmoothMove.endMove();

        }
        //------------------------------------------------------------
    }

    //void  Update (){
    //}
}