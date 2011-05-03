using UnityEngine;

public class zzTriggerDetectorEvent:zzTriggerDetector
{
    System.Action<Collider> triggerEnterEvent;
    System.Action<Collider> triggerExitEvent;

    public void addEnterEventReceiver(System.Action<Collider> pReceiver)
    {
        triggerEnterEvent += pReceiver;
    }

    public void addExitEventReceiver(System.Action<Collider> pReceiver)
    {
        triggerExitEvent += pReceiver;
    }

    protected override void addDetectedObject(Collider other)
    {
        triggerEnterEvent(other);
        base.addDetectedObject(other);
    }

    protected override void removeDetectedObject(Collider other)
    {
        triggerExitEvent(other);
        base.removeDetectedObject(other);
    }
}