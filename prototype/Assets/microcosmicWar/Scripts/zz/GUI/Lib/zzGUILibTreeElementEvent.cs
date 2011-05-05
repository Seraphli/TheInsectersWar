using UnityEngine;

public class zzGUILibTreeElementEvent:MonoBehaviour
{
    public delegate void StringCallFunc(string text);
    protected void nullStringCallFunc(string text) { }

    public StringCallFunc elementClickedEvent;
    public StringCallFunc nodeClickedEvent;
    public System.Action<Object> elementClickedObjectEvent;

    public void callElementClickedEvent(string pInfo)
    {
        elementClickedEvent(pInfo);
    }

    public void callNodeClickedEvent(string pInfo)
    {
        nodeClickedEvent(pInfo);
    }

    public void callElementClickedObjectEvent(Object pInfo)
    {
        elementClickedObjectEvent(pInfo);
    }

    static void nullClickedObjectEvent(Object p) { }

    public void addElementClickedEvent(StringCallFunc pReceiver)
    {
        elementClickedEvent += pReceiver;
    }

    public void addElementClickedObjectEvent(System.Action<Object> pReceiver)
    {
        elementClickedObjectEvent += pReceiver;
    }

    public void addNodeClickedEvent(StringCallFunc pReceiver)
    {
        nodeClickedEvent += pReceiver;
    }

    void Start()
    {
        if (elementClickedEvent == null)
            elementClickedEvent = nullStringCallFunc;
        if (nodeClickedEvent == null)
            nodeClickedEvent = nullStringCallFunc;
        if (elementClickedObjectEvent == null)
            elementClickedObjectEvent = nullClickedObjectEvent;
    }
}