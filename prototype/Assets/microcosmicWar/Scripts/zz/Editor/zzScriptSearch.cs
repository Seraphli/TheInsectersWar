using UnityEngine;
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
    public string scriptNameToSearch = "";
    public Vector2 scrollPos;
    public GameObject[] result = new GameObject[0]{};
    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        var lNewScriptToSearch = (MonoScript)EditorGUILayout.ObjectField(scriptToSearch, typeof(MonoScript));
        scriptNameToSearch = EditorGUILayout.TextField(scriptNameToSearch);
        if (scriptToSearch!=lNewScriptToSearch)
        {
            scriptToSearch = lNewScriptToSearch;
            scriptNameToSearch = scriptToSearch.name;
        }

        if(GUILayout.Button("search",GUILayout.ExpandWidth(false)) && scriptToSearch)
        {
            //var lType = System.Type.GetType(lScriptName);
            List<GameObject> lResult = new List<GameObject>();
            var lAllObject = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
            foreach (var lObject in lAllObject)
            {
                if (lObject.GetComponent(scriptNameToSearch))
                    lResult.Add(lObject);
            }
            result = lResult.ToArray();
        }
        GUILayout.EndHorizontal();
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        foreach (var lObject in result)
        {
            EditorGUILayout.ObjectField(lObject, typeof(GameObject));
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}