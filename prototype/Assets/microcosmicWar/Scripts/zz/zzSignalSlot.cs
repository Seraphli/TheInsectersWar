using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

public class zzSignalSlot : MonoBehaviour
{
    public Component signalComponent;

    //delegate or event
    public string signalMethodName = "signalMethodName";

    public Component slotComponent;
    public string slotMethodName = "slotMethodName";

    void Awake()
    {

        MemberInfo lSignalMemberInfo = getSignalMember(signalComponent, signalMethodName);
        if (lSignalMemberInfo == null)
        {
            Debug.LogError("There is not name in method,or it is not public");
            return;
        }
        Type lSignalDelegateType = getSignalDelegate(lSignalMemberInfo);

        Type ReturnType;
        Type[] ParameterTypes;

        getSignalMethod(lSignalDelegateType,
            out ReturnType, out ParameterTypes);

        MethodInfo lSlotMethod = slotComponent.GetType()
            .GetMethod(slotMethodName, ParameterTypes);


        if (lSlotMethod == null || lSlotMethod.ReturnType != ReturnType)
        {
            Debug.LogError("Slot Method isn't fit Signal,or it is not public");
            return;
        } 
        var lSlotDelegate = System.Delegate.CreateDelegate(
             lSignalDelegateType, slotComponent, lSlotMethod);

        linkSignalToSlot(signalComponent, lSignalMemberInfo, lSlotDelegate);
    }

    static void linkSignalToSlot(object pSignalObject, MemberInfo pSignalMemberInfo,
        System.Delegate pSlotDelegate)
    {
        if (pSignalMemberInfo is PropertyInfo)
            ((PropertyInfo)pSignalMemberInfo).SetValue(pSignalObject, pSlotDelegate,null);
        else if (pSignalMemberInfo is FieldInfo)
            ((FieldInfo)pSignalMemberInfo).SetValue(pSignalObject, pSlotDelegate);
        else
            Debug.LogError("linkSignalToSlot");

    }

    public static MemberInfo getSignalMember(object pSignalObject, string pMethodName)
    {
        var lProperty = pSignalObject.GetType().GetProperty(pMethodName);
        if (lProperty != null && lProperty.PropertyType.BaseType == typeof(MulticastDelegate))
            return lProperty;
        var lField = pSignalObject.GetType().GetField(pMethodName);
        if (lField != null && lField.FieldType.BaseType == typeof(MulticastDelegate))
            return lField;
        return null;
    }

    public static Type getSignalDelegate(MemberInfo pMemberInfo)
    {

        if(pMemberInfo is PropertyInfo)
            return ((PropertyInfo)pMemberInfo).PropertyType;

        if(pMemberInfo is FieldInfo)
            return ((FieldInfo)pMemberInfo).FieldType;

        return null;
    }

    public static void getSignalMethod(Type pDelegate, out Type ReturnType, out Type[] ParameterTypes)
    {
        ParameterTypes = GetDelegateParameterTypes(pDelegate);
        ReturnType = GetDelegateReturnType(pDelegate);
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

        ParameterInfo[] parameters = invoke.GetParameters();
        Type[] typeParameters = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            typeParameters[i] = parameters[i].ParameterType;
        }

        return typeParameters;
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