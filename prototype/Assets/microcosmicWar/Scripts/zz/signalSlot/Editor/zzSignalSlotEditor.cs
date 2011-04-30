using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

[CustomEditor(typeof(zzSignalSlot))]
public class zzSignalSlotEditor : Editor
{

    [MenuItem("Component/zz/Add SignalSlot")]
    static void addSignalSlot()
    {
        Selection.activeGameObject.AddComponent<zzSignalSlot>();
    }

    [MenuItem("Component/zz/Add SignalSlot", true)]
    static bool validateAddSignalSlot()
    {
        return Selection.activeTransform != null;
    }

    static UnityEngine.Object selectComponents(UnityEngine.Object pSelected)
    {
        Component[] lComponents ;
        GameObject lObject;
        if (pSelected is GameObject)
            lObject = (GameObject)pSelected;
        else if (pSelected is Component)
            lObject = ((Component)pSelected).gameObject;
        else
        {
            Debug.Log(pSelected.GetType() + ":" + pSelected);
            return pSelected;
        }
        
        lComponents = lObject.GetComponents<Component>();

        int lIndex = 0;
        string[] lComponentNames = new string[lComponents.Length+1];
        for (int i = 0; i < lComponents.Length;++i )
        {
            if (lComponents[i])//存在lComponents[i]==null的情况,原因不明
            {
                lComponentNames[i] = lComponents[i].GetType().ToString();
                if (lComponents[i] == pSelected)
                    lIndex = i;
            }
        }
        lComponentNames[lComponents.Length] = "GameObject";
        if(pSelected is GameObject)
        {
            lIndex = lComponents.Length;
        }
        int lNewIndex = EditorGUILayout.Popup(lIndex, lComponentNames);
        //Debug.Log(lNewIndex);
        //Debug.Log(lComponentNames[lNewIndex]);
        if (lNewIndex == lComponents.Length)
        {
            //Debug.Log("GameObject");
            //Debug.Log(lObject.ToString());
            return lObject;
        }
        return lComponents[lNewIndex];
        //return null;
    }

    void componentChange(string pComponentType, ref UnityEngine.Object pSelected,
        UnityEngine.Object pNewSelected)
    {
        if (pSelected != pNewSelected)
        {
            Undo.RegisterUndo(target, pComponentType + " Component Change");
            pSelected = pNewSelected;
            //Debug.Log(pSelected.ToString());
        }
    }

    void componentChange(string pComponentType, ref UnityEngine.Object pSelected)
    {
        GUILayout.Label(pComponentType, GUILayout.Width(35));

        componentChange(pComponentType, ref  pSelected,
            (UnityEngine.Object)EditorGUILayout.ObjectField(pSelected, typeof(UnityEngine.Object)));

        if (pSelected)
            componentChange(pComponentType, ref pSelected,
                selectComponents(pSelected));

    }


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

    void signalChange(ref string pSignalName)
    {
        zzSignalSlot lSignalSlot = (zzSignalSlot)target;
        var lSignalMethods = getAllSignalMethod(lSignalSlot.signalComponent.GetType());
        lSignalMethods.Add(pSignalName);

        int lSelected = EditorGUILayout.Popup(lSignalMethods.Count - 1, lSignalMethods.ToArray());
        if(lSignalMethods[lSelected]!=pSignalName)
        {
            Undo.RegisterUndo(target, "Signal Method Change");
            pSignalName = lSignalMethods[lSelected];
        }
    }

    void outError(string pInfo)
    {
        var lPreColor = GUI.color;
        GUI.color = Color.red;
        GUILayout.Label(pInfo);
        GUI.color = lPreColor;
    }

    public bool useBuiltinGUI = false;

    public override void OnInspectorGUI()
    {
        useBuiltinGUI = EditorGUILayout.Toggle("Default GUI", useBuiltinGUI);
        if(useBuiltinGUI)
        {
            base.OnInspectorGUI();
            return;
        }
        zzSignalSlot lSignalSlot = (zzSignalSlot)target;
        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("Description:", GUILayout.ExpandWidth(false));
                lSignalSlot.description = EditorGUILayout.TextField(lSignalSlot.description);
            }
            EditorGUILayout.EndHorizontal();

            lSignalSlot.destroyAfterAwake = EditorGUILayout.Toggle("Destroy After Awake", lSignalSlot.destroyAfterAwake);
            EditorGUILayout.BeginHorizontal();
            {
                componentChange("Signal", ref lSignalSlot.signalComponent);
                //lSignalSlot.signalMethodName = EditorGUILayout.TextField(lSignalSlot.signalMethodName);
                if (lSignalSlot.signalComponent)
                    signalChange(ref lSignalSlot.signalMethodName);
            }
            EditorGUILayout.EndHorizontal();

            if(lSignalSlot.signalComponent)
            {
                MemberInfo lSignalMemberInfo = zzSignalSlot
                    .getSignalMember(lSignalSlot.signalComponent, lSignalSlot.signalMethodName);
                if (lSignalMemberInfo == null)
                    outError("error in signal");
                else
                {
                    int lSignaMethodlSelectIndex=0;
                    EditorGUILayout.BeginHorizontal();

                    componentChange("Slot", ref lSignalSlot.slotComponent);

                    if (lSignalSlot.slotComponent)
                    {
                        Type lSignalDelegateType = zzSignalSlot.getSignalDelegate(lSignalMemberInfo);
                        Type ReturnType;
                        Type[] ParameterTypes;
                        zzSignalSlot.getSignalMethod(lSignalDelegateType,
                            out ReturnType, out ParameterTypes);

                        var lMethodNames = findMethodNames(lSignalSlot.slotComponent, ParameterTypes, ReturnType,
                            lSignalSlot.slotMethodName,out lSignaMethodlSelectIndex);
                        
                        lMethodNames.Add(lSignalSlot.slotMethodName);

                        int lSelected = EditorGUILayout.Popup(lMethodNames.Count - 1, lMethodNames.ToArray());
                        if(lSignalSlot.slotMethodName != lMethodNames[lSelected])
                        {
                            lSignalSlot.slotMethodName = lMethodNames[lSelected];

                            Undo.RegisterUndo(target, "Slot Method Change");
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (lSignaMethodlSelectIndex == -1)
                        outError("can not find the function in signal");
                }
            }

        }

        EditorGUILayout.EndVertical();
    }

    List<string>    findMethodNames(object pObject,Type[] pParameterTypes,Type pReturnType,
        string pSelectedName, out int pSelected)
    {
        pSelected = -1;
        List<string> lOut = new List<string>();
        Type lType = pObject.GetType();
        //Debug.Log("pObject:" + pObject.ToString());
        if(pParameterTypes.Length==1&&pReturnType==null)
        {
            var lParameterType = pParameterTypes[0];
            var lPropertyList = lType.GetProperties();
            foreach (var lProperty in lPropertyList)
            {
                if(lProperty.PropertyType ==lParameterType
                    || lParameterType.IsSubclassOf(lProperty.PropertyType))
                    lOut.Add(lProperty.Name);
                if (pSelectedName == lProperty.Name)
                    pSelected = lOut.Count - 1;
            }
            //var lField = lType.GetFields();
        }
        MethodInfo[] pMethodInfoList = lType.GetMethods();

        //Debug.Log("pMethodInfoList:" + pMethodInfoList.Length);
        foreach (var pMethodInfo in pMethodInfoList)
        {
            //Debug.Log(pMethodInfo.ReturnType.ToString());
            //Debug.Log(pReturnType.ToString());
            //Debug.Log("pMethodInfo.ReturnType == pReturnType:" + (pMethodInfo.ReturnType == pReturnType));
            if (
                (pMethodInfo.ReturnType == pReturnType || pMethodInfo.ReturnType.IsSubclassOf(pReturnType))
                && isEquals(
                        zzSignalSlot.toTypeArray(pMethodInfo.GetParameters()),
                        pParameterTypes))
            {
                lOut.Add(pMethodInfo.Name);
                if (pSelectedName == pMethodInfo.Name)
                    pSelected = lOut.Count-1;
            }
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