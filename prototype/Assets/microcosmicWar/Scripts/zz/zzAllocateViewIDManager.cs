using UnityEngine;
using System.Collections.Generic;


public class zzAllocateViewIDManager:MonoBehaviour
{
    public class IdToSetNetworkId<T>
    {
        /// <summary>
        /// 如果执行设置,返回真
        /// </summary>
        /// <param name="pObjectID"></param>
        /// <param name="pSetFunc"></param>
        /// <returns></returns>
        public bool getViewID(T pObjectID, SetViewIDFunc pSetFunc)
        {
            if (idToViewID.ContainsKey(pObjectID))
            {
                pSetFunc(idToViewID[pObjectID]);
                idToViewID.Remove(pObjectID);
                return true;
            }
            else
            {
                idToSetFunc[pObjectID] = pSetFunc;
                return false;
            }
        }

        /// <summary>
        /// 如果执行设置,返回真
        /// </summary>
        /// <param name="pObjectID"></param>
        /// <param name="pNetworkViewID"></param>
        /// <returns></returns>
        public bool setViewID(T pObjectID, NetworkViewID pNetworkViewID)
        {
            if(idToSetFunc.ContainsKey(pObjectID))
            {
                idToSetFunc[pObjectID](pNetworkViewID);
                idToSetFunc.Remove(pObjectID);
                return true;
            }
            else
            {
                idToViewID[pObjectID] = pNetworkViewID;
                return false;
            }
        }

        Dictionary<T, SetViewIDFunc> idToSetFunc = new Dictionary<T,SetViewIDFunc>();

        Dictionary<T, NetworkViewID> idToViewID = new Dictionary<T,NetworkViewID>();

    }

    public static zzAllocateViewIDManager Singleton
    {
        get { return singletonInstance; }
    }

    static zzAllocateViewIDManager singletonInstance = null;

    public delegate void SetViewIDFunc(NetworkViewID pNetworkViewID);

    IdToSetNetworkId<int> intToSetNetworkId = new IdToSetNetworkId<int>();
    IdToSetNetworkId<string> stringToSetNetworkId = new IdToSetNetworkId<string>();
    //IdToSetNetworkId<string> stringToSetNetworkId;

    //public void getViewID(int pObjectID, SetViewIDFunc pSetFunc)
    //{
    //    intToSetNetworkId.getViewID(pObjectID, pSetFunc);
    //}
    static List<KeyValuePair<string, SetViewIDFunc>> getViewIDList
        = new List<KeyValuePair<string, SetViewIDFunc>>();

    public static void getViewID(string pObjectID, SetViewIDFunc pSetFunc)
    {
        if (singletonInstance)
            singletonInstance._getViewID(pObjectID, pSetFunc);
        else
            getViewIDList
                .Add(new KeyValuePair<string, SetViewIDFunc>(pObjectID, pSetFunc));
    }

    //客户端调用
    void _getViewID(string pObjectID, SetViewIDFunc pSetFunc)
    {
        stringToSetNetworkId.getViewID(pObjectID, pSetFunc);
    }

    //public void setViewID(int pObjectID, NetworkViewID pNetworkViewID)
    //{
    //    networkView.RPC("intToViewID", RPCMode.Others, pObjectID, pNetworkViewID);
    //}

    static List<KeyValuePair<string, NetworkViewID>> setViewIDList
        = new List<KeyValuePair<string, NetworkViewID>>();

    public static void setViewID(string pObjectID, NetworkViewID pNetworkViewID)
    {
        if (singletonInstance)
            singletonInstance._setViewID(pObjectID, pNetworkViewID);
        else
            setViewIDList
                .Add(new KeyValuePair<string, NetworkViewID>(pObjectID, pNetworkViewID));

    }
    
    //服务端调用
    void _setViewID(string pObjectID, NetworkViewID pNetworkViewID)
    {
        networkView.RPC("stringToViewID", RPCMode.Others, pObjectID, pNetworkViewID);
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

    void OnDestroy()
    {
        singletonInstance = null;
    }

    void Awake()
    {
        if (singletonInstance)
            Debug.LogError("zzAllocateViewIDManager");
        singletonInstance = this;
        foreach (var lInfo in getViewIDList)
        {
            _getViewID(lInfo.Key, lInfo.Value);
        }
        getViewIDList.Clear();
        
        foreach (var lInfo in setViewIDList)
        {
            _setViewID(lInfo.Key, lInfo.Value);
        }
        setViewIDList.Clear();
    }
}