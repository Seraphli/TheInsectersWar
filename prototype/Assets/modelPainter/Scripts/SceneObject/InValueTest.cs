using UnityEngine;
using System.Collections;

class InValueTest : zzEditableObject
{
    public InPoint inPoint;

    void Start()
    {
        inPoint.addProcessFuncFloatArg(test);
    }

    void test(float pValue)
    {
        print(pValue);
    }

}