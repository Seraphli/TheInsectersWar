using UnityEngine;
using System.Collections;

public enum DragMode
{
    none,
    XY,
    XZ,
}

class zzRigidbodyDrag:MonoBehaviour
{

    public DragMode dragMode = DragMode.none;

    public Camera dragCamera;

    protected void Start()
    {
        dragCamera = Camera.main;
    }

    protected Ray getCameraRay()
    {
        var lMousePos = Input.mousePosition;
        var lRay = dragCamera.ScreenPointToRay(
            new Vector3(lMousePos.x, lMousePos.y, dragCamera.nearClipPlane));
        return lRay;

    }

    public static Vector3 getFlatCrossPoint(Vector3 pFlatPos, Ray pRay, DragMode pMode)
    {
        switch (pMode)
        {
            case DragMode.XY:
                return getZFlatCrossPoint(pFlatPos, pRay);
                break;
            case DragMode.XZ:
                return getYFlatCrossPoint(pFlatPos, pRay);
                break;
        }
        Debug.LogError("getFlatCrossPoint");
        return pFlatPos;
    }


    public static Vector3 getZFlatCrossPoint(Vector3 pFlatPos, Ray pRay)
    {
        if (Mathf.Approximately(pRay.origin.z, pFlatPos.z))
            return pFlatPos;
        return pRay.origin + pRay.direction * (pFlatPos.z - pRay.origin.z) / pRay.direction.z;
    }

    public static Vector3 getYFlatCrossPoint(Vector3 pFlatPos, Ray pRay)
    {
        if (Mathf.Approximately(pRay.origin.y, pFlatPos.y))
            return pFlatPos;
        return pRay.origin + pRay.direction * (pFlatPos.y - pRay.origin.y) / pRay.direction.y;
    }

}