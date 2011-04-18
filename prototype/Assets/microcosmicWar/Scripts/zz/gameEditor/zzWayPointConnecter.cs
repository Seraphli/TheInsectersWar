using UnityEngine;
using System.Collections.Generic;

public class zzWayPointConnecter : ObjectPickBase
{
    public string lineTypeName;

    public zzWayPoint beginPoint;

    public P2PLine connectLine;

    public Camera camera;

    public float lineZ = -3f;
    //public Transform choosedPointTransform;

    public System.Action<GameObject> objectAddedEvent;

    public override void OnLeftOn(GameObject pObject)
    {
        OnConnectDown(pObject);
    }

    public override void OnLeftOff(GameObject pObject)
    {
        OnConnectUp(pObject);
    }


    void Update()
    {
        //if(choosedPointTransform)
        //{
            refreshLineShow();
        //}
    }

    void refreshLineShow()
    {
        var lMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        lMousePos.z = lineZ;
        connectLine.endPosition = lMousePos;
    }

    //public bool setPoint(GameObject pObject)
    //{
    //    if (!pObject)
    //        return false;

    //    if (pObject.GetComponent<zzWayPoint>())
    //        choosedInPoint = pObject.GetComponent<zzWayPoint>();
    //    else if (pObject.GetComponent<zzWayPoint>())
    //        choosedOutPoint = pObject.GetComponent<zzWayPoint>();
    //    else
    //        return false;

    //    choosedPointTransform = pObject.transform;
    //    connectLine.visible = true;
    //    refreshLineShow();
    //    return true;
    //}

    static zzWayPoint getWayPoint(GameObject pObject)
    {
        if (!pObject)
            return null;
        var lWayPoint = pObject.GetComponent<zzWayPoint>();
        if (!lWayPoint)
            return pObject.transform.parent.GetComponent<zzWayPoint>();
        return lWayPoint;
    }

    public void OnConnectDown(GameObject pObject)
    {
        var lWayPoint = getWayPoint(pObject);
        if (lWayPoint)
        {
            beginPoint = lWayPoint;
            var lPosition = beginPoint.transform.position;
            lPosition.z = lineZ;
            //重置线
            connectLine.beginPosition = lPosition;
            connectLine.endPosition = lPosition;
            enabled = true;
            connectLine.visible = true;
        }
    }

    public void OnConnectUp(GameObject pObject)
    {
        if (beginPoint)
        {
            print("if (beginPoint)");
            var lWayPoint = getWayPoint(pObject);
            if (lWayPoint != beginPoint)
            {
                print("add line");
                var lObject = GameSystem.Singleton.createObject(lineTypeName);
                var lLine = lObject.GetComponent<zzWayPointLine>();
                lLine.lineZ = lineZ;
                lLine.setPoints(beginPoint, lWayPoint);
                objectAddedEvent(lObject);
            }
        }
        beginPoint = null;

        this.enabled = false;
        connectLine.visible = false;

    }


}