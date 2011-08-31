using UnityEngine;
using System.Collections;

public class Compass : zzEditableObject
{
    //原始朝向Vector3.Right
    public Transform compassPointPivot;

    public InPoint forwardInPoint;
    //const int forwardID = 0;

    public InPoint backwardInPoint;
    //const int backwardID = 1;

    public InPoint leftInPoint;
    //const int leftID = 2;

    public InPoint rightInPoint;
    //const int rightID = 3;


    void Start()
    {
        forwardInPoint.addProcessFuncVoidArg(updateFace);
        backwardInPoint.addProcessFuncVoidArg(updateFace);
        leftInPoint.addProcessFuncVoidArg(updateFace);
        rightInPoint.addProcessFuncVoidArg(updateFace);
    }

    void updateFace()
    {
        float lLeftRight = rightInPoint.powerValue - leftInPoint.powerValue;
        float lForbackward = forwardInPoint.powerValue - backwardInPoint.powerValue;
        if (Mathf.Approximately(lLeftRight, 0f) && Mathf.Approximately(lForbackward, 0f))
            return;
        setCompassPointForward(new Vector3(lLeftRight, 0f, lForbackward));
    }

    public void setCompassPointForward(Vector3 pForward)
    {
        Quaternion lRotation = new Quaternion();
        lRotation.SetFromToRotation(Vector3.right, pForward);
        compassPointPivot.localRotation = lRotation;
    }

    public override InPoint[] inPoints
    {
        get {
            return new InPoint[] {
                forwardInPoint,
                backwardInPoint,
                leftInPoint,
                rightInPoint };
        }
    }

}