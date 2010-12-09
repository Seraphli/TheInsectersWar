
using UnityEngine;
using System.Collections;

class NetworkHelpObject:MonoBehaviour
{

    GameObject _observedObject;
    //Component observedComponent;

    public NetworkView networkView
    {
        get { return gameObject.networkView; }
    }

    public GameObject observedObject
    {
        set
        {
            _observedObject = value;
            gameObject.networkView.RPC("RPCSetObservedObject",
                RPCMode.Others, value.networkView.viewID);
        }
    }

    [RPC]
    void RPCSetObservedObject(NetworkViewID pID)
    {
        _observedObject = NetworkView.Find(pID).gameObject;
    }

    Component AddComponentObserved<T>() where T : Component
    {
        Component lComponent = _observedObject.AddComponent<T>();
        gameObject.networkView.observed
            = lComponent;

        gameObject.networkView.RPC("RPCAddComponentObserved",
                RPCMode.Others, typeof(T).ToString());
        return lComponent;
    }

    [RPC]
    void RPCAddComponentObserved(string ComponentName)
    {
        gameObject.networkView.observed
            = _observedObject.AddComponent(ComponentName);
    }

    //Component AddComponent<T>() where T : Component
    //{
    //    Component lOut = gameObject.AddComponent<T>();
    //    gameObject.networkView.RPC("RPCAddComponent",
    //            RPCMode.Others, typeof(T).ToString());
    //    return lOut;
    //}

    //[RPC]
    //Component RPCAddComponent(string ComponentName)
    //{
    //    return _observedObject.AddComponent(ComponentName);
    //}
}