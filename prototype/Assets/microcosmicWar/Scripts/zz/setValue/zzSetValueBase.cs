using UnityEngine;
using System;

public class zzSetValueBase<T> : MonoBehaviour
{
    public T valueToSet;

    public bool implementWhenAwake;

    public void changeValue(T pValue)
    {
        valueToSet = pValue;
    }

    public void addReceiver(System.Action<T> pSetFunc)
    {
        setFunc += pSetFunc;
        if (implementWhenAwake)
            pSetFunc(valueToSet);
    }

    System.Action<T> setFunc;

    public void setValue()
    {
        setFunc(valueToSet);
    }
}
