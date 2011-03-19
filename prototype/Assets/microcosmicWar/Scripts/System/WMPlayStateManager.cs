﻿using UnityEngine;
using System.Collections;

public class WMPlayStateManager : PlayStateManager
{

    public GameObject managerObject;

    public bool play
    {
        get { return _inPlaying; }
        set { setPlay(value); }
    }

    void addPlayingObject(GameObject lClone)
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
        lClone.GetComponent<zzEditableObjectContainer>().play = true;
    }

    public override void setPlay(bool pIsPlay)
    {
        if (_inPlaying == pIsPlay)
            return;
        _inPlaying = pIsPlay;
        if (pIsPlay)//stop=>play
        {
            foreach (Transform lObject in managerObject.transform)
            {
                var lClone = (GameObject)Instantiate(lObject.gameObject);
                addPlayingObject(lClone);
            }
            managerObject.SetActiveRecursively(false);
        }
        else//play=>stop
        {
            GameSceneManager.Singleton.clearAllObject();
            managerObject.SetActiveRecursively(true);
        }
    }

    public override void updateObject(GameObject pOjbect)
    {
        pOjbect.GetComponent<zzEditableObjectContainer>().play = _inPlaying;
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