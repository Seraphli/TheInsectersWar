using UnityEngine;

public class zzOnEventBase : MonoBehaviour
{
    protected System.Action onEvent;

    static void nullEvent()
    {

    }

    public void addEventReceiver(System.Action pReceiver)
    {
        onEvent += pReceiver;
    }

    void Start()
    {
        if (onEvent == null)
            onEvent = nullEvent;
    }
}