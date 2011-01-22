using UnityEngine;
using System.Collections;

public class zzSetObjectValue:MonoBehaviour
{
    public Component setObject;
    public string valueName;

    public Component valueToSet;

    void Awake()
    {
        var lType = setObject.GetType();
        var lField = lType.GetField(valueName);
        lField.SetValue(setObject, valueToSet);
    }
}