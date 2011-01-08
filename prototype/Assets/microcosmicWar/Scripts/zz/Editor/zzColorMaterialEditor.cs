using UnityEngine;
using UnityEditor;
using System.Collections;

class zzColorMaterialEditor : EditorWindow
{

    [MenuItem("Window/Color Material")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(zzColorMaterialEditor), false, "颜色材质");
    }

    Color color;
    bool createNewMaterial = false;

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        color = EditorGUILayout.ColorField(color);
        createNewMaterial = GUILayout.Toggle(createNewMaterial, "新材质", GUILayout.Width(50.0f));
        if (GUILayout.Button("设置到选定物体", GUILayout.Width(100.0f)) 
            && Selection.activeGameObject
            && Selection.activeGameObject.GetComponent<Renderer>())
        {
            var lRender = Selection.activeGameObject.GetComponent<Renderer>();
            Material lMaterial;
            if (createNewMaterial)
                lMaterial = new Material(Shader.Find("Transparent/Diffuse"));
            else
                lMaterial = lRender.sharedMaterial;

            if (lMaterial)
            {
                lMaterial.color = color;
                lRender.sharedMaterial = lMaterial;

            }
        }
        EditorGUILayout.EndHorizontal();

    }

}