using UnityEngine;
using System.Collections;

class PointConnecter : ObjectPickBase
{
    public InPoint choosedInPoint;
    public OutPoint choosedOutPoint;

    public P2PLine connectLine;
    public Transform choosedPointTransform;

    public override void OnLeftOn(GameObject pObject)
    {
        OnConnectDown(pObject);
    }

    public override void OnLeftOff(GameObject pObject)
    {
        OnConnectUp(pObject);
    }

    public override void OnRightOn(GameObject pObject)
    {
        OnDisconnect(pObject);
    }


    void Update()
    {
        if(choosedPointTransform)
        {
            refreshLineShow();
        }
    }

    void refreshLineShow()
    {
        connectLine.beginPosition = choosedPointTransform.position;
        RaycastHit lRaycastHit;
        if (Physics.Raycast(zzObjectPicker.getMainCameraRay(), out lRaycastHit))
        {
            connectLine.endPosition = lRaycastHit.point;
        }
        else
        {
            connectLine.endPosition = connectLine.beginPosition;
        }
    }

    public bool setPoint(GameObject pObject)
    {
        if (!pObject)
            return false;

        if (pObject.GetComponent<InPoint>())
            choosedInPoint = pObject.GetComponent<InPoint>();
        else if (pObject.GetComponent<OutPoint>())
            choosedOutPoint = pObject.GetComponent<OutPoint>();
        else
            return false;

        choosedPointTransform = pObject.transform;
        connectLine.visible = true;
        refreshLineShow();
        return true;
    }

    public void OnConnectDown(GameObject pObject)
    {
        setPoint(pObject);
    }

    public void OnConnectUp(GameObject pObject)
    {
        setPoint(pObject);
        if (choosedInPoint && choosedOutPoint)
        {
            choosedOutPoint.connect(choosedInPoint);
            choosedInPoint.showLine();
        }
        choosedInPoint = null;
        choosedOutPoint = null;
        choosedPointTransform = null;

        connectLine.visible = false;

    }

    public void OnDisconnect(GameObject pObject)
    {
        if (pObject&&pObject.GetComponent<InPoint>())
        {
            var lChoosedInPoint = pObject.GetComponent<InPoint>();
            lChoosedInPoint.disconnect();
            lChoosedInPoint.hideLine();
        }

        //else if (pObject.GetComponent<OutPoint>())
        //    choosedOutPoint = pObject.GetComponent<OutPoint>();

    }

}