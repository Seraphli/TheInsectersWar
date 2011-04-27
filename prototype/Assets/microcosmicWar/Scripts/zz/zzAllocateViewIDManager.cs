using UnityEngine;
using System.Collections.Generic;


public class zzAllocateViewIDManager:MonoBehaviour
{
    public class IdToSetNetworkId<T>
    {

        public void getViewID(T pObjectID, SetViewIDFunc pSetFunc)
        {
            if (idToViewID.ContainsKey(pObjectID))
            {
                pSetFunc(idToViewID[pObjectID]);
                idToViewID.Remove(pObjectID);
            }
            else
            {
                idToSetFunc[pObjectID] = pSetFunc;
            }
        }

        public void setViewID(T pObjectID, NetworkViewID pNetworkViewID)
        {
            if(idToSetFunc.ContainsKey(pObjectID))
            {
                idToSetFunc[pObjectID](pNetworkViewID);
                idToSetFunc.Remove(pObjectID);
            }
            else
            {
                idToViewID[pObjectID] = pNetworkViewID;
            }
        }

        Dictionary<T, SetViewIDFunc> idToSetFunc = new Dictionary<T,SetViewIDFunc>();

        Dictionary<T, NetworkViewID> idToViewID = new Dictionary<T,NetworkViewID>();

    }

    public static zzAllocateViewIDManager Singleton
    {
        get { return singletonInstance; }
    }

    static zzAllocateViewIDManager singletonInstance;

    public delegate void SetViewIDFunc(NetworkViewID pNetworkViewID);

    IdToSetNetworkId<int> intToSetNetworkId = new IdToSetNetworkId<int>();
    IdToSetNetworkId<string> stringToSetNetworkId = new IdToSetNetworkId<string>();
    //IdToSetNetworkId<string> stringToSetNetworkId;

    public void getViewID(int pObjectID, SetViewIDFunc pSetFunc)
    {
        intToSetNetworkId.getViewID(pObjectID, pSetFunc);
    }

    public void getViewID(string pObjectID, SetViewIDFunc pSetFunc)
    {
        stringToSetNetworkId.getViewID(pObjectID, pSetFunc);
    }

    public void setViewID(int pObjectID, NetworkViewID pNetworkViewID)
    {
        networkView.RPC("intToViewID", RPCMode.Others, pObjectID, pNetworkViewID);
    }
    
    public void setViewID(string pObjectID, NetworkViewID pNetworkViewID)
    {
        networkView.RPC("stringToViewID", RPCMode.Others, pObjectID, pNetworkViewID);
    }

    public void beginGroup()
    {

    }

    public void endGroup()
    {

    }

    [RPC]
    public void intToViewID(int pObjectID, NetworkViewID pNetworkViewID)
    {
        intToSetNetworkId.setViewID(pObjectID, pNetworkViewID);
    }

    [RPC]
    public void stringToViewID(string pObjectID, NetworkViewID pNetworkViewID)
    {
        stringToSetNetworkId.setViewID(pObjectID, pNetworkViewID);
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("zzAllocateViewIDManager");
        singletonInstance = this;
    }
}