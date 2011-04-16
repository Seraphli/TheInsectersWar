using UnityEngine;

public class zzInputDownEvent:MonoBehaviour
{
    System.Action eventAction;

    public KeyCode button = KeyCode.Mouse0;

    public void addEventReceiver(System.Action pReceiver)
    {
        eventAction += pReceiver;
    }

    void Update()
    {
        if (Input.GetKeyDown(button))
            eventAction();
    }
}