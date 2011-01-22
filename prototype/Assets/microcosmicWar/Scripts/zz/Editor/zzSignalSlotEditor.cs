using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

[CustomEditor(typeof(zzSignalSlot))]
public class zzSignalSlotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        zzSignalSlot lSignalSlot = (zzSignalSlot)target;
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("说明:", GUILayout.ExpandWidth(false));
                lSignalSlot.describe = GUILayout.TextField(lSignalSlot.describe);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("signal", GUILayout.Width(35));
                lSignalSlot.signalComponent
                    = (Component)EditorGUILayout.ObjectField(lSignalSlot.signalComponent, typeof(Component));
                if (lSignalSlot.signalComponent)
                    GUILayout.Label(lSignalSlot.signalComponent.GetType().ToString());
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
                    GUILayout.Label("slot", GUILayout.Width(35));
                    lSignalSlot.slotComponent = (Component)EditorGUILayout
                        .ObjectField(lSignalSlot.slotComponent, typeof(Component));
                    if (lSignalSlot.slotComponent)
                        GUILayout.Label( lSignalSlot.slotComponent.GetType().ToString());

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