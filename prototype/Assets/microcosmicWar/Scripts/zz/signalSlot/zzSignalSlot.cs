using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

public class zzSignalSlot : MonoBehaviour
{

    //用于注释 便于观看
    public string description = "";

    public UnityEngine.Object signalComponent;

    //delegate or event
    public string signalMethodName = "signalMethodName";

    public UnityEngine.Object slotComponent;
    public string slotMethodName = "slotMethodName";

    public bool destroyAfterAwake = true;

    //just for show "enabled" in editor
    void Start(){}

    void Awake()
    {
        if(!enabled)
        {
            if(destroyAfterAwake)
                Destroy(this);
            return;
        }

        MemberInfo lSignalMemberInfo = getSignalMember(signalComponent, signalMethodName);
        if (lSignalMemberInfo == null)
        {
            Debug.LogError(gameObject.name+":There is not name in method,or it is not public");
            return;
        }
        Type lSignalDelegateType = getSignalDelegate(lSignalMemberInfo);

        Type ReturnType;
        Type[] ParameterTypes;

        getSignalMethod(lSignalDelegateType,
            out ReturnType, out ParameterTypes);

        MethodInfo lSlotMethod = slotComponent.GetType()
            .GetMethod(slotMethodName, ParameterTypes);


        if (lSlotMethod == null ||
                !(lSlotMethod.ReturnType == ReturnType
                ||lSlotMethod.ReturnType.IsSubclassOf(ReturnType))
            )
        {
            Debug.LogError(gameObject.name + ":Slot Method isn't fit Signal,or it is not public");
            return;
        } 
        Delegate lSlotDelegate;
        if (lSlotMethod.IsStatic)
            lSlotDelegate = System.Delegate.CreateDelegate(
                 lSignalDelegateType, lSlotMethod);
        else
            lSlotDelegate = System.Delegate.CreateDelegate(
                 lSignalDelegateType, slotComponent, lSlotMethod);

        linkSignalToSlot(signalComponent, lSignalMemberInfo, lSlotDelegate);
        
        if(destroyAfterAwake)
            Destroy(this);
    }

    public static void linkSignalToSlot(object pSignalObject, MemberInfo pSignalMemberInfo,
        System.Delegate pSlotDelegate)
    {
        if (pSignalMemberInfo is PropertyInfo)
            ((PropertyInfo)pSignalMemberInfo).SetValue(pSignalObject, pSlotDelegate, null);
        else if (pSignalMemberInfo is FieldInfo)
            ((FieldInfo)pSignalMemberInfo).SetValue(pSignalObject, pSlotDelegate);
        else if (pSignalMemberInfo is MethodInfo)
            ((MethodInfo)pSignalMemberInfo).Invoke(pSignalObject, new object[] { pSlotDelegate });
        else if (pSignalMemberInfo is EventInfo)
            ((EventInfo)pSignalMemberInfo).AddEventHandler(pSignalObject, pSlotDelegate );
        else
            Debug.LogError("linkSignalToSlot");

    }

    public static MemberInfo getSignalMember(object pSignalObject, string pMethodName)
    {
        var lType = pSignalObject.GetType();

        var lProperty = lType.GetProperty(pMethodName);
        if (lProperty != null && lProperty.PropertyType.BaseType == typeof(MulticastDelegate))
            return lProperty;

        var lField = lType.GetField(pMethodName);
        if (lField != null && lField.FieldType.BaseType == typeof(MulticastDelegate))
            return lField;

        var lEvent = lType.GetEvent(pMethodName);
        if (lEvent != null )
            return lEvent;
        //print("----------begin--------------");
        //print("type:" + lType.Name);
        //MethodInfo[] pMethodInfoList = lType.GetMethods();
        //foreach (var pMethodInfo in pMethodInfoList)
        //{
        //    string lFunction = pMethodInfo.Name+"(";
        //    foreach (var lParam in pMethodInfo.GetParameters())
        //    {
        //        lFunction = lFunction + ", " + lParam.Name + ":" + lParam.ParameterType;
        //    }
        //    lFunction = lFunction + "):" + pMethodInfo.ReturnType.Name;
        //    print(lFunction);
        //}
        //print("-----------end---------------");
        var lMethod = lType.GetMethod(pMethodName);

        if (lMethod != null
            && lMethod.GetParameters().Length == 1
            && lMethod.GetParameters()[0].ParameterType.BaseType == typeof(MulticastDelegate))
            return lMethod;

        return null;
    }

    public static Type getSignalDelegate(MemberInfo pMemberInfo)
    {

        if(pMemberInfo is PropertyInfo)
            return ((PropertyInfo)pMemberInfo).PropertyType;

        else if(pMemberInfo is FieldInfo)
            return ((FieldInfo)pMemberInfo).FieldType;

        else if (pMemberInfo is MethodInfo)
            return ((MethodInfo)pMemberInfo).GetParameters()[0].ParameterType;

        else if (pMemberInfo is EventInfo)
            return ((EventInfo)pMemberInfo).EventHandlerType;

        return null;
    }

    public static void getSignalMethod(Type pDelegate, out Type ReturnType, out Type[] ParameterTypes)
    {
        ParameterTypes = GetDelegateParameterTypes(pDelegate);
        ReturnType = GetDelegateReturnType(pDelegate);
    }

    public static Type[] toTypeArray(ParameterInfo[] parameters)
    {
        Type[] typeParameters = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            typeParameters[i] = parameters[i].ParameterType;
        }
        return typeParameters;
    }

    private static Type[] GetDelegateParameterTypes(Type d)
    {
        if (d.BaseType != typeof(MulticastDelegate))
        {
            throw new InvalidOperationException("Not a delegate.");
        }

        MethodInfo invoke = d.GetMethod("Invoke");
        if (invoke == null)
        {
            throw new InvalidOperationException("Not a delegate.");
        }

        return toTypeArray(invoke.GetParameters());
    }


    private static Type GetDelegateReturnType(Type d)
    {
        if (d.BaseType != typeof(MulticastDelegate))
        {
            throw new InvalidOperationException("Not a delegate.");
        }

        MethodInfo invoke = d.GetMethod("Invoke");
        if (invoke == null)
        {
            throw new InvalidOperationException("Not a delegate.");
        }

        return invoke.ReturnType;
    }


}