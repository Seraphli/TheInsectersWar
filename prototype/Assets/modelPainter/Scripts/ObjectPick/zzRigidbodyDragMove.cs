using UnityEngine;
using System.Collections;

class zzRigidbodyDragMove : ObjectPickBase
{
    public Vector3 originalPosition;
    public Vector3 dragPosInBody;
    public Rigidbody dragedRigidbody;
    public float dragForceRangeDistance;
    public float maxDragForce;
    public float maxSpeed;

    public Joint jointXyDrag;
    public Joint jointXzDrag;

    public Joint nowDrag;

    public DragMode dragMode = DragMode.none;


    Vector3 getXYWantPos()
    {
        return zzRigidbodyDrag.getZFlatCrossPoint(originalPosition, zzObjectPicker.getMainCameraRay());
    }

    Vector3 getXZWantPos()
    {
        return zzRigidbodyDrag.getYFlatCrossPoint(originalPosition, zzObjectPicker.getMainCameraRay());
    }


    Vector3 getToAimForce(Vector3 pOriginalPos, Vector3 pAimPos)
    {
        Vector3 lToAim = pAimPos - pOriginalPos;
        float lForce;
        float lDistance = lToAim.magnitude;
        if (lDistance > dragForceRangeDistance)
            lForce = maxDragForce;
        else
            lForce = maxDragForce * (dragForceRangeDistance - lDistance)
                / dragForceRangeDistance;
        return lToAim.normalized * lForce;
    }

    public Vector3 wantPos;

    public void endDrag()
    {
        if (dragMode == DragMode.none)
            return;
        dragedRigidbody = null;
        nowDrag.connectedBody = null;
        dragMode = DragMode.none;
        editableObject.draged = false;
        editableObject = null;
    }

    //bool dragCheck(out RaycastHit pRaycastHit)
    //{
    //    var lCameraRay = getCameraRay();
    //    if (Physics.Raycast(lCameraRay, out pRaycastHit, detectDistance, dragLayerMask)
    //        && pRaycastHit.rigidbody)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    public override void OnLeftOn(GameObject pObject)
    {
        OnXYDrag(pObject);
    }

    public override void OnLeftOff(GameObject pObject)
    {
        endDrag();
    }

    public override void OnRightOn(GameObject pObject)
    {
        OnXZDrag(pObject);
    }

    public override void OnRightOff(GameObject pObject)
    {
        endDrag();
    }

    public void OnXYDrag(GameObject pObject)
    {
        OnDragObject(pObject,DragMode.XY);
    }

    public void OnXZDrag(GameObject pObject)
    {
        OnDragObject(pObject, DragMode.XZ);
    }

    zzEditableObjectContainer editableObject;

    void OnDragObject(GameObject pObject,DragMode pMode)
    {
        if (dragMode != DragMode.none)
            return;

        var lNowDrag = pMode == DragMode.XY ? jointXyDrag : jointXzDrag;
        if (!lNowDrag)
            return;

        var lEditableObject = zzEditableObjectContainer.findRoot(pObject);
        if (!lEditableObject)
            return;
        
        editableObject = lEditableObject;
        lEditableObject.draged = true;
        dragMode = pMode;
        nowDrag = lNowDrag;
        dragedRigidbody = lEditableObject.rigidbody;
        var lDragedTransform = lEditableObject.transform;
        var lDragWorldPos =
            zzRigidbodyDrag.getZFlatCrossPoint(lDragedTransform.position,
                zzObjectPicker.getMainCameraRay());
        //getFlatCrossPoint(lDragedTransform.position, getCameraRay(), dragMode);
        originalPosition = lDragWorldPos;
        nowDrag.transform.position = lDragWorldPos;
        nowDrag.connectedBody = dragedRigidbody;
    }

    //void Update()
    //{
    //    RaycastHit lRaycastHit;
    //    bool lXYButton = Input.GetKey(KeyCode.Mouse0);
    //    bool lXZButton = Input.GetKey(KeyCode.Mouse1);

    //    if (dragMode == DragMode.none
    //        && (lXYButton | lXZButton)
    //        && dragCheck(out lRaycastHit))
    //    {
    //        dragMode = lXYButton ? DragMode.XY : DragMode.XZ;
    //        nowDrag = lXYButton ? jointXyDrag : jointXzDrag;
    //        dragedRigidbody = lRaycastHit.rigidbody;
    //        rigidbodyIsKinematic = dragedRigidbody.isKinematic;
    //        dragedRigidbody.isKinematic = false;
    //        dragedRigidbody.useGravity = false;
    //        var lDragedTransform = lRaycastHit.rigidbody.transform;
    //        var lDragWorldPos =
    //            getZFlatCrossPoint(lDragedTransform.position, getCameraRay());
    //        //getFlatCrossPoint(lDragedTransform.position, getCameraRay(), dragMode);
    //        originalPosition = lDragWorldPos;
    //        nowDrag.transform.position = lDragWorldPos;
    //        nowDrag.connectedBody = dragedRigidbody;

    //        pickEvent(dragedRigidbody.gameObject);

    //    }

    //    if (
    //        (dragMode == DragMode.XY && (!lXYButton))
    //        || (dragMode == DragMode.XZ && (!lXZButton))
    //        )
    //    {
    //        endDrag();
    //    }
    //}

    void FixedUpdate()
    {
        if (dragMode != DragMode.none)
        {
            switch (dragMode)
            {
                case DragMode.XY:
                    wantPos = getXYWantPos();
                    break;
                case DragMode.XZ:
                    wantPos = getXZWantPos();
                    break;
            }
            nowDrag.transform.position = wantPos;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(wantPos, 0.3f);
    }

}