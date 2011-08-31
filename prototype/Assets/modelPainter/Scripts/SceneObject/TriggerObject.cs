using UnityEngine;
using System.Collections;

class TriggerObject : zzEditableObject
{
    public OutPoint triggerPoint;

    bool isTriggered = false;

    void Update()
    {
        if (isTriggered)
        {
            triggerPoint.sendFull();
            isTriggered = false;
        }
        else
        {
            triggerPoint.sendNull();
        }
    }


    void OnTriggerStay(Collider other)
    {
        isTriggered = true;
    }

    public override OutPoint[] outPoints
    {
        get
        {
            return new OutPoint[] { triggerPoint };
        }
    }


}