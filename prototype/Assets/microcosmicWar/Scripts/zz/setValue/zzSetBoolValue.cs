using UnityEngine;
using System;

public class zzSetBoolValue : MonoBehaviour
{
    public Component setObject;
    public string valueName;

    public bool valueToSet;

    public bool implementWhenAwake = true;

    public void addReceiver(System.Action<bool> pSetFunc)
    {
        setFunc += pSetFunc;
        if (implementWhenAwake)
            pSetFunc(valueToSet);
    }

    System.Action<bool> setFunc;

    //void Awake()
    //{
    //    var lMember = zzSignalSlot.getSignalMember(setObject, valueName);
    //    if (implementWhenAwake)
    //        setValue();
    //}

    //void Start()
    //{

    //}

    public void setValue()
    {
        //var lType = setObject.GetType();
        //var lField = lType.GetField(valueName);
        //lField.SetValue(setObject, valueToSet);
        setFunc(valueToSet);
    }
}
