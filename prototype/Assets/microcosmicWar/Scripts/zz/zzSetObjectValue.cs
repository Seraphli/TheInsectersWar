using UnityEngine;
using System.Collections;

public class zzSetObjectValue:MonoBehaviour
{
    public Component setObject;
    public string valueName;

    public Component valueToSet;

    public bool implementWhenAwake = true;

    void Awake()
    {
        if (implementWhenAwake)
            setValue();
    }

    public void setValue()
    {
        var lType = setObject.GetType();
        var lField = lType.GetField(valueName);
        lField.SetValue(setObject, valueToSet);
    }
}