using UnityEngine;

public class zzGUIButtonEvent: zzInterfaceGUI
{
    System.Action keyEvent;

    public KeyCode keyCode;

    public void addKeyEventReceiver(System.Action pReceiver)
    {
        keyEvent += pReceiver;
    }

    public override void impGUI(Rect rect)
    {
        enabled |= (Event.current.type == EventType.KeyDown
            && Event.current.keyCode == keyCode);
    }

    void Update()
    {
        keyEvent();
        enabled = false;
    }
}