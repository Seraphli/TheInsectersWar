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

    public void changeAndSetValue(T pValue)
    {
        valueToSet = pValue;
        setFunc(valueToSet);
    }

    public void addReceiver(System.Action<T> pSetFunc)
    {
        setFunc += pSetFunc;
        if (implementWhenAwake)
            pSetFunc(valueToSet);
    }

    public void addVoidReceiver(System.Action pSetFunc)
    {
        System.Action<T> lSetFunc = (x) => pSetFunc();
        setFunc += lSetFunc;
        if (implementWhenAwake)
            lSetFunc(valueToSet);
    }

    System.Action<T> setFunc;

    [ContextMenu("Set Value")]
    public void setValue()
    {
        setFunc(valueToSet);
    }
}

public class zzTransmitValueBase<T> : MonoBehaviour
{
    System.Action<T> setFunc;
    System.Func<T> getFunc;

    public void addReceiver(System.Action<T> pSetFunc)
    {
        setFunc += pSetFunc;
    }

    public void addVoidReceiver(System.Action pSetFunc)
    {
        System.Action<T> lSetFunc = (x) => pSetFunc();
        setFunc += lSetFunc;
    }

    public void addGetter(System.Func<T> pGetFunc)
    {
        getFunc += pGetFunc;
    }

    public void setValue()
    {
        setFunc(getFunc());
    }

}