using UnityEngine;
using UnityEditor;
using System.Collections;


class zzEditorGUIUtility
{
    delegate T FieldFuc<T>(T t,params GUILayoutOption[] options);

    static T GenericLayoutField<T>(string pName, T pValue, FieldFuc<T> pFieldFuc,uint pSpaceNum)
    {
        EditorGUILayout.BeginHorizontal();
        while (pSpaceNum!=0)
        {
            EditorGUILayout.Space();
            --pSpaceNum;
        }
        GUILayout.Label(pName);
        T lOut = pFieldFuc(pValue);
        EditorGUILayout.EndHorizontal();
        return lOut;
    }

    //public static T LayoutField<T>(string name, Tvalue, uint pSpaceNum);

    public static System.Enum LayoutField(string name, System.Enum value, uint pSpaceNum)
    {
        //EditorGUILayout.BeginHorizontal();

        ////EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.TextField(name);
        ////EditorGUILayout.EndHorizontal();

        ////EditorGUILayout.BeginHorizontal();
        //var lOut = EditorGUILayout.EnumPopup(value);
        ////EditorGUILayout.EndHorizontal();

        //EditorGUILayout.EndHorizontal();
        return GenericLayoutField(name, value, EditorGUILayout.EnumPopup, pSpaceNum);
    }

    public static float LayoutField(string name, System.Enum value)
    {
        return LayoutField(name, value);
    }

    public static float LayoutField(string name, float value, uint pSpaceNum)
    {
        //EditorGUILayout.BeginHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.TextField(name);
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //var lOut = EditorGUILayout.FloatField(value);
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.EndHorizontal();
        return GenericLayoutField(name, value, EditorGUILayout.FloatField, pSpaceNum);
    }

    public static float LayoutField(string name, float value )
    {
        return LayoutField(name, value);
    }

    //public static T LayoutField<T>(string name, T value)
    //{
    //    LayoutField<T>(name, value, 0);
    //}
}