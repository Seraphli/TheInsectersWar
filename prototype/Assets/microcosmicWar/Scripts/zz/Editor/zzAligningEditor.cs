using UnityEngine;
using UnityEditor;

public class zzAligningEditor: EditorWindow
{
    [MenuItem("Window/zz Aligning")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(zzAligningEditor), false, "对齐");
    }

    public Transform copy;
    public Transform to;
    void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            copy = (Transform)EditorGUILayout.ObjectField("copy",copy, typeof(Transform));
            to = (Transform)EditorGUILayout.ObjectField("to", to, typeof(Transform));
            if (GUILayout.Button("Align"))
            {
                to.position = copy.position;
            }
        }
        GUILayout.EndVertical();
    }
}