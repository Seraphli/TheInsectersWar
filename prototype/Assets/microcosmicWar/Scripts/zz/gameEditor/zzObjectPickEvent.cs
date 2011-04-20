using UnityEngine;

public class zzObjectPickEvent : ObjectPickBase
{
    public delegate void ObjectEvent(GameObject pObject);

    ObjectEvent leftOnObjectEvent;

    public void addLeftOnObjectEventReceiver(ObjectEvent pReceiver)
    {
        leftOnObjectEvent += pReceiver;
    }

    System.Action leftOffEvent;

    public void addLeftOffEventReceiver(System.Action pReceiver)
    {
        leftOffEvent += pReceiver;
    }

    ObjectEvent rightOnObjectEvent;

    public void addRightOnObjectEventReceiver(ObjectEvent pReceiver)
    {
        rightOnObjectEvent += pReceiver;
    }

    System.Action rightOffEvent;

    public void addRightOffEventReceiver(System.Action pReceiver)
    {
        rightOffEvent += pReceiver;
    }

    public override void OnLeftOn(GameObject pObject) 
    {
        leftOnObjectEvent(pObject);
    }

    public override void OnLeftOff(GameObject pObject) 
    {
        leftOffEvent();
    }

    public override void OnRightOn(GameObject pObject) 
    {
        rightOnObjectEvent(pObject);
    }

    public override void OnRightOff(GameObject pObject) 
    {
        rightOffEvent();
    }
}