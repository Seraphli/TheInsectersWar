﻿using UnityEngine;
using System.Collections.Generic;

public class BoundNetworkScope : MonoBehaviour
{
    public Transform[] networkPlayerRoots;

    System.Func<Bounds> getBoundsFunc;

    public NetworkPlayer networkPlayer;

    public LinkedList<NetworkView> networkViewList = new LinkedList<NetworkView>();

    public void addScopeNetView(NetworkView[] pNetworkViews)
    {
        foreach (var lNetworkView in pNetworkViews)
        {
            addScopeNetView(lNetworkView);
        }
    }

    //public void addAndSetScopeNetView(Vector3 pPosition,NetworkView[] pNetworkViews)
    //{
    //    bool lContain = getBoundsFunc().Contains(pPosition);
    //    foreach (var lNetworkView in pNetworkViews)
    //    {
    //        addScopeNetView(lNetworkView);
    //        lNetworkView.SetScope(networkPlayer, lContain);
    //    }
    //}

    public void addScopeNetView(NetworkView pNetworkView)
    {
        networkViewList.AddLast(pNetworkView);
    }

    public void setGetBoundsFunc(System.Func<Bounds> pFunc)
    {
        getBoundsFunc = pFunc;
    }

    public void updateScope() 
    { 
        updateScope(getBoundsFunc()); 
    }

    public void updateScope(Bounds pBounds)
    {
        //Scope Transform中的子物体
        foreach (Transform lNetworkPlayerRoot in networkPlayerRoots)
        {
            setScope(pBounds, lNetworkPlayerRoot, networkPlayer);
        }
        //Scope 自定义物体
        var lNetworkViewNode = networkViewList.First;
        while (lNetworkViewNode != null)
        {
            var lNetworkView = lNetworkViewNode.Value;
            var lNextNode = lNetworkViewNode.Next;
            if (lNetworkView)
            {
                lNetworkView.SetScope(networkPlayer,
                    pBounds.Contains(lNetworkView.transform.position));
            }
            else
                networkViewList.Remove(lNetworkViewNode);
            lNetworkViewNode = lNextNode;
        }
    }

    public static void setScope(Bounds pBounds, Transform pParent,NetworkPlayer pPlayer)
    {
        foreach (Transform lTransform in pParent)
        {
            lTransform.networkView.SetScope(pPlayer,pBounds.Contains(lTransform.position));
        }
    }
}