using UnityEngine;

public class zzGameValue:MonoBehaviour
{
    public GameObject gameValue;

    System.Action<GameObject> valueChangedEvent;

    static void nullValueChangedEvent(GameObject p){}

    public void addValueChangedEventReceiver(System.Action<GameObject> pReceiver)
    {
        valueChangedEvent += pReceiver;
    }

    public void setValue(GameObject pValue)
    {
        gameValue = pValue;
        valueChangedEvent(pValue);
    }

    public GameObject getValue()
    {
        return gameValue;
    }

    public void clearValue()
    {
        setValue(null);
    }

    void Start()
    {
        if (valueChangedEvent == null)
            valueChangedEvent = nullValueChangedEvent;
    }
}