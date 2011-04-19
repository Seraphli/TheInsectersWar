﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class zzScriptSearch:EditorWindow
{
    [MenuItem("Window/Script Search")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(zzScriptSearch), false, "脚本搜索");
    }

    public MonoScript scriptToSearch;
    public GameObject[] result = new GameObject[0]{};
    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        scriptToSearch = (MonoScript)EditorGUILayout.ObjectField(scriptToSearch, typeof(MonoScript));

        if(GUILayout.Button("search",GUILayout.ExpandWidth(false)) && scriptToSearch)
        {
            string lScriptName = scriptToSearch.name;
            //var lType = System.Type.GetType(lScriptName);
            List<GameObject> lResult = new List<GameObject>();
            var lAllObject = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
            foreach (var lObject in lAllObject)
            {
                if (lObject.GetComponent(lScriptName))
                    lResult.Add(lObject);
            }
            result = lResult.ToArray();
        }

        GUILayout.EndHorizontal();
        foreach (var lObject in result)
        {
            EditorGUILayout.ObjectField(lObject, typeof(GameObject));
        }
        GUILayout.EndVertical();
    }
}