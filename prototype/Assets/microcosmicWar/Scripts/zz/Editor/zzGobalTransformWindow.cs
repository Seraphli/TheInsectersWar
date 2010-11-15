using UnityEngine;
using UnityEditor;
using System.Collections;

//[CustomEditor(typeof(GameObject))]
class zzGobalTransformWindow : EditorWindow
{
    Transform tempObject1;
    Transform tempObject2;
    Transform tempObject3;

    [MenuItem("Window/Gobal Transform")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(zzGobalTransformWindow),false,"全局变换");
    }

    //public override void OnInspectorGUI()
    void OnGUI() 
    {
        //base.OnInspectorGUI();
        Transform transform = Selection.activeTransform;
        if (transform)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Vector3Field("全局位置", transform.position);
            EditorGUILayout.Vector3Field("全局旋转", transform.rotation.eulerAngles);
            EditorGUILayout.Vector3Field("全局放缩", transform.lossyScale);
            EditorGUILayout.EndVertical();

        }
        tempObject1 = (Transform)EditorGUILayout.ObjectField(tempObject1, typeof(Transform));
        tempObject2 = (Transform)EditorGUILayout.ObjectField(tempObject2, typeof(Transform));
        tempObject3 = (Transform)EditorGUILayout.ObjectField(tempObject3, typeof(Transform));
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}