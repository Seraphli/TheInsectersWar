using UnityEngine;
using System.Collections.Generic;

public class WMPlayStateManager : PlayStateManager
{
    void Awake()
    {
        Network.isMessageQueueRunning = true;
    }

    public zzGroupViewIDManager groupViewIDManager;
    public GameObject managerObject;

    public override void setPlay(bool pIsPlay)
    {
        if (_inPlaying == pIsPlay)
            return;
        _inPlaying = pIsPlay;
        if (pIsPlay)//stop=>play
        {
            applyPlayState();
            //changedToPlayEvent();
        }
        else//play=>stop
        {
            applyStopState();
            doChangedToStopEvent();
        }
    }

    public static void addManagedObject(GameObject lClone)
    {
        var lGameSceneManager = GameSceneManager.Singleton;
        var lGameObjectType = lClone.GetComponent<WMGameObjectType>();
        if (lGameObjectType)
        {
            if (lGameObjectType.race != Race.eNone)
                lGameSceneManager.addObject(lGameObjectType.race,
                    lGameObjectType.unitType, lClone);
            else
                lGameSceneManager.addObject(lGameObjectType.mapType, lClone);
        }
        else
            lGameSceneManager.addObject(
                GameSceneManager.MapManagerType.moveableObject, lClone);

    }

    List<Transform> objectList;

    void getAllSceneObject()
    {
        objectList = new List<Transform>(managerObject.transform.childCount);

        foreach (Transform lObject in managerObject.transform)
        {
            objectList.Add(lObject); ;
        }

    }

    [RPC]
    void OnAfterLoaded()
    {
        foreach (Transform lObject in objectList)
        {
            addPlayingObject(lObject.gameObject);
        }
        doChangedToPlayEvent();
    }
    //public static void addViewID(GameObject lClone)
    //{
    //    var lNetworkView = lClone.networkView;
    //    if (lNetworkView 
    //        && lNetworkView.viewID == NetworkViewID.unassigned)
    //    {
    //        lNetworkView.viewID = Network.AllocateViewID();
    //    }
    //}

    public static void addPlayingObject(GameObject lClone)
    {
        addManagedObject(lClone);
        lClone.GetComponent<zzEditableObjectContainer>().play = true;
    }

    public bool directSetObject = false;

    void recoverablePlayState()
    {
        foreach (Transform lObject in managerObject.transform)
        {
            var lClone = (GameObject)Instantiate(lObject.gameObject);
            addPlayingObject(lClone);
        }
        managerObject.SetActiveRecursively(false);
        doChangedToPlayEvent();
    }

    void directPlayState()
    {
        getAllSceneObject();
        List<NetworkView> lNetworkViewList = new List<NetworkView>();
        foreach (var lObject in objectList)
        {
            lNetworkViewList.AddRange( lObject.GetComponentsInChildren<NetworkView>() );
        }
        if (Network.peerType != NetworkPeerType.Disconnected)
        {
            var lViewIDManager = groupViewIDManager;
            lViewIDManager.setGroupBegin(OnAfterLoaded);
            if (Network.isServer)
            {
                if (lNetworkViewList.Count > Network.minimumAllocatableViewIDs)
                {
                    Network.minimumAllocatableViewIDs
                        = (int)(lNetworkViewList.Count * 1.5f);
                }
                for (int i = 0; i < lNetworkViewList.Count; ++i)
                {
                    var lNetworkView = lNetworkViewList[i];
                    var lID = Network.AllocateViewID();
                    lNetworkView.viewID = lID;
                    lViewIDManager.setViewID(i, lNetworkView);
                }
                lViewIDManager.setGroupEnd();
            }
            else
            {
                for (int i = 0; i < lNetworkViewList.Count; ++i)
                {
                    var lNetworkView = lNetworkViewList[i];
                    lViewIDManager.getViewID(i, lNetworkView);
                }

            }

        }
        else
            OnAfterLoaded();
    }

    public override void applyPlayState()
    {
        if(directSetObject)
        {
            directPlayState();
        }
        else
        {
            recoverablePlayState();
        }
    }

    public override void applyStopState()
    {
        GameSceneManager.Singleton.clearAllObject();
        managerObject.SetActiveRecursively(true);
    }

    //public override void setPlay(bool pIsPlay)
    //{
    //    if (_inPlaying == pIsPlay)
    //        return;
    //    _inPlaying = pIsPlay;
    //    if (pIsPlay)//stop=>play
    //    {
    //    }
    //    else//play=>stop
    //    {
    //    }
    //}

    public override void updateObject(GameObject pOjbect)
    {
        if (_inPlaying)
        {
            addPlayingObject(pOjbect);
        }
        else if (pOjbect.networkView)
        {
            pOjbect.networkView.enabled = false;
        }
    }

    //public void updateObjects()
    //{
    //    foreach (Transform lTransform in enumerateObject)
    //    {
    //        lTransform.GetComponent<zzEditableObjectContainer>().play = _inPlaying;
    //    }
    //}
}