
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

class zzSceneObjectClick:MonoBehaviour
{
    public Component recieverComponent;
    public string recieverFunctionName;

    delegate void ClickReceiveFunc(object sender);

    static void nullClickReceiveFunc(object sender)
    {
    }

    ClickReceiveFunc clickReceiveFunc = nullClickReceiveFunc;

    void Start()
    {
        MethodInfo lRecieverMethodInfo = recieverComponent.GetType()
            .GetMethod(recieverFunctionName);
        clickReceiveFunc = (ClickReceiveFunc)System.Delegate.CreateDelegate(
            typeof(ClickReceiveFunc), recieverComponent, lRecieverMethodInfo);
    }

    void OnMouseUp()
    {
        clickReceiveFunc(this);
    }
}