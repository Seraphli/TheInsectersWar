using UnityEngine;
using System.Collections;

class JoystickBoard : zzEditableObject
{
    public OutPoint forwardOutPoint;

    public OutPoint backwardOutPoint;

    public OutPoint leftOutPoint;

    public OutPoint rightOutPoint;

    public OutPoint AButtonOutPoint;

    public OutPoint BButtonOutPoint;

    const float buttonUpValue = 0f;
    const float dirButtonDownValue = 1f;
    const float AbButtonDownValue = 1f;

    void Update()
    {
        forwardOutPoint.send(Input.GetKey(KeyCode.W) ?
            dirButtonDownValue : buttonUpValue);
        backwardOutPoint.send(Input.GetKey(KeyCode.S) ?
            dirButtonDownValue : buttonUpValue);
        leftOutPoint.send(Input.GetKey(KeyCode.A) ?
            dirButtonDownValue : buttonUpValue);
        rightOutPoint.send(Input.GetKey(KeyCode.D) ?
            dirButtonDownValue : buttonUpValue);


        AButtonOutPoint.send(Input.GetKey(KeyCode.J) ?
            AbButtonDownValue : buttonUpValue);
        BButtonOutPoint.send(Input.GetKey(KeyCode.K) ?
            AbButtonDownValue : buttonUpValue);
    }

    public override OutPoint[] outPoints
    {
        get 
        { 
            return new OutPoint[] {
                forwardOutPoint ,
                backwardOutPoint,
                leftOutPoint,
                rightOutPoint,
                AButtonOutPoint,
                BButtonOutPoint}; 
        }
    }

}