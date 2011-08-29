using UnityEngine;
using System.Collections.Generic;

public class zzGroupViewIDManager:MonoBehaviour
{

    zzAllocateViewIDManager.IdToSetNetworkId<int> intToSetNetworkId
        = new zzAllocateViewIDManager.IdToSetNetworkId<int>();
    
    int objectCount = 0;
    System.Action groupFinishEvent;

    //客户端已经设置的数量
    int haveSetCount = 0;

    List<NetworkView> networkViewList = new List<NetworkView>();


    public void setGroupBegin(System.Action groupFinishEventReceiver)
    {
        print("setGroupBegin:" + Network.peerType);
        groupFinishEvent += groupFinishEventReceiver;
    }

    public void getViewID(int pObjectID, NetworkView pNetworkView)
    {
        //print("getViewID:" + pNetworkView.name + ":"
            //+ pObjectID);
        pNetworkView.enabled = false;
        networkViewList.Add(pNetworkView);

        if(intToSetNetworkId.getViewID(pObjectID,
            (x) =>
            {
                pNetworkView.viewID = x;
                //print("delegate:"+x.ToString() + ":" + pNetworkView.name);
            }))
            addSetCountAndCheck();
    }

    //server
    public void setViewID(int pObjectID, NetworkView pNetworkView)
    {
        //print("setViewID:" + pNetworkView.name + ":"
            //+ pObjectID + " " + pNetworkView.viewID.ToString());

        pNetworkView.enabled = false;
        networkViewList.Add(pNetworkView);

        ++objectCount;
        networkView.RPC("groupIdSet", RPCMode.OthersBuffered,
            pObjectID, pNetworkView.viewID);
    }

    void addSetCountAndCheck()
    {
        ++haveSetCount;
        finishedCheck();
    }

    [RPC]
    void groupIdSet(int pObjectID, NetworkViewID pNetworkViewID)
    {
        //print("intToViewID:"+pNetworkViewID.ToString() + ":" + pObjectID);
        if (intToSetNetworkId.setViewID(pObjectID, pNetworkViewID))
            addSetCountAndCheck();
    }

    //server
    public void setGroupEnd()
    {
        print("server.&&&&&&setGroupEnd()&&&&&&&");
        networkView.RPC("RPCSetObjectCount", RPCMode.OthersBuffered, objectCount);
    }

    //client
    [RPC]
    void RPCSetObjectCount(int pCount)
    {
        print("RPCSetObjectCount:" + pCount + " haveSetCount:" + haveSetCount);
        objectCount = pCount;
        finishedCheck();
    }

    void finishedCheck()
    {
        if (objectCount == haveSetCount)
        {
            print("objectCount == haveSetCount,finished");
            networkView.RPC("RPCGroupFinish", RPCMode.Server);
            groupFinish();
        }
    }

    void groupFinish()
    {
        string lInfo = "groupFinish:\n";
        foreach (var lNetworkView in networkViewList)
        {
            lNetworkView.enabled = true;
            lInfo += lNetworkView.name + " ID:" + lNetworkView.viewID + "\n";
        }
        print(lInfo);
        networkViewList.Clear();
        playerList.Clear();
        groupFinishEvent();
    }

    void playersFinishCheck()
    {
        print("playersFinishCheck: playerList.Count:" + playerList.Count
            + " connections.Length:" + Network.connections.Length);
        if (playerList.Count == Network.connections.Length)
            groupFinish();
    }

    HashSet<NetworkPlayer> playerList = new HashSet<NetworkPlayer>();

    void OnPlayerDisconnected(NetworkPlayer pPlayer)
    {
        playerList.Remove(pPlayer);
        playersFinishCheck();
    }

    //server
    [RPC]
    void RPCGroupFinish(NetworkMessageInfo pInfo)
    {
        var lPlayer = pInfo.sender;
        print(lPlayer + " RPCGroupFinish");
        playerList.Add(lPlayer);
        playersFinishCheck();
    }
}