using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

[CustomEditor(typeof(zzSignalSlot))]
public class zzSignalSlotEditor : Editor
{
    static Component selectComponents(Component pSelected)
    {
        var lComponents = pSelected.GetComponents<Component>();
        int lIndex = 0;
        string[] lComponentNames = new string[lComponents.Length];
        for (int i = 0; i < lComponents.Length;++i )
        {
            lComponentNames[i] = lComponents[i].GetType().ToString();
            if (lComponents[i] == pSelected)
                lIndex = i;
        }
        int lNewIndex = EditorGUILayout.Popup(lIndex, lComponentNames);
        //if(lNewIndex!=lIndex)
        return lComponents[lNewIndex];
        //return null;
    }

    void componentChange(string pComponentType, ref Component pSelected,
        Component pNewSelected)
    {
        if (pSelected != pNewSelected)
        {
            Undo.RegisterUndo(target, pComponentType + " Component Change");
            pSelected = pNewSelected;
        }
    }

    void componentChange(string pComponentType, ref Component pSelected)
    {
        GUILayout.Label(pComponentType, GUILayout.Width(35));

        componentChange(pComponentType, ref  pSelected,
            (Component)EditorGUILayout.ObjectField(pSelected, typeof(Component)));

        if (pSelected)
            componentChange(pComponentType, ref pSelected,
                selectComponents(pSelected));

    }

    //void componentChange(string pComponentType)
    //{
    //    Undo.RegisterUndo(target, pComponentType + " Component Change");
    //}

    //void selectComponents(string pComponentType, ref Component pSelected)
    //{
    //    var lNew = selectComponents(pSelected);
    //    if (lNew)
    //    {
    //        componentChange(pComponentType);
    //        pSelected = lNew;
    //    }
    //}

    //void fieldComponents(string pComponentType, ref Component pSelected)
    //{
    //    var lNew = (Component)EditorGUILayout
    //        .ObjectField(lSignalSlot.signalComponent, typeof(Component));
    //    if(lNewComponent!=pSelected)
    //    {

    //    }
    //}

    static List<String> getAllSignalMethod(Type pType)
    {
        var lOut = new List<String>();
        var lMembers = pType.GetMembers();
        foreach (var llMember in lMembers)
        {
            if(llMember is MethodInfo)
            {
                var lMethodInfo = (MethodInfo)llMember;
                if (lMethodInfo.GetParameters().Length == 1
                    && lMethodInfo.GetParameters()[0]
                        .ParameterType.BaseType == typeof(MulticastDelegate))
                    lOut.Add(lMethodInfo.Name);
            }
            else if(llMember is PropertyInfo
                && ((PropertyInfo)llMember)
                    .PropertyType.BaseType == typeof(MulticastDelegate))
            {
                lOut.Add(llMember.Name);
            }
            else if(llMember is FieldInfo
                && ((FieldInfo)llMember)
                    .FieldType.BaseType == typeof(MulticastDelegate))
            {
                lOut.Add(llMember.Name);
            }
            else if (llMember is EventInfo)
            {
                lOut.Add(llMember.Name);
            }
        }
        return lOut;
    }

    public override void OnInspectorGUI()
    {
        zzSignalSlot lSignalSlot = (zzSignalSlot)target;
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("description:", GUILayout.ExpandWidth(false));
                lSignalSlot.describe = GUILayout.TextField(lSignalSlot.describe);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                //GUILayout.Label("signal", GUILayout.Width(35));
                //lSignalSlot.signalComponent
                //    = (Component)EditorGUILayout.ObjectField(lSignalSlot.signalComponent, typeof(Component));
                //if (lSignalSlot.signalComponent)
                componentChange("Signal", ref lSignalSlot.signalComponent);//,
                        //selectComponents(lSignalSlot.signalComponent));
                    //selectComponents(ref lSignalSlot.signalComponent, lSignalSlot);
                    //GUILayout.Label(lSignalSlot.signalComponent.GetType().ToString());
                lSignalSlot.signalMethodName = EditorGUILayout.TextField(lSignalSlot.signalMethodName);
            }
            EditorGUILayout.EndHorizontal();

            if(lSignalSlot.signalComponent)
            {
                MemberInfo lSignalMemberInfo = zzSignalSlot
                    .getSignalMember(lSignalSlot.signalComponent, lSignalSlot.signalMethodName);
                if (lSignalMemberInfo == null)
                    GUILayout.Label("error in signal");
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    //GUILayout.Label("slot", GUILayout.Width(35));
                    //lSignalSlot.slotComponent = (Component)EditorGUILayout
                    //    .ObjectField(lSignalSlot.slotComponent, typeof(Component));
                    //if (lSignalSlot.slotComponent)
                    componentChange("Slot", ref lSignalSlot.slotComponent);//,
                            //selectComponents(lSignalSlot.slotComponent));

                    if (lSignalSlot.slotComponent)
                    {
                        Type lSignalDelegateType = zzSignalSlot.getSignalDelegate(lSignalMemberInfo);
                        Type ReturnType;
                        Type[] ParameterTypes;
                        zzSignalSlot.getSignalMethod(lSignalDelegateType,
                            out ReturnType, out ParameterTypes);

                        //lSignalSlot.slotComponent.GetType().GetMethods()
                        var lMethodNames = findMethodNames(lSignalSlot.slotComponent, ParameterTypes, ReturnType);
                        lMethodNames.Add(lSignalSlot.slotMethodName);

                        int lSelected = EditorGUILayout.Popup(lMethodNames.Count - 1, lMethodNames.ToArray());
                        lSignalSlot.slotMethodName = lMethodNames[lSelected];
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

        }

        EditorGUILayout.EndVertical();
    }

    List<string>    findMethodNames(object pObject,Type[] pParameterTypes,Type pReturnType)
    {
        List<string> lOut = new List<string>();
        //Debug.Log("pObject:" + pObject.ToString());
        MethodInfo[] pMethodInfoList = pObject.GetType().GetMethods();

        //Debug.Log("pMethodInfoList:" + pMethodInfoList.Length);
        foreach (var pMethodInfo in pMethodInfoList)
        {
            //Debug.Log(pMethodInfo.ReturnType.ToString());
            //Debug.Log(pReturnType.ToString());
            //Debug.Log("pMethodInfo.ReturnType == pReturnType:" + (pMethodInfo.ReturnType == pReturnType));
            if (pMethodInfo.ReturnType == pReturnType
                && isEquals(
                        zzSignalSlot.toTypeArray(pMethodInfo.GetParameters()),
                        pParameterTypes))
                lOut.Add(pMethodInfo.Name);
        }
        return lOut;
    }

    static bool isEquals(Type[] listA, Type[] listB)
    {
        //Debug.Log("listA.Length != listB.Length:" + (listA.Length != listB.Length));
        if (listA.Length != listB.Length)
            return false;
        for (int i = 0; i < listA.Length;++i )
        {
            //Debug.Log("listA[i] != listB[i]:"+i+(listA[i] != listB[i]));
            if (listA[i] != listB[i])
                return false;
        }
        return true;
    }

    
}