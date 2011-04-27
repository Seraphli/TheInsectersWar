using UnityEngine;
using System.Collections.Generic;

public class WMPlayStateManager : PlayStateManager
{

    public GameObject managerObject;

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

        if (lClone.networkView)
            lClone.networkView.viewID = Network.AllocateViewID();

    }

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
    }

    void directPlayState()
    {
        List<Transform> lObjects = new List<Transform>(managerObject.transform.childCount);

        //int i = 0;
        //for (;i<lTransform.childCount;++i)
        //{
        //    lTransform.GetChild(i).name = "directPlayState";
        //    addPlayingObject(lTransform.GetChild(i).gameObject);
        //}
        //print(i);
        //print(managerObject.transform.childCount);
        //int i = 0;
        foreach (Transform lObject in managerObject.transform)
        {
            lObjects.Add(lObject); ;
        }
        foreach (Transform lObject in lObjects)
        {
            addPlayingObject(lObject.gameObject);
            //++i;
        }
        //print(i);

        //managerObject.transform.DetachChildren();
        //Destroy(managerObject);
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
            addPlayingObject(pOjbect);
    }

    //public void updateObjects()
    //{
    //    foreach (Transform lTransform in enumerateObject)
    //    {
    //        lTransform.GetComponent<zzEditableObjectContainer>().play = _inPlaying;
    //    }
    //}
}